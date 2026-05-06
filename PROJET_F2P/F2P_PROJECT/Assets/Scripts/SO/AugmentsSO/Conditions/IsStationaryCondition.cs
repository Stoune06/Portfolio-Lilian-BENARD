using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Conditions
{
    public class IsStationaryCondition : AugmentCondition
    {
        [SerializeField]
        private bool _Inverted;

        public override bool IsMet(IAugmentable pPlayer)
        {
            return _Inverted ? !pPlayer.isStationary : pPlayer.isStationary;
        }

        protected override string GetDescription()
        {
            return _Inverted ? "When moving" : "When stationary";
        }
    }
}