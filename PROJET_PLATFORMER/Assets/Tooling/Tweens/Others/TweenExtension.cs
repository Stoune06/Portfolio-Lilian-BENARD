using Tooling.Tweens.Tweenables;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling
{
    public static class TweenExtension
    {
        public const string POSITION = "position";
        public const string SCALE = "localScale";
        public const string ROTATION = "rotation";

        public const string COLOR = "color";

        #region Move Transform

        public static Tween MoveTo(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => MoveTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween MoveTo(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.Start(tTransform, POSITION, pEndPosition);
            return lTween;
        }

        public static Tween MoveXTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => MoveXTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween MoveXTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, POSITION, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(pEndPosition, lVector.y, lVector.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween MoveYTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => MoveYTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween MoveYTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, POSITION, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(lVector.x, pEndPosition, lVector.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween MoveZTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => MoveZTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween MoveZTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, POSITION, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(lVector.x, lVector.y, pEndPosition));
            lTween.StartTweening();

            return lTween;
        }

        #endregion

        #region Scale Transform

        public static Tween ScaleTo(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleTo(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.Start(tTransform, SCALE, pEndPosition);
            return lTween;
        }

        public static Tween ScaleXTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleXTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleXTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, SCALE, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(pEndPosition, lVector.y, lVector.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween ScaleYTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleYTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleYTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, SCALE, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(lVector.x, pEndPosition, lVector.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween ScaleZTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleZTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleZTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenVector3(tTransform, SCALE, Vector3.zero);

            Vector3 lVector = (Vector3)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Vector3(lVector.x, lVector.y, pEndPosition));
            lTween.StartTweening();

            return lTween;
        }

        #endregion

        #region Scale Conservation 1D

        private static Tween ScaleYAndKeep(this Transform tTransform, float pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, new Vector3(tTransform.localScale.x, pEndPosition, tTransform.localScale.z));
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x, tTransform.position.y + pOffset, tTransform.position.z));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleYAndKeepBottom(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleYAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleYAndKeepBottom(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleYAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition - tTransform.localScale.y) * 0.5f);

        public static Tween ScaleYAndKeepTop(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleYAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleYAndKeepTop(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleYAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition - tTransform.localScale.y) * 0.5f));

        private static Tween ScaleXAndKeep(this Transform tTransform, float pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, new Vector3(pEndPosition, tTransform.localScale.y, tTransform.localScale.z));
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x + pOffset, tTransform.position.y, tTransform.position.z));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleXAndKeepBottom(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleXAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleXAndKeepBottom(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleXAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition - tTransform.localScale.x) * 0.5f);

        public static Tween ScaleXAndKeepTop(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleXAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleXAndKeepTop(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleXAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition - tTransform.localScale.x) * 0.5f));

        private static Tween ScaleZAndKeep(this Transform tTransform, float pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, new Vector3(tTransform.localScale.x, tTransform.localScale.y, pEndPosition));
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x, tTransform.position.y, tTransform.position.z + pOffset));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleZAndKeepBottom(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleZAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleZAndKeepBottom(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleZAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition - tTransform.localScale.z) * 0.5f);

        public static Tween ScaleZAndKeepTop(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleZAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleZAndKeepTop(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
            => ScaleZAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition - tTransform.localScale.z) * 0.5f));

        #endregion

        #region Scale Conservation 3D
        private static Tween ScaleYXZAndKeep(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, pEndPosition);
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x, tTransform.position.y + pOffset, tTransform.position.z));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleYXZAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleYXZAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleYXZAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleYXZAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition.y - tTransform.localScale.y) * 0.5f);

        public static Tween ScaleYXZAndKeepTop(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleYXZAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleYXZAndKeepTop(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleYXZAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition.y - tTransform.localScale.y) * 0.5f));


        private static Tween ScaleXYZAndKeep(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, pEndPosition);
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x + pOffset, tTransform.position.y, tTransform.position.z));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleXYZAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleXYZAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleXYZAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleXYZAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition.x - tTransform.localScale.x) * 0.5f);

        public static Tween ScaleXYZAndKeepTop(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleXYZAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleXYZAndKeepTop(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleXYZAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition.x - tTransform.localScale.x) * 0.5f));


        private static Tween ScaleZXYAndKeep(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty, float pOffset)
        {
            Tween lScaleTween = new Tween(pProperty);
            lScaleTween.tweenable = new TweenVector3(tTransform, SCALE, pEndPosition);
            lScaleTween.StartTweening();

            Tween lPositionTween = new Tween(pProperty);
            lPositionTween.tweenable = new TweenVector3(tTransform, POSITION, new Vector3(tTransform.position.x, tTransform.position.y, tTransform.position.z + pOffset));
            lPositionTween.StartTweening();

            return lScaleTween;
        }

        public static Tween ScaleZXYAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleZXYAndKeepBottom(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleZXYAndKeepBottom(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleZXYAndKeep(tTransform, pEndPosition, pProperty, (pEndPosition.z - tTransform.localScale.z) * 0.5f);

        public static Tween ScaleZXYAndKeepTop(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => ScaleZXYAndKeepTop(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween ScaleZXYAndKeepTop(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
            => ScaleZXYAndKeep(tTransform, pEndPosition, pProperty, -((pEndPosition.z - tTransform.localScale.z) * 0.5f));

        #endregion

        #region Rotation

        public static Tween RotateTo(this Transform tTransform, Quaternion pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => RotateTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween RotateTo(this Transform tTransform, Quaternion pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.Start(tTransform, ROTATION, pEndPosition);
            return lTween;
        }

        public static Tween RotateTo(this Transform tTransform, Vector3 pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
             => RotateTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween RotateTo(this Transform tTransform, Vector3 pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.Start(tTransform, ROTATION, Quaternion.Euler(pEndPosition));
            return lTween;
        }

        public static Tween RotateXTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => RotateXTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween RotateXTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenQuaternion(tTransform, ROTATION, Quaternion.identity);

            Quaternion lQuaternion = (Quaternion)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(Quaternion.Euler(pEndPosition, lQuaternion.eulerAngles.y, lQuaternion.eulerAngles.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween RotateYTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => RotateYTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween RotateYTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenQuaternion(tTransform, ROTATION, Quaternion.identity);

            Quaternion lQuaternion = (Quaternion)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(Quaternion.Euler(lQuaternion.eulerAngles.x, pEndPosition, lQuaternion.eulerAngles.z));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween RotateZTo(this Transform tTransform, float pEndPosition, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
            => RotateZTo(tTransform, pEndPosition, new TweenProperty(pDuration, pTransition, pEase, pDelay));

        public static Tween RotateZTo(this Transform tTransform, float pEndPosition, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.tweenable = new TweenQuaternion(tTransform, ROTATION, Quaternion.identity);

            Quaternion lQuaternion = (Quaternion)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(Quaternion.Euler(lQuaternion.eulerAngles.x, lQuaternion.eulerAngles.y, pEndPosition));
            lTween.StartTweening();

            return lTween;
        }

        #endregion

        #region Color

        public static Tween ColorTo(this Material tTransform, Color pEndColor, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            Tween lTween = new Tween(pDuration, pTransition, pEase, pDelay);
            lTween.Start(tTransform, COLOR, pEndColor);
            return lTween;
        }

        public static Tween ColorTo(this Material tTransform, Color pEndColor, TweenProperty pProperty)
        {
            Tween lTween = new Tween(pProperty);
            lTween.Start(tTransform, COLOR, pEndColor);
            return lTween;
        }

        public static Tween ColorRTo(this Material tTransform, float pEndColor, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            Tween lTween = new Tween(pDuration, pTransition, pEase, pDelay);
            lTween.tweenable = new TweenColor(tTransform, COLOR, Color.black);

            Color lColor = (Color)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Color(pEndColor, lColor.g, lColor.b, lColor.a));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween ColorGTo(this Material tTransform, float pEndColor, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            Tween lTween = new Tween(pDuration, pTransition, pEase, pDelay);
            lTween.tweenable = new TweenColor(tTransform, COLOR, Color.black);

            Color lColor = (Color)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Color(lColor.r, pEndColor, lColor.b, lColor.a));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween ColorBTo(this Material tTransform, float pEndColor, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            Tween lTween = new Tween(pDuration, pTransition, pEase, pDelay);
            lTween.tweenable = new TweenColor(tTransform, COLOR, Color.black);

            Color lColor = (Color)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Color(lColor.r, lColor.g, lColor.b, lColor.a));
            lTween.StartTweening();

            return lTween;
        }

        public static Tween ColorATo(this Material tTransform, float pEndColor, float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            Tween lTween = new Tween(pDuration, pTransition, pEase, pDelay);
            lTween.tweenable = new TweenColor(tTransform, COLOR, Color.black);

            Color lColor = (Color)lTween.tweenable.GetStartValue();
            lTween.tweenable.SetEndValue(new Color(lColor.r, lColor.g, lColor.b, pEndColor));
            lTween.StartTweening();

            return lTween;
        }

        #endregion
    }
}