using System.Collections.Generic;
using Noted.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Noted.EditorTools
{
    /// <summary>
    /// Generates the MVP support facility scene: rooms, stations, signs, lights, the player, the pull rig, the UI and all the wiring.
    /// Idempotent: it opens the scene if it exists, destroys every root object and rebuilds, so re-running never duplicates content.
    /// Everything is made from cubes (greybox) in the palette colours.
    /// </summary>
    public static class FacilityBuilder
    {
        public const string ScenePath = "Assets/Scenes/Facility.unity";
        const string MatFolder = "Assets/Materials";
        const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
        const float H = 3.2f;
        const float T = 0.2f;
        const float DoorH = 2.4f;
        const float DoorW = 2.4f;

        static readonly Color Wall = C(0x45, 0x53, 0x50);
        static readonly Color Floor = C(0x5E, 0x5B, 0x57);
        static readonly Color Ceiling = C(0xB8, 0xB2, 0xA8);
        static readonly Color Cream = C(0xEE, 0xE8, 0xE1);
        static readonly Color Beige = C(0xA6, 0x8B, 0x73);
        static readonly Color DarkTeal = C(0x26, 0x2E, 0x2F);
        static readonly Color Navy = C(0x17, 0x1C, 0x23);
        static readonly Color Purple = C(0x78, 0x3F, 0x8C);
        static readonly Color GreenTeal = C(0x37, 0x74, 0x70);
        static readonly Color Yellow = C(0xDA, 0xBD, 0x6A);

        static Color C(byte r, byte g, byte b) => new Color32(r, g, b, 255);

        // Materials
        static Material mWall, mFloor, mCeiling, mCream, mBeige, mDark, mNavy, mDoor, mRack, mRackLight, mPurpleGlow, mScreen,
            mActive, mCoin, mLanyardGrey, mLanyardBlue, mPlant, mWood, mMarker, mRed, mVending;

        static Font font;
        static Transform rGeometry, rProps, rStations, rLights, rSigns, rSystems, rHall, rCeiling;
        static readonly List<TaskStation> stations = new List<TaskStation>();
        static readonly List<Light> officeLights = new List<Light>();

        public static string Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return "error: Editor is in Play Mode";
            if (System.Type.GetType("Noted.Game.GameFlow, Noted.Game") == null) return "error: Noted.Game is not compiled";

            stations.Clear();
            officeLights.Clear();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Scene scene;
            if (AssetDatabase.LoadAssetAtPath<Object>(ScenePath) != null)
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
                foreach (var go in scene.GetRootGameObjects()) Object.DestroyImmediate(go);
            }
            else
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }

            // Flat day ambient so edit-mode captures match gameplay (LightingController fades it for the quiet time).
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.52f, 0.58f, 0.57f);
            RenderSettings.fog = false;

            MakeMaterials();
            rGeometry = Root("Geometry");
            rProps = Root("Props");
            rStations = Root("Stations");
            rLights = Root("Lights");
            rSigns = Root("Signs");
            rHall = Root("ServerHall");
            rCeiling = Root("Ceiling");
            rSystems = Root("Systems");

            BuildShell();
            BuildAdmin(out var cot, out var terminal, out var consoleScreen, out var wake, out var stand);
            BuildBreakRoom();
            BuildPrintRoom();
            BuildOffice();
            BuildUtilityAndCorridor();
            BuildServerHall();
            BuildLights(out var sun);

            // Player (the prefab, with an Interactor added) and the systems.
            var player = BuildPlayer(out var motor, out var look, out var interactor, out var playerCam);
            var flowGo = new GameObject("GameFlow");
            flowGo.transform.SetParent(rSystems, false);
            var flow = flowGo.AddComponent<GameFlow>();

            var coinsGo = new GameObject("Coins");
            coinsGo.transform.SetParent(rSystems, false);
            var spawner = coinsGo.AddComponent<CoinSpawner>();
            Set(spawner, "coinMaterial", mCoin);
            Set(spawner, "flow", flow);
            Set(spawner, "player", player.GetComponent<CharacterController>());

            var lightingGo = new GameObject("Lighting");
            lightingGo.transform.SetParent(rSystems, false);
            var lighting = lightingGo.AddComponent<LightingController>();
            Set(lighting, "sun", sun);
            SetArray(lighting, "officeLights", officeLights.ToArray());

            var pullGo = BuildPullRig(consoleScreen, out var pull);

            // UI
            var canvas = BuildCanvas(out var hud, out var cards, out var complaint, out var shop);

            // Wire stations and interactables to the flow.
            foreach (var s in stations) Set(s, "flow", flow);
            Set(cot, "flow", flow);
            Set(terminal, "flow", flow);

            Set(flow, "motor", motor);
            Set(flow, "look", look);
            Set(flow, "interactor", interactor);
            Set(flow, "playerCamera", playerCam);
            Set(flow, "hud", hud);
            Set(flow, "cards", cards);
            Set(flow, "complaint", complaint);
            Set(flow, "shop", shop);
            Set(flow, "coins", spawner);
            Set(flow, "lighting", lighting);
            Set(flow, "pull", pull);
            Set(flow, "wakePoint", wake);
            Set(flow, "consoleStandPoint", stand);

            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            return "saved=" + saved + " stations=" + stations.Count + " roots=" + scene.rootCount;
        }

        // ---------- shell: floor, walls, ceiling ----------

        static void BuildShell()
        {
            Box(rGeometry, "Floor", new Vector3(0, -0.1f, 0), new Vector3(60.4f, 0.2f, 28.4f), mFloor);
            var ceil = Box(rCeiling, "Ceiling", new Vector3(0, H + 0.1f, 0), new Vector3(60.4f, 0.2f, 28.4f), mCeiling, false);
            ceil.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;

            // Outer walls.
            Box(rGeometry, "Wall_North", new Vector3(0, H / 2, 14f), new Vector3(60.4f, H, T), mWall);
            Box(rGeometry, "Wall_South", new Vector3(0, H / 2, -14f), new Vector3(60.4f, H, T), mWall);
            Box(rGeometry, "Wall_West", new Vector3(-30f, H / 2, 0), new Vector3(T, H, 28.4f), mWall);
            Box(rGeometry, "Wall_East", new Vector3(30f, H / 2, 0), new Vector3(T, H, 28.4f), mWall);

            // Corridor north wall (z = 2) with five door gaps; room dividers.
            WallAlongX("CorridorNorth", 2f, -30f, 30f, new[] { -25f, -14f, 0f, 14f, 25f }, DoorW, DoorH);
            foreach (float x in new[] { -20f, -8f, 8f, 20f })
            {
                Box(rGeometry, "Divider_" + x, new Vector3(x, H / 2, 8f), new Vector3(T, H, 12f), mWall);
            }

            // Corridor south wall (z = -2): two windows into the server hall and a locked door.
            float z = -2f;
            WallAlongXWithOpenings("CorridorSouth", z, -30f, 30f, new[]
            {
                new Opening(-16.5f, 5f, 0.9f, DoorH),   // window
                new Opening(-4f, 2.0f, 0f, DoorH),      // locked door
                new Opening(13f, 5f, 0.9f, DoorH),      // window
            });
            // Invisible barriers fill the window openings so the player cannot climb through.
            Barrier("WindowBarrier1", new Vector3(-16.5f, 1.65f, z), new Vector3(5f, 1.5f, T));
            Barrier("WindowBarrier2", new Vector3(13f, 1.65f, z), new Vector3(5f, 1.5f, T));
            // Window frames.
            Frame("WindowFrame1", new Vector3(-16.5f, 1.65f, z), 5f, 1.5f);
            Frame("WindowFrame2", new Vector3(13f, 1.65f, z), 5f, 1.5f);

            // Locked door: a slab with a purple slit and a red lock light, set into the corridor side.
            var door = Box(rGeometry, "LockedDoor", new Vector3(-4f, DoorH / 2, z), new Vector3(1.9f, DoorH, 0.12f), mDoor);
            Box(door.transform, "DoorSlit", new Vector3(-4f, 1.55f, z + 0.07f), new Vector3(0.18f, 0.7f, 0.04f), mPurpleGlow, false);
            Box(door.transform, "LockLight", new Vector3(-3.2f, 1.2f, z + 0.07f), new Vector3(0.1f, 0.1f, 0.04f), mRed, false);
        }

        struct Opening
        {
            public float Center, Width, Bottom, Top;
            public Opening(float c, float w, float b, float t) { Center = c; Width = w; Bottom = b; Top = t; }
        }

        // A wall along X with full-height door gaps (lintel above each).
        static void WallAlongX(string name, float z, float x0, float x1, float[] gapCenters, float gapW, float gapH)
        {
            var list = new List<Opening>();
            foreach (var c in gapCenters) list.Add(new Opening(c, gapW, 0f, gapH));
            WallAlongXWithOpenings(name, z, x0, x1, list.ToArray());
        }

        // A wall along X with openings; solid between them, a sill below and a lintel above each opening.
        static void WallAlongXWithOpenings(string name, float z, float x0, float x1, Opening[] openings)
        {
            System.Array.Sort(openings, (a, b) => a.Center.CompareTo(b.Center));
            float cursor = x0;
            int i = 0;
            foreach (var o in openings)
            {
                float left = o.Center - o.Width / 2f;
                float right = o.Center + o.Width / 2f;
                if (left > cursor) Slab(name + "_Seg" + i, cursor, left, 0f, H, z);
                if (o.Bottom > 0f) Slab(name + "_Sill" + i, left, right, 0f, o.Bottom, z);
                if (o.Top < H) Slab(name + "_Lintel" + i, left, right, o.Top, H, z);
                cursor = right;
                i++;
            }
            if (x1 > cursor) Slab(name + "_Seg" + i, cursor, x1, 0f, H, z);
        }

        static void Slab(string name, float x0, float x1, float y0, float y1, float z)
        {
            Box(rGeometry, name, new Vector3((x0 + x1) / 2f, (y0 + y1) / 2f, z), new Vector3(x1 - x0, y1 - y0, T), mWall);
        }

        static void Frame(string name, Vector3 center, float w, float h)
        {
            float t = 0.08f;
            Box(rGeometry, name + "_Top", center + new Vector3(0, h / 2, 0), new Vector3(w + 0.1f, t, T + 0.04f), mCream, false);
            Box(rGeometry, name + "_Bottom", center + new Vector3(0, -h / 2, 0), new Vector3(w + 0.1f, t, T + 0.04f), mCream, false);
            Box(rGeometry, name + "_Left", center + new Vector3(-w / 2, 0, 0), new Vector3(t, h, T + 0.04f), mCream, false);
            Box(rGeometry, name + "_Right", center + new Vector3(w / 2, 0, 0), new Vector3(t, h, T + 0.04f), mCream, false);
        }

        static void Barrier(string name, Vector3 center, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(rGeometry, false);
            go.transform.position = center;
            var col = go.AddComponent<BoxCollider>();
            col.size = size;
        }

        // ---------- rooms ----------

        static void BuildAdmin(out CotInteractable cot, out TerminalInteractable terminal, out Text consoleScreen,
            out Transform wake, out Transform stand)
        {
            // Cot (along z, against the west wall) with a mattress and a pillow.
            var cotBody = Box(rProps, "Cot", new Vector3(-28.4f, 0.28f, 11f), new Vector3(1.0f, 0.5f, 2.2f), mBeige);
            Box(cotBody.transform, "Mattress", new Vector3(-28.4f, 0.57f, 11f), new Vector3(0.92f, 0.12f, 2.1f), mNavy, false);
            Box(cotBody.transform, "Pillow", new Vector3(-28.4f, 0.68f, 11.8f), new Vector3(0.6f, 0.1f, 0.4f), mCream, false);
            cot = cotBody.AddComponent<CotInteractable>();

            // Console desk on the north wall, with a monitor showing the code entry text.
            Box(rProps, "ConsoleDesk", new Vector3(-25f, 0.5f, 13.4f), new Vector3(2.2f, 1.0f, 0.9f), mBeige);
            Box(rProps, "ConsoleMonitor", new Vector3(-25f, 1.45f, 13.55f), new Vector3(1.3f, 0.8f, 0.12f), mNavy);
            Box(rProps, "ConsoleScreenGlow", new Vector3(-25f, 1.45f, 13.47f), new Vector3(1.15f, 0.65f, 0.02f), mScreen, false);
            consoleScreen = Sign(rProps, "ConsoleText", "ENTER CODE\n> ", new Vector3(-25.5f, 1.65f, 13.44f), 0f, 0.045f, 36, Cream, TextAnchor.UpperLeft);

            // Complaint terminal on the east wall of the admin room, always brightly lit.
            Box(rProps, "TerminalDesk", new Vector3(-20.6f, 0.45f, 7f), new Vector3(0.7f, 0.9f, 1.5f), mBeige);
            var monitor = Box(rProps, "TerminalMonitor", new Vector3(-20.62f, 1.25f, 7f), new Vector3(0.14f, 0.75f, 1.0f), mNavy);
            Box(monitor.transform, "TerminalScreen", new Vector3(-20.7f, 1.25f, 7f), new Vector3(0.02f, 0.62f, 0.86f), mScreen, false);
            Sign(rProps, "TerminalText", "FEEDBACK\nPLEASE TELL US\nWHAT IS BOTHERING YOU", new Vector3(-20.715f, 1.45f, 7.38f), 90f, 0.04f, 32, Cream, TextAnchor.UpperLeft);
            terminal = monitor.AddComponent<TerminalInteractable>();

            // Signs and decor.
            Sign(rSigns, "SignInitiative", "THE SOLACE INITIATIVE", new Vector3(-27.3f, 2.55f, 13.88f), 0f, 0.07f, 48, Cream, TextAnchor.UpperLeft);
            Sign(rSigns, "SignSlogan", "Comfort Is Our Output", new Vector3(-27.0f, 2.1f, 13.88f), 0f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);
            Sign(rSigns, "SignAdminDoor", "ADMINISTRATION", new Vector3(-26.7f, 2.9f, 1.88f), 0f, 0.06f, 40, Cream, TextAnchor.UpperLeft);
            Box(rProps, "AdminRug", new Vector3(-25f, 0.01f, 8f), new Vector3(5f, 0.02f, 3f), mNavy, false);

            wake = new GameObject("WakePoint").transform;
            wake.SetParent(rSystems, false);
            wake.position = new Vector3(-26.6f, 0.05f, 10.6f);
            wake.rotation = Quaternion.Euler(0f, 180f, 0f);
            stand = new GameObject("ConsoleStandPoint").transform;
            stand.SetParent(rSystems, false);
            stand.position = new Vector3(-25f, 0.05f, 11.0f);
            stand.rotation = Quaternion.identity;
        }

        static void BuildBreakRoom()
        {
            Sign(rSigns, "SignBreak", "BREAK ROOM", new Vector3(-15.0f, 2.9f, 1.88f), 0f, 0.06f, 40, Cream, TextAnchor.UpperLeft);

            Box(rProps, "CoffeeCounter", new Vector3(-9.7f, 0.45f, 13.5f), new Vector3(2.4f, 0.9f, 0.8f), mBeige);
            var machine = Station("CoffeeMachine", "coffee_machine", new Vector3(-9.7f, 1.15f, 13.5f), new Vector3(0.6f, 0.55f, 0.45f), mNavy);
            Sign(rSigns, "SignCoffee", "COFFEE", new Vector3(-10.3f, 2.3f, 13.88f), 0f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);

            // Supply shelf (filters).
            Station("SupplyShelf", "filter_shelf", new Vector3(-19.6f, 1.0f, 11f), new Vector3(0.6f, 2.0f, 2.4f), mWood);
            Sign(rSigns, "SignSupplies", "SUPPLIES", new Vector3(-19.45f, 2.5f, 12.0f), 90f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);

            // Table, chairs, vending machine, plant.
            Box(rProps, "BreakTable", new Vector3(-14f, 0.4f, 8f), new Vector3(2.4f, 0.8f, 1.2f), mWood);
            Box(rProps, "BreakChair1", new Vector3(-15.4f, 0.25f, 8f), new Vector3(0.5f, 0.5f, 0.5f), mDark);
            Box(rProps, "BreakChair2", new Vector3(-12.6f, 0.25f, 8f), new Vector3(0.5f, 0.5f, 0.5f), mDark);
            Box(rProps, "VendingMachine", new Vector3(-13.4f, 0.95f, 13.5f), new Vector3(1.1f, 1.9f, 0.8f), mGreenBox());
            Plant(new Vector3(-18.7f, 0f, 3.2f));
        }

        static void BuildPrintRoom()
        {
            Sign(rSigns, "SignPrint", "PRINT ROOM", new Vector3(-1.8f, 2.9f, 1.88f), 0f, 0.06f, 40, Cream, TextAnchor.UpperLeft);
            Station("Printer", "printer", new Vector3(6.6f, 0.5f, 13.3f), new Vector3(1.5f, 1.0f, 0.9f), mBeige);
            Sign(rSigns, "SignPrinter", "PRINTER", new Vector3(6.0f, 2.3f, 13.88f), 0f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);
            Box(rProps, "PrintTable", new Vector3(0f, 0.4f, 8f), new Vector3(3f, 0.8f, 1.2f), mWood);
            Plant(new Vector3(-7f, 0f, 3.2f));
        }

        static void BuildOffice()
        {
            Sign(rSigns, "SignOffice", "OFFICE", new Vector3(13.4f, 2.9f, 1.88f), 0f, 0.06f, 40, Cream, TextAnchor.UpperLeft);
            Box(rProps, "ClipboardDesk", new Vector3(18.2f, 0.45f, 13.3f), new Vector3(2.2f, 0.9f, 0.9f), mWood);
            Station("Clipboard", "clipboard", new Vector3(18.2f, 0.98f, 13.2f), new Vector3(0.5f, 0.12f, 0.65f), mCream);
            Sign(rSigns, "SignClipboard", "LOG BOOK", new Vector3(17.6f, 2.3f, 13.88f), 0f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);
            for (int i = 0; i < 3; i++)
            {
                Box(rProps, "OfficeDesk" + i, new Vector3(11f + i * 3f, 0.4f, 7f), new Vector3(1.8f, 0.8f, 0.9f), mWood);
                Box(rProps, "OfficeChair" + i, new Vector3(11f + i * 3f, 0.25f, 6f), new Vector3(0.5f, 0.5f, 0.5f), mDark);
            }
            Plant(new Vector3(19f, 0f, 3.2f));
        }

        static void BuildUtilityAndCorridor()
        {
            Sign(rSigns, "SignUtility", "UTILITY", new Vector3(23.4f, 2.9f, 1.88f), 0f, 0.06f, 40, Cream, TextAnchor.UpperLeft);
            Box(rProps, "Boiler", new Vector3(24.5f, 1.1f, 6.5f), new Vector3(2f, 2.2f, 2f), mDark);
            Station("ShredderBin", "shredder", new Vector3(28.2f, 0.45f, 13.3f), new Vector3(0.8f, 0.9f, 0.7f), mDark);
            Sign(rSigns, "SignShredder", "SHREDDER", new Vector3(27.5f, 2.3f, 13.88f), 0f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);
            Box(rProps, "Crates", new Vector3(23f, 0.4f, 12.4f), new Vector3(1.6f, 0.8f, 1.2f), mWood);
            Box(rProps, "MopBucket", new Vector3(22.2f, 0.25f, 4f), new Vector3(0.5f, 0.5f, 0.5f), mDark);

            // The vent at the far east end of the corridor.
            var vent = Station("Vent", "vent", new Vector3(29.8f, 1.5f, 0f), new Vector3(0.2f, 1.1f, 1.5f), mDark);
            Sign(rSigns, "SignVent", "VENT 7", new Vector3(29.62f, 2.45f, 0.45f), 90f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);

            // Thermostat on the corridor's south wall, beside the east window.
            Station("Thermostat", "thermostat", new Vector3(8.7f, 1.4f, -1.88f), new Vector3(0.3f, 0.4f, 0.12f), mCream);
            Sign(rSigns, "SignThermostat", "TEMP", new Vector3(8.95f, 1.95f, -1.76f), 180f, 0.04f, 40, Yellow, TextAnchor.UpperLeft);

            // Corridor signs (viewed from the corridor).
            Sign(rSigns, "SignAuthorized", "AUTHORIZED PERSONNEL ONLY", new Vector3(-2.6f, 2.55f, -1.76f), 180f, 0.04f, 40, Cream, TextAnchor.UpperLeft);
            Sign(rSigns, "SignCorridorWelcome", "THE SOLACE INITIATIVE", new Vector3(-29.7f, 2.6f, 1.2f), -90f, 0.06f, 44, Cream, TextAnchor.UpperLeft);
            Sign(rSigns, "SignCorridorSlogan", "Always Within Tolerance", new Vector3(29.7f, 2.4f, -1.2f), 90f, 0.05f, 40, Yellow, TextAnchor.UpperLeft);
        }

        static void BuildServerHall()
        {
            // Rows of dark racks visible through the windows; the hall cannot be entered.
            int index = 0;
            foreach (float z in new[] { -5.2f, -8.4f, -11.6f })
            {
                for (float x = -28f; x <= 28f; x += 2f)
                {
                    var rack = Box(rHall, "Rack" + index, new Vector3(x, 1.1f, z), new Vector3(0.9f, 2.2f, 1.1f), mRack);
                    Box(rack.transform, "Strip", new Vector3(x, 1.5f, z + 0.56f), new Vector3(0.6f, 0.08f, 0.02f), mRackLight, false);
                    index++;
                }
            }
        }

        static void BuildLights(out Light sun)
        {
            var sunGo = new GameObject("Sun");
            sunGo.transform.SetParent(rLights, false);
            sunGo.transform.rotation = Quaternion.Euler(60f, -30f, 0f);
            sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.96f, 0.9f);
            sun.intensity = 1.1f;
            sun.shadows = LightShadows.Soft;

            // Office lights (dimmed in the quiet time): one per room and three in the corridor.
            foreach (var p in new[]
            {
                new Vector3(-25f, 2.9f, 8f), new Vector3(-14f, 2.9f, 8f), new Vector3(0f, 2.9f, 8f), new Vector3(14f, 2.9f, 8f), new Vector3(25f, 2.9f, 8f),
                new Vector3(-18f, 2.9f, 0f), new Vector3(0f, 2.9f, 0f), new Vector3(18f, 2.9f, 0f),
            })
            {
                officeLights.Add(PointLight("OfficeLight", p, new Color(1f, 0.97f, 0.92f), 1.6f, 11f));
            }

            // Mood lights that stay on: purple in the server hall and by the locked door, a bright lamp at the terminal.
            PointLight("HallPurpleWest", new Vector3(-14f, 2.6f, -7f), Purple, 5f, 16f);
            PointLight("HallPurpleEast", new Vector3(14f, 2.6f, -7f), Purple, 5f, 16f);
            PointLight("DoorGlow", new Vector3(-4f, 1.8f, -1.2f), Purple, 2.5f, 5f);
            PointLight("TerminalLamp", new Vector3(-21.8f, 2.2f, 7f), new Color(0.85f, 1f, 0.97f), 4.5f, 7f);
        }

        // ---------- player, pull rig, UI ----------

        static GameObject BuildPlayer(out PlayerMotor motor, out PlayerLook look, out Interactor interactor, out Camera cam)
        {
            // Add an Interactor to the prefab if it lacks one, and give its camera a dark solid background.
            var contents = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
            try
            {
                var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
                var camera = contents.GetComponentInChildren<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.05f, 0.07f, 0.08f);
                camera.farClipPlane = 120f;
                var it = contents.GetComponent<Interactor>();
                if (it == null) it = contents.AddComponent<Interactor>();
                Set(it, "actions", actions);
                Set(it, "playerCamera", camera);
                PrefabUtility.SaveAsPrefabAsset(contents, PlayerPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            var player = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());
            player.transform.position = new Vector3(-26.6f, 0.05f, 10.6f);
            player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            motor = player.GetComponent<PlayerMotor>();
            look = player.GetComponent<PlayerLook>();
            interactor = player.GetComponent<Interactor>();
            cam = player.GetComponentInChildren<Camera>();
            return player;
        }

        static GameObject BuildPullRig(Text consoleScreen, out PullSequence pull)
        {
            var go = new GameObject("PullRig");
            go.transform.SetParent(rSystems, false);
            pull = go.AddComponent<PullSequence>();

            var camGo = new GameObject("PullCamera");
            camGo.transform.SetParent(go.transform, false);
            camGo.transform.position = new Vector3(-21.2f, 2.8f, 3.4f);
            camGo.transform.rotation = Quaternion.LookRotation(new Vector3(-26.5f, 1.1f, 11f) - camGo.transform.position);
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 62f;
            cam.nearClipPlane = 0.05f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.07f, 0.08f);
            cam.depth = 10f;
            cam.enabled = false;

            // The avatar: a capsule body, a head, and a lanyard.
            var avatar = new GameObject("Avatar");
            avatar.transform.SetParent(go.transform, false);
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(avatar.transform, false);
            body.transform.localPosition = new Vector3(0, 0.9f, 0);
            body.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
            body.GetComponent<Renderer>().sharedMaterial = mNavy;
            Object.DestroyImmediate(body.GetComponent<Collider>());
            var lanyard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lanyard.name = "Lanyard";
            lanyard.transform.SetParent(avatar.transform, false);
            lanyard.transform.localPosition = new Vector3(0, 1.2f, 0.28f);
            lanyard.transform.localScale = new Vector3(0.1f, 0.55f, 0.03f);
            lanyard.GetComponent<Renderer>().sharedMaterial = mLanyardGrey;
            Object.DestroyImmediate(lanyard.GetComponent<Collider>());
            avatar.SetActive(false);

            var start = new GameObject("AvatarStart").transform;
            start.SetParent(go.transform, false);
            start.position = new Vector3(-23.4f, 0.05f, 3.2f);
            var end = new GameObject("AvatarEnd").transform;
            end.SetParent(go.transform, false);
            end.position = new Vector3(-23.4f, 0.05f, 12.0f);

            Set(pull, "pullCamera", cam);
            Set(pull, "avatar", avatar.transform);
            Set(pull, "lanyardRenderer", lanyard.GetComponent<Renderer>());
            Set(pull, "lanyardGrey", mLanyardGrey);
            Set(pull, "lanyardBlue", mLanyardBlue);
            Set(pull, "avatarStart", start);
            Set(pull, "avatarEnd", end);
            Set(pull, "consoleScreen", consoleScreen);
            return go;
        }

        static Canvas BuildCanvas(out Hud hud, out CardScreen cards, out ComplaintScreen complaint, out ShopScreen shop)
        {
            var uiRoot = new GameObject("UI");
            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            canvasGo.transform.SetParent(uiRoot.transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            hud = NewScreen<Hud>(canvasGo.transform, "HudComponent");
            hud.Build(canvasGo.transform);
            cards = NewScreen<CardScreen>(canvasGo.transform, "CardScreenComponent");
            cards.Build(canvasGo.transform);
            complaint = NewScreen<ComplaintScreen>(canvasGo.transform, "ComplaintScreenComponent");
            complaint.Build(canvasGo.transform);
            shop = NewScreen<ShopScreen>(canvasGo.transform, "ShopScreenComponent");
            shop.Build(canvasGo.transform);

            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule), typeof(UiInputBootstrap));
            es.transform.SetParent(uiRoot.transform, false);
            return canvas;
        }

        static T NewScreen<T>(Transform parent, string name) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.AddComponent<T>();
        }

        // ---------- helpers ----------

        static Transform Root(string name)
        {
            var go = new GameObject(name);
            return go.transform;
        }

        static GameObject Box(Transform parent, string name, Vector3 center, Vector3 size, Material mat, bool collider = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, true);
            go.transform.position = center;
            SetWorldScale(go.transform, size);
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        // Sets a size in world units even when the parent is scaled (lossy scale is the parent's, so divide it out).
        static void SetWorldScale(Transform t, Vector3 world)
        {
            var p = t.parent != null ? t.parent.lossyScale : Vector3.one;
            t.localScale = new Vector3(world.x / p.x, world.y / p.y, world.z / p.z);
        }

        // A task station: a box with a TaskStation component, a coin origin above it, and a tall marker beam.
        static GameObject Station(string name, string stationId, Vector3 center, Vector3 size, Material mat)
        {
            var go = Box(rStations, name, center, size, mat);
            var ts = go.AddComponent<TaskStation>();
            var coinOrigin = new GameObject("CoinOrigin");
            coinOrigin.transform.SetParent(go.transform, true);
            coinOrigin.transform.position = center + new Vector3(0, size.y / 2f + 0.5f, 0);

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = "Marker";
            beam.transform.SetParent(go.transform, true);
            beam.transform.position = new Vector3(center.x, 3.4f, center.z);
            SetWorldScale(beam.transform, new Vector3(0.18f, 3.4f, 0.18f));
            beam.GetComponent<Renderer>().sharedMaterial = mMarker;
            beam.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            Object.DestroyImmediate(beam.GetComponent<Collider>());
            beam.SetActive(false);

            Set(ts, "stationId", stationId);
            Set(ts, "coinOrigin", coinOrigin.transform);
            Set(ts, "highlight", go.GetComponent<Renderer>());
            Set(ts, "normalMaterial", mat);
            Set(ts, "activeMaterial", mActive);
            Set(ts, "marker", beam);
            stations.Add(ts);
            return go;
        }

        static Material mGreenBox() => mVending;

        static void Plant(Vector3 basePos)
        {
            Box(rProps, "PlantPot", basePos + new Vector3(0, 0.25f, 0), new Vector3(0.5f, 0.5f, 0.5f), mWood);
            var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.name = "PlantLeaves";
            leaves.transform.SetParent(rProps, true);
            leaves.transform.position = basePos + new Vector3(0, 0.9f, 0);
            leaves.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            leaves.GetComponent<Renderer>().sharedMaterial = mPlant;
            Object.DestroyImmediate(leaves.GetComponent<Collider>());
        }

        static Light PointLight(string name, Vector3 pos, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(rLights, false);
            go.transform.position = pos;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.None;
            return l;
        }

        // A world-space sign: a World Space canvas with one Text, so it is depth-tested and the font atlas works.
        // The position is the text's upper-left corner. yaw 0 reads from the south, 180 from the north,
        // +90 from the west (for walls facing west) and -90 from the east.
        // characterSize and fontSize are kept from the earlier TextMesh version; text height in metres is about characterSize * fontSize * 0.09.
        static Text Sign(Transform parent, string name, string text, Vector3 pos, float yaw, float characterSize, int fontSize,
            Color color, TextAnchor anchor)
        {
            float meters = characterSize * fontSize * 0.09f;
            int lines = text.Split('\n').Length;
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, true);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            go.transform.localScale = Vector3.one * 0.001f;
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rt = (RectTransform)go.transform;
            rt.pivot = new Vector2(0f, 1f);
            rt.sizeDelta = new Vector2(7000f, meters * 1000f * 1.35f * lines);

            var t = UiKit.Label(go.transform, "Text", text, Mathf.RoundToInt(meters * 1000f), color, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        // ---------- materials ----------

        static void MakeMaterials()
        {
            if (!AssetDatabase.IsValidFolder(MatFolder)) AssetDatabase.CreateFolder("Assets", "Materials");
            mWall = Mat("Noted_Wall", Wall);
            mFloor = Mat("Noted_Floor", Floor);
            mCeiling = Mat("Noted_Ceiling", Ceiling);
            mCream = Mat("Noted_Cream", Cream);
            mBeige = Mat("Noted_Beige", Beige);
            mDark = Mat("Noted_DarkTeal", DarkTeal);
            mNavy = Mat("Noted_Navy", Navy);
            mDoor = Mat("Noted_Door", C(0x2C, 0x34, 0x38));
            mRack = Mat("Noted_Rack", C(0x14, 0x19, 0x1E));
            mRackLight = Mat("Noted_RackLight", C(0x22, 0x66, 0x55), C(0x1E, 0x90, 0x70));
            mPurpleGlow = Mat("Noted_PurpleGlow", Purple, new Color(0.55f, 0.25f, 0.75f));
            mScreen = Mat("Noted_Screen", GreenTeal, new Color(0.15f, 0.45f, 0.42f));
            mActive = Mat("Noted_StationActive", Yellow, new Color(0.55f, 0.42f, 0.12f));
            mCoin = Mat("Noted_Coin", Yellow, new Color(0.5f, 0.38f, 0.08f));
            mLanyardGrey = Mat("Noted_LanyardGrey", C(0x8A, 0x8F, 0x94));
            mLanyardBlue = Mat("Noted_LanyardBlue", C(0x2F, 0x5F, 0xA8));
            mPlant = Mat("Noted_Plant", C(0x3E, 0x6B, 0x3A));
            mWood = Mat("Noted_Wood", C(0x7A, 0x5E, 0x46));
            mMarker = Mat("Noted_Marker", Yellow, new Color(0.9f, 0.7f, 0.2f));
            mRed = Mat("Noted_RedLight", C(0x90, 0x20, 0x20), new Color(0.9f, 0.1f, 0.1f));
            mVending = Mat("Noted_Vending", C(0x2D, 0x5E, 0x5B));
        }

        static Material Mat(string name, Color color, Color? emission = null)
        {
            string path = MatFolder + "/" + name + ".mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            {
                m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(m, path);
            }
            m.SetColor("_BaseColor", color);
            m.SetFloat("_Smoothness", 0.12f);
            m.SetFloat("_Metallic", 0f);
            if (emission.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", emission.Value);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                m.DisableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", Color.black);
            }
            EditorUtility.SetDirty(m);
            return m;
        }

        // ---------- serialized wiring ----------

        static void Set(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            if (p == null) throw new System.Exception("No serialized field '" + field + "' on " + target.GetType().Name);
            p.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Set(Object target, string field, string value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            if (p == null) throw new System.Exception("No serialized field '" + field + "' on " + target.GetType().Name);
            p.stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetArray(Object target, string field, Object[] values)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            if (p == null) throw new System.Exception("No serialized field '" + field + "' on " + target.GetType().Name);
            p.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
