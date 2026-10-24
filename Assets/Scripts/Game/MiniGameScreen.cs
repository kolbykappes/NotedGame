using System;
using System.Collections;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// The three short mini-games: sort the mail, match cables to ports, repeat a light pattern. The day clock keeps running behind it;
    /// a wrong answer costs a couple of seconds, never the day. Keyboard or mouse; Esc cancels.
    /// The rules live in Noted.Core (SortGame, CableGame, PatternGame); this class only draws them.
    /// </summary>
    public class MiniGameScreen : MonoBehaviour
    {
        const int MaxCables = 6;

        static readonly Color[] CableColors =
        {
            new Color32(0xC8, 0x3A, 0x32, 0xFF), // red
            new Color32(0x2F, 0x6F, 0xC8, 0xFF), // blue
            new Color32(0x35, 0x9A, 0x4A, 0xFF), // green
            new Color32(0xD8, 0xB8, 0x30, 0xFF), // yellow
            new Color32(0xD8, 0x7A, 0x2A, 0xFF), // orange
            new Color32(0xE6, 0xE2, 0xDA, 0xFF), // white
        };

        static readonly Key[] CableKeys = { Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y };
        static readonly Key[] NumberKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6 };

        [SerializeField] GameObject root;
        [SerializeField] Text titleText;
        [SerializeField] Text hintText;
        [SerializeField] Text statusText;
        [SerializeField] Button cancelButton;

        [SerializeField] GameObject sortView;
        [SerializeField] Text itemText;
        [SerializeField] Text sortProgressText;
        [SerializeField] Button[] binButtons;

        [SerializeField] GameObject cableView;
        [SerializeField] Button[] cableButtons;
        [SerializeField] Button[] portButtons;

        [SerializeField] GameObject patternView;
        [SerializeField] Button[] padButtons;

        MiniGameKind kind;
        SortGame sort;
        CableGame cable;
        PatternGame pattern;
        int selectedCable = -1;
        bool showingPattern;
        Coroutine routine;
        Action onWin;
        Action onCancel;
        Action onWrong;
        float openedAt;

        public bool IsOpen => root != null && root.activeSelf;

        // ---------- building (called once by the scene builder) ----------

        public void Build(Transform canvasRoot)
        {
            // A card in the middle only, so the day clock at the top of the screen stays visible while you play.
            var card = UiKit.Panel(canvasRoot, "MiniGameScreen", UiKit.DarkTeal,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-520, -330), new Vector2(520, 330));
            root = card.gameObject;

            titleText = UiKit.Label(card.transform, "Title", "MINI-GAME", 40, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(40, -80), new Vector2(-40, -20));
            hintText = UiKit.Label(card.transform, "Hint", "", 24, UiKit.Cream, TextAnchor.MiddleLeft,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(40, -120), new Vector2(-40, -80));
            statusText = UiKit.Label(card.transform, "Status", "", 28, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0f, 0f), new Vector2(0.7f, 0f), new Vector2(40, 20), new Vector2(0, 70));
            cancelButton = UiKit.MakeButton(card.transform, "Cancel", "Cancel (Esc)", 24, UiKit.Navy, UiKit.Cream,
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-260, 20), new Vector2(-30, 74));

            // Sort view
            var sv = UiKit.Rect(card.transform, "SortView", Vector2.zero, Vector2.one, new Vector2(20, 90), new Vector2(-20, -140));
            sortView = sv.gameObject;
            itemText = UiKit.Label(sv, "Item", "Item", 56, UiKit.Cream, TextAnchor.MiddleCenter,
                new Vector2(0f, 0.45f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            sortProgressText = UiKit.Label(sv, "Progress", "0/5", 26, UiKit.Yellow, TextAnchor.MiddleCenter,
                new Vector2(0f, 0.32f), new Vector2(1f, 0.45f), Vector2.zero, Vector2.zero);
            binButtons = new Button[3];
            for (int i = 0; i < 3; i++)
            {
                float x0 = 0.04f + i * 0.32f;
                binButtons[i] = UiKit.MakeButton(sv, "Bin" + i, (i + 1) + "  " + SortGame.BinNames[i], 32, UiKit.Navy, UiKit.Cream,
                    new Vector2(x0, 0f), new Vector2(x0 + 0.28f, 0.28f), Vector2.zero, Vector2.zero);
            }

            // Cable view
            var cv = UiKit.Rect(card.transform, "CableView", Vector2.zero, Vector2.one, new Vector2(20, 90), new Vector2(-20, -140));
            cableView = cv.gameObject;
            UiKit.Label(cv, "CablesLabel", "CABLES", 24, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0.04f, 0.92f), new Vector2(0.46f, 1f), Vector2.zero, Vector2.zero);
            UiKit.Label(cv, "PortsLabel", "PORTS", 24, UiKit.Yellow, TextAnchor.MiddleLeft,
                new Vector2(0.54f, 0.92f), new Vector2(0.96f, 1f), Vector2.zero, Vector2.zero);
            cableButtons = new Button[MaxCables];
            portButtons = new Button[MaxCables];
            for (int i = 0; i < MaxCables; i++)
            {
                float top = 0.9f - i * 0.15f;
                cableButtons[i] = UiKit.MakeButton(cv, "Cable" + i, "", 28, CableColors[i], UiKit.Navy,
                    new Vector2(0.04f, top - 0.13f), new Vector2(0.46f, top), Vector2.zero, Vector2.zero);
                portButtons[i] = UiKit.MakeButton(cv, "Port" + i, "", 28, UiKit.Navy, UiKit.Cream,
                    new Vector2(0.54f, top - 0.13f), new Vector2(0.96f, top), Vector2.zero, Vector2.zero);
            }

            // Pattern view
            var pv = UiKit.Rect(card.transform, "PatternView", Vector2.zero, Vector2.one, new Vector2(20, 90), new Vector2(-20, -140));
            patternView = pv.gameObject;
            padButtons = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                float x0 = 0.12f + (i % 2) * 0.42f;
                float y0 = 0.52f - (i / 2) * 0.5f;
                padButtons[i] = UiKit.MakeButton(pv, "Pad" + i, (i + 1).ToString(), 44, CableColors[i], UiKit.Navy,
                    new Vector2(x0, y0), new Vector2(x0 + 0.34f, y0 + 0.44f), Vector2.zero, Vector2.zero);
            }

            root.SetActive(false);
        }

        // ---------- lifecycle ----------

        void Awake()
        {
            if (cancelButton == null) return;
            cancelButton.onClick.AddListener(Cancel);
            for (int i = 0; i < binButtons.Length; i++) { int b = i; binButtons[i].onClick.AddListener(() => ChooseBin(b)); }
            for (int i = 0; i < cableButtons.Length; i++) { int c = i; cableButtons[i].onClick.AddListener(() => SelectCable(c)); }
            for (int i = 0; i < portButtons.Length; i++) { int p = i; portButtons[i].onClick.AddListener(() => PlugPort(p)); }
            for (int i = 0; i < padButtons.Length; i++) { int p = i; padButtons[i].onClick.AddListener(() => PressPad(p)); }
            if (root != null) root.SetActive(false);
        }

        public void Open(MiniGameKind gameKind, int seed, int rank, string title, Action win, Action cancel, Action wrong)
        {
            kind = gameKind;
            onWin = win;
            onCancel = cancel;
            onWrong = wrong;
            openedAt = Time.unscaledTime;
            selectedCable = -1;
            showingPattern = false;
            var rng = new System.Random(seed);

            sortView.SetActive(kind == MiniGameKind.Sort);
            cableView.SetActive(kind == MiniGameKind.CableMatch);
            patternView.SetActive(kind == MiniGameKind.Pattern);
            titleText.text = title.ToUpperInvariant();
            statusText.text = "";
            root.SetActive(true);

            switch (kind)
            {
                case MiniGameKind.Sort:
                    sort = new SortGame(rng, 5, seed % 3 == 0);
                    hintText.text = "Press 1, 2 or 3 (or click) to put each item in the right bin. A wrong bin costs " + MiniGameRules.WrongAnswerPenaltySeconds.ToString("0") + " seconds.";
                    RefreshSort();
                    break;
                case MiniGameKind.CableMatch:
                    cable = new CableGame(rng, rank >= 2 ? 5 : 4);
                    hintText.text = "Pick a cable (Q W E R T or click), then the port that wants its colour (1 to 5 or click).";
                    RefreshCable();
                    break;
                case MiniGameKind.Pattern:
                    for (int i = 0; i < padButtons.Length; i++) SetPad(i, false);
                    pattern = new PatternGame(rng, rank >= 2 ? 5 : 4, 4);
                    hintText.text = "Watch the lights, then repeat the pattern with 1 2 3 4 (or click).";
                    if (routine != null) StopCoroutine(routine);
                    routine = StartCoroutine(ShowPattern());
                    break;
            }
        }

        /// <summary>Closes without calling anything back (the day ended while it was open).</summary>
        public void ForceClose()
        {
            if (routine != null) StopCoroutine(routine);
            routine = null;
            onWin = onCancel = onWrong = null;
            if (root != null) root.SetActive(false);
        }

        void Cancel()
        {
            if (!IsOpen) return;
            var cb = onCancel;
            ForceClose();
            cb?.Invoke();
        }

        void Win()
        {
            if (!IsOpen) return;
            var cb = onWin;
            ForceClose();
            cb?.Invoke();
        }

        void Wrong()
        {
            statusText.text = "Wrong. -" + MiniGameRules.WrongAnswerPenaltySeconds.ToString("0") + " s";
            onWrong?.Invoke();
        }

        void Update()
        {
            if (!IsOpen || Time.unscaledTime - openedAt < 0.25f) return;
            var k = Keyboard.current;
            if (k == null) return;
            if (k.escapeKey.wasPressedThisFrame) { Cancel(); return; }

            switch (kind)
            {
                case MiniGameKind.Sort:
                    for (int i = 0; i < 3; i++) if (k[NumberKeys[i]].wasPressedThisFrame) ChooseBin(i);
                    break;
                case MiniGameKind.CableMatch:
                    for (int i = 0; i < cable.Count; i++)
                    {
                        if (k[CableKeys[i]].wasPressedThisFrame) SelectCable(i);
                        if (k[NumberKeys[i]].wasPressedThisFrame) PlugPort(i);
                    }
                    break;
                case MiniGameKind.Pattern:
                    for (int i = 0; i < 4; i++) if (k[NumberKeys[i]].wasPressedThisFrame) PressPad(i);
                    break;
            }
        }

        // ---------- sort ----------

        void ChooseBin(int bin)
        {
            if (!IsOpen || kind != MiniGameKind.Sort || sort.IsComplete) return;
            bool ok = sort.Choose(bin);
            RefreshSort();
            if (sort.IsComplete) { Win(); return; }
            if (!ok) Wrong(); else statusText.text = "";
        }

        void RefreshSort()
        {
            itemText.text = sort.IsComplete ? "Done" : sort.Current.Label;
            sortProgressText.text = sort.Sorted + "/" + sort.Total;
        }

        // ---------- cables ----------

        void SelectCable(int color)
        {
            if (!IsOpen || kind != MiniGameKind.CableMatch || color >= cable.Count || cable.IsCableConnected(color)) return;
            selectedCable = color;
            statusText.text = CableGame.Colors[color] + " selected";
            RefreshCable();
        }

        void PlugPort(int port)
        {
            if (!IsOpen || kind != MiniGameKind.CableMatch || port >= cable.Count || cable.IsPortConnected(port)) return;
            if (selectedCable < 0) { statusText.text = "Pick a cable first."; return; }
            bool ok = cable.Plug(selectedCable, port);
            if (ok) selectedCable = -1;
            RefreshCable();
            if (cable.IsComplete) { Win(); return; }
            if (!ok) Wrong(); else statusText.text = "";
        }

        void RefreshCable()
        {
            for (int i = 0; i < MaxCables; i++)
            {
                bool used = i < cable.Count;
                cableButtons[i].gameObject.SetActive(used);
                portButtons[i].gameObject.SetActive(used);
                if (!used) continue;

                bool done = cable.IsCableConnected(i);
                var cableImg = (Image)cableButtons[i].targetGraphic;
                cableImg.color = done ? Color.Lerp(CableColors[i], UiKit.DarkTeal, 0.75f) : CableColors[i];
                UiKit.ButtonText(cableButtons[i]).text = CableKeys[i].ToString() + "  " + CableGame.Colors[i] + (done ? "  (in)" : "") + (selectedCable == i ? "  <" : "");

                int want = cable.ColorOfPort(i);
                bool portDone = cable.IsPortConnected(i);
                var portImg = (Image)portButtons[i].targetGraphic;
                portImg.color = portDone ? Color.Lerp(CableColors[want], UiKit.DarkTeal, 0.75f) : CableColors[want];
                UiKit.ButtonText(portButtons[i]).text = (i + 1) + "  " + CableGame.Colors[want] + (portDone ? "  (in)" : "");
                UiKit.ButtonText(portButtons[i]).color = UiKit.Navy;
            }
        }

        // ---------- pattern ----------

        IEnumerator ShowPattern()
        {
            showingPattern = true;
            statusText.text = "Watch...";
            yield return new WaitForSecondsRealtime(0.6f);
            for (int step = 0; step < pattern.Length; step++)
            {
                SetPad(pattern.PadAt(step), true);
                yield return new WaitForSecondsRealtime(0.45f);
                SetPad(pattern.PadAt(step), false);
                yield return new WaitForSecondsRealtime(0.18f);
            }
            showingPattern = false;
            statusText.text = "Your turn.";
            routine = null;
        }

        void SetPad(int pad, bool lit)
        {
            var img = (Image)padButtons[pad].targetGraphic;
            img.color = lit ? Color.Lerp(CableColors[pad], Color.white, 0.55f) : CableColors[pad];
            if (lit) img.transform.localScale = Vector3.one * 1.06f; else img.transform.localScale = Vector3.one;
        }

        void PressPad(int pad)
        {
            if (!IsOpen || kind != MiniGameKind.Pattern || showingPattern || pattern.IsComplete) return;
            bool ok = pattern.Press(pad);
            StartCoroutine(Flash(pad));
            if (pattern.IsComplete) { Win(); return; }
            if (!ok)
            {
                Wrong();
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(ShowPattern());
            }
            else
            {
                statusText.text = pattern.Progress + "/" + pattern.Length;
            }
        }

        IEnumerator Flash(int pad)
        {
            SetPad(pad, true);
            yield return new WaitForSecondsRealtime(0.15f);
            if (IsOpen) SetPad(pad, false);
        }
    }
}
