using System;

namespace Noted.Core
{
    /// <summary>Plain-C# math for the first-person controller. No engine references, so it is covered by Edit Mode tests.</summary>
    public static class FirstPersonMath
    {
        /// <summary>Moves a planar velocity toward a target by at most <paramref name="maxDelta"/>, without overshooting.</summary>
        public static (float x, float z) ApproachPlanar(float currentX, float currentZ, float targetX, float targetZ, float maxDelta)
        {
            float dx = targetX - currentX;
            float dz = targetZ - currentZ;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance <= maxDelta || distance < 1e-6f)
            {
                return (targetX, targetZ);
            }

            float scale = maxDelta / distance;
            return (currentX + dx * scale, currentZ + dz * scale);
        }

        /// <summary>Units per second needed to cover <paramref name="speed"/> in <paramref name="seconds"/>. Zero or negative time means instant.</summary>
        public static float RateFor(float speed, float seconds)
        {
            return seconds <= 0f ? float.PositiveInfinity : speed / seconds;
        }

        /// <summary>Clamps a pitch angle in degrees to +/- <paramref name="limitDegrees"/>.</summary>
        public static float ClampPitch(float pitchDegrees, float limitDegrees)
        {
            if (pitchDegrees > limitDegrees) return limitDegrees;
            if (pitchDegrees < -limitDegrees) return -limitDegrees;
            return pitchDegrees;
        }

        /// <summary>
        /// Applies a vertical mouse movement to the pitch. Pitch is positive when looking down, so moving the mouse up (positive
        /// delta) looks up when not inverted and looks down when inverted. The result is clamped to the pitch limit.
        /// </summary>
        public static float ApplyPitch(float pitchDegrees, float deltaYDegrees, bool invertY, float limitDegrees)
        {
            float next = pitchDegrees + (invertY ? deltaYDegrees : -deltaYDegrees);
            return ClampPitch(next, limitDegrees);
        }

        /// <summary>Wraps a yaw angle in degrees into [0, 360).</summary>
        public static float WrapYaw(float yawDegrees)
        {
            float wrapped = yawDegrees % 360f;
            return wrapped < 0f ? wrapped + 360f : wrapped;
        }
    }
}
