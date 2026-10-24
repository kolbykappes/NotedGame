using NUnit.Framework;

namespace Noted.Core.Tests
{
    public class FirstPersonMathTests
    {
        const float Tolerance = 1e-4f;

        [Test]
        public void ApproachPlanar_MovesByAtMostMaxDelta()
        {
            var result = FirstPersonMath.ApproachPlanar(0f, 0f, 10f, 0f, 3f);
            Assert.AreEqual(3f, result.x, Tolerance);
            Assert.AreEqual(0f, result.z, Tolerance);
        }

        [Test]
        public void ApproachPlanar_DoesNotOvershootTarget()
        {
            var result = FirstPersonMath.ApproachPlanar(9f, 0f, 10f, 0f, 5f);
            Assert.AreEqual(10f, result.x, Tolerance);
            Assert.AreEqual(0f, result.z, Tolerance);
        }

        [Test]
        public void ApproachPlanar_DiagonalStepHasLengthMaxDelta()
        {
            var result = FirstPersonMath.ApproachPlanar(0f, 0f, 10f, 10f, 2f);
            float length = (float)System.Math.Sqrt(result.x * result.x + result.z * result.z);
            Assert.AreEqual(2f, length, Tolerance);
            Assert.AreEqual(result.x, result.z, Tolerance);
        }

        [Test]
        public void ApproachPlanar_AlreadyAtTarget_StaysPut()
        {
            var result = FirstPersonMath.ApproachPlanar(2f, -3f, 2f, -3f, 1f);
            Assert.AreEqual(2f, result.x, Tolerance);
            Assert.AreEqual(-3f, result.z, Tolerance);
        }

        [Test]
        public void ApproachPlanar_ZeroMaxDelta_DoesNotMove()
        {
            var result = FirstPersonMath.ApproachPlanar(0f, 0f, 10f, 0f, 0f);
            Assert.AreEqual(0f, result.x, Tolerance);
            Assert.AreEqual(0f, result.z, Tolerance);
        }

        [Test]
        public void RateFor_ReachesSpeedInGivenTime()
        {
            Assert.AreEqual(50f, FirstPersonMath.RateFor(4f, 0.08f), Tolerance);
        }

        [Test]
        public void RateFor_NonPositiveTime_IsInstant()
        {
            Assert.IsTrue(float.IsPositiveInfinity(FirstPersonMath.RateFor(4f, 0f)));
            Assert.IsTrue(float.IsPositiveInfinity(FirstPersonMath.RateFor(4f, -1f)));
        }

        [TestCase(0f, 85f, 0f)]
        [TestCase(90f, 85f, 85f)]
        [TestCase(-120f, 85f, -85f)]
        [TestCase(85f, 85f, 85f)]
        public void ClampPitch_ClampsToLimit(float pitch, float limit, float expected)
        {
            Assert.AreEqual(expected, FirstPersonMath.ClampPitch(pitch, limit), Tolerance);
        }

        [Test]
        public void ApplyPitch_MouseUpLooksUp_WhenNotInverted()
        {
            // Pitch is positive when looking down, so looking up means the pitch goes down.
            Assert.AreEqual(-5f, FirstPersonMath.ApplyPitch(0f, 5f, false, 85f), Tolerance);
            Assert.AreEqual(5f, FirstPersonMath.ApplyPitch(0f, -5f, false, 85f), Tolerance);
        }

        [Test]
        public void ApplyPitch_MouseUpLooksDown_WhenInverted()
        {
            Assert.AreEqual(5f, FirstPersonMath.ApplyPitch(0f, 5f, true, 85f), Tolerance);
            Assert.AreEqual(-5f, FirstPersonMath.ApplyPitch(0f, -5f, true, 85f), Tolerance);
        }

        [Test]
        public void ApplyPitch_StaysWithinTheLimit_BothWays()
        {
            Assert.AreEqual(-85f, FirstPersonMath.ApplyPitch(-80f, 30f, false, 85f), Tolerance);
            Assert.AreEqual(85f, FirstPersonMath.ApplyPitch(80f, 30f, true, 85f), Tolerance);
        }

        [TestCase(0f, 0f)]
        [TestCase(360f, 0f)]
        [TestCase(-90f, 270f)]
        [TestCase(725f, 5f)]
        public void WrapYaw_WrapsIntoZeroTo360(float yaw, float expected)
        {
            Assert.AreEqual(expected, FirstPersonMath.WrapYaw(yaw), Tolerance);
        }
    }
}
