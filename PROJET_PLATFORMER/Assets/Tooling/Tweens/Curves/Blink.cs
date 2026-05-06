using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Blink : LighterCurve
    {
        private const int CYCLES = 3;
        protected override float In(float pWeight) => ((int)(pWeight * CYCLES * 2f) % 2 == 0) ? 1f : 0f;
    }
}