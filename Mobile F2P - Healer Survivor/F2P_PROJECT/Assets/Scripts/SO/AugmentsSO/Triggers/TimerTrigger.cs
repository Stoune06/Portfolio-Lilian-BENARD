using System;
using System.Collections;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class TimerTrigger : AugmentTrigger
    {
        [SerializeField]
        private float _Interval = 1f;

        private Coroutine _Coroutine;

        public override void Subscribe()
        {
            if (_Coroutine != null) return;
            _Coroutine = m_Player.StartCoroutine(TimerCoroutine());
        }

        public override void Unsubscribe()
        {
            if (_Coroutine != null)
                m_Player.StopCoroutine(_Coroutine);
            _Coroutine = null;
        }

        private IEnumerator TimerCoroutine()
        {
            float lSafeInterval = Mathf.Max(_Interval, 0.01f);
            while (true)
            {
                yield return new WaitForSeconds(lSafeInterval);
                m_Callback();
            }
        }
    }
}