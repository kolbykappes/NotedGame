using System;

namespace Noted.Core
{
    /// <summary>The visible day timer.</summary>
    public sealed class DayClock
    {
        public float Total { get; private set; }
        public float Remaining { get; private set; }
        public bool Expired => Remaining <= 0f;

        public DayClock(float totalSeconds)
        {
            Total = Math.Max(0f, totalSeconds);
            Remaining = Total;
        }

        /// <summary>Advances the clock. Returns true on the tick that makes it reach zero, and never again after.</summary>
        public bool Tick(float dt)
        {
            if (Expired) return false;
            Remaining -= dt;
            if (Remaining <= 0f)
            {
                Remaining = 0f;
                return true;
            }
            return false;
        }

        /// <summary>Takes time off the clock (a wrong answer in a mini-game). It never reaches zero here: the next Tick does that, so the day-ending tick still fires.</summary>
        public void Penalty(float seconds)
        {
            if (Expired) return;
            Remaining = Math.Max(0.001f, Remaining - Math.Max(0f, seconds));
        }

        /// <summary>Adds time (for example Tidy Habits). A clock that has already run out stays out.</summary>
        public void AddSeconds(float seconds)
        {
            if (Expired) return;
            Remaining += seconds;
            Total += seconds;
        }
    }
}
