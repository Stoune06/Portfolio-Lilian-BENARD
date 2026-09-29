using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens.Tweenables
{
    internal class TweenSbyte : Tweenable<sbyte>
    {
        public TweenSbyte(object pObject, string pPropertyName, sbyte pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override sbyte LerpToT() => (sbyte)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenChar : Tweenable<char>
    {
        public TweenChar(object pObject, string pPropertyName, char pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override char LerpToT() => (char)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenShort : Tweenable<short>
    {
        public TweenShort(object pObject, string pPropertyName, short pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override short LerpToT() => (short)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenInt : Tweenable<int>
    {
        public TweenInt(object pObject, string pPropertyName, int pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override int LerpToT() => (int)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenLong : Tweenable<long>
    {
        public TweenLong(object pObject, string pPropertyName, long pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override long LerpToT() => (long)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    //unsigned
    internal class TweenByte : Tweenable<byte>
    {
        public TweenByte(object pObject, string pPropertyName, byte pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override byte LerpToT() => (byte)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenUShort : Tweenable<ushort>
    {
        public TweenUShort(object pObject, string pPropertyName, ushort pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override ushort LerpToT() => (ushort)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenUInt : Tweenable<uint>
    {
        public TweenUInt(object pObject, string pPropertyName, uint pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override uint LerpToT() => (uint)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    internal class TweenULong : Tweenable<ulong>
    {
        public TweenULong(object pObject, string pPropertyName, ulong pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override ulong LerpToT() => (ulong)(StartValue + (EndValue - StartValue) * curve.Evaluate(weight));
    }

    //floatable

    internal class TweenFloat : Tweenable<float>
    {
        public TweenFloat(object pObject, string pPropertyName, float pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override float LerpToT() => StartValue + (EndValue - StartValue) * curve.Evaluate(weight);
    }

    internal class TweenDouble : Tweenable<double>
    {
        public TweenDouble(object pObject, string pPropertyName, double pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override double LerpToT() => StartValue + (EndValue - StartValue) * curve.Evaluate(weight);
    }
}