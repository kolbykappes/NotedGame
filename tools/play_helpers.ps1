# Scratch helpers for driving the running game from PowerShell (Play Mode only).
# Researched approach (see Docs/research/unity-cli-play-and-frames.md):
#   - PlayBegin enters Play Mode, turns on set_autotick and Application.runInBackground so frames advance with the Editor
#     in the background, and proves it with wait_for on Time.frameCount. PlayEnd restores everything.
#   - Snap uses capture_game_view (the real Game view, overlay UI included).
#   - Keyboard and mouse devices are disabled while the Editor is in the background, so tests call the game's own methods,
#     and hold-E uses Interactor.SimulateInteractHeld.
# C# strings inside eval code must use double quotes; the PowerShell side uses single-quoted here-strings.
$script:proj = "C:/Code/Noted"

function Ev([string]$code) {
    $raw = unity command eval --code $code --project-path $script:proj -- --timeout 60000 2>&1 | Out-String
    if ($raw -match 'Compilation Failed|"success":false|Error:') { return "EVAL FAILED: " + $raw.Substring(0, [Math]::Min(500, $raw.Length)) }
    $m = [regex]::Match($raw, '"result":"(.*?)"\}\s*\{"code"', 'Singleline')
    if ($m.Success) { return $m.Groups[1].Value }
    return $raw.Substring(0, [Math]::Min(300, $raw.Length))
}

function EvFile([string]$path) {
    $raw = unity command eval_file --file $path --project-path $script:proj -- --timeout 60000 2>&1 | Out-String
    if ($raw -match 'Compilation Failed|"success":false|Error:') { return "EVAL FAILED: " + $raw.Substring(0, [Math]::Min(500, $raw.Length)) }
    $m = [regex]::Match($raw, '"result":"(.*?)"\}\s*\{"file"', 'Singleline')
    if ($m.Success) { return $m.Groups[1].Value }
    return $raw.Substring(0, [Math]::Min(300, $raw.Length))
}

function Probe() { return EvFile "C:/Code/Noted/tools/probe.cs" }

function PlayBegin() {
    unity command editor_play --project-path $script:proj 2>&1 | Out-Null
    unity status --until-ready --project-path $script:proj 2>&1 | Out-Null
    unity command set_autotick --project-path $script:proj 2>&1 | Out-Null
    $null = Ev 'UnityEngine.Application.runInBackground = true; return "ok";'
    $w = unity command wait_for --project-path $script:proj -- --condition '{"member":"UnityEngine.Time.frameCount","op":"changed"}' --timeout_s 10 2>&1 | Out-String
    if ($w -match '"met":true') { return "playing, frames advancing" } else { return "FROZEN: " + $w.Substring(0, [Math]::Min(200, $w.Length)) }
}

function PlayEnd() {
    $null = Ev 'UnityEngine.Application.runInBackground = false; return "ok";'
    unity command set_autotick --project-path $script:proj -- --enable false 2>&1 | Out-Null
    unity command editor_stop --project-path $script:proj 2>&1 | Out-Null
    unity status --until-ready --project-path $script:proj 2>&1 | Out-Null
    Remove-Item C:\Code\Noted\Assets\Screenshots -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item C:\Code\Noted\Assets\Screenshots.meta -Force -ErrorAction SilentlyContinue
    return (Ev 'return "playing=" + UnityEngine.Application.isPlaying + " playerSettingsRunInBackground=" + UnityEditor.PlayerSettings.runInBackground;')
}

function Snap([string]$name) {
    unity command capture_game_view --project-path $script:proj -- --source screen --width 1600 --height 900 --save_path "Screenshots/$name.png" 2>&1 | Out-Null
    $src = "C:\Code\Noted\Assets\Screenshots\$name.png"
    if (Test-Path $src) { Copy-Item $src "C:\Code\Noted\tools\out\$name.png" -Force; return "C:/Code/Noted/tools/out/$name.png" }
    return "capture failed"
}

# Hold E on whatever the crosshair is on, using the interactor's test hook.
function HoldFor([double]$seconds) {
    $null = Ev 'UnityEngine.Object.FindAnyObjectByType<Noted.Game.Interactor>().SimulateInteractHeld = true; return "held";'
    Start-Sleep -Milliseconds ([int]($seconds * 1000))
    $null = Ev 'UnityEngine.Object.FindAnyObjectByType<Noted.Game.Interactor>().SimulateInteractHeld = false; return "released";'
}

function Place([double]$x, [double]$z, [double]$yaw, [double]$pitch) {
    $code = 'var f = UnityEngine.Object.FindAnyObjectByType<Noted.Game.GameFlow>(); var motor = f.PlayerTransform.GetComponent<Noted.Game.PlayerMotor>(); motor.TeleportTo(new UnityEngine.Vector3(' + $x + 'f, 0.05f, ' + $z + 'f), ' + $yaw + 'f); var look = f.PlayerTransform.GetComponent<Noted.Game.PlayerLook>(); typeof(Noted.Game.PlayerLook).GetField("pitch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(look, ' + $pitch + 'f); return "placed";'
    $null = Ev $code
    Start-Sleep -Milliseconds 500
}

function Target() {
    return (Ev 'var i = UnityEngine.Object.FindAnyObjectByType<Noted.Game.Interactor>(); return "target=" + i.HasTarget + " prompt=" + i.Prompt + " progress=" + i.Progress01.ToString("0.00");')
}
