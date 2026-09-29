using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Conditions
{
    public class AllyCountCondition : AugmentCondition
    {
        [SerializeField]
        private int _MinAllyCount;

        public override bool IsMet(IAugmentable pPlayer)
        {
            return Character.allyAliveCount >= _MinAllyCount;

        }
        protected override string GetDescription()
        {
            return $"When at least {_MinAllyCount} allies are alive";

        }
    }
}
