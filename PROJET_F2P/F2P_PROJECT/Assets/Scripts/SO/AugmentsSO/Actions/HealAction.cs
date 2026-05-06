using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class HealAction : AugmentAction
    {
        [SerializeField]
        private float _HealAmount;

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            pPlayer.InvokeHealCast();
            List<Transform> lTargets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            foreach (Transform lTarget in lTargets)
            {
                if (lTarget == pPlayer.transform)
                    pPlayer.Heal(_HealAmount);
                else
                {
                    Character lCharacter = lTarget.GetComponent<Character>();
                    if (lCharacter)
                    {
                        lCharacter.Heal(_HealAmount);
                        pPlayer.InvokeOnHealAlly(_HealAmount, lTarget);
                    }
                }
            }
        }

        public override string GetDescription()
        {
            return $"Heal {_HealAmount} HP";
        }
    }
}