#!/usr/bin/env bash
# Unity Editor ops: preflight | save | compile | test | play-begin | play-end | snap. Fail-closed: anything unexpected exits non-zero.
# Exit codes: 0 ok, 1 real failure, 2 refused (unsafe state), 3 unparseable/unexpected output, 4 no tests ran.
set -u

# Unity project root: $UNITY_PROJECT_PATH, else the nearest ancestor of this script (then of the cwd) that holds
# ProjectSettings/ProjectVersion.txt. A git worktree therefore resolves to its own project, never to another checkout's.
find_project() {
  local d
  for d in "$(cd "$(dirname "$0")" && pwd)" "$(pwd)"; do
    while [ -n "$d" ] && [ "$d" != "/" ]; do
      if [ -f "$d/ProjectSettings/ProjectVersion.txt" ]; then (cd "$d" && { pwd -W 2>/dev/null || pwd; }); return 0; fi
      d=$(dirname "$d")
    done
  done
  return 1
}
PROJ="${UNITY_PROJECT_PATH:-$(find_project)}"
[ -n "$PROJ" ] || { echo "[unity-ops] REFUSED: no Unity project found (set UNITY_PROJECT_PATH)"; exit 2; }
echo "[unity-ops] target=$PROJ"
STATE="$PROJ/Temp/unity-ops-play.state"   # written by play-begin, required by play-end (only stop sessions you started: rule 25)

U() { unity command "$@" --project-path "$PROJ" --result-only; }

# jq-less JSON read: jget '<js expr over j>' ; prints value, exits 3 if parse fails or value is undefined
jget() {
  node -e 'let s="";process.stdin.on("data",d=>s+=d).on("end",()=>{let j;try{j=JSON.parse(s)}catch(e){process.exit(3)}
    let v;try{v=eval(process.argv[1])}catch(e){process.exit(3)}
    if(v===undefined)process.exit(3);console.log(typeof v==="object"?JSON.stringify(v):v)})' "$1"
}

need() { local v; v=$(printf '%s' "$2" | jget "$1") || { echo "[unity-ops] UNPARSEABLE: $1"; printf '%s\n' "$2" | head -c 600; exit 3; }; printf '%s' "$v"; }

preflight() {
  local es sc
  es=$(U editor_status) || { echo "[unity-ops] editor_status failed - Editor not reachable (Safe Mode? still loading? run: unity pipeline list)"; exit 3; }
  [ "$(need 'j.status' "$es")" = ready ] || { echo "[unity-ops] REFUSED: Editor status not ready"; exit 2; }
  [ "$(need 'j.compiling||j.domainReloadInProgress' "$es")" = false ] || { echo "[unity-ops] REFUSED: compiling/reloading - wait and retry"; exit 2; }
  echo "[unity-ops] unity=$(need 'j.unityVersion' "$es") playMode=$(need 'j.playMode' "$es")"
  PLAY=$(need 'j.playMode' "$es")
  sc=$(U list_open_scenes) || exit 3
  DIRTY=$(need 'j.scenes.filter(s=>s.isDirty).map(s=>s.path).join(",")' "$sc")
  echo "[unity-ops] dirty scenes: ${DIRTY:-none}"
}

