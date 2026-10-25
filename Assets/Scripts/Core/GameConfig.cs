using System;

namespace Noted.Core
{
    /// <summary>Tuning numbers for the core loop. Everything here is a first guess to be tuned by hand.</summary>
    public sealed class GameConfig
    {
        /// <summary>Day length, list size, promotion thresholds and story stage for every level.</summary>
        public LevelTable Levels = LevelTable.Default();

        /// <summary>Multiplies every task hold time. One knob for how tight the day feels.</summary>
        public float TaskHoldScale = 1f;

        /// <summary>How many things the player can carry at once before buying a cart.</summary>
        public int BaseCarryCapacity = 1;

        /// <summary>Uptime paid for each task finished before the timer ends.</summary>
        public int CoinsPerTask = 10;

        /// <summary>How many physical coins a finished task sprays. Each is worth CoinsPerTask / this.</summary>
        public int CoinPickupsPerTask = 5;

        /// <summary>Walk speed bonus from the comfortable shoes given at rank 2.</summary>
        public float ShoesSpeedBonus = 0.08f;

        public string OrganizationName = "The Solace Initiative";

        /// <summary>Total completed tasks needed to reach each rank after the first.</summary>
        public int[] RankThresholds => Levels.Thresholds();

        public int MaxRank => Levels.MaxRank;

        public string TitleFor(int rank) => Levels.For(rank).Title;

        public int TasksFor(int rank) => Levels.For(rank).TaskCount;

        public float DayLengthFor(int rank) => Levels.For(rank).DaySeconds;

        public int RevealStageFor(int rank) => Levels.For(rank).RevealStage;
    }
}
