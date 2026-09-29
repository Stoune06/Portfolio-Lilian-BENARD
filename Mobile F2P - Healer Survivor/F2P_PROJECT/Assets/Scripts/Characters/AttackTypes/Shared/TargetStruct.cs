using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared
{
    public struct TargetStruct
    {
        public Transform transform;
        public float distanceFromCenter;
        private ITarget _TargetInterface; 

        public TargetStruct(Transform pTransform, float pDistanceFromCenter)
        {
            transform = pTransform;
            distanceFromCenter = pDistanceFromCenter;
            _TargetInterface = null;
        }

        public ITarget targetInterface
        {
            get
            {
                if (_TargetInterface == null && transform != null)
                    transform.TryGetComponent(out _TargetInterface);
                return _TargetInterface;
            }
        }
    }
}
