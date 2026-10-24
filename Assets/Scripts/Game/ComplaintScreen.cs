using System;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>The feedback form: a handful of preset complaints, each answered with a canned non-answer. No free text.</summary>
    public class ComplaintScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text replyText;
        [SerializeField] Button[] presetButtons;
        [SerializeField] Button closeButton;

        Action onSubmit;
        Action onClose;
        float shownAt;

        public bool IsOpen => root != null && root.activeSelf;

        public void Build(Transform canvasRoot)
        {
            var dim = UiKit.Panel(canvasRoot, "ComplaintScreen", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.94f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root = dim.gameObject;
            var card = UiKit.Panel(dim.transform, "Card", UiKit.DarkTeal,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-620, -440), new Vector2(620, 440));
            UiKit.Label(card.transform, "Title", "FEEDBACK FORM", 44, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -100), new Vector2(-50, -30));
            UiKit.Label(card.transform, "Prompt", "Please tell us what is bothering you:", 28, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -150), new Vector2(-50, -104));

            presetButtons = new Button[ComplaintBank.Presets.Length];
            for (int i = 0; i < presetButtons.Length; i++)
            {
                float top = -170f - i * 52f;
                presetButtons[i] = UiKit.MakeButton(card.transform, "Preset" + i, ComplaintBank.Presets[i], 26, UiKit.Navy, UiKit.Cream,
                    new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, top - 46), new Vector2(-50, top));
                UiKit.ButtonText(presetButtons[i]).alignment = TextAnchor.MiddleLeft;
            }

            replyText = UiKit.Label(card.transform, "Reply", "", 28, UiKit.Yellow, TextAnchor.UpperLeft,
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(50, 120), new Vector2(-50, 260));
            closeButton = UiKit.MakeButton(card.transform, "Close", "Close", 30, UiKit.Green, UiKit.Cream,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-150, 30), new Vector2(150, 94));
            root.SetActive(false);
        }

        void Awake()
        {
            for (int i = 0; i < presetButtons.Length; i++)
            {
                int index = i;
                presetButtons[i].onClick.AddListener(() => Choose(index));
            }
            closeButton.onClick.AddListener(Close);
            if (root != null) root.SetActive(false);
        }

        /// <param name="reply">Called with the index of the chosen preset; returns the reply to show.</param>
        Func<int, string> replyFor;

        public void Show(Func<int, string> getReply, Action submitted, Action closed)
        {
            replyFor = getReply;
            onSubmit = submitted;
            onClose = closed;
            replyText.text = "";
            shownAt = Time.unscaledTime;
            root.SetActive(true);
        }

        void Choose(int index)
        {
            if (!IsOpen) return;
            replyText.text = replyFor(index);
            onSubmit?.Invoke();
        }

        public void Close()
        {
            if (!IsOpen) return;
            root.SetActive(false);
            var action = onClose;
            onClose = null;
            action?.Invoke();
        }

        void Update()
        {
            if (!IsOpen || Time.unscaledTime - shownAt < 0.3f) return;
            var k = Keyboard.current;
            if (k != null && k.escapeKey.wasPressedThisFrame) Close();
        }
    }
}
