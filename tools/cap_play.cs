var flow = UnityEngine.Object.FindFirstObjectByType<Noted.Game.GameFlow>();
var cam = flow.PlayerTransform.GetComponentInChildren<UnityEngine.Camera>();
return Noted.EditorTools.OffscreenCapture.Render(cam, "C:/Code/Noted/tools/out/play_intro.png", 1600, 900);
