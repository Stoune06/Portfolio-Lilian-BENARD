using System;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling
{
    [Serializable]
    public class TweenProperty
    {
        [SerializeField] public TransitionType transitionType = 0;
        [SerializeField] public EaseType easeType = 0;

        [SerializeField, HideInInspector] private bool loopThenReverseOnce = false;
        [SerializeField, HideInInspector] private bool loopThenReverse = false;
        [SerializeField, HideInInspector] private bool loop = false;
        public bool isPauseAffected = true;

        [SerializeField, HideInInspector] internal bool IsALoopActivated = false;

        [SerializeField] public float Duration = 1f;
        [SerializeField] public float Delay = 0f;

        public TweenProperty(float pDuration = 1f, TransitionType pTransition = 0, EaseType pEase = 0, float pDelay = 0f)
        {
            transitionType = pTransition;
            easeType = pEase;
            Duration = pDuration;
            Delay = pDelay;
        }

        public bool GetLoop() => loop;
        public bool GetLoopThenReverse() => loopThenReverse;
        public bool GetLoopThenReverseOnce() => loopThenReverseOnce;

        public void SetLoop(bool pBool = true)
        {
            loop = pBool;
            loopThenReverse = pBool ? false : loopThenReverse;
            loopThenReverseOnce = loop;
            SetIsALoopActivated();
        }

        public void SetLoopThenReverse(bool pBool = true)
        {
            loopThenReverse = pBool;
            loop = pBool ? false : loop;
            loopThenReverseOnce = loop;
            SetIsALoopActivated();
        }

        public void SetLoopThenReverseOnce(bool pBool = true)
        {
            loopThenReverseOnce = pBool;
            loop = pBool ? false : loop;
            loopThenReverse = loop;
            SetIsALoopActivated();
        }

        private void SetIsALoopActivated() => IsALoopActivated = loop || loopThenReverse || loopThenReverseOnce;
    }
}