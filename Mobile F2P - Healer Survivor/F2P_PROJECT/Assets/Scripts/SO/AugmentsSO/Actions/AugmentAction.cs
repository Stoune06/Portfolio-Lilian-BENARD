using System;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public abstract class AugmentAction
    {
        [SerializeField]
        protected TargetMode m_TargetMode;
        
        public abstract void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger);
        public virtual void OnApply(IAugmentable pPlayer) { }
        public virtual void OnRemove(IAugmentable pPlayer) { }
        public abstract string GetDescription();
    }
}