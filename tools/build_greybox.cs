// Scratch script (run with: unity command eval_file tools/build_greybox.cs). Builds the Player prefab and a greybox test scene.
// Idempotent: refuses to run if the scene already exists, so it never duplicates content.
var scenePath = "Assets/Scenes/Greybox.unity";
var prefabPath = "Assets/Prefabs/Player.prefab";
if (UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath) != null)
{
    return "refused: " + scenePath + " already exists";
}

if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Prefabs"))
{
    UnityEditor.AssetDatabase.CreateFolder("Assets", "Prefabs");
}

var actions = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/InputSystem_Actions.inputactions");
if (actions == null) return "missing InputSystem_Actions.inputactions";

var motorType = System.Type.GetType("Noted.Game.PlayerMotor, Noted.Game");
var lookType = System.Type.GetType("Noted.Game.PlayerLook, Noted.Game");
if (motorType == null || lookType == null) return "player scripts not compiled yet";

// Player prefab: body (CharacterController + motor + look) with a camera child.
UnityEngine.GameObject temp = null;
try
{
    temp = new UnityEngine.GameObject("Player");
    var cc = temp.AddComponent<UnityEngine.CharacterController>();
    cc.height = 1.8f;
    cc.radius = 0.3f;
    cc.center = new UnityEngine.Vector3(0f, 0.9f, 0f);

    var camGo = new UnityEngine.GameObject("PlayerCamera");
    camGo.transform.SetParent(temp.transform, false);
    camGo.transform.localPosition = new UnityEngine.Vector3(0f, 1.6f, 0f);
    camGo.tag = "MainCamera";
    var cam = camGo.AddComponent<UnityEngine.Camera>();
    cam.nearClipPlane = 0.05f;
    camGo.AddComponent<UnityEngine.AudioListener>();

    var motor = temp.AddComponent(motorType);
    var look = temp.AddComponent(lookType);

    var motorSo = new UnityEditor.SerializedObject(motor);
    motorSo.FindProperty("actions").objectReferenceValue = actions;
    motorSo.ApplyModifiedPropertiesWithoutUndo();

    var lookSo = new UnityEditor.SerializedObject(look);
    lookSo.FindProperty("actions").objectReferenceValue = actions;
    lookSo.FindProperty("playerCamera").objectReferenceValue = cam;
    lookSo.ApplyModifiedPropertiesWithoutUndo();

    UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
}
finally
{
    if (temp != null) UnityEngine.Object.DestroyImmediate(temp);
}

var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefabPath);
if (prefab == null) return "prefab was not saved";

// Greybox room: 20 x 20 m floor, four walls, a console block and a cot block to walk around.
var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
    UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);

System.Func<string, UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.GameObject> block = (name, pos, scale) =>
{
    var go = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
    go.name = name;
    go.transform.position = pos;
    go.transform.localScale = scale;
    return go;
};

block("Floor", new UnityEngine.Vector3(0f, -0.1f, 0f), new UnityEngine.Vector3(20f, 0.2f, 20f));
block("Wall_N", new UnityEngine.Vector3(0f, 1.5f, 10f), new UnityEngine.Vector3(20f, 3f, 0.2f));
block("Wall_S", new UnityEngine.Vector3(0f, 1.5f, -10f), new UnityEngine.Vector3(20f, 3f, 0.2f));
block("Wall_E", new UnityEngine.Vector3(10f, 1.5f, 0f), new UnityEngine.Vector3(0.2f, 3f, 20f));
block("Wall_W", new UnityEngine.Vector3(-10f, 1.5f, 0f), new UnityEngine.Vector3(0.2f, 3f, 20f));
block("Console", new UnityEngine.Vector3(0f, 0.5f, 6f), new UnityEngine.Vector3(1.6f, 1f, 0.7f));
block("Cot", new UnityEngine.Vector3(-6f, 0.25f, -6f), new UnityEngine.Vector3(2f, 0.5f, 0.9f));
block("Step", new UnityEngine.Vector3(5f, 0.1f, -3f), new UnityEngine.Vector3(2f, 0.2f, 2f));

var lightGo = new UnityEngine.GameObject("Directional Light");
var light = lightGo.AddComponent<UnityEngine.Light>();
light.type = UnityEngine.LightType.Directional;
lightGo.transform.rotation = UnityEngine.Quaternion.Euler(50f, -30f, 0f);

var player = (UnityEngine.GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, scene);
player.transform.position = new UnityEngine.Vector3(0f, 0f, -5f);

var saved = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
return "saved=" + saved + " scene=" + scenePath + " prefab=" + prefabPath;
