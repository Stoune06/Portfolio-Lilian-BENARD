using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Quart : LighterCurve
    {
        protected override float In(float pWeight) => pWeight * pWeight * pWeight * pWeight;
    }
}