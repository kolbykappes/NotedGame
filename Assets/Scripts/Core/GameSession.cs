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

    /// <summary>
    /// All the rules of the core loop in plain C#: the day clock, the task list, carrying, coins, rank, upgrades, which rooms are open,
    /// readings and oddities. The scene drives it and reports back; it holds no engine objects.
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

        readonly int seed;
        readonly HashSet<string> seen = new HashSet<string>();
        readonly List<TaskInstance> today = new List<TaskInstance>();
        readonly List<string> carried = new List<string>();
        readonly Dictionary<string, int> jobCounts = new Dictionary<string, int>();
        readonly HashSet<string> unlocked = new HashSet<string>();
        readonly Dictionary<string, string> readings = new Dictionary<string, string>();
        Dictionary<string, string> oddities = new Dictionary<string, string>();
        float elapsed;
        int coinsOutstanding;
        int coinsEarnedToday;

        public int Day { get; private set; }
        public string Code { get; }
        public DayClock Clock { get; private set; }

        /// <summary>Items being carried right now (a bean sack, a filter...), up to the carry capacity.</summary>
        public IReadOnlyList<string> Carried => carried;
        public IReadOnlyList<TaskInstance> TodaysTasks => today;
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
        public Modifiers Mods => Ledger.Compute(Config, Rank.Rank);
        public int CoinsOutstanding => coinsOutstanding;
        public float ElapsedToday => elapsed;

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

        // ---- The day ----

        /// <summary>Starts the next day: plans the list, starts the clock, and auto-completes automated tasks.</summary>
        public IReadOnlyList<TaskInstance> BeginDay()
        {
            Day++;
            coinsEarnedToday = 0;
            elapsed = 0f;
            carried.Clear();
            readings.Clear();
            var mods = Mods;
            today.Clear();
            today.AddRange(DayPlanner.Plan(Config, Tasks, Rank.Rank, seen, new HashSet<string>(mods.AutomatedTaskIds), DaySeed(Day),
                IsJobAvailable, UnlockPriorities()));
            Clock = new DayClock(Config.DayLengthSeconds + mods.DayBonusSeconds);
            oddities = OddityBank.Pick(Rank.Rank, today.SelectMany(t => t.Definition.Steps).Select(s => s.StationId), DaySeed(Day) + 99);
            return today;
        }

        /// <summary>Starts a day with a hand-picked list (used by tests and by debug tools).</summary>
        public IReadOnlyList<TaskInstance> BeginDayWith(IEnumerable<TaskInstance> tasks)
        {
            BeginDay();
            today.Clear();
            today.AddRange(tasks);
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
            if (Clock == null || Clock.Expired) return new StepResult(false, false, 0, null);
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
            }
            return new StepResult(true, done, coins, task, step, readout, oddity, opened);
        }

        int CoinsFor(TaskDefinition def) => def.Coins > 0 ? def.Coins : Config.CoinsPerTask;

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

        /// <summary>Ends the day: pays automated tasks, counts completions and records rank progress.</summary>
        public DaySummary EndDay()
        {
            carried.Clear();
            var summary = new DaySummary { Day = Day, TasksOnList = today.Count };
            int automatedCoins = 0;
            foreach (var t in today)
            {
                if (t.Status == TaskStatus.Done) summary.TasksCompleted++;
                else if (t.Status == TaskStatus.Automated)
                {
                    summary.TasksAutomated++;
                    automatedCoins += CoinsFor(t.Definition);
                }
            }

            // Automated tasks pay coins straight away but do not count toward rank or unlocks.
            if (automatedCoins > 0)
            {
                Wallet.Add(automatedCoins);
                coinsEarnedToday += automatedCoins;
            }

            summary.CoinsEarned = coinsEarnedToday;
            summary.RankedUp = Rank.Record(summary.TasksCompleted);
            summary.NewRank = Rank.Rank;
            summary.NewlyUnlocked = RefreshUnlocks();
            return summary;
        }
    }
}
