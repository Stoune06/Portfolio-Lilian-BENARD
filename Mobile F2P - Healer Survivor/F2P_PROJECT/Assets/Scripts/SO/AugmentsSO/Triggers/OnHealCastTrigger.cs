using System;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnHealCastTrigger : AugmentTrigger
    {
        public override void Subscribe() => m_Player.OnHealCast += DoAction;
        public override void Unsubscribe() => m_Player.OnHealCast -= DoAction;

        private void DoAction()
        {
            m_Callback();
        }
    }
}