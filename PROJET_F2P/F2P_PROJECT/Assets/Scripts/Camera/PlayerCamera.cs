using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Other;
using Platformer.GameCamera;
using UnityEngine;
namespace DefaultNamespace
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform _Target;
        [SerializeField] private Camera _Camera;
        [SerializeField] private PlayerSpawner _Spawner;
        [SerializeField] private float _FollowDamping = 0.2f; //Camera Inert time

        private Vector3 _CameraVelocity = Vector3.zero;
        private Coroutine _TransitionCoroutine;

        private List<CameraParameter> _ParameterQueue = new List<CameraParameter>();

        private void LateUpdate() => FollowTarget();
        private void OnDisable() => StopAllCoroutines();

        private void FollowTarget()
        {
            if (!_Target) return;

            Vector3 lTargetPosition = new Vector3(_Target.position.x, _Target.position.y, _Target.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, lTargetPosition, ref _CameraVelocity, _FollowDamping);
        }
        public void AddToQueue(CameraParameter pParam)
        {
            if (IsHigherPriority(pParam.Priority)) _ParameterQueue.Insert(0, pParam); //If higher priority camera execute its parameter
            else _ParameterQueue.Add(pParam); //Else it goes on the bottom

            SortQueueByPriority(); //Sort list for security
            ChangeParameter(pParam); //Update parameter
        }
        public void RemoveFromQueue(CameraParameter pParam)
        {
            _ParameterQueue.Remove(pParam);
            SortQueueByPriority(); //Sort list for security
            if(_ParameterQueue.Count != 0) ChangeParameter(_ParameterQueue[0]); //Get new parameter
        }
        private void ChangeParameter(CameraParameter pParam)
        {
            Vector3 lNewOffset = new Vector3(pParam.CameraOffset.x, pParam.CameraOffset.y, _Camera.transform.localPosition.z); //Create new offset without changing z pos

            if (_TransitionCoroutine != null) StopCoroutine(_TransitionCoroutine);
            if (_Camera == null) return;
            _TransitionCoroutine = StartCoroutine(ParameterCoroutine(_Camera.orthographicSize, pParam.CameraZoom, lNewOffset, pParam.LerpCurve, pParam.Duration, pParam.Loop));
        }
        private bool IsHigherPriority(int pNewPriority)
        {
            if(_ParameterQueue.Count == 0) return false; //Security
            else if (pNewPriority >= _ParameterQueue[0].Priority) return true; //If higher return true
            else return false; //Else return false
        }
        private void SortQueueByPriority()
        {
            _ParameterQueue.Sort((pA, pB) => pB.Priority.CompareTo(pA.Priority)); //Sort by priority variable in parameter
        }
        private IEnumerator ParameterCoroutine(float pStartZoom, float pEndZoom, Vector3 pEndOffset, AnimationCurve pCurve, float pTime = 1f, bool pLoop = false)
        {
            float lElapsedTime = 0f;
            float lRatio = 0;
            Vector3 lStartOffset = _Camera.transform.localPosition;

            while (lElapsedTime < pTime)
            {
                lElapsedTime += Time.deltaTime / pTime;
                
                if (pCurve != null) lRatio = pCurve.Evaluate(lElapsedTime);
                else lRatio = lElapsedTime;

                _Camera.orthographicSize = Mathf.Lerp(pStartZoom, pEndZoom, lRatio); //Lerp zoom to new value
                _Camera.transform.localPosition = Vector3.Lerp(lStartOffset, pEndOffset, lRatio); //lerp child camera to new pos

                yield return null;
            }
            //Security clamp
            _Camera.orthographicSize = pEndZoom;
            _Camera.transform.localPosition = pEndOffset;

            if (pLoop) StartCoroutine(ParameterCoroutine(pEndZoom, pStartZoom, lStartOffset, pCurve));
        }
        
        public void SetTarget(Transform pNewTarget)
        {
            _Target = pNewTarget;
        }
    }
}
