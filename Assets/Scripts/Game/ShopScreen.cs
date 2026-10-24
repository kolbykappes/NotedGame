using System;
using System.Linq;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>The night shop at the cot: spend Uptime on upgrades, preview tomorrow with Morning Briefing, then wake up.</summary>
    public class ShopScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text headerText;
        [SerializeField] Text briefingText;
        [SerializeField] RectTransform list;
        [SerializeField] Button wakeButton;

        GameSession session;
        Action onWake;
        Action onPurchased;
        float shownAt;

        public bool IsOpen => root != null && root.activeSelf;

        public void Build(Transform canvasRoot)
        {
            var dim = UiKit.Panel(canvasRoot, "ShopScreen", new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.94f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root = dim.gameObject;
            var card = UiKit.Panel(dim.transform, "Card", UiKit.DarkTeal,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-620, -480), new Vector2(620, 480));
            UiKit.Label(card.transform, "Title", "NIGHT SHOP", 44, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -90), new Vector2(-400, -30));
            headerText = UiKit.Label(card.transform, "Balance", "Uptime 0", 36, UiKit.Cream, TextAnchor.MiddleRight,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(400, -90), new Vector2(-50, -30));
            briefingText = UiKit.Label(card.transform, "Briefing", "", 24, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -130), new Vector2(-50, -94));
            list = UiKit.Rect(card.transform, "List", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -760), new Vector2(-50, -140));
            wakeButton = UiKit.MakeButton(card.transform, "Wake", "Wake Up", 32, UiKit.Green, UiKit.Cream,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-200, 30), new Vector2(200, 100));
            root.SetActive(false);
        }

        void Awake()
        {
            wakeButton.onClick.AddListener(Wake);
            if (root != null) root.SetActive(false);
        }

        public void Show(GameSession s, Action wake, Action purchased)
        {
            session = s;
            onWake = wake;
            onPurchased = purchased;
            shownAt = Time.unscaledTime;
            root.SetActive(true);
            Refresh();
        }

        void Refresh()
        {
            headerText.text = "Uptime " + session.Wallet.Balance;
            var mods = session.Mods;
            if (mods.BriefingEnabled)
            {
                var preview = session.PreviewNextDay();
                briefingText.text = "Tomorrow: " + string.Join(", ", preview.Select(t => t.Definition.Name + (t.Status == TaskStatus.Automated ? " (auto)" : "")));
            }
            else
            {
                briefingText.text = "";
            }

            UiKit.ClearChildren(list);
            float y = 0f;
            foreach (var def in session.Upgrades.All)
            {
                int level = session.Ledger.Level(def.Id);
                int cost = session.Ledger.CostFor(def);
                bool maxed = cost < 0;
                bool canBuy = !maxed && session.Wallet.Balance >= cost;
                string levelText = def.MaxLevel > 1 ? "  (" + level + "/" + def.MaxLevel + ")" : "";
                string costText = maxed ? "OWNED" : cost + " Uptime";
                string label = def.Name + levelText + "  —  " + def.Description + "   [" + costText + "]";
                var b = UiKit.MakeButton(list, "Row_" + def.Id, label, 24, canBuy ? UiKit.Navy : new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.55f),
                    canBuy ? UiKit.Cream : new Color(UiKit.Cream.r, UiKit.Cream.g, UiKit.Cream.b, 0.5f),
                    new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, y - 46), new Vector2(0, y));
                UiKit.ButtonText(b).alignment = TextAnchor.MiddleLeft;
                b.interactable = canBuy;
                var captured = def;
                b.onClick.AddListener(() => Buy(captured));
                y -= 52f;
            }
        }

        void Buy(UpgradeDefinition def)
        {
            if (session.Ledger.TryBuy(session.Wallet, def))
            {
                onPurchased?.Invoke();
                Refresh();
            }
        }

        void Wake()
        {
            if (!IsOpen) return;
            root.SetActive(false);
            var action = onWake;
            onWake = null;
            action?.Invoke();
        }

        void Update()
        {
            if (!IsOpen || Time.unscaledTime - shownAt < 0.3f) return;
            var k = Keyboard.current;
            if (k != null && k.enterKey.wasPressedThisFrame) Wake();
        }
    }
}
