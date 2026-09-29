using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Back : LighterCurve
    {
        private const float S = 1.70158f;
        private const float S1 = S + 1f;
        protected override float In(float pWeight)
            => S1 * pWeight * pWeight * pWeight - S * pWeight * pWeight;
    }
}