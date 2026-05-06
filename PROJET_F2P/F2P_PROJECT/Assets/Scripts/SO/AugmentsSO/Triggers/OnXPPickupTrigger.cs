using System;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnXPPickupTrigger : AugmentTrigger
    {
        public override void Subscribe() => m_Player.OnXPPickup += DoAction;
        public override void Unsubscribe() => m_Player.OnXPPickup -= DoAction;

        private void DoAction()
        {
            m_Callback();
        }
    }
}