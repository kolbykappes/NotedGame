// Scratch script: eye-level views of the facility, written to tools/out/. Run with eval_file after building the scene.
var views = new (string name, UnityEngine.Vector3 pos, UnityEngine.Vector3 euler)[]
{
    ("v1_wake_pov", new UnityEngine.Vector3(-26.6f, 1.6f, 10.6f), new UnityEngine.Vector3(0f, 180f, 0f)),
    ("v2_console_from_door", new UnityEngine.Vector3(-25f, 1.6f, 5f), new UnityEngine.Vector3(5f, 0f, 0f)),
    ("v3_corridor_east", new UnityEngine.Vector3(-28f, 1.6f, 0f), new UnityEngine.Vector3(0f, 90f, 0f)),
    ("v4_corridor_windows", new UnityEngine.Vector3(5f, 1.6f, 1.2f), new UnityEngine.Vector3(5f, 112f, 0f)),
    ("v5_coffee", new UnityEngine.Vector3(-14f, 1.6f, 7f), new UnityEngine.Vector3(5f, 33f, 0f)),
    ("v6_printer", new UnityEngine.Vector3(0f, 1.6f, 7f), new UnityEngine.Vector3(5f, 46f, 0f)),
    ("v7_terminal", new UnityEngine.Vector3(-24f, 1.6f, 7f), new UnityEngine.Vector3(10f, 90f, 0f)),
};
var outs = new System.Text.StringBuilder();
foreach (var v in views)
{
    outs.Append(Noted.EditorTools.OffscreenCapture.FromPose(v.pos, v.euler, 70f, "C:/Code/Noted/tools/out/" + v.name + ".png", 1280, 720)).Append(';');
}
return outs.ToString();
