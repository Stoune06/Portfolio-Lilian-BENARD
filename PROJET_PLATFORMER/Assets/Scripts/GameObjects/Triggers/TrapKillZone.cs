using Platformer.Tools;
using System.Collections;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Areas
{
    public class TrapKillZone : Area_System
    {
        [SerializeField] private float _ChargeDuration = 1f;
        [SerializeField] private float _ActivationDuration = 0.5f;
        [SerializeField] private Vector3 _PosAdded = new Vector3(0,1,0);
        [SerializeField] private Transform _KillZone;

        private bool _IsOpen = false;

        private WaitForEndOfFrame _WaitForEndOfFrame = new WaitForEndOfFrame();
        private Coroutine _TransitionCoroutine;

        protected override void OnCollision()
        {
            if (_TransitionCoroutine == null) StartTransition();
        }

        private void StartTransition()
        {
            StopAllCoroutines();
            StartCoroutine(CustomTools.Timer(_ChargeDuration, () => _TransitionCoroutine = StartCoroutine(ShowKillZone())));
        }

        private IEnumerator ShowKillZone()
        {
            float lElapsedTime = 0f;
            Vector3 lStartPos = _KillZone.position;
            Vector3 lEndPos = _KillZone.position + _PosAdded;

            if(_IsOpen) lEndPos = _KillZone.position - _PosAdded;

            while (lElapsedTime < _ActivationDuration)
            {
                lElapsedTime += Time.deltaTime;

                _KillZone.position = Vector3.Lerp(lStartPos, lEndPos, lElapsedTime / _ActivationDuration);

                yield return _WaitForEndOfFrame;
            }

            _KillZone.position = lEndPos;
            _IsOpen = !_IsOpen;

            if (_IsOpen) StartTransition();
            _TransitionCoroutine = null;
        }
    }
}

