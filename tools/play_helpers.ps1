# Scratch helpers for driving the running game from PowerShell (Play Mode only).
# C# strings inside the eval code must use double quotes; the PowerShell side uses single-quoted here-strings.
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

function KeyDown([string]$key) {
    $code = 'var kb = UnityEngine.InputSystem.Keyboard.current; UnityEngine.InputSystem.InputSystem.QueueStateEvent(kb, new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.' + $key + ')); return "down";'
    return Ev $code
}

function KeyUp() {
    $code = 'var kb = UnityEngine.InputSystem.Keyboard.current; UnityEngine.InputSystem.InputSystem.QueueStateEvent(kb, new UnityEngine.InputSystem.LowLevel.KeyboardState()); return "up";'
    return Ev $code
}

function Snap([string]$name) {
    $code = 'var f = UnityEngine.Object.FindAnyObjectByType<Noted.Game.GameFlow>(); var cam = f.PlayerTransform.GetComponentInChildren<UnityEngine.Camera>(); return Noted.EditorTools.OffscreenCapture.Render(cam, "C:/Code/Noted/tools/out/' + $name + '.png", 1600, 900);'
    return Ev $code
}
