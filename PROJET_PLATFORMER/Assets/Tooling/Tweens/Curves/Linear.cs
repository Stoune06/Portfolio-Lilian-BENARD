using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Linear : LighterCurve
    {
        protected override float In(float pWeight) => pWeight;
        protected override float Out(float pWeight) => pWeight;
        protected override float OutIn(float pWeight) => pWeight;
        protected override float InOut(float pWeight) => pWeight;
    }
}