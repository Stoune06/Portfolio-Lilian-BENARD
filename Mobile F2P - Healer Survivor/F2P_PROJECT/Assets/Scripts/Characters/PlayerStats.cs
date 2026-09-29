using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    public enum StatType
    {
        Damage,
        Range,
        AttackSpeed,
        MoveSpeed,
        MaxHP,
        PickupRange,
    }
        
    public enum ModifierType
    {
        Additive,
        Multiplicative
    }
        
    [System.Serializable]
    public struct StatModifier
    {
        public StatType statType;
        public ModifierType modifierType;
        public float  value;
    }
}
