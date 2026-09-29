using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Punch : LighterCurve
    {
        private const float AMPLITUDE = 0.2f;
        private const int PULSES = 2;

        protected override float In(float pWeight) => MathF.Sin(pWeight * MathF.PI * PULSES) * (1f - pWeight) * AMPLITUDE;
    }
}