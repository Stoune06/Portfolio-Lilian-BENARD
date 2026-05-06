using System;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnKillTrigger : AugmentTrigger
    {
        public override void Subscribe() => Character.OnKill += DoAction;
        public override void Unsubscribe() => Character.OnKill -= DoAction;

        private void DoAction(Transform pTransform)
        {
            lastTarget = pTransform;
            m_Callback();
        }
    }
}
