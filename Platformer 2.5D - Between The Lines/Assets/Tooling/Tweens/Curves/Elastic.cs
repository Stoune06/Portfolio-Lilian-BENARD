using System;

namespace Tooling.Tweens
{
    internal class Elastic : LighterCurve
    {
        private const float RAD = 2f * MathF.PI / 3f;

        protected override float In(float pWeight) => 1f - Out(1f - pWeight);

        protected override float Out(float pWeight) 
            => pWeight == 0f ? 0f : pWeight == 1f ? 1f : -MathF.Pow(2f, 10f * (pWeight - 1f)) * MathF.Sin((pWeight * 10f - 10.75f) * RAD);
    }
}