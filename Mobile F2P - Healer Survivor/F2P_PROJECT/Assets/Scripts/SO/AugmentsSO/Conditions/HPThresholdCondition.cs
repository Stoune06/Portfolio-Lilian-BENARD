using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Conditions
{
    public class HPThresholdCondition : AugmentCondition
    {
        [SerializeField, Range(0, 1f)]
        private float _Threshold;
        [SerializeField]
        private bool _BelowThreshold;
        
        
        public override bool IsMet(IAugmentable pPlayer)
        {
            if (pPlayer.targetMaxHealth <= 0f) return false;
            float lHpPercent = pPlayer.health / pPlayer.targetMaxHealth;
            return _BelowThreshold ? lHpPercent <= _Threshold : lHpPercent >= _Threshold;
        }
        protected override string GetDescription()
        {
            string lComparison = _BelowThreshold ? "below" : "above";
            return $"When HP is {lComparison} {_Threshold * 100}%";

        }
    }
}