case "${1:-}" in
  preflight) preflight ;;

  save) # rule 4/5: never write scenes in Play Mode; confirm clean after
    preflight
    [ "$PLAY" = stopped ] || { echo "[unity-ops] REFUSED: Play Mode is $PLAY - scene writes revert on exit"; exit 2; }
    [ -z "$DIRTY" ] && { echo "[unity-ops] nothing to save"; exit 0; }
    U save_all >/dev/null || exit 3
    sc=$(U list_open_scenes) || exit 3
    [ -z "$(need 'j.scenes.filter(s=>s.isDirty).length||""' "$sc")" ] || { echo "[unity-ops] FAILED: scenes still dirty after save_all"; exit 1; }
    echo "[unity-ops] saved; now verify the FILE on disk (git diff), not this message" ;;

  compile) # rule 5/19: save first (never recompile with a dirty scene), then recompile and require a clean result
    preflight
    [ -z "$DIRTY" ] || { echo "[unity-ops] REFUSED: dirty scenes ($DIRTY) - run: unity-ops.sh save"; exit 2; }
    out=$(unity recompile --project-path "$PROJ" --json) ; rc=$?
    ok=$(printf '%s' "$out" | jget 'j.success===true&&j.data.failed===false&&j.data.errors.length===0&&["up_to_date","completed"].includes(j.data.status)') || { echo "[unity-ops] UNPARSEABLE recompile output (rc=$rc)"; printf '%s\n' "$out" | head -c 800; exit 3; }
    [ "$ok" = true ] || { echo "[unity-ops] COMPILE FAILED"; printf '%s\n' "$out" | head -c 2000; exit 1; }
    echo "[unity-ops] compile OK (status=$(printf '%s' "$out" | jget 'j.data.status'))"
    # a real recompile is followed by a domain reload; commands sent during it fail with a network error
    unity status --until-ready --project-path "$PROJ" --timeout 180 >/dev/null 2>&1 || { echo "[unity-ops] Editor not ready after compile"; exit 3; }
    for _ in $(seq 1 30); do
      es=$(U editor_status 2>/dev/null) && [ "$(printf '%s' "$es" | jget 'j.status==="ready"&&!j.compiling&&!j.domainReloadInProgress' 2>/dev/null)" = true ] && break
      sleep 2
    done ;;

  test) # usage: test [editor|playmode|all] [name-filter]. Uses the LIVE Editor, never `unity test` (spawns a 2nd batch Editor -> rule 8).
    mode="${2:-editor}"; filter="${3:-}"
    case "$mode" in editor|playmode|all) ;; *) echo "[unity-ops] REFUSED: mode must be editor|playmode|all (got '$mode')"; exit 2 ;; esac
    "$0" compile || exit $?
    args=(--mode "$mode"); [ -n "$filter" ] && args+=(--filter "$filter")
    [ "$mode" = editor ] || args+=(--async_tests true)
    out=$(U run_tests "${args[@]}") || { echo "[unity-ops] run_tests call failed"; exit 3; }
    total=$(need 'j.Summary.Total' "$out"); failed=$(need 'j.Summary.Failed' "$out")
    if [ "$(printf '%s' "$out" | jget 'j.result')" = playmode_running ] && [ "$mode" != editor ]; then
      for _ in $(seq 1 150); do
        st=$(U test_status) || exit 3
        [ "$(need 'j.status' "$st")" = completed ] && break
        sleep 2
      done
      [ "$(need 'j.status' "$st")" = completed ] || { echo "[unity-ops] TIMEOUT waiting for PlayMode tests"; exit 1; }
      total=$(need 'j.summary.total' "$st"); failed=$(need 'j.summary.failed' "$st"); out="$st"
    fi
    echo "[unity-ops] total=$total failed=$failed"
    [ "$total" -gt 0 ] || { echo "[unity-ops] NO TESTS RAN (0/0 is not a pass)"; exit 4; }
    if [ "$failed" -ne 0 ]; then
      echo "[unity-ops] TESTS FAILED"
      # EditMode shape (observed): Results[].{FullName,Status,Message,StackTrace}; run_tests returns success:true even on failures
      fl=$(printf '%s' "$out" | jget '(j.Results||j.results||[]).filter(r=>(r.Status||r.status)==="Failed").map(r=>(r.FullName||r.fullName)+"\n    "+String(r.Message||r.message).trim().replace(/\s+/g," ")+"\n    "+String(r.StackTrace||r.stackTrace).split("\n")[0].trim()).join("\n")')
      if [ -n "$fl" ]; then printf '%s\n' "$fl"; else printf '%s\n' "$out" | head -c 4000; fi
      exit 1
    fi
    echo "[unity-ops] TESTS PASSED" ;;

  play-begin) # rules 3a, 25, 35, 38: enter Play Mode so frames advance even with the Editor in the background, and prove it
    preflight
    [ "$PLAY" = stopped ] || { echo "[unity-ops] REFUSED: Play Mode is $PLAY - stop it first (rule 25 lets you): unity command editor_stop --project-path <proj>, then run play-begin again"; exit 2; }
    [ -z "$DIRTY" ] || { echo "[unity-ops] REFUSED: dirty scenes ($DIRTY) - run: unity-ops.sh save"; exit 2; }
    [ ! -f "$STATE" ] || { echo "[unity-ops] REFUSED: a play-begin is already recorded ($STATE) - run play-end first"; exit 2; }
    orig=$(need 'j.result' "$(U eval --code 'return UnityEngine.Application.runInBackground.ToString();')") || exit 3
    mkdir -p "$PROJ/Temp"; printf '%s\n' "$orig" > "$STATE"   # remember the project's value so play-end can restore it
    U set_autotick >/dev/null || { rm -f "$STATE"; exit 3; }
    U editor_play >/dev/null || { rm -f "$STATE"; exit 3; }
    unity status --until-ready --project-path "$PROJ" >/dev/null 2>&1 || { echo "[unity-ops] Editor not ready after editor_play"; exit 3; }
    U eval --code 'UnityEngine.Application.runInBackground = true; return "ok";' >/dev/null || exit 3
    w=$(unity command wait_for --project-path "$PROJ" --result-only -- --condition '{"member":"UnityEngine.Time.frameCount","op":"changed"}' --timeout_s 10) || { echo "[unity-ops] wait_for failed"; exit 3; }
    [ "$(need 'j.met' "$w")" = true ] || { echo "[unity-ops] FROZEN: frames are not advancing (rule 3a). Run play-end."; printf '%s\n' "$w" | head -c 400; exit 1; }
    echo "[unity-ops] play-begin OK: frames advancing (runInBackground was $orig; play-end restores it)" ;;

  play-end) # rule 38: restore every override, stop the session this script started, and check settings did not change
    [ -f "$STATE" ] || { echo "[unity-ops] REFUSED: no play-begin recorded - play-begin did not start this session, so play-end will not touch its settings; to just stop it use: unity command editor_stop (rule 25)"; exit 2; }
    orig=$(tr -d '\r\n' < "$STATE"); case "$orig" in True|False) ;; *) orig=False ;; esac
    low=$(printf '%s' "$orig" | tr 'A-Z' 'a-z')
    U eval --code "UnityEngine.Time.timeScale = 1f; UnityEngine.Application.runInBackground = $low; return \"ok\";" >/dev/null || { echo "[unity-ops] could not restore runInBackground; state file kept ($STATE)"; exit 3; }
    unity command set_autotick --project-path "$PROJ" --result-only -- --enable false >/dev/null 2>&1 || true
    U editor_stop >/dev/null || { echo "[unity-ops] editor_stop failed; state file kept"; exit 3; }
    unity status --until-ready --project-path "$PROJ" >/dev/null 2>&1 || { echo "[unity-ops] Editor not ready after editor_stop"; exit 3; }
    now=$(need 'j.result' "$(U eval --code 'return UnityEditor.PlayerSettings.runInBackground.ToString();')") || exit 3
    [ "$now" = "$orig" ] || { echo "[unity-ops] FAILED: runInBackground is $now, expected $orig"; exit 1; }
    rm -f "$STATE"
    # the screenshot folder from `snap` becomes an asset; remove it
    rm -rf "$PROJ/Assets/Screenshots" "$PROJ/Assets/Screenshots.meta"
    if command -v git >/dev/null 2>&1 && git -C "$PROJ" rev-parse --git-dir >/dev/null 2>&1; then
      ch=$(git -C "$PROJ" status --short -- ProjectSettings 2>/dev/null)
      [ -z "$ch" ] || { echo "[unity-ops] WARNING: ProjectSettings changed during the session:"; printf '%s\n' "$ch"; exit 1; }
    fi
    echo "[unity-ops] play-end OK: stopped, runInBackground=$orig restored, no ProjectSettings change" ;;

  snap) # usage: snap <name> [width] [height]. Rule 36: capture the real Game view (overlay UI included); needs frames advancing (play-begin)
    name="${2:-snap}"; w="${3:-1600}"; h="${4:-900}"
    case "$name" in *[!A-Za-z0-9_-]*|"") echo "[unity-ops] REFUSED: snap name must be letters, digits, _ or -"; exit 2 ;; esac
    es=$(U editor_status) || exit 3
    [ "$(need 'j.playMode' "$es")" = playing ] || { echo "[unity-ops] REFUSED: not in Play Mode (run play-begin first)"; exit 2; }
    out=$(unity command capture_game_view --project-path "$PROJ" --result-only -- --source screen --width "$w" --height "$h" --save_path "Screenshots/$name.png") || { echo "[unity-ops] capture_game_view failed"; exit 3; }
    [ -f "$PROJ/Assets/Screenshots/$name.png" ] || { echo "[unity-ops] capture reported but no file at Assets/Screenshots/$name.png"; printf '%s\n' "$out" | head -c 400; exit 3; }
    mkdir -p "$PROJ/Temp/unity-ops-shots"; cp "$PROJ/Assets/Screenshots/$name.png" "$PROJ/Temp/unity-ops-shots/$name.png"
    echo "[unity-ops] snap saved: $PROJ/Temp/unity-ops-shots/$name.png (Assets/Screenshots is removed by play-end)" ;;

  *) echo "usage: $0 preflight|save|compile|test [editor|playmode|all] [filter]|play-begin|play-end|snap <name> [w] [h]"; exit 2 ;;
esac
