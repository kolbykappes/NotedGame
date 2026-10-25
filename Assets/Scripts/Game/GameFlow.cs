using System;
using System.Collections.Generic;
using System.Text;
using Noted.Core;
using UnityEngine;

namespace Noted.Game
{
    public enum FlowPhase
    {
        Boot,
        Setup,
        Intro,
        HowTo,
        Day,
        Pull,
        Quiet,
        Complaint,
        RankUp,
        Shop,
        MiniGame,
    }

    /// <summary>
    /// Runs the loop: intro card, the morning cards, the timed day, the pull, the quiet time, the complaint form,
    /// the promotion card and the night shop. The rules live in Noted.Core.GameSession; this class only connects them to the scene.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        struct CardSpec
        {
            public string Title;
            public string Body;
            public string Label;

            public CardSpec(string title, string body, string label)
            {
                Title = title;
                Body = body;
                Label = label;
            }
        }

        [Header("Player")]
        [SerializeField] PlayerMotor motor;
        [SerializeField] PlayerLook look;
        [SerializeField] Interactor interactor;
        [SerializeField] Camera playerCamera;

        [Header("Screens and systems")]
        [SerializeField] Hud hud;
        [SerializeField] SetupScreen setup;
        [SerializeField] CardScreen cards;
        [SerializeField] ComplaintScreen complaint;
        [SerializeField] ShopScreen shop;
        [SerializeField] CoinSpawner coins;
        [SerializeField] LightingController lighting;
        [SerializeField] PullSequence pull;
        [SerializeField] Sfx sfx;
        [SerializeField] MiniGameScreen miniGame;
        [SerializeField] AreaDoor[] doors;
        [SerializeField] RolloutSign rolloutSign;

        [Header("Places")]
        [SerializeField] Transform wakePoint;
        [SerializeField] Transform consoleStandPoint;

        [Header("Tuning")]
        [Tooltip("0 picks a random seed each run.")]
        [SerializeField] int seedOverride;
        [SerializeField] float baseCoinPickupRadius = 1.2f;

        [Header("Feel test")]
        [Tooltip("0 is a normal new game. 1 to 5 starts at that level with tools, every room open and no how-to cards. F6 changes level while playing (this field is reset when the scene is rebuilt).")]
        [SerializeField] int debugStartLevel;
        [Tooltip("0 uses the level's usual starting day.")]
        [SerializeField] int debugStartDay;

        System.Random complaintRng;
        FlowPhase returnPhase;
        int pendingRank;
        int sessionSeed;
        DaySummary lastSummary;
        SecondListPanel secondList;
        DebugOverlay overlay;
        readonly StringBuilder text = new StringBuilder();
        readonly List<CardSpec> morningCards = new List<CardSpec>();
        int nextErrandToOffer;

        public GameSession Session { get; private set; }
        public FlowPhase Phase { get; private set; } = FlowPhase.Boot;
        public bool IsDay => Phase == FlowPhase.Day;
        public bool IsQuiet => Phase == FlowPhase.Quiet;
        public DaySummary LastSummary => lastSummary;
        public Transform PlayerTransform => motor != null ? motor.transform : null;
        public float CoinPickupRadius => baseCoinPickupRadius + (Session != null ? Session.Mods.MagnetRadius : 0f);

        void Start()
        {
            sessionSeed = seedOverride != 0 ? seedOverride : UnityEngine.Random.Range(1, 1000000);
            Session = NewSession(debugStartLevel, debugStartDay);
            complaintRng = new System.Random(sessionSeed);

            pull.ResetState();
            lighting.SetMode(LightMode.Day, true);
            SyncDoors(true);
            SetControls(false);
            hud.SetVisible(false);
            motor.TeleportTo(wakePoint.position, wakePoint.eulerAngles.y);
            look.ResetPitch();

            // The feel-test tools exist in the Editor and development builds only.
            if (Debug.isDebugBuild)
            {
                overlay = DebugOverlay.Create(this);
            }
            secondList = SecondListPanel.Create(this);

            // The setup menu comes first (invert mouse, sensitivity, field of view), then the intro card.
            Phase = FlowPhase.Setup;
            setup.Show(look, ShowIntro);
        }

