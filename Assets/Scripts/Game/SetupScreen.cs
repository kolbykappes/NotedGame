using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// The setup menu shown first when the game starts: invert the mouse, mouse sensitivity and field of view.
    /// Choices are applied to the player straight away and remembered between runs (PlayerPrefs).
    /// </summary>
    public class SetupScreen : MonoBehaviour
    {
        public const string KeyInvertY = "noted.invertY";
        public const string KeyMouseSensitivity = "noted.mouseSensitivity";
        public const string KeyHorizontalFov = "noted.horizontalFov";

        [SerializeField] GameObject root;
        [SerializeField] Button invertButton;
        [SerializeField] Text invertText;
        [SerializeField] Button sensDown;
        [SerializeField] Button sensUp;
        [SerializeField] Text sensText;
        [SerializeField] Button fovDown;
        [SerializeField] Button fovUp;
        [SerializeField] Text fovText;
        [SerializeField] Button startButton;

        PlayerLook look;
        Action done;
        float shownAt;

        public bool IsOpen => root != null && root.activeSelf;

        public void Build(Transform canvasRoot)
        {
            var dim = UiKit.Panel(canvasRoot, "SetupScreen", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.97f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root = dim.gameObject;
            var card = UiKit.Panel(dim.transform, "Card", UiKit.DarkTeal,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-520, -330), new Vector2(520, 330));
            UiKit.Label(card.transform, "Title", "SETUP", 44, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -100), new Vector2(-50, -30));
            UiKit.Label(card.transform, "Subtitle", "Set these before your first shift. Your choices are remembered.", 24, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -150), new Vector2(-50, -104));

            // Row 1: invert mouse
            UiKit.Label(card.transform, "InvertLabel", "Invert mouse (up and down)", 30, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(50, -240), new Vector2(560, -180));
            invertButton = UiKit.MakeButton(card.transform, "InvertToggle", "OFF", 30, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -240), new Vector2(-50, -180));
            invertText = UiKit.ButtonText(invertButton);

            // Row 2: sensitivity
            UiKit.Label(card.transform, "SensLabel", "Mouse sensitivity", 30, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(50, -320), new Vector2(560, -260));
            sensDown = UiKit.MakeButton(card.transform, "SensDown", "-", 34, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -320), new Vector2(-270, -260));
            sensText = UiKit.Label(card.transform, "SensValue", "0.10", 30, UiKit.Yellow, TextAnchor.MiddleCenter,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-270, -320), new Vector2(-110, -260));
            sensUp = UiKit.MakeButton(card.transform, "SensUp", "+", 34, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-110, -320), new Vector2(-50, -260));

            // Row 3: field of view
            UiKit.Label(card.transform, "FovLabel", "Field of view (horizontal)", 30, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(50, -400), new Vector2(560, -340));
            fovDown = UiKit.MakeButton(card.transform, "FovDown", "-", 34, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-330, -400), new Vector2(-270, -340));
            fovText = UiKit.Label(card.transform, "FovValue", "100", 30, UiKit.Yellow, TextAnchor.MiddleCenter,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-270, -400), new Vector2(-110, -340));
            fovUp = UiKit.MakeButton(card.transform, "FovUp", "+", 34, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-110, -400), new Vector2(-50, -340));

            startButton = UiKit.MakeButton(card.transform, "Start", "Start", 32, UiKit.Green, UiKit.Cream,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-160, 40), new Vector2(160, 110));
            UiKit.Label(card.transform, "Hint", "I toggles invert. Enter starts.", 20, new Color(UiKit.Cream.r, UiKit.Cream.g, UiKit.Cream.b, 0.6f), TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-260, 10), new Vector2(260, 38));
            root.SetActive(false);
        }

        void Awake()
        {
            invertButton.onClick.AddListener(ToggleInvert);
            sensDown.onClick.AddListener(() => ChangeSensitivity(-0.02f));
            sensUp.onClick.AddListener(() => ChangeSensitivity(0.02f));
            fovDown.onClick.AddListener(() => ChangeFov(-5f));
            fovUp.onClick.AddListener(() => ChangeFov(5f));
            startButton.onClick.AddListener(Begin);
            if (root != null) root.SetActive(false);
        }

        public void Show(PlayerLook playerLook, Action finished)
        {
            look = playerLook;
            done = finished;

            // Remembered choices win over the prefab's defaults.
            look.InvertY = PlayerPrefs.GetInt(KeyInvertY, look.InvertY ? 1 : 0) != 0;
            look.MouseSensitivity = PlayerPrefs.GetFloat(KeyMouseSensitivity, look.MouseSensitivity);
            look.HorizontalFov = PlayerPrefs.GetFloat(KeyHorizontalFov, look.HorizontalFov);

            Refresh();
            shownAt = Time.unscaledTime;
            root.SetActive(true);
        }

        void Refresh()
        {
            invertText.text = look.InvertY ? "ON" : "OFF";
            sensText.text = look.MouseSensitivity.ToString("0.00");
            fovText.text = Mathf.RoundToInt(look.HorizontalFov).ToString();
        }

        void ToggleInvert()
        {
            look.InvertY = !look.InvertY;
            PlayerPrefs.SetInt(KeyInvertY, look.InvertY ? 1 : 0);
            PlayerPrefs.Save();
            Refresh();
        }

        void ChangeSensitivity(float delta)
        {
            look.MouseSensitivity = Mathf.Clamp(Mathf.Round((look.MouseSensitivity + delta) * 100f) / 100f, 0.04f, 0.4f);
            PlayerPrefs.SetFloat(KeyMouseSensitivity, look.MouseSensitivity);
            PlayerPrefs.Save();
            Refresh();
        }

        void ChangeFov(float delta)
        {
            look.HorizontalFov = Mathf.Clamp(look.HorizontalFov + delta, 70f, 120f);
            PlayerPrefs.SetFloat(KeyHorizontalFov, look.HorizontalFov);
            PlayerPrefs.Save();
            Refresh();
        }

        public void Begin()
        {
            if (!IsOpen) return;
            root.SetActive(false);
            var action = done;
            done = null;
            action?.Invoke();
        }

        void Update()
        {
            if (!IsOpen || Time.unscaledTime - shownAt < 0.3f) return;
            var k = Keyboard.current;
            if (k == null) return;
            if (k.iKey.wasPressedThisFrame) ToggleInvert();
            if (k.enterKey.wasPressedThisFrame) Begin();
        }
    }
}
