# Scratch: plays several days of the loop in Play Mode using direct session calls for the tasks
# (a real hold-E run was verified separately). Usage: . tools/play_full.ps1; RunDays 1 4
. C:\Code\Noted\tools\play_helpers.ps1

function DismissCards() {
    for ($i = 0; $i -lt 8; $i++) {
        $p = Probe
        if ($p -match 'cardOpen=True') { Ev 'var c = UnityEngine.Object.FindAnyObjectByType<Noted.Game.CardScreen>(); c.Continue(); return "ok";' | Out-Null; Start-Sleep -Milliseconds 700 } else { break }
    }
}

function SleepAtCot() {
    $setup = @'
var f = UnityEngine.Object.FindAnyObjectByType<Noted.Game.GameFlow>();
var motor = f.PlayerTransform.GetComponent<Noted.Game.PlayerMotor>();
motor.TeleportTo(new UnityEngine.Vector3(-26.9f, 0.05f, 11.0f), 270f);
var look = f.PlayerTransform.GetComponent<Noted.Game.PlayerLook>();
typeof(Noted.Game.PlayerLook).GetField("pitch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(look, 40f);
return "at cot";
'@
    Ev $setup | Out-Null; Start-Sleep -Milliseconds 700
    KeyDown "E" | Out-Null; Start-Sleep -Milliseconds 250; KeyUp | Out-Null; Start-Sleep -Milliseconds 600
}

$script:finish = @'
var f = UnityEngine.Object.FindAnyObjectByType<Noted.Game.GameFlow>(); var s = f.Session;
int n = 0;
foreach (var t in s.TodaysTasks) { if (t.Status == Noted.Core.TaskStatus.Automated) continue; int guard = 0; while (!t.IsFinished && guard++ < 5) { s.CompleteStep(t.CurrentStep.StationId); } if (t.IsFinished) n++; }
f.DebugExpireClock();
return "finished " + n + " of " + s.TodaysTasks.Count;
'@
$script:wakeJs = 'var buttons = UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(UnityEngine.FindObjectsInactive.Exclude, UnityEngine.FindObjectsSortMode.None); foreach (var b in buttons) { if (b.name == "Wake") { b.onClick.Invoke(); return "woke"; } } return "no wake";'
$script:pullSnap = @'
var pull = UnityEngine.Object.FindAnyObjectByType<Noted.Game.PullSequence>();
var cam = pull.GetComponentInChildren<UnityEngine.Camera>(true);
return Noted.EditorTools.OffscreenCapture.Render(cam, "C:/Code/Noted/tools/out/PULLNAME.png", 1600, 900);
'@

function RunDays([int]$from, [int]$to) {
    foreach ($day in $from..$to) {
        "--- day $day ---"
        DismissCards
        "start: " + (Probe)
        "finish: " + (Ev $script:finish)
        Start-Sleep -Milliseconds 1700
        "pull snap: " + (Ev ($script:pullSnap -replace 'PULLNAME', "play_pull_day$day"))
        $deadline = (Get-Date).AddSeconds(30)
        do { Start-Sleep -Milliseconds 700; $p = Probe } while ($p -notmatch 'phase=Quiet' -and (Get-Date) -lt $deadline)
        "quiet: " + $p
        SleepAtCot
        $p = Probe
        "after sleep: " + $p
        if ($p -match 'phase=RankUp') {
            Snap "play_promotion_day$day" | Out-Null
            Ev 'var c = UnityEngine.Object.FindAnyObjectByType<Noted.Game.CardScreen>(); c.Continue(); return "ok";' | Out-Null
            Start-Sleep -Milliseconds 600
            "promotion dismissed: " + (Probe)
        }
        Ev $script:wakeJs | Out-Null
        Start-Sleep -Milliseconds 900
    }
    "final: " + (Probe)
}
