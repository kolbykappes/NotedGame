namespace Noted.Core
{
    /// <summary>How many tools a debug start hands out.</summary>
    public enum ToolLoadout
    {
        /// <summary>Nothing.</summary>
        None,
        /// <summary>One level of the everyday tools plus the keycard and strap (what a player who has played a while would own).</summary>
        Basic,
        /// <summary>Every buyable tool at its highest level.</summary>
        All,
    }

    /// <summary>
    /// Where to start a session (debug and feel-test use). A level of 1 with default values is an ordinary new game.
    /// The level is 1-based (Level 3 is rank 2, Specialist).
    /// </summary>
    public sealed class StartProfile
    {
        public int Level = 1;

        /// <summary>The day number to start on; 0 means the level's usual starting day.</summary>
        public int Day;

        public int Coins;
        public ToolLoadout Loadout = ToolLoadout.Basic;
        public bool AllRoomsOpen = true;
        public bool MarkAllJobsSeen = true;

        /// <summary>Starting reliability (0 to 100).</summary>
        public float Reliability = 100f;

        /// <summary>Starting rollout; negative means the level's usual value.</summary>
        public float Rollout = -1f;

        public int Rank => Level < 1 ? 0 : Level - 1;

        public static StartProfile ForLevel(int level, int coins = 150)
        {
            return new StartProfile { Level = level, Coins = coins };
        }
    }
}
