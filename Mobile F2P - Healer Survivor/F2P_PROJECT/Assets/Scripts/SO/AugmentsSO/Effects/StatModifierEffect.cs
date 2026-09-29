using Com.IsartDigital.HealerSurvivor.Gameplay;
using Managers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO
{
    [System.Serializable]
    public class StatModifierEffect : AugmentEffect
    {
        
        [SerializeField] private StatType _StatType;
        [SerializeField] private ModifierType _ModifierType;
        [SerializeField] private float _Value;

        private StatModifier _StatModifier;

        public override void Apply(IAugmentable pPlayer)
        {
            _StatModifier = new StatModifier();
            _StatModifier.statType = _StatType;
            _StatModifier.modifierType = _ModifierType;
            _StatModifier.value = _Value;
            pPlayer.statsManager.AddModifier(_StatModifier);
        }
        
        public override void Remove(IAugmentable pPlayer)
        {
            pPlayer.statsManager.RemoveModifier(_StatModifier);
        }
        
        public override string GetDescription()
        {
            string lPrefix = _ModifierType == ModifierType.Multiplicative ? "x" : "+";
            return lPrefix + _Value + " " + _StatType.ToString();
        }
    }
}
