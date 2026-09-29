using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;

namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable] public class ApplyHoTAction : AugmentAction
    {
        [SerializeField] private float _TotalHeal;
        [SerializeField] private float _Duration;
        [SerializeField] private float _TickInterval;

        private Coroutine _ActiveCoroutine;
        private IAugmentable _Player;
        private List<Transform> _Targets;

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            _Player = pPlayer;
            if (_ActiveCoroutine != null) pPlayer.StopCoroutine(_ActiveCoroutine);
            _Targets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            pPlayer.InvokeHealCast();
            _ActiveCoroutine = pPlayer.StartCoroutine(HoTRoutine());
        }

        private IEnumerator HoTRoutine()
        {
            float lSafeInterval = Mathf.Max(_TickInterval, 0.01f);
            int lTickCount = Mathf.Max(1, Mathf.FloorToInt(_Duration / lSafeInterval));
            float lHealPerTick = _TotalHeal / lTickCount;
            for (int i = 0; i < lTickCount; i++)
            {
                yield return new WaitForSeconds(lSafeInterval);
                foreach (Transform lTarget in _Targets)
                {
                    if (!lTarget) continue;
                    if (lTarget == _Player.transform)
                        _Player.Heal(lHealPerTick);
                    else
                    {
                        Character lCharacter = lTarget.GetComponent<Character>();
                        if (lCharacter) lCharacter.Heal(lHealPerTick);
                    }
                }
            }
            _ActiveCoroutine = null;
        }

        public override void OnRemove(IAugmentable pPlayer)
        {
            if (_ActiveCoroutine != null)
            {
                pPlayer.StopCoroutine(_ActiveCoroutine);
                _ActiveCoroutine = null;
            }
        }

        public override string GetDescription()
        {
            return $"HoT: {_TotalHeal}HP over {_Duration}s";
    }
}

}
