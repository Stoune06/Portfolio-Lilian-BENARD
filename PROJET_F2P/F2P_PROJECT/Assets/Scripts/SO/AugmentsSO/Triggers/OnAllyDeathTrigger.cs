using System;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnAllyDeathTrigger : AugmentTrigger
    {

        public override void Subscribe() => Character.OnAllyDied += DoAction;
        
        public override void Unsubscribe() => Character.OnAllyDied  -= DoAction;
        
        private void DoAction(Transform pTransform)
        {
            lastTarget = pTransform;
            m_Callback();
        }
    }
}
