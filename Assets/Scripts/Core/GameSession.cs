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
    }

    /// <summary>What happened when a step was finished.</summary>
    public readonly struct StepResult
    {
        public readonly bool Accepted;
        public readonly bool TaskDone;
        public readonly int CoinsToSpawn;
        public readonly TaskInstance Task;

        public StepResult(bool accepted, bool taskDone, int coinsToSpawn, TaskInstance task)
        {
            Accepted = accepted;
            TaskDone = taskDone;
            CoinsToSpawn = coinsToSpawn;
            Task = task;
        }
    }

    /// <summary>
    /// All the rules of the core loop in plain C#: the day clock, the task list, carrying, coins, rank and upgrades.
    /// The scene drives it and reports back; it holds no engine objects.
    /// </summary>
    public sealed class GameSession
    {
        public readonly GameConfig Config;
        public readonly TaskCatalog Tasks;
        public readonly UpgradeCatalog Upgrades;
        public readonly Wallet Wallet = new Wallet();
        public readonly RankTracker Rank;
        public readonly UpgradeLedger Ledger;
        public readonly ComplaintBank Complaints = new ComplaintBank();

        readonly int seed;
        readonly HashSet<string> seen = new HashSet<string>();
        readonly List<TaskInstance> today = new List<TaskInstance>();
        readonly List<string> carried = new List<string>();
        int coinsOutstanding;
        int coinsEarnedToday;

        public int Day { get; private set; }
        public string Code { get; }
        public DayClock Clock { get; private set; }
        /// <summary>Items being carried right now (a bean sack, a filter...), up to the carry capacity.</summary>
        public IReadOnlyList<string> Carried => carried;
        public IReadOnlyList<TaskInstance> TodaysTasks => today;

        public GameSession(GameConfig config, int randomSeed)
        {
            Config = config;
            seed = randomSeed;
            Tasks = TaskCatalog.Default();
            Upgrades = UpgradeCatalog.Default();
            Rank = new RankTracker(config.RankThresholds);
            Ledger = new UpgradeLedger(Upgrades);
            Code = CodeGenerator.Generate(randomSeed);
        }

        public int CarryCapacity => Config.BaseCarryCapacity + Mods.CarryBonus;
        public bool IsCarrying(string token) => carried.Contains(token);

        public int CurrentRank => Rank.Rank;
        public Modifiers Mods => Ledger.Compute(Config, Rank.Rank);
        public int CoinsOutstanding => coinsOutstanding;

        int DaySeed(int day) => unchecked(seed * 31 + day);

        /// <summary>Starts the next day: plans the list, starts the clock, and auto-completes automated tasks.</summary>
        public IReadOnlyList<TaskInstance> BeginDay()
        {
            Day++;
            coinsEarnedToday = 0;
            carried.Clear();
            var mods = Mods;
            today.Clear();
            today.AddRange(DayPlanner.Plan(Config, Tasks, Rank.Rank, seen, new HashSet<string>(mods.AutomatedTaskIds), DaySeed(Day)));
            Clock = new DayClock(Config.DayLengthSeconds + mods.DayBonusSeconds);
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

        /// <summary>Tomorrow's list, for Morning Briefing. Same seed as the real plan, so it matches.</summary>
        public List<TaskInstance> PreviewNextDay()
        {
            var seenAfter = new HashSet<string>(seen);
            foreach (var t in today) seenAfter.Add(t.Definition.Id);
            var automated = new HashSet<string>(Mods.AutomatedTaskIds);
            return DayPlanner.Plan(Config, Tasks, Rank.Rank, seenAfter, automated, DaySeed(Day + 1));
        }

        /// <summary>Advances the clock. Returns true on the tick the day ends.</summary>
        public bool Tick(float dt) => Clock != null && Clock.Tick(dt);

        /// <summary>The unfinished task step available at this station right now, given what the player is carrying.</summary>
        public bool TryGetAvailableStep(string stationId, out TaskInstance task, out TaskStep step)
        {
            foreach (var t in today)
            {
                if (t.IsFinished) continue;
                var s = t.CurrentStep;
                if (s.StationId != stationId) continue;
                if (s.RequiresCarry != null && !carried.Contains(s.RequiresCarry)) continue;
                if (s.GivesCarry != null && carried.Count >= CarryCapacity) continue;
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

        /// <summary>Records a finished hold at a station. Does nothing if no step is available there.</summary>
        public StepResult CompleteStep(string stationId)
        {
            if (Clock == null || Clock.Expired) return new StepResult(false, false, 0, null);
            if (!TryGetAvailableStep(stationId, out var task, out var step)) return new StepResult(false, false, 0, null);

            if (step.RequiresCarry != null) carried.Remove(step.RequiresCarry);
            if (step.GivesCarry != null) carried.Add(step.GivesCarry);

            bool done = task.AdvanceStep();
            int coins = 0;
            if (done)
            {
                coins = Config.CoinsPerTask;
                coinsOutstanding += coins;
                coinsEarnedToday += coins;
                float perTask = Mods.SecondsPerTask;
                if (perTask > 0f) Clock.AddSeconds(perTask);
            }
            return new StepResult(true, done, coins, task);
        }

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
            foreach (var t in today)
            {
                if (t.Status == TaskStatus.Done) summary.TasksCompleted++;
                else if (t.Status == TaskStatus.Automated) summary.TasksAutomated++;
            }

            // Automated tasks pay coins straight away but do not count toward rank.
            int automatedCoins = summary.TasksAutomated * Config.CoinsPerTask;
            if (automatedCoins > 0)
            {
                Wallet.Add(automatedCoins);
                coinsEarnedToday += automatedCoins;
            }

            summary.CoinsEarned = coinsEarnedToday;
            summary.RankedUp = Rank.Record(summary.TasksCompleted);
            summary.NewRank = Rank.Rank;
            return summary;
        }
    }
}
