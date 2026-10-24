using System;

namespace Noted.Core
{
    /// <summary>Tuning numbers for the core loop. Everything here is a first guess to be tuned by hand.</summary>
    public sealed class GameConfig
    {
        public float DayLengthSeconds = 60f;

        /// <summary>Multiplies every task hold time. One knob for how tight the day feels.</summary>
        public float TaskHoldScale = 1.5f;

        /// <summary>Uptime paid for each task finished before the timer ends.</summary>
        public int CoinsPerTask = 10;

        /// <summary>How many physical coins a finished task sprays. Each is worth CoinsPerTask / this.</summary>
        public int CoinPickupsPerTask = 5;

        /// <summary>Tasks on the list per rank (Trainee, Associate, Specialist).</summary>
        public int[] TasksPerRank = { 2, 3, 5 };

        /// <summary>Total tasks completed needed to reach rank 1, then rank 2.</summary>
        public int[] RankThresholds = { 5, 12 };

        public string[] RankTitles = { "Trainee", "Associate", "Specialist" };

        /// <summary>Walk speed bonus from the comfortable shoes given at rank 2.</summary>
        public float ShoesSpeedBonus = 0.08f;

        public string OrganizationName = "The Solace Initiative";

        public int MaxRank => RankThresholds.Length;

        public string TitleFor(int rank)
        {
            int i = Math.Max(0, Math.Min(rank, RankTitles.Length - 1));
            return RankTitles[i];
        }

        public int TasksFor(int rank)
        {
            int i = Math.Max(0, Math.Min(rank, TasksPerRank.Length - 1));
            return TasksPerRank[i];
        }
    }
}
