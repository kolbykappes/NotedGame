using System;
using System.Collections.Generic;
using System.Linq;

namespace Noted.Core
{
    public sealed class DaySummary
    {
        public int Day;
        public int TasksOnList;
        public int TasksCompleted;
        public int TasksAutomated;
        public int CoinsEarned;
        public bool RankedUp;
        public int NewRank;
        public IReadOnlyList<string> NewlyUnlocked = new List<string>();

        /// <summary>Enough tasks were done for the next rank, but Reliability is below the gate so the promotion is held.</summary>
        public bool PromotionHeld;

        public float Reliability;
        public float Rollout;
        public float RolloutChange;
        public int ErrandsDone;
        public DayStats Stats;
    }

    /// <summary>What happened when a step was finished.</summary>
    public readonly struct StepResult
    {
        public readonly bool Accepted;
        public readonly bool TaskDone;
        public readonly int CoinsToSpawn;
        public readonly TaskInstance Task;
        public readonly TaskStep Step;

        /// <summary>A reading to show (for example a temperature), or null.</summary>
        public readonly string Readout;

        /// <summary>Something odd to mention, or null. Shown at most once per station per day.</summary>
        public readonly string Oddity;

        /// <summary>Rooms that opened as a result of this step.</summary>
        public readonly IReadOnlyList<string> Unlocked;

        public StepResult(bool accepted, bool taskDone, int coinsToSpawn, TaskInstance task, TaskStep step = null,
            string readout = null, string oddity = null, IReadOnlyList<string> unlocked = null)
        {
            Accepted = accepted;
            TaskDone = taskDone;
            CoinsToSpawn = coinsToSpawn;
            Task = task;
            Step = step;
            Readout = readout;
            Oddity = oddity;
            Unlocked = unlocked ?? Array.Empty<string>();
        }
    }

    /// <summary>What happened when a step of a second-list errand was finished.</summary>
    public readonly struct AlternateResult
    {
        public readonly bool Accepted;
        public readonly bool ErrandDone;
        public readonly ErrandInstance Errand;
        public readonly ErrandStep Step;

        public AlternateResult(bool accepted, bool errandDone, ErrandInstance errand, ErrandStep step)
        {
            Accepted = accepted;
            ErrandDone = errandDone;
            Errand = errand;
            Step = step;
        }
    }

    /// <summary>
    /// All the rules of the core loop in plain C#: the day clock, the task list, carrying, coins, rank, upgrades, which rooms are open,
    /// readings and oddities, reliability, rollout and the second list. The scene drives it and reports back; it holds no engine objects.
    /// </summary>
    public sealed class GameSession
    {
        public readonly GameConfig Config;
        public readonly TaskCatalog Tasks;
        public readonly UpgradeCatalog Upgrades;
        public readonly AreaCatalog Areas;
        public readonly Wallet Wallet = new Wallet();
        public readonly RankTracker Rank;
        public readonly UpgradeLedger Ledger;
        public readonly ComplaintBank Complaints = new ComplaintBank();
        public readonly Reliability Reliability = new Reliability();
        public readonly Rollout Rollout = new Rollout();

        readonly int seed;
        readonly HashSet<string> seen = new HashSet<string>();
        readonly List<TaskInstance> today = new List<TaskInstance>();
        readonly List<ErrandInstance> todayErrands = new List<ErrandInstance>();
        readonly List<string> carried = new List<string>();
        readonly Dictionary<string, int> jobCounts = new Dictionary<string, int>();
        readonly HashSet<string> unlocked = new HashSet<string>();
        readonly HashSet<string> seenErrands = new HashSet<string>();
        readonly HashSet<string> scrapsFound = new HashSet<string>();
        readonly Dictionary<string, JobTextEntry> proceduresShown = new Dictionary<string, JobTextEntry>();
        readonly Dictionary<string, string> readings = new Dictionary<string, string>();
        Dictionary<string, string> oddities = new Dictionary<string, string>();
        DaySnapshot daySnapshot;
        float elapsed;
        float officialFinishedAt = -1f;
        bool dayEnded;
        int coinsOutstanding;
        int coinsEarnedToday;
        int errandsStartedToday;
        int restoredYesterday;
        int wellnessSeen;

        public int Day { get; private set; }
        public string Code { get; }
        public DayClock Clock { get; private set; }

        /// <summary>The numbers for the day that just ended (null before the first day ends).</summary>
        public DayStats LastStats { get; private set; }

