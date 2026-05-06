using UnityEngine;
using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class DamageAreaAction : AugmentAction
    {
        [SerializeField] private float _Damage;
        [SerializeField] private float _Radius;
        [SerializeField] private LayerMask _Layer;

        private static readonly Collider[] _Buffer = new Collider[32];

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            List<Transform> lOrigins = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            foreach (Transform lOrigin in lOrigins)
            {
                int lCount = Physics.OverlapSphereNonAlloc(lOrigin.position, _Radius, _Buffer, _Layer);
                for (int i = 0; i < lCount; i++)
                {
                    Character lTarget = _Buffer[i].GetComponentInParent<Character>();
                    if (lTarget != null) lTarget.TakeDamage(_Damage);
                }
            }
        }

        public override string GetDescription()
        {
            return $"Deal {_Damage} damage in {_Radius}m radius";
        }
    }
}
