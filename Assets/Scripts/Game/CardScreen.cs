using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>A full-screen text card: the intro, how-to cards and training tapes. Space or Enter continues.</summary>
    public class CardScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text titleText;
        [SerializeField] Text bodyText;
        [SerializeField] Text buttonText;
        [SerializeField] Button button;
        [SerializeField] RectTransform cardRect;

        Action pending;
        float shownAt;

        public bool IsOpen => root != null && root.activeSelf;

        public void Build(Transform canvasRoot)
        {
            var dim = UiKit.Panel(canvasRoot, "CardScreen", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.94f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root = dim.gameObject;

            var card = UiKit.Panel(dim.transform, "Card", UiKit.DarkTeal,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-620, -440), new Vector2(620, 440));
            cardRect = card.rectTransform;
            titleText = UiKit.Label(card.transform, "Title", "TITLE", 44, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -110), new Vector2(-50, -30));
            bodyText = UiKit.Label(card.transform, "Body", "Body", 27, UiKit.Cream, TextAnchor.UpperLeft,
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(50, 130), new Vector2(-50, -120));
            button = UiKit.MakeButton(card.transform, "Continue", "Continue", 30, UiKit.Green, UiKit.Cream,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180, 40), new Vector2(180, 104));
            buttonText = UiKit.ButtonText(button);
            UiKit.Label(card.transform, "Hint", "Space or Enter", 20, new Color(UiKit.Cream.r, UiKit.Cream.g, UiKit.Cream.b, 0.6f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180, 10), new Vector2(180, 38));
            root.SetActive(false);
        }

        void Awake()
        {
            if (button != null) button.onClick.AddListener(Continue);
            if (root != null) root.SetActive(false);
        }

        public void Show(string title, string body, string label, Action onContinue)
        {
            titleText.text = title;
            bodyText.text = body;
            buttonText.text = label;
            pending = onContinue;
            shownAt = Time.unscaledTime;
            root.SetActive(true);

            // Fit the card to its text (the title, the button and the padding take about 330 px).
            if (cardRect != null)
            {
                Canvas.ForceUpdateCanvases();
                float height = Mathf.Clamp(bodyText.preferredHeight + 330f, 420f, 880f);
                cardRect.offsetMin = new Vector2(-620f, -height / 2f);
                cardRect.offsetMax = new Vector2(620f, height / 2f);
            }
        }

        public void Hide()
        {
            pending = null;
            if (root != null) root.SetActive(false);
        }

        public void Continue()
        {
            if (!IsOpen) return;
            var action = pending;
            pending = null;
            root.SetActive(false);
            action?.Invoke();
        }

        void Update()
        {
            if (!IsOpen || Time.unscaledTime - shownAt < 0.3f) return;
            var k = Keyboard.current;
            if (k != null && (k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame)) Continue();
        }
    }
}
