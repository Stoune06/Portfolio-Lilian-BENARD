using Tooling.Tweens.Tweenables;
using Tooling.Tweens;
using UnityEngine;
using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    [System.Diagnostics.DebuggerStepThrough, Serializable]
    public class Tween
    {
        [SerializeField] internal TweenProperty property;
        public float InvertedDuration { get; private set; }
        public bool IsRunning { get; protected set; } = false;
        private bool _HasReversed = false;

        public event Action finished, loopFinished, loopThenReverseFinished;

        public event Action WhileTweening { remove { whileTweening = value; TweenManager.whileTweening -= value; } add { whileTweening = value; TweenManager.whileTweening += value; } }

        internal Action whileTweening;

        internal bool isRemovalAsked = false;
        internal BaseTweenable tweenable;

        public Tween(float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            property = new TweenProperty(pDuration, pTransition, pEase, pDelay);
        }

        public Tween(TweenProperty pProperty)
        {
            property = pProperty;
        }

        internal void StartTweening()
        {
            isRemovalAsked = false;
            tweenable.curve = LighterCurve.GetEasedCurve(property.transitionType, property.easeType);

            SetDuration(property.Duration);
            IsRunning = true;
            TweenManager.instance.Add(this);
        }

        public void Pause()
        {
            isRemovalAsked = true;
            IsRunning = false;
        }

        public void Resume()
        {
            isRemovalAsked = false;
            IsRunning = true;
            TweenManager.instance.Add(this);
        }

        /// <summary>
        /// Start value becomes the end one and vice-versa
        /// </summary>
        public void Invert() => tweenable.Invert();

        internal void CallFinished()
        {
            if (property.GetLoop())
            {
                loopFinished?.Invoke();
            }
            else if (property.GetLoopThenReverse())
            {
                if (_HasReversed) loopThenReverseFinished?.Invoke();
                tweenable.Invert();
                loopFinished?.Invoke();
                _HasReversed = !_HasReversed;
            }
            else if (property.GetLoopThenReverseOnce())
            {
                if (_HasReversed)
                {
                    finished?.Invoke();
                    loopThenReverseFinished?.Invoke();
                    isRemovalAsked = true;
                    IsRunning = false;
                }

                loopFinished?.Invoke();
                tweenable.Invert();
                _HasReversed = !_HasReversed;
            }
            else
            {
                finished?.Invoke();
                isRemovalAsked = true;
                IsRunning = false;
            }
            tweenable.weight = 0f;
        }

        /// <summary>
        /// It is way more optimized to multiply than to divide so, I transform the Duration
        /// once and then TweenManager will divide by multiplying
        /// </summary>
        private void SetDurationOptimization() => InvertedDuration = 1f / property.Duration;

        public void SetDuration(float pDuration)
        {
            if (pDuration <= 0f) pDuration = 1f;
            property.Duration = pDuration;
            SetDurationOptimization();
        }

        #region Numerable

        public void Start(object pObject, string pPropertyName, sbyte pEndValue)
        {
            tweenable = new TweenSbyte(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, char pEndValue)
        {
            tweenable = new TweenChar(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, short pEndValue)
        {
            tweenable = new TweenShort(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, int pEndValue)
        {
            tweenable = new TweenInt(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, long pEndValue)
        {
            tweenable = new TweenLong(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        #endregion

        #region UnsignedNumerable

        public void Start(object pObject, string pPropertyName, ushort pEndValue)
        {
            tweenable = new TweenUShort(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, uint pEndValue)
        {
            tweenable = new TweenUInt(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, ulong pEndValue)
        {
            tweenable = new TweenULong(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, byte pEndValue)
        {
            tweenable = new TweenByte(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        #endregion

        #region Floatable

        public void Start(object pObject, string pPropertyName, float pEndValue)
        {
            tweenable = new TweenFloat(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, double pEndValue)
        {
            tweenable = new TweenDouble(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        #endregion

        #region Vectors

        public void Start(object pObject, string pPropertyName, Vector2 pEndValue)
        {
            tweenable = new TweenVector2(pObject, pPropertyName, pEndValue);
            StartTweening();
        }
        public void Start(object pObject, string pPropertyName, Vector3 pEndValue)
        {
            tweenable = new TweenVector3(pObject, pPropertyName, pEndValue);
            StartTweening();
        }
        public void Start(object pObject, string pPropertyName, Vector4 pEndValue)
        {
            tweenable = new TweenVector4(pObject, pPropertyName, pEndValue);
            StartTweening();
        }
        public void Start(object pObject, string pPropertyName, Vector3Int pEndValue)
        {
            tweenable = new TweenVector3Int(pObject, pPropertyName, pEndValue);
            StartTweening();
        }
        public void Start(object pObject, string pPropertyName, Vector2Int pEndValue)
        {
            tweenable = new TweenVector2Int(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        #endregion

        #region Other

        public void Start(object pObject, string pPropertyName, string pEndValue)
        {
            tweenable = new TweenString(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, Quaternion pEndValue)
        {
            tweenable = new TweenQuaternion(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, Color pEndValue)
        {
            tweenable = new TweenColor(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, Bounds pEndValue)
        {
            tweenable = new TweenBounds(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, BoundsInt pEndValue)
        {
            tweenable = new TweenBoundsInt(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        public void Start(object pObject, string pPropertyName, BoundingSphere pEndValue)
        {
            tweenable = new TweenBoundingSphere(pObject, pPropertyName, pEndValue);
            StartTweening();
        }

        #endregion

        public void SetLoop(bool pBool = true) => property.SetLoop(pBool);

        /// <summary>
        /// At the end of the normal iteration, your tween is going to revert with the same properties
        /// </summary>
        public void SetLoopThenReverse(bool pBool = true) => property.SetLoopThenReverse(pBool);

        /// <summary>
        /// Will paste the following property to the <see langword="param"/> <see cref="Tween"></see> as <c>REFERENCE</c>
        /// <br/> 
        /// <see cref="TweenProperty.transitionType"></see>, <see cref="TweenProperty.easeType"></see>,
        /// <see cref="TweenProperty.Duration"></see>, <see cref="TweenProperty.Delay"></see>,
        /// <see cref="TweenProperty.loop"></see>, <see cref="TweenProperty.loopThenReverse"></see>,
        /// </summary>
        public void PastePropertyReferenceTo(Tween pTween) => pTween.property = property;


        /// <summary>
        /// Will Force the tween to end his animation
        /// </summary>
        public void ForceEnd()
        {
            tweenable.weight = 1f;
            tweenable.Lerp();
            TweenManager.RemoveFromRunning(this);
            CallFinished();
        }

        /// <summary>
        /// Do not pause the <see cref="Tween"></see> with <see cref="TweenProperty.isPauseAffected"></see> as <see langword="true"/> 
        /// </summary>
        public static void PauseAll() => TweenManager.isNotPaused = false;

        /// <summary>
        /// UnPause all the <see cref="Tween"></see> that were affected by <see cref="Tween.PauseAll"></see> 
        /// </summary>
        public static void UnPauseAll() => TweenManager.isNotPaused = true;
    }
}