        GameSession NewSession(int startLevel, int startDay)
        {
            var session = new GameSession(new GameConfig(), sessionSeed);
            if (startLevel > 0)
            {
                session.ApplyStartProfile(new StartProfile { Level = Mathf.Clamp(startLevel, 1, session.Config.Levels.Count), Day = startDay, Coins = 150 });
            }
            return session;
        }

        void ShowIntro()
        {
            // A debug start skips the story intro and goes straight to the first morning.
            if (Session.Day > 0 || debugStartLevel > 0)
            {
                BeginDayFlow();
                return;
            }
            Phase = FlowPhase.Intro;
            cards.Show("THE SOLACE INITIATIVE", CardText.Intro(Session.Config, Session.Code), "Begin my first day", () => BeginDayFlow());
        }

        void Update()
        {
            if (Session == null) return;

            if (Phase == FlowPhase.Day || Phase == FlowPhase.MiniGame)
            {
                hud.SetTimer(Session.Clock.Remaining);
                if (Session.Tick(Time.deltaTime))
                {
                    hud.SetTimer(0f);
                    if (Phase == FlowPhase.MiniGame) miniGame.ForceClose();
                    BeginPull();
                    return;
                }
            }

            bool free = Phase == FlowPhase.Day || Phase == FlowPhase.Quiet;
            hud.SetPrompt(free && interactor.HasTarget, interactor.Prompt, interactor.Progress01);
        }

        // ---- Day setup ----

        void BeginDayFlow(bool skipCards = false)
        {
            Session.BeginDay();
            ApplyModifiers();
            var newTypes = Session.NewTaskTypesToday();
            Session.MarkSeen();
            BuildMorningCards(newTypes, skipCards);
            Session.MarkProceduresShown();
            Phase = FlowPhase.HowTo;
            ShowNextMorningCard(0);
        }

        // The cards before a day, in order: the town's slip, a scrap, changed procedures, new job types, the second list.
        void BuildMorningCards(List<TaskDefinition> newTypes, bool skip)
        {
            morningCards.Clear();
            if (skip) return;

            int stage = Session.RevealStage;

            string slip = Session.LedgerText();
            if (slip != null) morningCards.Add(new CardSpec("MORNING SLIP", slip, "Understood"));

            var scrap = Session.NextScrap();
            if (scrap != null)
            {
                Session.MarkScrapFound(scrap.Id);
                morningCards.Add(new CardSpec("SCRAP FOUND", "It was folded small and left where you would look.\n\n" + scrap.Text, "Hide it"));
            }

            // Jobs the player has already met whose wording has changed. New job types get the changed wording in their how-to card.
            var updates = Session.ProcedureUpdatesToday();
            text.Clear();
            foreach (var def in updates)
            {
                if (newTypes.Contains(def)) continue;
                var entry = JobText.For(def.Id, stage);
                text.Append(entry.Name).Append('\n').Append("   ").Append(entry.Procedure).Append("\n\n");
            }
            if (text.Length > 0)
            {
                text.Append("Thank you for your understanding.");
                morningCards.Add(new CardSpec("UPDATED PROCEDURES", text.ToString(), "Understood"));
            }

            foreach (var def in newTypes)
            {
                morningCards.Add(new CardSpec("NEW TASK: " + JobText.NameFor(def, stage).ToUpperInvariant(), JobText.HowToFor(def, stage), "Understood"));
            }

            if (Session.TodaysErrands.Count > 0)
            {
                morningCards.Add(new CardSpec("SECOND LIST",
                    SecondListPanel.Describe(Session, new StringBuilder()) + "\n\nHold Tab during the day to read this again. Press F at the stations it names.",
                    "Memorise it"));
            }
        }

        void ShowNextMorningCard(int index)
        {
            if (index >= morningCards.Count)
            {
                StartDay();
                return;
            }
            var spec = morningCards[index];
            cards.Show(spec.Title, spec.Body, spec.Label, () => ShowNextMorningCard(index + 1));
        }

