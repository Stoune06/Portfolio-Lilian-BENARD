using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tooling
{
    [System.Diagnostics.DebuggerStepThrough, Serializable, Obsolete]
    public class Timer
    {
        //[SerializeField] public float duration = 1f;
        //internal bool isRemovalAsked { get; set; } = false;
        //public bool IsRunning { get; protected set; } = false;
        //public float CurrentTime { get; internal set; } = 0f;
        //public bool loop = true;

        //public event Action timeout;

        //private static List<Timer> _TimerList = new List<Timer>();
        //private static bool _IsCoroutineRunning = false;

        //private static IEnumerator Startr()
        //{
        //    int lIteration = _TimerList.Count - 1;

        //    for (int i = lIteration; i >= 0; i--)
        //    {
        //        yield return new WaitForEndOfFrame();
        //    }
        //}

        ///// <summary>
        ///// Start the <see cref="Timer"/> with the given <paramref name="pDuration"/>, if the duration is
        ///// negative it will the value of <see cref="Timer"/>.duration
        ///// </summary>
        //public void Start(float pDuration = -1f)
        //{
        //    if (IsRunning)
        //    {
        //        Debug.LogError("You cannot start a Timer that is currently running");
        //        return;
        //    }

        //    if (pDuration > 0f) duration = pDuration;
        //    _TimerList.Add(this);
        //    IsRunning = true;
        //}

        ///// <summary>
        ///// Reset the <see cref="Timer"/> to 0 and stop it
        ///// </summary>
        //public void Reset()
        //{
        //    Pause();
        //    CurrentTime = 0f;
        //}

        ///// <summary>
        ///// Pause the <see cref="Timer"/>, you can use the <see cref="Resume"/> to Resume
        ///// </summary>
        //public void Pause()
        //{
        //    isRemovalAsked = true;
        //    IsRunning = false;
        //}

        ///// <summary>
        ///// Resume the <see cref="Timer"/> if it was paused with <see cref="Resume"/>
        ///// </summary>
        //public void Resume()
        //{
        //    if (IsRunning)
        //    {
        //        Debug.LogError("You cannot Resume a Timer that is currently running");
        //        return;
        //    }

        //    _TimerList.Add(this);
        //    IsRunning = true;
        //}

        //internal void CallFinished()
        //{
        //    if (!loop) IsRunning = false;
        //    CurrentTime = 0f;
        //    timeout?.Invoke();
        //}
    }
}