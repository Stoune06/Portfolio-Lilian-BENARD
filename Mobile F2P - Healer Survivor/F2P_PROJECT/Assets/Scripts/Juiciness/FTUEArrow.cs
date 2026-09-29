using System;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Juiciness
{
    public class FTUEArrow : MonoBehaviour
    {
        [SerializeField] private float _HoverHeight = 2f;
        [SerializeField] private float _BobSpeed = 3f;
        [SerializeField] private float _BobAmplitude = 0.2f;
        [SerializeField] private bool _BillboardToCamera = true;

        private Transform _TargetTransform;
        private Character _TargetCharacter;
        private Camera _MainCamera;
        private bool _Completed;

        public static event Action<FTUEArrow> OnArrowCompleted;

        public Character Target => _TargetCharacter;

        public void Initialize(Character pTarget)
        {
            _TargetCharacter = pTarget;
            _TargetTransform = pTarget.transform;
            _MainCamera = Camera.main;
        }

        private void Update()
        {
            // Target gone (ally died or was destroyed): complete the arrow so FTUEManager's _RemainingArrows
            // counter decrements properly — otherwise the FTUE routine is stuck forever on an orphan count.
            if (_TargetTransform == null || _TargetCharacter == null)
            {
                if (!_Completed)
                {
                    _Completed = true;
                    OnArrowCompleted?.Invoke(this);
                }
                Destroy(gameObject);
                return;
            }

            Vector3 lBase = _TargetTransform.position + Vector3.up * _HoverHeight;
            float lBob = Mathf.Sin(Time.time * _BobSpeed) * _BobAmplitude;
            transform.position = lBase + Vector3.up * lBob;

            if (_BillboardToCamera && _MainCamera != null)
                transform.rotation = Quaternion.LookRotation(transform.position - _MainCamera.transform.position);

            if (!_Completed && _TargetCharacter.health >= _TargetCharacter.targetMaxHealth)
            {
                _Completed = true;
                OnArrowCompleted?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
