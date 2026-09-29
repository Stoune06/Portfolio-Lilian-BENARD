using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom
{
    [RequireComponent(typeof(Collider))]
    public class ColliderAttack : CustomAttack
    {
        protected List<Collider> m_Targets = new();


        private void OnTriggerEnter(Collider pCollider)
        {
            if (!m_Targets.Contains(pCollider))
                AddCollider(pCollider);
        }

        protected ITarget GetTarget(Collider pCollider)
        {
            ITarget lTarget;
            pCollider.TryGetComponent(out lTarget);

            return lTarget;
        }

        protected virtual void AddCollider(Collider pCollider)
        {
            m_Targets.Add(pCollider);
        }


        protected void EmitAttack(Collider pCollider)
        {
            HitInfoStruct lAttack = new HitInfoStruct(pCollider.transform, GetTarget(pCollider), transform.position, m_stats.damage, m_stats.knockback);
            m_stats.canal?.Invoke(lAttack);
        }

        public Transform InstantiateLocalColliders(ColliderAttack pObject, Vector3 pLocalPosition, Quaternion pRotation = default)
        {
            ColliderAttack lAttack;
            lAttack = Instantiate(pObject, transform.position + pLocalPosition, pRotation);
            lAttack.SetStats(m_stats);

            return lAttack.transform;
        }

        public Transform InstantiateAdditionalColliders(ColliderAttack pObject, Vector3 pGlobalPosition, Quaternion pRotation = default)
        {
            ColliderAttack lAttack;
            lAttack = Instantiate(pObject, pGlobalPosition, pRotation);
            lAttack.SetStats(m_stats);

            return lAttack.transform;
        }

        public Transform InstantiateCollidersWithCustomStats(ColliderAttack pObject, float pDamage, float pKnockback, Vector3 pGlobalPosition, Quaternion pRotation = default)
        {
            ColliderAttack lAttack;
            lAttack = Instantiate(pObject, pGlobalPosition, pRotation);
            lAttack.SetStats(m_stats);

            return lAttack.transform;
        }
    }
}
