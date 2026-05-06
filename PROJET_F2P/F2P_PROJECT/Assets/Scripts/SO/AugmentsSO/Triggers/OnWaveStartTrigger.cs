using System;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public class OnWaveStartTrigger : AugmentTrigger
    {
        public override void Subscribe() => m_Player.OnWaveStart += DoAction;
        public override void Unsubscribe() => m_Player.OnWaveStart -= DoAction;

        private void DoAction(int pWaveNumber)
        {
            m_Callback();
        }
    }
}