        void StartDay()
        {
            pull.ResetState();
            playerCamera.enabled = true;
            lighting.SetMode(LightMode.Day);
            motor.TeleportTo(wakePoint.position, wakePoint.eulerAngles.y);
            look.ResetPitch();
            Phase = FlowPhase.Day;
            SetControls(true);
            hud.SetVisible(true);
            hud.SetTimer(Session.Clock.Remaining);
            RefreshHud();
            hud.Toast("Day " + Session.Day, 2.5f);
            Beep(SfxId.Wake);
        }

        void ApplyModifiers()
        {
            var m = Session.Mods;
            motor.SpeedMultiplier = m.SpeedMultiplier;
            interactor.RangeBonus = m.ReachBonus;
        }

        // ---- The pull and the quiet time ----

        void BeginPull()
        {
            Phase = FlowPhase.Pull;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            hud.Toast(CardText.Pull(), 3.5f);
            Beep(SfxId.PullStart);
            playerCamera.enabled = false;
            pull.Play(Session.Code, Session.CurrentRank, OnPullFinished);
        }

        void OnPullFinished()
        {
            lastSummary = Session.EndDay();
            DayLogWriter.Append(lastSummary.Stats);
            if (lastSummary.RankedUp) pendingRank = lastSummary.NewRank;

            playerCamera.enabled = true;
            motor.TeleportTo(consoleStandPoint.position, consoleStandPoint.eulerAngles.y);
            look.ResetPitch();
            lighting.SetMode(LightMode.Quiet);
            Phase = FlowPhase.Quiet;
            SetControls(true);
            RefreshHud();
            hud.Toast(SummaryText(lastSummary), 8f);
            HandleUnlocks(lastSummary.NewlyUnlocked, null);
        }

        string SummaryText(DaySummary d)
        {
            text.Clear();
            text.Append("Day ").Append(d.Day).Append(" complete. Tasks finished: ").Append(d.TasksCompleted).Append('/').Append(d.TasksOnList).Append(". ");
            if (Session.RevealStage >= 1)
            {
                text.Append("Reliability ").Append(Mathf.RoundToInt(d.Reliability)).Append("%. Rollout ").Append(Mathf.RoundToInt(d.Rollout)).Append("%.");
            }
            if (d.ErrandsDone > 0) text.Append("\nSecond list: ").Append(d.ErrandsDone).Append(" done.");
            if (d.PromotionHeld) text.Append("\nPromotion held: Reliability is below ").Append(Reliability.PromotionGate).Append("%.");
            text.Append("\nCollect your Uptime, file a complaint, or sleep at the cot.");
            return text.ToString();
        }

        // ---- Station and coin callbacks ----

        /// <summary>The player finished the hold or the press at a station. A mini-game step opens its game first.</summary>
        public void ActivateStation(TaskStation station)
        {
            if (Phase != FlowPhase.Day) return;
            if (!Session.TryGetAvailableStep(station.StationId, out var task, out var step)) return;

            if (step.Kind == StepKind.MiniGame)
            {
                OpenMiniGame(station, step);
                return;
            }
            CompleteStationStep(station);
        }

        /// <summary>The player finished the off-book action (F) at a station.</summary>
        public void ActivateAlternate(TaskStation station)
        {
            if (Phase != FlowPhase.Day) return;
            var result = Session.CompleteAlternate(station.StationId);
            if (!result.Accepted) return;

            string line;
            if (result.ErrandDone)
            {
                line = "Second list: " + result.Errand.Definition.Name + " done.";
                Beep(SfxId.TaskDone);
            }
            else
            {
                line = "Done. Next: " + result.Errand.CurrentStep.Prompt;
                Beep(SfxId.StepDone);
            }
            hud.Toast(line, 3f);
            RefreshHud();
        }

        void OpenMiniGame(TaskStation station, TaskStep step)
        {
            Phase = FlowPhase.MiniGame;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            int seed = Session.Day * 1009 + Mathf.Abs(station.StationId.Length * 31 + Session.Rank.CompletedTasks);
            miniGame.Open(step.Game, seed, Session.CurrentRank, step.Prompt,
                () =>
                {
                    Phase = FlowPhase.Day;
                    SetControls(true);
                    CompleteStationStep(station);
                },
                () =>
                {
                    Phase = FlowPhase.Day;
                    SetControls(true);
                },
                () =>
                {
                    Session.ApplyTimePenalty(MiniGameRules.WrongAnswerPenaltySeconds);
                    Beep(SfxId.Wrong);
                });
        }

