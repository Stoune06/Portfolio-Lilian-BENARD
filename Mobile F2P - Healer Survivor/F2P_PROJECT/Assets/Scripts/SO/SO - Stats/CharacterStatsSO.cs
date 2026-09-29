using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    [CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStatsSO : ScriptableObject
    {
        [Header("Movements Stats")]
        public float moveSpeed;
        public float acceleration;
        public float deceleration;
        
        [Header("Attack Stats")]

        [Tooltip("Final AttackSpeed = attackSpeedFactor * weaponAttackSpeed")]
        public float attackSpeedFactor;
        [Tooltip("Final AttackDamage = AttackDamageFactor * weaponAttackDamage")]
        public float AttackDamageFactor;
        [Tooltip("Final range = additionalRange + weaponRange")]
        public float additionalRange;
        [Tooltip("Permet de définir la distance avec l'enenmi avant le lancement de la première attaque")]
        public float attackRangeHysteresis = 1.2f;
        
        [Header("Character Stats")]
        public float maxHealth;
        public float pickupRange;
    }
}