        /// <summary>Items being carried right now (a bean sack, a filter...), up to the carry capacity.</summary>
        public IReadOnlyList<string> Carried => carried;
        public IReadOnlyList<TaskInstance> TodaysTasks => today;
        public IReadOnlyList<ErrandInstance> TodaysErrands => todayErrands;
        public IEnumerable<string> UnlockedAreas => unlocked;

        public GameSession(GameConfig config, int randomSeed)
        {
            Config = config;
            seed = randomSeed;
            Tasks = TaskCatalog.Default();
            Upgrades = UpgradeCatalog.Default();
            Areas = AreaCatalog.Default();
            Rank = new RankTracker(config.RankThresholds);
            Ledger = new UpgradeLedger(Upgrades);
            Code = CodeGenerator.Generate(randomSeed);
            RefreshUnlocks();
        }

        public int CarryCapacity => Config.BaseCarryCapacity + Mods.CarryBonus;
        public bool IsCarrying(string token) => carried.Contains(token);

        public int CurrentRank => Rank.Rank;
        public LevelDef CurrentLevel => Config.Levels.For(Rank.Rank);

        /// <summary>How far the story has been revealed at the current level (0 clean to 3 harm in numbers).</summary>
        public int RevealStage => CurrentLevel.RevealStage;

        public Modifiers Mods => Ledger.Compute(Config, Rank.Rank);
        public int CoinsOutstanding => coinsOutstanding;
        public float ElapsedToday => elapsed;

        /// <summary>Seconds into today when the last official task was finished, or -1 if the list is not finished.</summary>
        public float OfficialFinishedAt => officialFinishedAt;

        int DaySeed(int day) => unchecked(seed * 31 + day);

        // ---- Rooms ----

        public int JobCount(string jobId) => jobCounts.TryGetValue(jobId, out int n) ? n : 0;
        public bool HasTool(string toolId) => Ledger.Level(toolId) > 0;
        public bool IsAreaUnlocked(string areaId) => unlocked.Contains(areaId);

        /// <summary>Re-checks every room's opening conditions. Returns the rooms that opened just now.</summary>
        public IReadOnlyList<string> RefreshUnlocks()
        {
            var opened = new List<string>();
            foreach (var rule in Areas.Rules)
            {
                if (unlocked.Contains(rule.AreaId)) continue;
                if (Areas.IsUnlocked(rule.AreaId, Rank.Rank, JobCount, HasTool))
                {
                    unlocked.Add(rule.AreaId);
                    opened.Add(rule.AreaId);
                }
            }
            return opened;
        }

        /// <summary>Opens every room (tests and debug tools).</summary>
        public void ForceUnlockAll()
        {
            foreach (var rule in Areas.Rules) unlocked.Add(rule.AreaId);
        }

        /// <summary>Whether a job may be on the list: rank reached, tool owned, and every room it visits is open.</summary>
        public bool IsJobAvailable(TaskDefinition def)
        {
            if (def.MinRank > Rank.Rank) return false;
            if (def.RequiresTool != null && !HasTool(def.RequiresTool)) return false;
            foreach (var step in def.Steps)
            {
                if (!unlocked.Contains(Areas.AreaOfStation(step.StationId))) return false;
            }
            return true;
        }

        /// <summary>Jobs that would open the next closed room, in the order the rooms open.</summary>
        List<string> UnlockPriorities()
        {
            var result = new List<string>();
            foreach (var rule in Areas.Rules)
            {
                if (unlocked.Contains(rule.AreaId) || rule.UnlockJobId == null) continue;
                var def = Tasks.Find(rule.UnlockJobId);
                if (def != null && IsJobAvailable(def) && !result.Contains(def.Id)) result.Add(def.Id);
            }
            return result;
        }

        // ---- Debug starts and replaying a day ----

        /// <summary>Puts a fresh session at a chosen level (feel-test and debug use). Call before the first BeginDay.</summary>
        public void ApplyStartProfile(StartProfile profile)
        {
            Rank.SetRank(profile.Rank);
            var level = CurrentLevel;
            Day = (profile.Day > 0 ? profile.Day : level.StartDay) - 1;
            Wallet.Set(profile.Coins);
            Ledger.Grant(profile.Loadout, Rank.Rank);
            if (profile.AllRoomsOpen) ForceUnlockAll();
            RefreshUnlocks();
            if (profile.MarkAllJobsSeen)
            {
                foreach (var def in Tasks.All) seen.Add(def.Id);
            }
            Reliability.Set(profile.Reliability);
            Rollout.RaiseFloor(level.RolloutFloor);
            Rollout.Set(profile.Rollout >= 0f ? profile.Rollout : level.StartRollout);
        }