        public void CompleteStationStep(TaskStation station)
        {
            var result = Session.CompleteStep(station.StationId);
            if (!result.Accepted) return;

            string line;
            if (result.TaskDone)
            {
                coins.Spawn(station.CoinOrigin, result.CoinsToSpawn, Session.Config.CoinPickupsPerTask);
                line = JobText.DoneLineFor(result.Task.Definition, Session.RevealStage);
                Beep(SfxId.TaskDone);
            }
            else if (result.Step != null && result.Step.Kind == StepKind.Flip)
            {
                line = result.Task.CurrentStep != null ? "Done. Next: " + result.Task.CurrentStep.Prompt : "Done.";
                Beep(SfxId.Flip);
            }
            else
            {
                line = "Step done. " + (result.Task.CurrentStep != null ? "Next: " + result.Task.CurrentStep.Prompt : "");
                Beep(SfxId.StepDone);
            }

            // A reading and a small oddity can ride along with the result.
            float seconds = 3f;
            if (!string.IsNullOrEmpty(result.Readout)) { line += "\nReading: " + result.Readout; seconds = 4f; }
            if (!string.IsNullOrEmpty(result.Oddity)) { line += "\n" + result.Oddity; seconds = 7f; }
            hud.Toast(line, seconds);

            RefreshHud();
            HandleUnlocks(result.Unlocked, null);
        }

        // ---- Rooms that open as you work ----

        void SyncDoors(bool instant)
        {
            if (doors == null) return;
            foreach (var d in doors)
            {
                if (d != null) d.SetOpen(Session.IsAreaUnlocked(d.AreaId), instant);
            }
        }

        void HandleUnlocks(IReadOnlyList<string> opened, string toastPrefix)
        {
            if (opened == null || opened.Count == 0) return;
            SyncDoors(false);
            var names = new List<string>();
            foreach (var id in opened)
            {
                var rule = Session.Areas.Find(id);
                names.Add(rule != null ? rule.Name : id);
            }
            string message = "Access granted: " + string.Join(", ", names) + ".";
            hud.Toast(string.IsNullOrEmpty(toastPrefix) ? message : toastPrefix + "\n" + message, 7f);
            Beep(SfxId.Unlock);
        }

        public void CollectCoin(CoinPickup coin)
        {
            Session.CollectCoins(coin.Value);
            Beep(SfxId.Coin, UnityEngine.Random.Range(0.92f, 1.18f));
            coins.Forget(coin);
            Destroy(coin.gameObject);
            RefreshHud();
        }

        // ---- Complaint form ----

        public void OpenComplaint()
        {
            if (Phase != FlowPhase.Day && Phase != FlowPhase.Quiet) return;
            returnPhase = Phase;
            Phase = FlowPhase.Complaint;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            complaint.Show(index =>
            {
                Beep(SfxId.Reply);
                return Session.Complaints.PickReply(Session.CurrentRank, complaintRng, Session.RevealStage);
            }, null, CloseComplaint);
        }

        void CloseComplaint()
        {
            Phase = returnPhase;
            SetControls(true);
        }

        // ---- The tool locker in the quiet time (no need to sleep) ----

        public void OpenLocker()
        {
            if (Phase != FlowPhase.Quiet) return;
            returnPhase = FlowPhase.Quiet;
            Phase = FlowPhase.Shop;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            shop.Show(Session, CloseLocker, () =>
            {
                Beep(SfxId.Buy);
                ApplyModifiers();
                RefreshHud();
                HandleUnlocks(Session.RefreshUnlocks(), null);
            }, "Close");
        }

        void CloseLocker()
        {
            Phase = returnPhase;
            SetControls(true);
            RefreshHud();
        }

        // ---- Night ----

        public void Sleep()
        {
            if (Phase != FlowPhase.Quiet) return;
            Beep(SfxId.Sleep);
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            Session.CollectAllOutstanding();
            coins.ClearAll();
            RefreshHud();

            if (pendingRank > 0)
            {
                int rank = pendingRank;
                pendingRank = 0;
                Phase = FlowPhase.RankUp;
                cards.Show("PROMOTION", CardText.RankUp(Session.Config, rank), "Continue", ShowShop);
            }
            else
            {
                ShowShop();
            }
        }

