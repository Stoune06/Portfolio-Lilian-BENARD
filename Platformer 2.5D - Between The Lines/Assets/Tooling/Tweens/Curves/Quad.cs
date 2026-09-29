using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Quad : LighterCurve
    {
        protected override float In(float pWeight) => pWeight * pWeight;
    }
}