        /// <summary>Hands out tools and coins (debug).</summary>
        public void GrantTools(ToolLoadout loadout, int coins)
        {
            Ledger.Grant(loadout, Rank.Rank);
            Wallet.Add(coins);
        }

        sealed class DaySnapshot
        {
            public int Day;
            public int Balance;
            public Dictionary<string, int> Tools;
            public int CompletedTasks;
            public int RankValue;
            public Dictionary<string, int> JobCounts;
            public HashSet<string> Unlocked;
            public HashSet<string> Seen;
            public HashSet<string> SeenErrands;
            public HashSet<string> ScrapsFound;
            public Dictionary<string, JobTextEntry> ProceduresShown;
            public float Reliability;
            public float RolloutValue;
            public float RolloutFloor;
            public int Outstanding;
            public int RestoredYesterday;
            public int WellnessSeen;
        }

        DaySnapshot Capture()
        {
            return new DaySnapshot
            {
                Day = Day,
                Balance = Wallet.Balance,
                Tools = Ledger.Snapshot(),
                CompletedTasks = Rank.CompletedTasks,
                RankValue = Rank.Rank,
                JobCounts = new Dictionary<string, int>(jobCounts),
                Unlocked = new HashSet<string>(unlocked),
                Seen = new HashSet<string>(seen),
                SeenErrands = new HashSet<string>(seenErrands),
                ScrapsFound = new HashSet<string>(scrapsFound),
                ProceduresShown = new Dictionary<string, JobTextEntry>(proceduresShown),
                Reliability = Reliability.Value,
                RolloutValue = Rollout.Value,
                RolloutFloor = Rollout.Floor,
                Outstanding = coinsOutstanding,
                RestoredYesterday = restoredYesterday,
                WellnessSeen = wellnessSeen,
            };
        }

        /// <summary>True when a day has started, so there is a snapshot to go back to.</summary>
        public bool CanReplayDay => daySnapshot != null;

        /// <summary>
        /// Goes back to the state at the start of the current day (before its list was planned). Call BeginDay afterwards:
        /// the same seed gives the same list.
        /// </summary>
        public bool RestoreDayStart()
        {
            var s = daySnapshot;
            if (s == null) return false;

            Day = s.Day;
            Wallet.Set(s.Balance);
            Ledger.Restore(s.Tools);
            Rank.Restore(s.CompletedTasks, s.RankValue);
            jobCounts.Clear();
            foreach (var pair in s.JobCounts) jobCounts[pair.Key] = pair.Value;
            Replace(unlocked, s.Unlocked);
            Replace(seen, s.Seen);
            Replace(seenErrands, s.SeenErrands);
            Replace(scrapsFound, s.ScrapsFound);
            proceduresShown.Clear();
            foreach (var pair in s.ProceduresShown) proceduresShown[pair.Key] = pair.Value;
            Reliability.Set(s.Reliability);
            Rollout.Restore(s.RolloutValue, s.RolloutFloor);
            coinsOutstanding = s.Outstanding;
            restoredYesterday = s.RestoredYesterday;
            wellnessSeen = s.WellnessSeen;

            carried.Clear();
            today.Clear();
            todayErrands.Clear();
            dayEnded = false;
            return true;
        }

        static void Replace(HashSet<string> target, HashSet<string> source)
        {
            target.Clear();
            foreach (var item in source) target.Add(item);
        }

        // ---- The day ----

        /// <summary>Starts the next day: plans the list, starts the clock, and auto-completes automated tasks.</summary>
        public IReadOnlyList<TaskInstance> BeginDay()
        {
            daySnapshot = Capture();
            Day++;
            coinsEarnedToday = 0;
            elapsed = 0f;
            dayEnded = false;
            errandsStartedToday = 0;
            carried.Clear();
            readings.Clear();
            var mods = Mods;
            today.Clear();
            today.AddRange(DayPlanner.Plan(Config, Tasks, Rank.Rank, seen, new HashSet<string>(mods.AutomatedTaskIds), DaySeed(Day),
                IsJobAvailable, UnlockPriorities()));
            Clock = new DayClock(Config.DayLengthFor(Rank.Rank) + mods.DayBonusSeconds);
            oddities = OddityBank.Pick(Rank.Rank, today.SelectMany(t => t.Definition.Steps).Select(s => s.StationId), DaySeed(Day) + 99);

            todayErrands.Clear();
            todayErrands.AddRange(ErrandPlanner.Plan(Rank.Rank, DaySeed(Day) + 7, seenErrands,
                station => unlocked.Contains(Areas.AreaOfStation(station))));
            foreach (var e in todayErrands) seenErrands.Add(e.Definition.Id);

            officialFinishedAt = today.All(t => t.IsFinished) ? 0f : -1f;
            return today;
        }