        void ShowShop()
        {
            Phase = FlowPhase.Shop;
            shop.Show(Session, () => BeginDayFlow(), () =>
            {
                Beep(SfxId.Buy);
                ApplyModifiers();
                RefreshHud();
                HandleUnlocks(Session.RefreshUnlocks(), null);
            });
        }

        // ---- Helpers ----

        void Beep(SfxId id, float pitch = 1f)
        {
            if (sfx != null) sfx.Play(id, pitch);
        }

        void SetControls(bool on)
        {
            motor.enabled = on;
            look.enabled = on;
            interactor.enabled = on;
        }

        void RefreshHud()
        {
            var rank = Session.CurrentRank;
            int stage = Session.RevealStage;
            string progress = Session.Rank.NextThreshold > 0
                ? Session.Rank.CompletedTasks + "/" + Session.Rank.NextThreshold
                : "";
            hud.SetStatus(Session.Wallet.Balance, "Level " + (rank + 1) + " " + Session.Config.TitleFor(rank), progress);
            hud.SetMetrics(stage >= 1
                ? "Reliability " + Mathf.RoundToInt(Session.Reliability.Value) + "%   Rollout " + Mathf.RoundToInt(Session.Rollout.Value) + "%"
                : "");
            hud.SetTasks(Session.TodaysTasks, def => JobText.NameFor(def, stage));
            hud.SetCarrying(CarryText());
            hud.SetTools(ToolsText());
            if (rolloutSign != null) rolloutSign.Set(stage, Session.Rollout.Value, Session.Reliability.Value);
        }

        string CarryText()
        {
            int capacity = Session.CarryCapacity;
            if (Session.Carried.Count == 0 && capacity <= 1) return "";
            var names = new List<string>();
            foreach (var token in Session.Carried) names.Add(CarryName(token));
            string list = names.Count > 0 ? ": " + string.Join(", ", names) : "";
            return "Carrying " + Session.Carried.Count + "/" + capacity + list;
        }

        string ToolsText()
        {
            var names = new List<string>();
            foreach (var def in Session.Upgrades.All)
            {
                if (def.Category == UpgradeCategory.Later) continue;
                int level = Session.Ledger.Level(def.Id);
                if (level <= 0) continue;
                names.Add(def.MaxLevel > 1 ? def.Name + " x" + level : def.Name);
            }
            return names.Count == 0 ? "TOOLS: none yet" : "TOOLS: " + string.Join(", ", names);
        }

        static string CarryName(string token)
        {
            switch (token)
            {
                case "beans": return "bean sack";
                case "paper": return "ream of paper";
                case "bag": return "shred bag";
                case "filter": return "air filter";
                case "drive": return "spare drive";
                case "reading": return "temperature reading";
                case "blank": return "blank stock";
                case "appeal": return "unshredded appeal";
                case "copy": return "copied page";
                default: return token;
            }
        }

        // ---- Debug helpers (the F-keys and the overlay buttons) ----

        /// <summary>Opens every room (verification scripts only).</summary>
        public void DebugUnlockAll()
        {
            Session.ForceUnlockAll();
            SyncDoors(false);
        }

        /// <summary>Runs the clock out and starts the pull now, as if the timer had reached zero.</summary>
        public void DebugExpireClock()
        {
            if (Phase != FlowPhase.Day) return;
            Session.Clock.Tick(10000f);
            hud.SetTimer(0f);
            BeginPull();
        }

        bool CanDebugReset => Phase == FlowPhase.Day || Phase == FlowPhase.Quiet || Phase == FlowPhase.MiniGame;

        // Puts everything on screen back to rest before a debug jump starts a morning.
        void ClearForDebugJump()
        {
            if (Phase == FlowPhase.MiniGame) miniGame.ForceClose();
            cards.Hide();
            coins.ClearAll();
            pull.ResetState();
            playerCamera.enabled = true;
            pendingRank = 0;
        }

