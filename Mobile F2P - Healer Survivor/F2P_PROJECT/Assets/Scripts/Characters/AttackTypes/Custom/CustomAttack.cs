using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom
{
    [RequireComponent(typeof(Collider))]
    public class CustomAttack : MonoBehaviour
    {
        [SerializeField] protected float m_LifeTime = 10f;
        private float _Time = 0f;

        protected CustomAttackStats m_stats;

        public void SetStats(float pDamage, float pKnockback, IStatProvider pStatProvider, LayerMask pAttackLayers, Action<HitInfoStruct> pCanal)
        {
            m_stats = new CustomAttackStats(transform, pDamage, pKnockback, pStatProvider, pAttackLayers, pCanal);
        }

        public void SetStats(CustomAttackStats pStats)
        {
            m_stats = pStats;

            Collider lCollider = GetComponent<Collider>();

            lCollider.includeLayers = pStats.attackLayers;
            lCollider.excludeLayers = ~pStats.attackLayers;
        }

        protected virtual void Update()
        {
            CheckLifeTime();
        }

        protected void CheckLifeTime()
        {
            _Time += Time.deltaTime;
            if (_Time > m_LifeTime) Destroy(gameObject);
        }
    }

    public struct CustomAttackStats
    {
        private float _BaseDamage;
        public float damage => statProvider != null ? _BaseDamage * statProvider.GetStat(StatType.Damage) : 0f;

        public float knockback;

        public LayerMask attackLayers;
        public IStatProvider statProvider;
        public Action<HitInfoStruct> canal;

        public CustomAttackStats(float pDamage, float pKnockback, IStatProvider pStatProvider, LayerMask pAttackLayers, Action<HitInfoStruct> pCanal)
        {
            _BaseDamage = pDamage;
            knockback = pKnockback;
            statProvider = pStatProvider;
            attackLayers = pAttackLayers;
            canal = pCanal;
        }

        public CustomAttackStats(Transform pOwner, float pDamage, float pKnockback, IStatProvider pStatProvider, LayerMask pAttackLayers, Action<HitInfoStruct> pCanal)
        {
            _BaseDamage = pDamage;
            knockback = pKnockback;
            statProvider = pStatProvider;
            attackLayers = pAttackLayers;
            canal = pCanal;

            Collider lCollider = pOwner.GetComponent<Collider>();

            lCollider.includeLayers = pAttackLayers;
            lCollider.excludeLayers = ~pAttackLayers;
        }
    }
}
