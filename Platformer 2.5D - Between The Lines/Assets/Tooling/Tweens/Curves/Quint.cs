using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Quint : LighterCurve
    {
        protected override float In(float pWeight) => pWeight * pWeight * pWeight * pWeight * pWeight;
    }
}