using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared
{
    public struct HitInfoStruct
    {
        public Transform hitTransform;
        public ITarget target;
        public Vector3 origin;
        public float damage;
        public float knockBack;

        public HitInfoStruct(Transform pHitObject, ITarget pTarget, Vector3 pOrigine, float pDamage, float pKnockBack)
        {
            hitTransform = pHitObject;
            target = pTarget;
            damage = pDamage;
            knockBack = pKnockBack;
            origin = pOrigine;
        }
    }
}