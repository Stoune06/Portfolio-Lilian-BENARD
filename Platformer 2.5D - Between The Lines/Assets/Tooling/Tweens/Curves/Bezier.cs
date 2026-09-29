using System;

namespace Tooling.Tweens
{
    internal class Bezier : LighterCurve //TODO Ask Valentin
    {
        private const float Y1 = 0.1f;
        private const float Y2 = 1f;
        protected override float In(float pWeight)
        {
            float lU = 1f - pWeight;
            return (lU * lU * lU) * 0f + (3f * lU * lU * pWeight) * Y1 + (3f * lU * pWeight * pWeight) * Y2 + (pWeight * pWeight * pWeight) * Y2;
        }
    }
}