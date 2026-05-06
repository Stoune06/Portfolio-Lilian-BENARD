using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared
{
    public interface IKnockbackable
    {
        public void TakeKnockback(Vector3 pKnockback);
    }
}
