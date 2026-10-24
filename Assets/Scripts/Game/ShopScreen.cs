using System;
using System.Linq;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// The tool locker, opened at the cot each night: spend Uptime on tools that make the work easier, preview tomorrow with the Day Planner,
    /// see tools for later jobs (locked), then wake up. The list scrolls with the mouse wheel.
    /// </summary>
    public class ShopScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text headerText;
        [SerializeField] Text briefingText;
        [SerializeField] ScrollRect scroll;
        [SerializeField] RectTransform content;
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
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-620, -500), new Vector2(620, 500));
            UiKit.Label(card.transform, "Title", "TOOL LOCKER", 44, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -90), new Vector2(-400, -30));
            headerText = UiKit.Label(card.transform, "Balance", "Uptime 0", 36, UiKit.Cream, TextAnchor.MiddleRight,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(400, -90), new Vector2(-50, -30));
            briefingText = UiKit.Label(card.transform, "Briefing", "", 24, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(50, -130), new Vector2(-50, -94));

            // Scroll area: viewport with a rect mask, content with a vertical layout that sizes itself to its rows.
            var scrollRt = UiKit.Panel(card.transform, "Scroll", new Color(0f, 0f, 0f, 0.15f),
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(50, 130), new Vector2(-50, -140));
            var viewport = UiKit.Rect(scrollRt.transform, "Viewport", Vector2.zero, Vector2.one, new Vector2(6, 6), new Vector2(-6, -6));
            viewport.gameObject.AddComponent<RectMask2D>();
            content = UiKit.Rect(viewport, "Content", new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            content.pivot = new Vector2(0.5f, 1f);
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll = scrollRt.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40f;

            UiKit.Label(card.transform, "ScrollHint", "Scroll for more tools", 20, new Color(UiKit.Cream.r, UiKit.Cream.g, UiKit.Cream.b, 0.6f), TextAnchor.MiddleLeft,
                new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(50, 100), new Vector2(0, 128));
            wakeButton = UiKit.MakeButton(card.transform, "Wake", "Wake Up", 32, UiKit.Green, UiKit.Cream,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-200, 24), new Vector2(200, 94));
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
            Refresh(true);
        }

        void Refresh(bool resetScroll)
        {
            float keep = scroll.verticalNormalizedPosition;
            headerText.text = "Uptime " + session.Wallet.Balance;
            var mods = session.Mods;
            if (mods.BriefingEnabled)
            {
                var preview = session.PreviewNextDay();
                briefingText.text = "Tomorrow: " + string.Join(", ", preview.Select(t => t.Definition.Name + (t.Status == TaskStatus.Automated ? " (auto)" : "")));
            }
            else
            {
                briefingText.text = "Tools make the work easier. Buy what you need.";
            }

            UiKit.ClearChildren(content);
            string category = null;
            foreach (var def in session.Upgrades.All)
            {
                if (def.Category != category)
                {
                    category = def.Category;
                    AddHeader(category);
                }
                AddRow(def);
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            scroll.verticalNormalizedPosition = resetScroll ? 1f : keep;
        }

        void AddHeader(string text)
        {
            var t = UiKit.Label(content, "Header_" + text, text, 24, UiKit.Yellow, TextAnchor.MiddleLeft,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var le = t.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 38f;
            le.minHeight = 38f;
        }

        void AddRow(UpgradeDefinition def)
        {
            int rank = session.CurrentRank;
            int level = session.Ledger.Level(def.Id);
            int cost = session.Ledger.CostFor(def);
            bool maxed = cost < 0;
            bool unlocked = session.Ledger.IsUnlocked(def, rank);
            bool canBuy = unlocked && !maxed && session.Wallet.Balance >= cost;

            string levelText = def.MaxLevel > 1 ? "  (" + level + "/" + def.MaxLevel + ")" : "";
            string status;
            if (!def.Available) status = "Needed for later jobs";
            else if (!unlocked) status = "Requires " + session.Config.TitleFor(def.MinRank);
            else if (maxed) status = "OWNED";
            else status = cost + " Uptime";
            string label = def.Name + levelText + "  —  " + def.Description + "   [" + status + "]";

            var bg = canBuy ? UiKit.Navy : new Color(UiKit.Navy.r, UiKit.Navy.g, UiKit.Navy.b, 0.55f);
            var fg = canBuy ? UiKit.Cream : new Color(UiKit.Cream.r, UiKit.Cream.g, UiKit.Cream.b, 0.5f);
            var b = UiKit.MakeButton(content, "Row_" + def.Id, label, 23, bg, fg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            UiKit.ButtonText(b).alignment = TextAnchor.MiddleLeft;
            var le = b.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 46f;
            le.minHeight = 46f;
            b.interactable = canBuy;
            var captured = def;
            b.onClick.AddListener(() => Buy(captured));
        }

        void Buy(UpgradeDefinition def)
        {
            if (session.Ledger.TryBuy(session.Wallet, def, session.CurrentRank))
            {
                onPurchased?.Invoke();
                Refresh(false);
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
