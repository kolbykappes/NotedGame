using System;

namespace Noted.Core
{
    /// <summary>
    /// How reliable the company thinks you are. It drops when the official list is left unfinished and recovers slowly on a clean day.
    /// Promotion needs it at or above the gate. It never ends the game.
    /// </summary>
    public sealed class Reliability
    {
        public const float Max = 100f;
        public const float PromotionGate = 80f;

        /// <summary>First guesses, to be tuned from the day log.</summary>
        public float PenaltyPerUnfinishedTask = 8f;
        public float RecoveryPerCleanDay = 2f;

        public float Value { get; private set; } = Max;

        public bool CanPromote => Value >= PromotionGate;

        public void Set(float value)
        {
            Value = Math.Max(0f, Math.Min(Max, value));
        }

        /// <summary>Applies a day's result. A day with nothing left unfinished is a clean day.</summary>
        public void EndOfDay(int unfinishedTasks)
        {
            if (unfinishedTasks > 0) Set(Value - unfinishedTasks * PenaltyPerUnfinishedTask);
            else Set(Value + RecoveryPerCleanDay);
        }
    }

    /// <summary>
    /// The company's rollout, as a percentage. It moves forward by itself each day and your sabotage pushes it back. It never reaches
    /// 100 before the final act (the cap), and it cannot fall below a floor that rises with each promotion.
    /// </summary>
    public sealed class Rollout
    {
        public const float Cap = 99f;

        /// <summary>When rollout is this high the resistance sends a lifeline (stub for later).</summary>
        public const float LifelineAt = 70f;

        /// <summary>When rollout is this low the company adds crunch days (stub for later).</summary>
        public const float CrunchBelow = 15f;

        public float Value { get; private set; }
        public float Floor { get; private set; }

        public bool WantsLifeline => Value >= LifelineAt;
        public bool WantsCrunch => Value <= CrunchBelow;

        /// <summary>The number of locks on the final door: one per 20 points, at least one, at most five.</summary>
        public int Locks => Math.Max(1, Math.Min(5, (int)Math.Ceiling(Value / 20f)));

        public void Set(float value)
        {
            Value = Math.Max(Floor, Math.Min(Cap, value));
        }

        /// <summary>Puts rollout back to a saved state (replaying a day).</summary>
        public void Restore(float value, float floor)
        {
            Floor = Math.Max(0f, floor);
            Value = Math.Max(Floor, Math.Min(Cap, value));
        }

        public void RaiseFloor(float floor)
        {
            if (floor > Floor) Floor = floor;
            if (Value < Floor) Value = Floor;
        }

        /// <summary>
        /// Applies a day: the company's par moves it forward and completed errands push it back. Returns the change.
        /// <paramref name="errandEffect"/> is negative or zero.
        /// </summary>
        public float EndOfDay(float par, float errandEffect)
        {
            float before = Value;
            Set(Value + par + Math.Min(0f, errandEffect));
            return Value - before;
        }
    }
}