        /// <summary>Starts a day with a hand-picked list (used by tests and by debug tools).</summary>
        public IReadOnlyList<TaskInstance> BeginDayWith(IEnumerable<TaskInstance> tasks)
        {
            BeginDay();
            today.Clear();
            today.AddRange(tasks);
            officialFinishedAt = today.All(t => t.IsFinished) ? 0f : -1f;
            return today;
        }

        /// <summary>Task types on today's list that the player has not met before. Call after BeginDay and before MarkSeen.</summary>
        public List<TaskDefinition> NewTaskTypesToday()
        {
            return today.Select(t => t.Definition).Distinct().Where(d => !seen.Contains(d.Id)).ToList();
        }

        public void MarkSeen()
        {
            foreach (var t in today) seen.Add(t.Definition.Id);
        }

        /// <summary>Tomorrow's list, for the Day Planner. Same seed as the real plan, so it matches (unless a room opens in between).</summary>
        public List<TaskInstance> PreviewNextDay()
        {
            var seenAfter = new HashSet<string>(seen);
            foreach (var t in today) seenAfter.Add(t.Definition.Id);
            var automated = new HashSet<string>(Mods.AutomatedTaskIds);
            return DayPlanner.Plan(Config, Tasks, Rank.Rank, seenAfter, automated, DaySeed(Day + 1), IsJobAvailable, UnlockPriorities());
        }

        /// <summary>Advances the clock. Returns true on the tick the day ends.</summary>
        public bool Tick(float dt)
        {
            if (Clock == null) return false;
            if (!Clock.Expired) elapsed += dt;
            ExpireErrandWindows();
            return Clock.Tick(dt);
        }

        /// <summary>A wrong answer in a mini-game costs time.</summary>
        public void ApplyTimePenalty(float seconds)
        {
            Clock?.Penalty(seconds);
        }

        // ---- Steps ----

        /// <summary>The unfinished step available at this station right now, given what the player is carrying and any waiting time.</summary>
        public bool TryGetAvailableStep(string stationId, out TaskInstance task, out TaskStep step)
        {
            foreach (var t in today)
            {
                if (t.IsFinished) continue;
                var s = t.CurrentStep;
                if (s.StationId != stationId) continue;
                if (s.RequiresCarry != null && !carried.Contains(s.RequiresCarry)) continue;
                if (s.GivesCarry != null && carried.Count >= CarryCapacity) continue;
                if (s.MinSecondsAfterPrevious > 0f && t.StepIndex > 0 && elapsed - t.LastStepAt < s.MinSecondsAfterPrevious) continue;
                task = t;
                step = s;
                return true;
            }
            task = null;
            step = null;
            return false;
        }

        /// <summary>True when the step at this station would hand the player something but their hands are full.</summary>
        public bool IsHandsFullAt(string stationId)
        {
            foreach (var t in today)
            {
                if (t.IsFinished) continue;
                var s = t.CurrentStep;
                if (s.StationId == stationId && s.GivesCarry != null && carried.Count >= CarryCapacity) return true;
            }
            return false;
        }

        /// <summary>When a step at this station must wait (switch off, wait, switch on), the seconds still to wait; otherwise false.</summary>
        public bool IsWaitingAt(string stationId, out float secondsLeft)
        {
            foreach (var t in today)
            {
                if (t.IsFinished) continue;
                var s = t.CurrentStep;
                if (s.StationId != stationId || s.MinSecondsAfterPrevious <= 0f || t.StepIndex == 0) continue;
                float left = s.MinSecondsAfterPrevious - (elapsed - t.LastStepAt);
                if (left > 0f)
                {
                    secondsLeft = left;
                    return true;
                }
            }
            secondsLeft = 0f;
            return false;
        }

