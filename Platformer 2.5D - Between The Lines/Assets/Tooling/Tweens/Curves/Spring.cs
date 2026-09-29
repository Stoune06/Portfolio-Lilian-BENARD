using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Spring : LighterCurve
    {
        private const float DAMPING = 5f;
        private const float FREQUENCY = 10f;

        protected override float In(float pWeight) => 1f - MathF.Exp(-DAMPING * pWeight) * MathF.Cos(FREQUENCY * pWeight);

        protected override float Out(float pWeight) => MathF.Exp(-DAMPING * (1f - pWeight)) * MathF.Cos(FREQUENCY * (1f - pWeight));

        protected override float InOut(float pWeight) => pWeight < 0.5f ? 0.5f * In(pWeight * 2f) : 0.5f + 0.5f * Out(pWeight * 2f - 1f);
    }
}