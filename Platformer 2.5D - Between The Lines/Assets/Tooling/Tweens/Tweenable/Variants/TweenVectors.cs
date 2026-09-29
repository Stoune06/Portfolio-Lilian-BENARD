using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens.Tweenables
{
    internal class TweenVector4 : Tweenable<Vector4>
    {
        public TweenVector4(object pObject, string pPropertyName, Vector3 pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Vector4 LerpToT() => Vector4.LerpUnclamped(StartValue, EndValue, curve.Evaluate(weight));
    }

    internal class TweenVector3 : Tweenable<Vector3>
    {
        public TweenVector3(object pObject, string pPropertyName, Vector3 pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Vector3 LerpToT() => Vector3.LerpUnclamped(StartValue, EndValue, curve.Evaluate(weight));
    }

    internal class TweenVector2 : Tweenable<Vector2>
    {
        public TweenVector2(object pObject, string pPropertyName, Vector3 pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Vector2 LerpToT() => Vector2.LerpUnclamped(StartValue, EndValue, curve.Evaluate(weight));
    }

    internal class TweenVector3Int : Tweenable<Vector3Int>
    {
        public TweenVector3Int(object pObject, string pPropertyName, Vector3Int pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Vector3Int LerpToT() =>
            new Vector3Int(
                (int)(StartValue.x + (EndValue.x - StartValue.x) * curve.Evaluate(weight)),
                (int)(StartValue.y + (EndValue.y - StartValue.y) * curve.Evaluate(weight)),
                (int)(StartValue.z + (EndValue.z - StartValue.z) * curve.Evaluate(weight)));
    }

    internal class TweenVector2Int : Tweenable<Vector2Int>
    {
        public TweenVector2Int(object pObject, string pPropertyName, Vector2Int pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Vector2Int LerpToT() =>
            new Vector2Int(
                (int)(StartValue.x + (EndValue.x - StartValue.x) * curve.Evaluate(weight)),
                (int)(StartValue.y + (EndValue.y - StartValue.y) * curve.Evaluate(weight)));
    }
}