        /// <summary>Records a finished step at a station. Does nothing if no step is available there.</summary>
        public StepResult CompleteStep(string stationId)
        {
            if (Clock == null || Clock.Expired || dayEnded) return new StepResult(false, false, 0, null);
            if (!TryGetAvailableStep(stationId, out var task, out var step)) return new StepResult(false, false, 0, null);

            if (step.RequiresCarry != null) carried.Remove(step.RequiresCarry);
            if (step.GivesCarry != null) carried.Add(step.GivesCarry);

            string readout = null;
            if (step.IsReadout)
            {
                readout = Readouts.ForStation(stationId, Day, Rank.Rank, seed);
                if (readout != null) readings[stationId] = readout;
            }
            else if (step.RequiresCarry == "reading")
            {
                // The clipboard step writes down what was read earlier.
                readout = readings.Values.FirstOrDefault();
            }

            string oddity = null;
            if (oddities.TryGetValue(stationId, out var text))
            {
                oddity = text;
                oddities.Remove(stationId);
            }

            task.LastStepAt = elapsed;
            bool done = task.AdvanceStep();
            int coins = 0;
            IReadOnlyList<string> opened = null;
            if (done)
            {
                coins = CoinsFor(task.Definition);
                coinsOutstanding += coins;
                coinsEarnedToday += coins;
                jobCounts[task.Definition.Id] = JobCount(task.Definition.Id) + 1;
                float perTask = Mods.SecondsPerTask;
                if (perTask > 0f) Clock.AddSeconds(perTask);
                opened = RefreshUnlocks();
                if (officialFinishedAt < 0f && today.All(t => t.IsFinished)) officialFinishedAt = elapsed;
            }
            return new StepResult(true, done, coins, task, step, readout, oddity, opened);
        }

        int CoinsFor(TaskDefinition def) => def.Coins > 0 ? def.Coins : Config.CoinsPerTask;

        // ---- The second list ----

        /// <summary>Adds a specific errand to today's second list (tests and the debug overlay). Does nothing if it is unknown or already offered.</summary>
        public bool OfferErrand(string errandId)
        {
            var def = ErrandCatalog.Find(errandId);
            if (def == null || todayErrands.Any(e => e.Definition.Id == errandId)) return false;
            todayErrands.Add(new ErrandInstance(def));
            seenErrands.Add(errandId);
            return true;
        }

        /// <summary>An errand step that can be done at this station right now, given what the player is carrying.</summary>
        public bool TryGetAlternateStep(string stationId, out ErrandInstance errand, out ErrandStep step)
        {
            foreach (var e in todayErrands)
            {
                if (e.IsDone) continue;
                var s = e.CurrentStep;
                if (s.StationId != stationId) continue;
                if (s.RequiresCarry != null && !carried.Contains(s.RequiresCarry)) continue;
                if (s.GivesCarry != null && carried.Count >= CarryCapacity) continue;
                errand = e;
                step = s;
                return true;
            }
            errand = null;
            step = null;
            return false;
        }

        /// <summary>Records a finished errand step. Does nothing if none is available at the station.</summary>
        public AlternateResult CompleteAlternate(string stationId)
        {
            if (Clock == null || Clock.Expired || dayEnded) return new AlternateResult(false, false, null, null);
            if (!TryGetAlternateStep(stationId, out var errand, out var step)) return new AlternateResult(false, false, null, null);

            if (step.RequiresCarry != null) carried.Remove(step.RequiresCarry);
            if (step.GivesCarry != null) carried.Add(step.GivesCarry);

            if (errand.Status == ErrandStatus.Offered) errandsStartedToday++;
            bool done = errand.Advance(elapsed);
            return new AlternateResult(true, done, errand, step);
        }

        // An errand step with a window starts the errand again if the window runs out.
        void ExpireErrandWindows()
        {
            foreach (var e in todayErrands)
            {
                if (e.IsDone || e.StepIndex == 0) continue;
                var s = e.CurrentStep;
                if (s.MaxSecondsAfterPrevious > 0f && elapsed - e.LastStepAt > s.MaxSecondsAfterPrevious) e.Restart();
            }
        }

        // ---- The reveal: procedures, scraps, the morning slip ----

        /// <summary>Jobs on today's list whose wording has changed since the player last saw it.</summary>
        public List<TaskDefinition> ProcedureUpdatesToday()
        {
            var result = new List<TaskDefinition>();
            foreach (var def in today.Select(t => t.Definition).Distinct())
            {
                var entry = JobText.For(def.Id, RevealStage);
                if (entry == null) continue;
                if (proceduresShown.TryGetValue(def.Id, out var shown) && ReferenceEquals(shown, entry)) continue;
                result.Add(def);
            }
            return result;
        }

