using System;
using System.Collections.Generic;
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
    /// Runs the loop: intro card, how-to cards, the timed day, the pull, the quiet time, the complaint form,
    /// the promotion card and the night shop. The rules live in Noted.Core.GameSession; this class only connects them to the scene.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
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

        [Header("Places")]
        [SerializeField] Transform wakePoint;
        [SerializeField] Transform consoleStandPoint;

        [Header("Tuning")]
        [Tooltip("0 picks a random seed each run.")]
        [SerializeField] int seedOverride;
        [SerializeField] float baseCoinPickupRadius = 1.2f;

        System.Random complaintRng;
        FlowPhase returnPhase;
        int pendingRank;
        DaySummary lastSummary;

        public GameSession Session { get; private set; }
        public FlowPhase Phase { get; private set; } = FlowPhase.Boot;
        public bool IsDay => Phase == FlowPhase.Day;
        public bool IsQuiet => Phase == FlowPhase.Quiet;
        public DaySummary LastSummary => lastSummary;
        public Transform PlayerTransform => motor != null ? motor.transform : null;
        public float CoinPickupRadius => baseCoinPickupRadius + (Session != null ? Session.Mods.MagnetRadius : 0f);

        void Start()
        {
            var config = new GameConfig();
            int seed = seedOverride != 0 ? seedOverride : UnityEngine.Random.Range(1, 1000000);
            Session = new GameSession(config, seed);
            complaintRng = new System.Random(seed);

            pull.ResetState();
            lighting.SetMode(LightMode.Day, true);
            SyncDoors(true);
            SetControls(false);
            hud.SetVisible(false);
            motor.TeleportTo(wakePoint.position, wakePoint.eulerAngles.y);
            look.ResetPitch();

            // The setup menu comes first (invert mouse, sensitivity, field of view), then the intro card.
            Phase = FlowPhase.Setup;
            setup.Show(look, ShowIntro);
        }

        void ShowIntro()
        {
            Phase = FlowPhase.Intro;
            cards.Show("THE SOLACE INITIATIVE", CardText.Intro(Session.Config, Session.Code), "Begin my first day", BeginDayFlow);
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

        void BeginDayFlow()
        {
            Session.BeginDay();
            ApplyModifiers();
            var newTypes = Session.NewTaskTypesToday();
            Session.MarkSeen();
            Phase = FlowPhase.HowTo;
            ShowHowTo(newTypes, 0);
        }

        void ShowHowTo(List<TaskDefinition> types, int index)
        {
            if (index >= types.Count)
            {
                StartDay();
                return;
            }
            var def = types[index];
            cards.Show("NEW TASK: " + def.Name.ToUpperInvariant(), def.HowTo, "Understood", () => ShowHowTo(types, index + 1));
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
            if (lastSummary.RankedUp) pendingRank = lastSummary.NewRank;

            playerCamera.enabled = true;
            motor.TeleportTo(consoleStandPoint.position, consoleStandPoint.eulerAngles.y);
            look.ResetPitch();
            lighting.SetMode(LightMode.Quiet);
            Phase = FlowPhase.Quiet;
            SetControls(true);
            RefreshHud();
            string summary = "Day " + lastSummary.Day + " complete. Tasks finished: " + lastSummary.TasksCompleted + "/" + lastSummary.TasksOnList +
                             ". Collect your Uptime, file a complaint, or sleep at the cot.";
            hud.Toast(summary, 7f);
            HandleUnlocks(lastSummary.NewlyUnlocked, summary);
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
                line = "Done: " + result.Task.Definition.Name;
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
                return Session.Complaints.PickReply(Session.CurrentRank, complaintRng);
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
            shop.Show(Session, BeginDayFlow, () =>
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
            string progress = Session.Rank.NextThreshold > 0
                ? Session.Rank.CompletedTasks + "/" + Session.Rank.NextThreshold + " tasks"
                : "";
            hud.SetStatus(Session.Wallet.Balance, Session.Config.TitleFor(rank), progress);
            hud.SetTasks(Session.TodaysTasks);
            hud.SetCarrying(CarryText());
            hud.SetTools(ToolsText());
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
                default: return token;
            }
        }

        // ---- Debug helpers, used by verification scripts ----

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
    }
}
