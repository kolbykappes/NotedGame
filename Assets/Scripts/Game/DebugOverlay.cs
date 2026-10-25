using System.Text;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// The feel-test controls. F1 shows or hides the readout; F2 to F6 work whether or not it is showing.
    /// F2 replay this day, F3 skip to the next day, F4 grant tools and coins, F5 end the day now, F6 jump to the next level
    /// (hold Shift for the previous one). Keys are read straight from the keyboard, so no input binding is touched.
    /// Built in code at start-up and only created in the Editor and development builds.
    /// </summary>
    public class DebugOverlay : MonoBehaviour
    {
        GameFlow flow;
        GameObject root;
        Text text;
        bool visible;
        readonly StringBuilder sb = new StringBuilder();

        public static DebugOverlay Create(GameFlow owner)
        {
            var go = new GameObject("DebugOverlay");
            var overlay = go.AddComponent<DebugOverlay>();
            overlay.flow = owner;
            overlay.Build();
            return overlay;
        }

        void Build()
        {
            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = UiKit.Panel(canvasGo.transform, "Panel", new Color(0f, 0f, 0f, 0.72f),
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(16, 16), new Vector2(640, 420));
            root = panel.gameObject;
            text = UiKit.Label(panel.transform, "Text", "", 20, UiKit.Cream, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, new Vector2(14, 70), new Vector2(-14, -10));

            // Mouse buttons (press Esc to free the cursor first).
            AddButton(panel.transform, "Wellness tape", 0, () => flow.DebugShowWellness());
            AddButton(panel.transform, "Offer errand", 1, () => flow.DebugOfferNextErrand());
            AddButton(panel.transform, "Show scrap", 2, () => flow.DebugShowNextScrap());
            AddButton(panel.transform, "Morning slip", 3, () => flow.DebugShowLedger());
            root.SetActive(false);
        }

        static void AddButton(Transform parent, string label, int index, System.Action onClick)
        {
            float left = 14f + index * 152f;
            var button = UiKit.MakeButton(parent, label, label, 17, UiKit.Navy, UiKit.Cream,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(left, 14), new Vector2(left + 144f, 54));
            button.onClick.AddListener(() => onClick());
        }

        void Update()
        {
            var k = Keyboard.current;
            if (k != null && flow != null)
            {
                if (k.f1Key.wasPressedThisFrame)
                {
                    visible = !visible;
                    root.SetActive(visible);
                }
                if (k.f2Key.wasPressedThisFrame) flow.DebugReplayDay();
                if (k.f3Key.wasPressedThisFrame) flow.DebugSkipDay();
                if (k.f4Key.wasPressedThisFrame) flow.DebugGrantTools();
                if (k.f5Key.wasPressedThisFrame) flow.DebugExpireClock();
                if (k.f6Key.wasPressedThisFrame)
                {
                    bool shift = k.leftShiftKey.isPressed || k.rightShiftKey.isPressed;
                    flow.DebugJumpLevel(shift ? -1 : 1);
                }
            }

            if (visible && flow != null && flow.Session != null) text.text = Describe(flow, sb);
        }

        static string Describe(GameFlow flow, StringBuilder into)
        {
            var s = flow.Session;
            into.Clear();
            into.Append("<b>FEEL TEST</b>   F1 hide\n");
            into.Append("Level ").Append(s.CurrentRank + 1).Append(' ').Append(s.CurrentLevel.Title)
                .Append("   Day ").Append(s.Day).Append("   reveal stage ").Append(s.RevealStage).Append('\n');
            if (s.Clock != null)
            {
                into.Append("Clock ").Append(s.Clock.Remaining.ToString("0.0")).Append(" / ").Append(s.Clock.Total.ToString("0"))
                    .Append("   phase ").Append(flow.Phase).Append('\n');
            }
            int done = 0;
            foreach (var t in s.TodaysTasks) if (t.IsFinished) done++;
            into.Append("Official ").Append(done).Append('/').Append(s.TodaysTasks.Count)
                .Append("   finished at ").Append(s.OfficialFinishedAt < 0f ? "--" : s.OfficialFinishedAt.ToString("0.0")).Append('\n');
            into.Append("Reliability ").Append(s.Reliability.Value.ToString("0")).Append("   Rollout ").Append(s.Rollout.Value.ToString("0.0"))
                .Append(" (floor ").Append(s.Rollout.Floor.ToString("0")).Append(", locks ").Append(s.Rollout.Locks).Append(")\n");
            into.Append("Second list: ");
            if (s.TodaysErrands.Count == 0) into.Append("none");
            foreach (var e in s.TodaysErrands)
            {
                string state = e.IsDone ? " [done]" : e.StepIndex == 0 ? " [not started]" : " [step " + (e.StepIndex + 1) + " of " + e.Definition.Steps.Count + "]";
                into.Append(e.Definition.Name).Append(state).Append("; ");
            }
            into.Append("\nScraps found ").Append(s.ScrapsFound.Count).Append("/").Append(ScrapBank.All.Length).Append('\n');
            into.Append("F2 replay day   F3 skip day   F4 tools   F5 end day   F6 level + (Shift -)\n");
            into.Append("Log: ").Append(DayLogWriter.Path);
            return into.ToString();
        }
    }
}
