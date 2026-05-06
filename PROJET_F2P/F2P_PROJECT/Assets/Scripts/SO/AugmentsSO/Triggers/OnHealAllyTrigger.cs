using System;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnHealAllyTrigger : AugmentTrigger
    {
        public override void Subscribe() => m_Player.OnHealAlly += DoAction;
        public override void Unsubscribe() => m_Player.OnHealAlly -= DoAction;

        private void DoAction(float pAmount, Transform pTarget)
        {
            lastAmount = pAmount;
            lastTarget = pTarget;
            m_Callback();
        }
    }
}