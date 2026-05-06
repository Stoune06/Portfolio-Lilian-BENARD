using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens.Tweenables
{
    internal class TweenQuaternion : Tweenable<Quaternion>
    {
        public TweenQuaternion(object pObject, string pPropertyName, Quaternion pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Quaternion LerpToT() => Quaternion.LerpUnclamped(StartValue, EndValue, curve.Evaluate(weight));
    }

    internal class TweenColor : Tweenable<Color>
    {
        public TweenColor(object pObject, string pPropertyName, Color pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Color LerpToT() => Color.LerpUnclamped(StartValue, EndValue, curve.Evaluate(weight));
    }

    internal class TweenRect : Tweenable<Rect>
    {
        public TweenRect(object pObject, string pPropertyName, Rect pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Rect LerpToT() => new Rect(
                StartValue.x + (EndValue.x - StartValue.x) * curve.Evaluate(weight),
                StartValue.y + (EndValue.y - StartValue.y) * curve.Evaluate(weight),
                StartValue.width + (EndValue.width - StartValue.width) * curve.Evaluate(weight),
                StartValue.height + (EndValue.height - StartValue.height) * curve.Evaluate(weight));
    }

    internal class TweenRectInt : Tweenable<RectInt>
    {
        public TweenRectInt(object pObject, string pPropertyName, RectInt pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override RectInt LerpToT() => new RectInt(
                (int)(StartValue.x + (EndValue.x - StartValue.x) * curve.Evaluate(weight)),
                (int)(StartValue.y + (EndValue.y - StartValue.y) * curve.Evaluate(weight)),
                (int)(StartValue.width + (EndValue.width - StartValue.width) * curve.Evaluate(weight)),
                (int)(StartValue.height + (EndValue.height - StartValue.height) * curve.Evaluate(weight)));
    }

    internal class TweenBounds : Tweenable<Bounds>
    {
        public TweenBounds(object pObject, string pPropertyName, Bounds pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override Bounds LerpToT() => new Bounds(
            Vector3.LerpUnclamped(StartValue.center, EndValue.center, curve.Evaluate(weight)),
            Vector3.LerpUnclamped(StartValue.size, EndValue.size, curve.Evaluate(weight))
            );
    }

    internal class TweenBoundsInt : Tweenable<BoundsInt>
    {
        public TweenBoundsInt(object pObject, string pPropertyName, BoundsInt pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override BoundsInt LerpToT() => new BoundsInt(
                (int)(StartValue.center.x + (EndValue.center.x - StartValue.center.x) * curve.Evaluate(weight)),
                (int)(StartValue.center.y + (EndValue.center.y - StartValue.center.y) * curve.Evaluate(weight)),
                (int)(StartValue.center.z + (EndValue.center.z - StartValue.center.z) * curve.Evaluate(weight)),

                (int)(StartValue.size.x + (EndValue.size.x - StartValue.size.x) * curve.Evaluate(weight)),
                (int)(StartValue.size.y + (EndValue.size.y - StartValue.size.y) * curve.Evaluate(weight)),
                (int)(StartValue.size.z + (EndValue.size.z - StartValue.size.z) * curve.Evaluate(weight)));
    }

    internal class TweenBoundingSphere : Tweenable<BoundingSphere>
    {
        public TweenBoundingSphere(object pObject, string pPropertyName, BoundingSphere pEndValue) : base(pObject, pPropertyName, pEndValue) { }
        protected override BoundingSphere LerpToT() => new BoundingSphere(
            Vector3.LerpUnclamped(StartValue.position, EndValue.position, curve.Evaluate(weight)),
            StartValue.radius + (EndValue.radius - StartValue.radius) * curve.Evaluate(weight));
    }

    internal class TweenString : Tweenable<string>
    {
        private readonly int _EndValuePlus;
        public TweenString(object pObject, string pPropertyName, string pEndValue) : base(pObject, pPropertyName, pEndValue)
        {
            _EndValuePlus = pEndValue.Length + 1;
        }

        protected override string LerpToT() => EndValue.Substring(0, (int)Mathf.Clamp(_EndValuePlus * curve.Evaluate(weight), 0f, EndValue.Length));
    }
}