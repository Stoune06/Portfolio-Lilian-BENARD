using Platformer.Tools;
using System.Collections;
using UnityEngine;

namespace Platformer.Platforms
{
    public class DestroyablePlatform : PlatformCollision
    {
        [SerializeField] private float _BreakDuration = 1f;
        [SerializeField] private float _TimeBeforeRespawn = 1f;
        [SerializeField] private bool _Active = true;

        private float _MinDissolveValue = -0.1f;
        private float _MaxDissolveValue = 1.1f;

        private Coroutine _StateCoroutine = null;
        private Coroutine _RespawnTimerCoroutine = null;

        private int _DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        private Material _Material;

        private float _CurrentProgress = 0f;
        private bool _IsTargetingDestruction = false;

        private new void Start()
        {
            base.Start();
            _Material = m_Renderer.material;
            _Material.SetFloat(_DissolveAmount, _MinDissolveValue);
        }

        public void DestroyPlatform()
        {
            if (!_Active) return;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/INTERRACTIONS/Platforms/Platform_Destroy");
            if (_RespawnTimerCoroutine != null) StopCoroutine(_RespawnTimerCoroutine);

            _IsTargetingDestruction = true;
            StartTransition();

            _RespawnTimerCoroutine = StartCoroutine(CustomTools.Timer(_TimeBeforeRespawn + _BreakDuration, RestorePlatform));
        }

        private void RestorePlatform()
        {
            if (!_Active) return;
            _IsTargetingDestruction = false;
            StartTransition();
        }

        private void StartTransition()
        {
            if (_StateCoroutine != null) StopCoroutine(_StateCoroutine);
            _StateCoroutine = StartCoroutine(TransitionCoroutine());
        }

        private IEnumerator TransitionCoroutine()
        {
            if (!_IsTargetingDestruction) ChangePlatformState(true);

            float lTargetValue = _IsTargetingDestruction ? 1f : 0f;

            while (!Mathf.Approximately(_CurrentProgress, lTargetValue))
            {
                float lStep = Time.deltaTime / _BreakDuration;

                _CurrentProgress = Mathf.MoveTowards(_CurrentProgress, lTargetValue, lStep);

                float lDissolveValue = Mathf.Lerp(_MinDissolveValue, _MaxDissolveValue, _CurrentProgress);
                _Material.SetFloat(_DissolveAmount, lDissolveValue);

                yield return null;
            }

            if (_IsTargetingDestruction) ChangePlatformState(false);

            _StateCoroutine = null;
        }

        private void ChangePlatformState(bool pVisible)
        {
            if (m_Body != null) m_Body.gameObject.SetActive(pVisible);
            if (m_Collider != null) m_Collider.enabled = pVisible;
        }
    }
}