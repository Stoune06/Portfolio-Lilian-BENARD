using Platformer.Player;
using System.Collections;
using Tooling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//Author : Noé SALES
namespace Platformer
{
    public class DeathCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _TargetAngle = 30f;
        [SerializeField] private float _TransitionTime = 0.3f;
        [SerializeField] private AnimationCurve _TransitionCurve = default;
        [SerializeField] private float _FinalOrthographicSize = 2f;
        [SerializeField] private Vector3 _TargetOffset = new Vector3(0, 1f, 0);
        [SerializeField] private float _RotationSmoothSpeed = 5f;

        [Header("Objects References")]
        [SerializeField] private Volume _Volume;
        [SerializeField] private Transform _PlayerTransform;
        [SerializeField] private Camera _PlayerCamera;
        [SerializeField] private Camera _BackGroundCamera;
        [SerializeField] private Camera _OnlyPlayerCamera;

        private ShadowsMidtonesHighlights _ShadowsMidtonesHighlights;
        private WaitForEndOfFrame _WaitForEndOfFrame = new WaitForEndOfFrame();

        private Vector3 _CalculatedPos;
        private Quaternion _CalculatedRot;
        private bool _IsDeathActive = false;

        void Start()
        {
            if (_Volume.profile.TryGet(out _ShadowsMidtonesHighlights))
            {
                _ShadowsMidtonesHighlights.shadows.overrideState = true;
                _ShadowsMidtonesHighlights.midtones.overrideState = true;
                _ShadowsMidtonesHighlights.highlights.overrideState = true;
            }
        }
        private void LateUpdate()
        {
            if (_IsDeathActive)
            {
                _OnlyPlayerCamera.transform.position = _BackGroundCamera.transform.position = _CalculatedPos;
                _OnlyPlayerCamera.transform.rotation = _CalculatedRot;
            }
        }

        public void Init()
        {
            _IsDeathActive = true;
            SetIntensity(0, 0, 0);

            _BackGroundCamera.enabled = _OnlyPlayerCamera.enabled = true;
            _BackGroundCamera.orthographicSize = _OnlyPlayerCamera.orthographicSize = _PlayerCamera.orthographicSize;

            if(_TransitionTime > 0.01f) StartCoroutine(DarkCoroutine(_TransitionTime));
            else
            {
                _OnlyPlayerCamera.orthographicSize = _FinalOrthographicSize;
                CameraTrans(Mathf.Abs(_TargetAngle), -10f);
                SetIntensity(-1, -1, -1);
            }
        }

        public void ResetState()
        {
            _IsDeathActive = false;
            _BackGroundCamera.enabled = _OnlyPlayerCamera.enabled = false;
            _OnlyPlayerCamera.transform.rotation = Quaternion.identity;
            SetIntensity(0, 0, 0);
        }

        private IEnumerator DarkCoroutine(float pTime)
        {
            float lElapsedTime = 0f;
            float lStartSize = _OnlyPlayerCamera.orthographicSize;
            float lCameraZOffset = -10f;

            while (lElapsedTime < pTime)
            {
                lElapsedTime += Time.deltaTime;
                float lRatio = _TransitionCurve.Evaluate(lElapsedTime / pTime);

                float lTransitionValue = Mathf.Lerp(0, -1, lRatio);
                SetIntensity(lTransitionValue, lTransitionValue, lTransitionValue);

                _OnlyPlayerCamera.orthographicSize = Mathf.Lerp(lStartSize, _FinalOrthographicSize, lRatio);

                float lCurrentAngle = Mathf.Lerp(0, Mathf.Abs(_TargetAngle), lRatio);

                CameraTrans(lCurrentAngle, lCameraZOffset);

                yield return _WaitForEndOfFrame;
            }

            _OnlyPlayerCamera.orthographicSize = _FinalOrthographicSize;
            CameraTrans(Mathf.Abs(_TargetAngle), lCameraZOffset);
            SetIntensity(-1, -1, -1);
        }

        private void CameraTrans(float pCurrentAngle, float pCameraZOffset)
        {
            Quaternion lRotation = Quaternion.Euler(pCurrentAngle, 0f, 0f);
            Vector3 lPositionOffset = lRotation * new Vector3(0, 0, pCameraZOffset);
            Vector3 lTargetLookAt = _PlayerTransform.position + _TargetOffset;

            _CalculatedPos = lTargetLookAt + lPositionOffset;
            _CalculatedRot = Quaternion.LookRotation(lTargetLookAt - _CalculatedPos);
        }

        private void SetIntensity(float pShadowsValue, float pMidtonesValue, float pHighlightsValue)
        {
            if (_ShadowsMidtonesHighlights != null)
            {
                Vector4 lShadows = _ShadowsMidtonesHighlights.shadows.value;
                Vector4 lMidtones = _ShadowsMidtonesHighlights.midtones.value;
                Vector4 lHighlight = _ShadowsMidtonesHighlights.highlights.value;
                lShadows.w = pShadowsValue;
                lMidtones.w = pMidtonesValue;
                lHighlight.w = pHighlightsValue;
                _ShadowsMidtonesHighlights.shadows.value = lShadows;
                _ShadowsMidtonesHighlights.midtones.value = lMidtones;
                _ShadowsMidtonesHighlights.highlights.value = lHighlight;
            }
        }
    }
}