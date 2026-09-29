using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens.Tweenables
{
    internal abstract class BaseTweenable
    {
        internal LighterCurve curve;
        public float weight = 0f;
        public object m_Object;
        public abstract void Lerp();
        public abstract void Invert();
        public abstract object GetStartValue();
        public abstract object GetEndValue();
        public abstract void SetStartValue(object pValue);
        public abstract void SetEndValue(object pValue);
    }
}