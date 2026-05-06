using Com.IsartDigital.HealerSurvivor.Manager;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Conditions
{
    public class WaveCondition : AugmentCondition
    {
        [SerializeField]
        private int _MinWave;

        public override bool IsMet(IAugmentable pPlayer)
        {
            return GameManager.Instance != null && GameManager.Instance.CurrentWave >= _MinWave;
        }
        protected override string GetDescription()
        {
            return $"From wave {_MinWave}";
        }
    }
}
