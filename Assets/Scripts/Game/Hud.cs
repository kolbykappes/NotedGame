using System.Collections.Generic;
using System.Text;
using Noted.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>The always-on display: day clock, Uptime, rank, task list, interaction prompt, carried item and toasts.</summary>
    public class Hud : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text timerText;
        [SerializeField] Text uptimeText;
        [SerializeField] Text rankText;
        [SerializeField] Text tasksText;
        [SerializeField] RectTransform tasksBack;
        [SerializeField] Text promptText;
        [SerializeField] Text carryText;
        [SerializeField] Text toolsText;
        [SerializeField] RectTransform toolsBack;
        [SerializeField] Text toastText;
        [SerializeField] Image progressFill;
        [SerializeField] GameObject progressRoot;

        float toastUntil;
        readonly StringBuilder sb = new StringBuilder();

        public void Build(Transform canvasRoot)
        {
            UiKit.ClearChildren(transform);
            var r = UiKit.Rect(canvasRoot, "Hud", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root = r.gameObject;

            // Crosshair
            UiKit.Panel(r, "Crosshair", UiKit.Cream, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-3, -3), new Vector2(3, 3));

            // Clock (top centre)
            UiKit.Panel(r, "ClockBack", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.7f),
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-110, -92), new Vector2(110, -16));
            timerText = UiKit.Label(r, "Timer", "1:00", 56, UiKit.Cream, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-110, -92), new Vector2(110, -16));

            // Status (top right)
            UiKit.Panel(r, "StatusBack", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.7f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -100), new Vector2(-16, -16));
            uptimeText = UiKit.Label(r, "Uptime", "Uptime 0", 30, UiKit.Yellow, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -52), new Vector2(-28, -22));
            rankText = UiKit.Label(r, "Rank", "Trainee", 22, UiKit.Cream, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -92), new Vector2(-28, -56));

            // Tools owned (under the status panel)
            toolsBack = UiKit.Panel(r, "ToolsBack", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.7f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -214), new Vector2(-16, -108)).rectTransform;
            toolsText = UiKit.Label(r, "Tools", "TOOLS: none yet", 20, UiKit.Cream, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-322, -208), new Vector2(-24, -114));

            // Task list (top left)
            tasksBack = UiKit.Panel(r, "TasksBack", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.7f),
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16, -280), new Vector2(560, -16)).rectTransform;
            tasksText = UiKit.Label(r, "Tasks", "", 24, UiKit.Cream, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30, -270), new Vector2(548, -26));

            // Prompt and progress (bottom centre)
            promptText = UiKit.Label(r, "Prompt", "", 30, UiKit.Cream, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-500, 150), new Vector2(500, 200));
            var barBack = UiKit.Panel(r, "ProgressBack", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.85f),
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180, 124), new Vector2(180, 144));
            progressRoot = barBack.gameObject;
            // The fill is a plain rect whose right anchor grows from 0 to 1 (a null-sprite Image cannot use fillAmount).
            progressFill = UiKit.Panel(barBack.transform, "Fill", UiKit.Green, Vector2.zero, new Vector2(0f, 1f), new Vector2(3, 3), new Vector2(0, -3));
            progressRoot.SetActive(false);

            carryText = UiKit.Label(r, "Carry", "", 24, UiKit.Yellow, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-300, 90), new Vector2(300, 122));
            UiKit.AddOutline(carryText);
            UiKit.AddOutline(promptText);

            // Toast (upper middle)
            toastText = UiKit.Label(r, "Toast", "", 34, UiKit.Cream, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-600, -190), new Vector2(600, -110));
            UiKit.AddOutline(toastText);
        }

        public void SetVisible(bool visible)
        {
            if (root != null) root.SetActive(visible);
        }

        public void SetTimer(float seconds)
        {
            int s = Mathf.CeilToInt(Mathf.Max(0f, seconds));
            timerText.text = (s / 60) + ":" + (s % 60).ToString("00");
            timerText.color = seconds <= 10f ? UiKit.Yellow : UiKit.Cream;
        }

        public void SetStatus(int uptime, string rank, string progress)
        {
            uptimeText.text = "Uptime " + uptime;
            rankText.text = rank + (string.IsNullOrEmpty(progress) ? "" : "   " + progress);
        }

        public void SetTasks(IReadOnlyList<TaskInstance> tasks)
        {
            sb.Clear();
            sb.Append("<b>TODAY'S TASKS</b>\n");
            if (tasks != null)
            {
                foreach (var t in tasks)
                {
                    string mark;
                    string tail = "";
                    switch (t.Status)
                    {
                        case TaskStatus.Done: mark = "[x]"; break;
                        case TaskStatus.Automated: mark = "[auto]"; break;
                        default:
                            mark = "[ ]";
                            if (t.Definition.Steps.Count > 1) tail = "  (" + (t.StepIndex + 1) + "/" + t.Definition.Steps.Count + ")";
                            break;
                    }
                    string line = mark + " " + t.Definition.Name + tail;
                    if (t.IsFinished) sb.Append("<color=#9AA39F>").Append(line).Append("</color>\n");
                    else sb.Append(line).Append('\n');
                }
            }
            tasksText.text = sb.ToString();

            // Size the panel to the list: a heading plus one line per task.
            int lines = 1 + (tasks != null ? tasks.Count : 0);
            if (tasksBack != null) tasksBack.offsetMin = new Vector2(16f, -(34f + lines * 32f));
        }

        public void SetPrompt(bool show, string prompt, float progress01)
        {
            if (!show)
            {
                promptText.text = "";
                progressRoot.SetActive(false);
                return;
            }
            promptText.text = prompt;
            bool hold = progress01 > 0f;
            progressRoot.SetActive(hold);
            if (hold)
            {
                var rt = (RectTransform)progressFill.transform;
                rt.anchorMax = new Vector2(Mathf.Clamp01(progress01), 1f);
                rt.offsetMax = new Vector2(-3f, -3f);
            }
        }

        public void SetTools(string text)
        {
            toolsText.text = text;
            Canvas.ForceUpdateCanvases();
            if (toolsBack != null) toolsBack.offsetMin = new Vector2(-330f, -(108f + Mathf.Clamp(toolsText.preferredHeight + 18f, 36f, 200f)));
        }

        public void SetCarrying(string text)
        {
            carryText.text = string.IsNullOrEmpty(text) ? "" : text;
        }

        public void Toast(string text, float seconds)
        {
            toastText.text = text;
            toastUntil = Time.unscaledTime + seconds;
        }

        void Update()
        {
            if (toastText != null && toastText.text.Length > 0 && Time.unscaledTime > toastUntil)
            {
                toastText.text = "";
            }
        }
    }
}
