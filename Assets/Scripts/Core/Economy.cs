using System;
using System.Collections.Generic;

namespace Noted.Core
{
    public sealed class Wallet
    {
        public int Balance { get; private set; }

        public void Add(int amount)
        {
            if (amount > 0) Balance += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0 || amount > Balance) return false;
            Balance -= amount;
            return true;
        }
    }

    /// <summary>Rank is earned by completing tasks. Levelling up consumes no coins.</summary>
    public sealed class RankTracker
    {
        readonly int[] thresholds;

        public int CompletedTasks { get; private set; }
        public int Rank { get; private set; }

        public RankTracker(int[] rankThresholds)
        {
            thresholds = rankThresholds;
        }

        /// <summary>Adds completed tasks. Returns true when the rank went up as a result.</summary>
        public bool Record(int completed)
        {
            if (completed <= 0) return false;
            CompletedTasks += completed;
            int newRank = 0;
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (CompletedTasks >= thresholds[i]) newRank = i + 1;
            }
            bool up = newRank > Rank;
            Rank = newRank;
            return up;
        }

        /// <summary>Total completed tasks needed for the next rank, or -1 at the top.</summary>
        public int NextThreshold => Rank < thresholds.Length ? thresholds[Rank] : -1;
    }

    public enum UpgradeKind
    {
        LongerDay,
        TidyHabits,
        FasterHands,
        RunningShoes,
        LongReach,
        TaskMarker,
        MorningBriefing,
        CoinMagnet,
        Automation,
    }

    public sealed class UpgradeDefinition
    {
        public readonly string Id;
        public readonly string Name;
        public readonly string Description;
        public readonly UpgradeKind Kind;
        public readonly int BaseCost;
        public readonly float CostGrowth;
        public readonly int MaxLevel;
        public readonly string AutomatedTaskId;

        public UpgradeDefinition(string id, string name, string description, UpgradeKind kind, int baseCost, float costGrowth, int maxLevel, string automatedTaskId = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Kind = kind;
            BaseCost = baseCost;
            CostGrowth = costGrowth;
            MaxLevel = maxLevel;
            AutomatedTaskId = automatedTaskId;
        }
    }

    public sealed class UpgradeCatalog
    {
        public readonly IReadOnlyList<UpgradeDefinition> All;

        public UpgradeCatalog(IReadOnlyList<UpgradeDefinition> all)
        {
            All = all;
        }

        public UpgradeDefinition Find(string id)
        {
            foreach (var u in All)
            {
                if (u.Id == id) return u;
            }
            return null;
        }

        /// <summary>First-pass costs; all of this is for tuning by hand.</summary>
        public static UpgradeCatalog Default()
        {
            return new UpgradeCatalog(new[]
            {
                new UpgradeDefinition("longer_day", "Longer Day", "+15 seconds on the clock.", UpgradeKind.LongerDay, 40, 1.6f, 3),
                new UpgradeDefinition("faster_hands", "Faster Hands", "Tasks take 15% less time to finish.", UpgradeKind.FasterHands, 30, 1.6f, 3),
                new UpgradeDefinition("running_shoes", "Running Shoes", "Walk and run 10% faster.", UpgradeKind.RunningShoes, 40, 1.6f, 2),
                new UpgradeDefinition("long_reach", "Long Reach", "Work from 1 metre farther away.", UpgradeKind.LongReach, 30, 1.6f, 2),
                new UpgradeDefinition("tidy_habits", "Tidy Habits", "Each finished task adds 3 seconds to the clock.", UpgradeKind.TidyHabits, 70, 1f, 1),
                new UpgradeDefinition("task_marker", "Task Marker", "Beacons show where your tasks are.", UpgradeKind.TaskMarker, 30, 1f, 1),
                new UpgradeDefinition("morning_briefing", "Morning Briefing", "See tomorrow's list before you sleep.", UpgradeKind.MorningBriefing, 25, 1f, 1),
                new UpgradeDefinition("coin_magnet", "Coin Magnet", "Coins find you from farther away.", UpgradeKind.CoinMagnet, 25, 1.6f, 2),
                new UpgradeDefinition("auto_coffee", "Automatic Coffee Maker", "Coffee is refilled for you every day.", UpgradeKind.Automation, 90, 1f, 1, "coffee"),
                new UpgradeDefinition("auto_printer", "Printer Service Contract", "The printer is cleared for you every day.", UpgradeKind.Automation, 100, 1f, 1, "printer"),
                new UpgradeDefinition("auto_shredder", "Shredder Schedule", "The shredder empties itself every day.", UpgradeKind.Automation, 90, 1f, 1, "shredder"),
            });
        }
    }

    /// <summary>What the player's upgrades add up to. Recomputed on demand; never stored.</summary>
    public readonly struct Modifiers
    {
        public readonly float DayBonusSeconds;
        public readonly float SecondsPerTask;
        public readonly float HoldMultiplier;
        public readonly float SpeedMultiplier;
        public readonly float ReachBonus;
        public readonly float MagnetRadius;
        public readonly bool MarkerEnabled;
        public readonly bool BriefingEnabled;
        public readonly IReadOnlyCollection<string> AutomatedTaskIds;

        public Modifiers(float dayBonus, float perTask, float hold, float speed, float reach, float magnet, bool marker, bool briefing, IReadOnlyCollection<string> automated)
        {
            DayBonusSeconds = dayBonus;
            SecondsPerTask = perTask;
            HoldMultiplier = hold;
            SpeedMultiplier = speed;
            ReachBonus = reach;
            MagnetRadius = magnet;
            MarkerEnabled = marker;
            BriefingEnabled = briefing;
            AutomatedTaskIds = automated;
        }
    }

    public sealed class UpgradeLedger
    {
        readonly UpgradeCatalog catalog;
        readonly Dictionary<string, int> levels = new Dictionary<string, int>();

        public UpgradeLedger(UpgradeCatalog upgradeCatalog)
        {
            catalog = upgradeCatalog;
        }

        public int Level(string id) => levels.TryGetValue(id, out int l) ? l : 0;

        public bool IsMaxed(UpgradeDefinition def) => Level(def.Id) >= def.MaxLevel;

        /// <summary>Cost of the next level, or -1 when maxed.</summary>
        public int CostFor(UpgradeDefinition def)
        {
            int level = Level(def.Id);
            if (level >= def.MaxLevel) return -1;
            return (int)Math.Round(def.BaseCost * Math.Pow(def.CostGrowth, level));
        }

        public bool TryBuy(Wallet wallet, UpgradeDefinition def)
        {
            int cost = CostFor(def);
            if (cost < 0) return false;
            if (!wallet.TrySpend(cost)) return false;
            levels[def.Id] = Level(def.Id) + 1;
            return true;
        }

        public Modifiers Compute(GameConfig config, int rank)
        {
            float dayBonus = 15f * Level("longer_day");
            float perTask = 3f * Level("tidy_habits");
            float hold = (float)Math.Pow(0.85, Level("faster_hands"));
            float speed = 1f + 0.1f * Level("running_shoes");
            if (rank >= 2) speed *= 1f + config.ShoesSpeedBonus;
            float reach = 1f * Level("long_reach");
            float magnet = 2.5f * Level("coin_magnet");
            var automated = new List<string>();
            foreach (var def in catalog.All)
            {
                if (def.Kind == UpgradeKind.Automation && Level(def.Id) > 0) automated.Add(def.AutomatedTaskId);
            }
            return new Modifiers(dayBonus, perTask, hold, speed, reach, magnet,
                Level("task_marker") > 0, Level("morning_briefing") > 0, automated);
        }
    }
}
