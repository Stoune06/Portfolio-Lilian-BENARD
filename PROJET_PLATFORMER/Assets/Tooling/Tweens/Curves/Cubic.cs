using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Cubic : LighterCurve
    {
        protected override float In(float pWeight) => pWeight * pWeight * pWeight;
    }
}