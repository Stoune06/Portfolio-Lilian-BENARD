using UnityEngine;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CreateAssetMenu(fileName = "AttackStats", menuName = "Scriptable Objects/AttacksStats")]
    public class AttackStatsSO : ScriptableObject
    {
        [Tooltip("Range final = baseRange + EntityRange")]
        public float baseRange;
        [Tooltip("Damage final = _BaseDamage * EntityDamageFactor")]
        public float baseDamage;
        [Tooltip("Attack speed final = baseAttackSpeed * EntityAttackSpeedFactor")]
        public float baseAttackSpeed;

        public float AttackPreparationTime;

        [Tooltip("Weapon KnockBack")]
        public float knockback;


        public ScanPaternEnum paternEnum;

        //Box
        public float width;

        //Cone
        public float angle;

        //Custom
        public CustomAttack customAttack;
        public bool isInstanciateOnSelf = true;

        //Ray
        public float maxRange;

        public bool toTarget = true;
    }
}
