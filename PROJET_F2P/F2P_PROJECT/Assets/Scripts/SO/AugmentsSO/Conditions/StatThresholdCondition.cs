using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Conditions
{
    public class StatThresholdCondition : AugmentCondition
    {
        [SerializeField]
        private StatType _StatType;
        [SerializeField]
        private float _Threshold;
        [SerializeField]
        private bool _Above;

        public override bool IsMet(IAugmentable pPlayer)
        {
            float lValue = pPlayer.GetStat(_StatType);    
            return _Above ? lValue >= _Threshold : lValue < _Threshold; 
        }
        protected override string GetDescription()
        {
            return $"When {_StatType} is {(_Above ? "above" : "below")} {_Threshold}";
        }
    }
}
