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

        /// <summary>Sets the balance outright (debug starts and replaying a day).</summary>
        public void Set(int balance)
        {
            Balance = Math.Max(0, balance);
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

        /// <summary>The rank the completed-task count has earned, ignoring any hold on promotion.</summary>
        public int EarnedRank
        {
            get
            {
                int earned = 0;
                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (CompletedTasks >= thresholds[i]) earned = i + 1;
                }
                return earned;
            }
        }

        /// <summary>True when enough tasks are done for the next rank but the promotion is being held back.</summary>
        public bool PromotionWaiting => EarnedRank > Rank;

        /// <summary>
        /// Adds completed tasks. Returns true when the rank went up as a result. With <paramref name="canPromote"/> false the tasks
        /// still count but the rank stays where it is until a later call allows it.
        /// </summary>
        public bool Record(int completed, bool canPromote = true)
        {
            if (completed > 0) CompletedTasks += completed;
            int newRank = EarnedRank;
            if (!canPromote && newRank > Rank) newRank = Rank;
            bool up = newRank > Rank;
            Rank = newRank;
            return up;
        }

        /// <summary>Puts the tracker back to a saved state (replaying a day).</summary>
        public void Restore(int completedTasks, int rank)
        {
            CompletedTasks = Math.Max(0, completedTasks);
            Rank = Math.Max(0, Math.Min(rank, thresholds.Length));
        }

        /// <summary>Jumps straight to a rank (debug starts). The completed-task count is raised to that rank's threshold if it is lower.</summary>
        public void SetRank(int rank)
        {
            rank = Math.Max(0, Math.Min(rank, thresholds.Length));
            Rank = rank;
            int needed = rank > 0 ? thresholds[rank - 1] : 0;
            if (CompletedTasks < needed) CompletedTasks = needed;
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
        CarryCapacity,
        /// <summary>A tool that opens a door or enables a job rather than changing a number.</summary>
        AccessTool,
        /// <summary>A tool for jobs that do not exist yet. Shown locked in the locker; has no effect.</summary>
        FutureTool,
    }

    public static class UpgradeCategory
    {
        public const string Tools = "TOOLS";
        public const string Contracts = "SERVICE CONTRACTS";
        public const string Later = "FOR LATER JOBS";
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
        public readonly string Category;
        public readonly int MinRank;

        /// <summary>False for tools whose jobs are not in the game yet. They are listed but cannot be bought.</summary>
        public readonly bool Available;

        public UpgradeDefinition(string id, string name, string description, UpgradeKind kind, int baseCost, float costGrowth, int maxLevel,
            string automatedTaskId = null, string category = UpgradeCategory.Tools, int minRank = 0, bool available = true)
        {
            Id = id;
            Name = name;
            Description = description;
            Kind = kind;
            BaseCost = baseCost;
            CostGrowth = costGrowth;
            MaxLevel = maxLevel;
            AutomatedTaskId = automatedTaskId;
            Category = category;
            MinRank = minRank;
            Available = available;
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

        /// <summary>
        /// The tool locker. Ids are kept from the first version of the shop so saved tests and notes still match.
        /// First-pass costs; all of this is for tuning by hand.
        /// </summary>
        public static UpgradeCatalog Default()
        {
            return new UpgradeCatalog(new[]
            {
                // Tools that help with today's jobs.
                new UpgradeDefinition("hand_cart", "Hand Cart", "Carry 2 more things at once.", UpgradeKind.CarryCapacity, 35, 1f, 1),
                new UpgradeDefinition("faster_hands", "Multitool", "Each use takes 15% less time.", UpgradeKind.FasterHands, 30, 1.6f, 3),
                new UpgradeDefinition("running_shoes", "Running Shoes", "Walk and run 10% faster.", UpgradeKind.RunningShoes, 40, 1.6f, 2),
                new UpgradeDefinition("long_reach", "Reach Grabber", "Work from 1 metre farther away.", UpgradeKind.LongReach, 30, 1.6f, 2),
                new UpgradeDefinition("task_marker", "Two-Way Radio", "Beacons show where your tasks are.", UpgradeKind.TaskMarker, 30, 1f, 1),
                new UpgradeDefinition("morning_briefing", "Day Planner", "See tomorrow's list before you sleep.", UpgradeKind.MorningBriefing, 25, 1f, 1),
                new UpgradeDefinition("coin_magnet", "Magnetic Pouch", "Coins find you from farther away.", UpgradeKind.CoinMagnet, 25, 1.6f, 2),
                new UpgradeDefinition("longer_day", "Overtime Slip", "+15 seconds on the clock.", UpgradeKind.LongerDay, 40, 1.6f, 3),
                new UpgradeDefinition("tidy_habits", "Label Maker", "Each finished task adds 3 seconds to the clock.", UpgradeKind.TidyHabits, 70, 1f, 1),

                // Specialist tools: each opens something. The keycard opens the server hall; the strap unlocks drive jobs there.
                new UpgradeDefinition("staff_keycard", "Staff Keycard", "Opens the server hall door.", UpgradeKind.AccessTool, 80, 1f, 1, null, UpgradeCategory.Tools, 2),
                new UpgradeDefinition("anti_static_strap", "Anti-Static Strap", "Needed before touching server hardware. Adds drive jobs.", UpgradeKind.AccessTool, 70, 1f, 1, null, UpgradeCategory.Tools, 2),

                // Service contracts: the building does one task for you every day (pays coins, does not count toward rank).
                new UpgradeDefinition("auto_coffee", "Coffee Subscription", "Coffee is restocked for you every day.", UpgradeKind.Automation, 90, 1f, 1, "coffee", UpgradeCategory.Contracts),
                new UpgradeDefinition("auto_printer", "Printer Service Contract", "The printer is loaded for you every day.", UpgradeKind.Automation, 100, 1f, 1, "printer", UpgradeCategory.Contracts),
                new UpgradeDefinition("auto_shredder", "Shredder Schedule", "The shredder empties itself every day.", UpgradeKind.Automation, 90, 1f, 1, "shredder", UpgradeCategory.Contracts),

                // Tools for jobs deeper in the building. Listed so the player can see what is coming; not buyable yet.
                new UpgradeDefinition("flashlight", "Flashlight", "For dark corridors and cable trays.", UpgradeKind.FutureTool, 60, 1f, 1, null, UpgradeCategory.Later, 1, false),
                
                new UpgradeDefinition("cable_tester", "Cable Tester", "Finds the one bad cable in a hundred.", UpgradeKind.FutureTool, 90, 1f, 1, null, UpgradeCategory.Later, 2, false),
                new UpgradeDefinition("crash_cart", "Crash Cart", "Carry five things at once.", UpgradeKind.FutureTool, 140, 1f, 1, null, UpgradeCategory.Later, 2, false),
                new UpgradeDefinition("step_ladder", "Step Ladder", "For high shelves and ceiling panels.", UpgradeKind.FutureTool, 90, 1f, 1, null, UpgradeCategory.Later, 2, false),
                new UpgradeDefinition("thermal_camera", "Thermal Camera", "Find the racks that are running hot.", UpgradeKind.FutureTool, 150, 1f, 1, null, UpgradeCategory.Later, 2, false),
                new UpgradeDefinition("fire_extinguisher", "Fire Extinguisher", "Because it is a data center.", UpgradeKind.FutureTool, 60, 1f, 1, null, UpgradeCategory.Later, 2, false),
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
        public readonly int CarryBonus;
        public readonly bool MarkerEnabled;
        public readonly bool BriefingEnabled;
        public readonly IReadOnlyCollection<string> AutomatedTaskIds;

        public Modifiers(float dayBonus, float perTask, float hold, float speed, float reach, float magnet, int carry, bool marker, bool briefing, IReadOnlyCollection<string> automated)
        {
            DayBonusSeconds = dayBonus;
            SecondsPerTask = perTask;
            HoldMultiplier = hold;
            SpeedMultiplier = speed;
            ReachBonus = reach;
            MagnetRadius = magnet;
            CarryBonus = carry;
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

        public void SetLevel(string id, int level)
        {
            if (level <= 0) levels.Remove(id);
            else levels[id] = level;
        }

        /// <summary>A copy of every owned level (used to replay a day).</summary>
        public Dictionary<string, int> Snapshot() => new Dictionary<string, int>(levels);

        public void Restore(IDictionary<string, int> snapshot)
        {
            levels.Clear();
            foreach (var pair in snapshot) levels[pair.Key] = pair.Value;
        }

        static readonly string[] BasicLoadout =
        {
            "hand_cart", "faster_hands", "running_shoes", "morning_briefing", "staff_keycard", "anti_static_strap",
        };

        /// <summary>Hands out tools for a debug start. Service contracts are never granted (they would do the jobs for the player).</summary>
        public void Grant(ToolLoadout loadout, int rank)
        {
            if (loadout == ToolLoadout.None) return;
            foreach (var def in catalog.All)
            {
                if (!def.Available || rank < def.MinRank || def.Kind == UpgradeKind.Automation) continue;
                if (loadout == ToolLoadout.All) SetLevel(def.Id, def.MaxLevel);
                else if (Array.IndexOf(BasicLoadout, def.Id) >= 0) SetLevel(def.Id, 1);
            }
        }

        /// <summary>Whether the tool can be bought at this rank. Tools for jobs not yet in the game never can.</summary>
        public bool IsUnlocked(UpgradeDefinition def, int rank) => def.Available && rank >= def.MinRank;

        /// <summary>Cost of the next level, or -1 when maxed.</summary>
        public int CostFor(UpgradeDefinition def)
        {
            int level = Level(def.Id);
            if (level >= def.MaxLevel) return -1;
            return (int)Math.Round(def.BaseCost * Math.Pow(def.CostGrowth, level));
        }

        public bool TryBuy(Wallet wallet, UpgradeDefinition def, int rank = int.MaxValue)
        {
            if (!IsUnlocked(def, rank)) return false;
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
            int carry = 2 * Level("hand_cart");
            var automated = new List<string>();
            foreach (var def in catalog.All)
            {
                if (def.Kind == UpgradeKind.Automation && Level(def.Id) > 0) automated.Add(def.AutomatedTaskId);
            }
            return new Modifiers(dayBonus, perTask, hold, speed, reach, magnet, carry,
                Level("task_marker") > 0, Level("morning_briefing") > 0, automated);
        }
    }
}
