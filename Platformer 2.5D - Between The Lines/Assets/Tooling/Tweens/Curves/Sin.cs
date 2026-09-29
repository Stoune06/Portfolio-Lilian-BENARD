using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    internal class Sin : LighterCurve
    {
        protected override float In(float pWeight) => 1f - MathF.Cos(pWeight * MathF.PI * .5f);

        protected override float Out(float pWeight) => MathF.Sin(pWeight * MathF.PI * .5f);

        protected override float InOut(float pWeight) => -(MathF.Cos(MathF.PI * pWeight) - 1f) * .5f;
    }
}