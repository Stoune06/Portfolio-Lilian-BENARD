using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom
{
    [RequireComponent(typeof(Collider))]
    public class DuringCollisionAttack : ColliderAttack
    {
        private void OnTriggerExit(Collider pCollider)
        {
            m_Targets.Remove(pCollider);
        }

        protected override void Update()
        {
            base.Update();

            int lLength = m_Targets.Count - 1;
            for (int i = lLength; i >= 0; i--)
                EmitFrameAttack(m_Targets[i]);
        }

        protected void EmitFrameAttack(Collider pCollider)
        {
            HitInfoStruct lAttack = new HitInfoStruct(pCollider.transform, GetTarget(pCollider), transform.position, m_stats.damage * Time.deltaTime, m_stats.knockback * Time.deltaTime);
            m_stats.canal?.Invoke(lAttack);
        }
    }
}
