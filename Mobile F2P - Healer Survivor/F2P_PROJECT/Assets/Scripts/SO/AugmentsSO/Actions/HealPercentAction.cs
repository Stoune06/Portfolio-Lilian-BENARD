using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class HealPercentAction : AugmentAction
    {
        [SerializeField]
        private float _HealPercent;

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            pPlayer.InvokeHealCast();
            List<Transform> lTargets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            foreach (Transform lTarget in lTargets)
            {
                if (lTarget == pPlayer.transform)
                    pPlayer.Heal(pPlayer.GetStat(StatType.MaxHP) * (_HealPercent / 100f));
                else
                {
                    Character lCharacter = lTarget.GetComponent<Character>();
                    if (lCharacter)
                    {
                        float lHealAmount = lCharacter.targetMaxHealth * (_HealPercent / 100f);
                        lCharacter.Heal(lHealAmount);
                        pPlayer.InvokeOnHealAlly(lHealAmount, lTarget);
                    }
                }
            }
        }
        public override string GetDescription()
        {
            return $"Heal {_HealPercent}% of MaxHP";
        }
    }
}