        public void MarkProceduresShown()
        {
            foreach (var t in today)
            {
                var entry = JobText.For(t.Definition.Id, RevealStage);
                if (entry != null) proceduresShown[t.Definition.Id] = entry;
            }
        }

        /// <summary>The next scrap to find, or null. It is not marked found until <see cref="MarkScrapFound"/>.</summary>
        public Scrap NextScrap() => ScrapBank.Next(RevealStage, JobCount, scrapsFound);

        public void MarkScrapFound(string id) => scrapsFound.Add(id);

        public IReadOnlyCollection<string> ScrapsFound => scrapsFound;

        /// <summary>The morning slip from the town, or null before the reveal reaches it.</summary>
        public string LedgerText()
        {
            if (RevealStage < 2 || Day <= 1) return null;
            return LedgerSlip.Text(RevealStage, Rollout.Value, restoredYesterday);
        }

        /// <summary>The next mandatory wellness session (title, body). Counts how many have been shown.</summary>
        public (string title, string body) NextWellness()
        {
            var result = (WellnessText.Title(wellnessSeen), WellnessText.Body(wellnessSeen));
            wellnessSeen++;
            return result;
        }

        // ---- Coins ----

        /// <summary>The player picked up coins.</summary>
        public void CollectCoins(int value)
        {
            int v = Math.Min(value, coinsOutstanding);
            if (v <= 0) return;
            coinsOutstanding -= v;
            Wallet.Add(v);
        }

        /// <summary>Anything left on the floor goes to the player when they sleep. Nothing is lost.</summary>
        public int CollectAllOutstanding()
        {
            int v = coinsOutstanding;
            if (v > 0) Wallet.Add(v);
            coinsOutstanding = 0;
            return v;
        }

        /// <summary>Ends the day: pays automated tasks, counts completions, applies reliability and rollout, and records rank progress.</summary>
        public DaySummary EndDay()
        {
            dayEnded = true;
            carried.Clear();
            var summary = new DaySummary { Day = Day, TasksOnList = today.Count };
            int automatedCoins = 0;
            int unfinished = 0;
            foreach (var t in today)
            {
                if (t.Status == TaskStatus.Done) summary.TasksCompleted++;
                else if (t.Status == TaskStatus.Automated)
                {
                    summary.TasksAutomated++;
                    automatedCoins += CoinsFor(t.Definition);
                }
                else unfinished++;
            }

            // Automated tasks pay coins straight away but do not count toward rank or unlocks.
            if (automatedCoins > 0)
            {
                Wallet.Add(automatedCoins);
                coinsEarnedToday += automatedCoins;
            }

            // Reliability, then the company's rollout with whatever the second list pushed back.
            Reliability.EndOfDay(unfinished);
            float errandEffect = 0f;
            int restored = 0;
            int errandsDone = 0;
            foreach (var e in todayErrands)
            {
                if (!e.IsDone) continue;
                errandsDone++;
                errandEffect += e.Definition.RolloutEffect;
                restored += e.Definition.Households;
            }
            summary.RolloutChange = Rollout.EndOfDay(CurrentLevel.ParRolloutPerDay, errandEffect);
            restoredYesterday = restored;

            summary.CoinsEarned = coinsEarnedToday;
            summary.RankedUp = Rank.Record(summary.TasksCompleted, Reliability.CanPromote);
            summary.NewRank = Rank.Rank;
            summary.PromotionHeld = Rank.PromotionWaiting;
            if (summary.RankedUp) Rollout.RaiseFloor(CurrentLevel.RolloutFloor);
            summary.NewlyUnlocked = RefreshUnlocks();

            summary.Reliability = Reliability.Value;
            summary.Rollout = Rollout.Value;
            summary.ErrandsDone = errandsDone;

            var stats = new DayStats
            {
                Level = Rank.Rank + 1,
                Day = Day,
                DaySeconds = Clock != null ? Clock.Total : 0f,
                TasksTotal = today.Count,
                TasksDone = summary.TasksCompleted + summary.TasksAutomated,
                OfficialFinishedAt = officialFinishedAt,
                ErrandsOffered = todayErrands.Count,
                ErrandsStarted = errandsStartedToday,
                ErrandsDone = errandsDone,
                Reliability = Reliability.Value,
                Rollout = Rollout.Value,
                Coins = coinsEarnedToday,
            };
            summary.Stats = stats;
            LastStats = stats;
            return summary;
        }
    }
}
