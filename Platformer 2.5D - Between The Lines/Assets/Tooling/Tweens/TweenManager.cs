using System;
using System.Collections.Generic;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    [AutoLoad]
    internal class TweenManager : MonoBehaviour
    {
        private static List<Tween> _TweenList = new List<Tween>();
        private static List<Tween> _UnPausableTweenList = new List<Tween>();
        private static List<Tween> _DelayedList = new List<Tween>();

        internal static TweenManager instance;
        internal static bool isNotPaused = true;
        static internal Action whileTweening;

        [RuntimeAutoLoadMethod(RuntimeAutoLoadType = RuntimeAutoLoadType.BeforeAutoLoad)]
        private static void Clean()
        {
            _TweenList.Clear();
            _DelayedList.Clear();
            _UnPausableTweenList.Clear();
            whileTweening = null;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            enabled = false;
            instance = this;
            Time.timeScale = 1f;
        }

        void Update()
        {
            if (isNotPaused) PlayTween(_TweenList);
            PlayTween(_UnPausableTweenList);
            PlayDelayed();
            whileTweening?.Invoke();
        }

        private void PlayTween(List<Tween> pList)
        {
            int lInt = pList.Count - 1;
            if (lInt < 0) return;

            Tween lTween;
            for (int i = lInt; i >= 0; i--)
            {
                lTween = pList[i];

                if (lTween.isRemovalAsked)
                {
                    pList.RemoveAt(i);
                    whileTweening -= lTween.whileTweening;
                    CheckIfEnableNecessary();
                }
                else
                {
                    try
                    {
                        lTween.tweenable.weight += Time.unscaledDeltaTime * lTween.InvertedDuration;
                        lTween.tweenable.Lerp();

                        if (lTween.tweenable.weight >= 1f)
                        {
                            lTween.tweenable.weight = 1f;
                            lTween.tweenable.Lerp();

                            if (!lTween.property.IsALoopActivated)
                            {
                                whileTweening -= lTween.whileTweening;
                                pList.RemoveAt(i);
                            }

                            whileTweening -= lTween.whileTweening;

                            lTween.CallFinished();
                            CheckIfEnableNecessary();
                        }
                    }
                    catch { pList.RemoveAt(i); }
                }
            }
        }

        private void PlayDelayed()
        {
            int lInt = _DelayedList.Count - 1;
            if (lInt <= 0) return;
            Tween lTween;

            for (int i = lInt; i >= 0; i--)
            {
                lTween = _DelayedList[i];
                lTween.property.Delay -= Time.deltaTime;
                if (lTween.property.Delay <= 0)
                {
                    DelayedToActive(lTween);
                    lTween.property.Delay = 0f;
                }
            }
        }

        internal void Add(Tween pTween)
        {
            enabled = true;
            if (pTween.property.Delay > 0f)
            {
                _DelayedList.Add(pTween);
            }
            else
            {
                AddTweenToAdequateList(pTween);
            }
        }

        internal static void RemoveFromRunning(Tween pTween) => _TweenList.Remove(pTween);

        private static void DelayedToActive(Tween pTween)
        {
            _DelayedList.Remove(pTween);
            AddTweenToAdequateList(pTween);
        }

        internal static void AddTweenToAdequateList(Tween pTween)
        {
            if (pTween.property.isPauseAffected) _TweenList.Add(pTween);
            else _UnPausableTweenList.Add(pTween);
        }

        private void CheckIfEnableNecessary()
        {
            if (_TweenList.Count + _UnPausableTweenList.Count + _DelayedList.Count <= 0) enabled = false;
        }
    }
}