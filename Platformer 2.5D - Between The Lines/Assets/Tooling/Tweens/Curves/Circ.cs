using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Circ : LighterCurve
    {
        protected override float In(float pWeight) => 1f - MathF.Sqrt(1f - pWeight * pWeight);
    }
}