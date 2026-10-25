using System.Text;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// Today's second list as a scrap of paper. It is not on the HUD: hold Tab during the day to read it. Built in code at start-up,
    /// so it needs nothing in the scene.
    /// </summary>
    public class SecondListPanel : MonoBehaviour
    {
        GameFlow flow;
        GameObject root;
        Text body;
        readonly StringBuilder sb = new StringBuilder();

        public static SecondListPanel Create(GameFlow owner)
        {
            var go = new GameObject("SecondListPanel");
            var panel = go.AddComponent<SecondListPanel>();
            panel.flow = owner;
            panel.Build();
            return panel;
        }

        /// <summary>The text of the second list as it stands now (shared with the morning card).</summary>
        public static string Describe(GameSession session, StringBuilder into)
        {
            into.Clear();
            if (session == null || session.TodaysErrands.Count == 0) return "Nothing on the list today.";
            int n = 1;
            foreach (var e in session.TodaysErrands)
            {
                string mark = e.IsDone ? "[x] " : "[ ] ";
                // Show the step only once the errand is under way.
                string progress = e.IsDone || e.StepIndex == 0 ? "" : "  (step " + (e.StepIndex + 1) + " of " + e.Definition.Steps.Count + ")";
                into.Append(n++).Append(". ").Append(mark).Append(e.Definition.Name).Append(progress).Append('\n');
                into.Append("   ").Append(e.Definition.Briefing.Replace("\n", "\n   ")).Append("\n\n");
            }
            return into.ToString().TrimEnd();
        }

        void Build()
        {
            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 300;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var paper = UiKit.Panel(canvasGo.transform, "Paper", UiKit.Cream,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-380, -300), new Vector2(380, 300));
            root = paper.gameObject;
            UiKit.Label(paper.transform, "Title", "SECOND LIST", 36, UiKit.DarkTeal, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(36, -80), new Vector2(-36, -24));
            body = UiKit.Label(paper.transform, "Body", "", 26, UiKit.Navy, TextAnchor.UpperLeft,
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(36, 50), new Vector2(-36, -90));
            UiKit.Label(paper.transform, "Hint", "Hold Tab to read. Press F at the stations it names.", 20,
                new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.7f), TextAnchor.MiddleCenter,
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(20, 12), new Vector2(-20, 44));
            root.SetActive(false);
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            bool show = flow != null && flow.IsDay && flow.Session != null && keyboard != null && keyboard.tabKey.isPressed;
            if (show != root.activeSelf) root.SetActive(show);
            if (show) body.text = Describe(flow.Session, sb);
        }
    }
}
