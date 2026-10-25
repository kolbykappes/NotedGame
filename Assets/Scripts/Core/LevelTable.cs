using System;

namespace Noted.Core
{
    /// <summary>
    /// One level of the ladder: how long the day is, how many jobs are on the list, how far the story has been revealed, and how
    /// fast the company's rollout moves. The index in the table is the old "rank" (0 is Trainee); the UI says "Level" (rank + 1).
    /// Every number here is a first guess to be tuned from the day log.
    /// </summary>
    public sealed class LevelDef
    {
        public readonly string Title;
        public readonly float DaySeconds;
        public readonly int TaskCount;

        /// <summary>Total completed tasks needed to reach this level (0 for the first).</summary>
        public readonly int TasksToReach;

        /// <summary>0 clean, 1 first scrap, 2 sinister job names, 3 harm in numbers.</summary>
        public readonly int RevealStage;

        /// <summary>How far the company's rollout moves each day with no sabotage, in percentage points.</summary>
        public readonly float ParRolloutPerDay;

        /// <summary>Rollout cannot fall below this once the level is reached.</summary>
        public readonly float RolloutFloor;

        /// <summary>Where a jump to this level starts rollout and the day counter (used by the debug start).</summary>
        public readonly float StartRollout;
        public readonly int StartDay;

        /// <summary>The training tape shown on promotion into this level. Null where the older built-in tapes apply.</summary>
        public readonly string TapeText;

        public LevelDef(string title, float daySeconds, int taskCount, int tasksToReach, int revealStage, float par, float floor,
            float startRollout, int startDay, string tapeText = null)
        {
            Title = title;
            DaySeconds = daySeconds;
            TaskCount = taskCount;
            TasksToReach = tasksToReach;
            RevealStage = revealStage;
            ParRolloutPerDay = par;
            RolloutFloor = floor;
            StartRollout = startRollout;
            StartDay = startDay;
            TapeText = tapeText;
        }
    }

    public sealed class LevelTable
    {
        readonly LevelDef[] levels;

        public LevelTable(LevelDef[] defs)
        {
            if (defs == null || defs.Length == 0) throw new ArgumentException("A level table needs at least one level.");
            levels = defs;
        }

        public int Count => levels.Length;

        /// <summary>The highest rank index in the table.</summary>
        public int MaxRank => levels.Length - 1;

        public LevelDef For(int rank) => levels[Math.Max(0, Math.Min(rank, levels.Length - 1))];

        /// <summary>Total completed tasks needed for each rank after the first (the array a RankTracker takes).</summary>
        public int[] Thresholds()
        {
            var t = new int[levels.Length - 1];
            for (int i = 1; i < levels.Length; i++) t[i - 1] = levels[i].TasksToReach;
            return t;
        }

        /// <summary>
        /// The five levels used for the feel test. Day lengths are set so a walker only just finishes the list (measured: a Trainee
        /// pair takes 52 to 62 s walking, a Specialist list of five about 73 s sprinting, so about 110 s walking).
        /// Floors rise 2 points a level; par is 1.06 points a day, so a player who never sabotages reaches the cap around day 90.
        /// </summary>
        public static LevelTable Default()
        {
            const float par = 1.06f;
            return new LevelTable(new[]
            {
                new LevelDef("Trainee",    60f,  2,  0, 0, par, 0f,  0f,  1),
                new LevelDef("Associate",  80f,  3,  5, 1, par, 2f,  4f,  5),
                new LevelDef("Specialist", 110f, 5, 12, 2, par, 4f, 11f, 11),
                new LevelDef("Technician", 130f, 6, 42, 2, par, 6f, 19f, 19,
                    "TRAINING TAPE 3\n\n" +
                    "Congratulations. You are now a Technician of The Solace Initiative.\n\n" +
                    "Your access has been extended. You will notice that the work is the same. We ask that you do not look for differences. " +
                    "If you find any, kindly report them to this tape.\n\n" +
                    "Please remain within tolerance."),
                new LevelDef("Steward",    150f, 6, 90, 3, par, 8f, 30f, 29,
                    "TRAINING TAPE 4\n\n" +
                    "You are now a Steward. Stewardship means caring for what cannot care for itself.\n\n" +
                    "You have been entrusted with the schedule. The schedule is on the wall in the administration room. " +
                    "Please do not let it fall behind.\n\n" +
                    "Thank you for your continued reliability."),
            });
        }
    }
}