        /// <summary>F2: go back to the start of this day (same list) and play it again.</summary>
        public void DebugReplayDay()
        {
            if (!CanDebugReset || !Session.CanReplayDay) return;
            ClearForDebugJump();
            Session.RestoreDayStart();
            SyncDoors(true);
            BeginDayFlow(true);
            hud.Toast("Replaying day " + Session.Day, 2.5f);
        }

        /// <summary>F3: end this day as it stands and go straight to the next morning.</summary>
        public void DebugSkipDay()
        {
            if (!CanDebugReset) return;
            bool wasDay = Phase == FlowPhase.Day || Phase == FlowPhase.MiniGame;
            ClearForDebugJump();
            if (wasDay)
            {
                lastSummary = Session.EndDay();
                DayLogWriter.Append(lastSummary.Stats);
            }
            Session.CollectAllOutstanding();
            BeginDayFlow(true);
        }

        /// <summary>F4: tools and coins, so a high level can be tried with the gear a player would have.</summary>
        public void DebugGrantTools()
        {
            if (Session == null) return;
            Session.GrantTools(ToolLoadout.All, 500);
            ApplyModifiers();
            RefreshHud();
            hud.Toast("Tools granted. Uptime +500.", 2.5f);
        }

        /// <summary>F6: restart at another level (Shift+F6 goes down). Keeps the seed; starts with the basic tool loadout.</summary>
        public void DebugJumpLevel(int delta)
        {
            if (!CanDebugReset) return;
            int target = Mathf.Clamp(Session.CurrentRank + 1 + delta, 1, Session.Config.Levels.Count);
            ClearForDebugJump();
            Session = NewSession(target, 0);
            SyncDoors(true);
            BeginDayFlow();
        }

        /// <summary>Overlay button: shows the next mandatory wellness tape.</summary>
        public void DebugShowWellness()
        {
            if (Phase != FlowPhase.Day && Phase != FlowPhase.Quiet) return;
            var (title, body) = Session.NextWellness();
            returnPhase = Phase;
            Phase = FlowPhase.HowTo;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            cards.Show(title, body, "Continue", () =>
            {
                Phase = returnPhase;
                SetControls(true);
            });
        }

        /// <summary>Overlay button: adds the next errand in the catalogue to today's second list.</summary>
        public void DebugOfferNextErrand()
        {
            if (Phase != FlowPhase.Day) return;
            for (int i = 0; i < ErrandCatalog.All.Count; i++)
            {
                var def = ErrandCatalog.All[(nextErrandToOffer + i) % ErrandCatalog.All.Count];
                if (Session.OfferErrand(def.Id))
                {
                    nextErrandToOffer = (nextErrandToOffer + i + 1) % ErrandCatalog.All.Count;
                    hud.Toast("Offered: " + def.Name, 2.5f);
                    return;
                }
            }
            hud.Toast("Every errand is already on today's list.", 2.5f);
        }

        /// <summary>Overlay button: shows the next unfound scrap now.</summary>
        public void DebugShowNextScrap()
        {
            if (Phase != FlowPhase.Day && Phase != FlowPhase.Quiet) return;
            var scrap = Session.NextScrap();
            if (scrap == null)
            {
                hud.Toast("No scrap is due at this level and job count.", 2.5f);
                return;
            }
            Session.MarkScrapFound(scrap.Id);
            returnPhase = Phase;
            Phase = FlowPhase.HowTo;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            cards.Show("SCRAP FOUND", scrap.Text, "Hide it", () =>
            {
                Phase = returnPhase;
                SetControls(true);
            });
        }

        /// <summary>Overlay button: shows this morning's slip from the town.</summary>
        public void DebugShowLedger()
        {
            if (Phase != FlowPhase.Day && Phase != FlowPhase.Quiet) return;
            string slip = Session.LedgerText() ?? LedgerSlip.Text(Mathf.Max(2, Session.RevealStage), Session.Rollout.Value, 0);
            returnPhase = Phase;
            Phase = FlowPhase.HowTo;
            SetControls(false);
            hud.SetPrompt(false, "", 0f);
            cards.Show("MORNING SLIP", slip, "Understood", () =>
            {
                Phase = returnPhase;
                SetControls(true);
            });
        }
    }
}
