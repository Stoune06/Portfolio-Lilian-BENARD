using System;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    public abstract class LighterCurve
    {
        EasedCurve easedCurve;

        public static LighterCurve GetEasedCurve(TransitionType pTransition, EaseType pEase)
        {
            LighterCurve lLighterCurve = default;

            switch (pTransition)
            {
                case TransitionType.Linear:
                    lLighterCurve = new Linear();
                    break;

                case TransitionType.Sin:
                    lLighterCurve = new Sin();
                    break;

                case TransitionType.Quint:
                    lLighterCurve = new Quint();
                    break;

                case TransitionType.Quart:
                    lLighterCurve = new Quart();
                    break;

                case TransitionType.Quad:
                    lLighterCurve = new Quad();
                    break;

                case TransitionType.Expo:
                    lLighterCurve = new Expo();
                    break;

                case TransitionType.Elastic:
                    lLighterCurve = new Cubic();
                    break;

                case TransitionType.Cubic:
                    lLighterCurve = new Cubic();
                    break;

                case TransitionType.Circ:
                    lLighterCurve = new Circ();
                    break;

                case TransitionType.Bounce:
                    lLighterCurve = new Bounce();
                    break;

                case TransitionType.Back:
                    lLighterCurve = new Back();
                    break;

                case TransitionType.Spring:
                    lLighterCurve = new Spring();
                    break;

                case TransitionType.Blink:
                    lLighterCurve = new Blink();
                    break;

                case TransitionType.Punch:
                    lLighterCurve = new Punch();
                    break;

                case TransitionType.Bezier:
                    lLighterCurve = new Bezier();
                    break;

                default:
                    lLighterCurve = new Linear();
                    break;
            }

            switch (pEase)
            {
                case EaseType.In:
                    lLighterCurve.easedCurve = lLighterCurve.In;
                    break;

                case EaseType.Out:
                    lLighterCurve.easedCurve = lLighterCurve.Out;
                    break;

                case EaseType.InOut:
                    lLighterCurve.easedCurve = lLighterCurve.InOut;
                    break;

                case EaseType.OutIn:
                    lLighterCurve.easedCurve = lLighterCurve.OutIn;
                    break;

                default:
                    lLighterCurve.easedCurve = lLighterCurve.In;
                    break;
            }

            return lLighterCurve;
        }

        protected abstract float In(float pWeight);

        protected virtual float Out(float pWeight) => 1f - In(1f - pWeight);

        protected virtual float InOut(float pWeight) => ComposedCurve(In, pWeight);

        protected virtual float OutIn(float pWeight) => ComposedCurve(Out, pWeight);

        public float Evaluate(float pWeight) => easedCurve(pWeight);

        private float ComposedCurve(Func<float, float> pFunc, float pWeight) => pWeight < 0.5f ? pFunc(pWeight * 2f) * .5f : 1f - pFunc((1f - pWeight) * 2f) * .5f;
    }

    internal delegate float EasedCurve(float pValue);
}