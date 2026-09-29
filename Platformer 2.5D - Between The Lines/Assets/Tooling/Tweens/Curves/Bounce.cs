using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Bounce : LighterCurve
    {
        private const float MULTIPLY = 7.5625f;
        private const float PSEUDO_DIVIDE = 1f / 2.75f;

        protected override float In(float pWeight) => 1f - Out(1f - pWeight);

        protected override float Out(float pWeight)
        {
            if (pWeight < 1f * PSEUDO_DIVIDE)
                return MULTIPLY * pWeight * pWeight;

            else if (pWeight < 2f * PSEUDO_DIVIDE)
                return MULTIPLY * (pWeight -= 1.5f * PSEUDO_DIVIDE) * pWeight + 0.75f;

            else if (pWeight < 2.5f * PSEUDO_DIVIDE)
                return MULTIPLY * (pWeight -= 2.25f * PSEUDO_DIVIDE) * pWeight + 0.9375f;

            else
                return MULTIPLY * (pWeight -= 2.625f * PSEUDO_DIVIDE) * pWeight + 0.984375f;
        }
    }
}