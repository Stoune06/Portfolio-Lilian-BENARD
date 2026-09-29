using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom
{
    [RequireComponent(typeof(Collider))]
    public class OnCollisionAttack : ColliderAttack
    {
        [SerializeField] private int _MaxTarget = 1;

        protected override void AddCollider(Collider pCollider)
        {
            base.AddCollider(pCollider);
            EmitAttack(pCollider);

            if (m_Targets.Count >= _MaxTarget) Destroy(gameObject);
        }
    }
}
