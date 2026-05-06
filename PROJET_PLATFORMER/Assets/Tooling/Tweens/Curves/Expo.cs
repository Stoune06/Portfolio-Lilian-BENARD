using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Expo : LighterCurve
    {
        protected override float In(float pWeight) => pWeight == 0f ? 0f : MathF.Pow(2f, 10f * (pWeight - 1f));
    }
}