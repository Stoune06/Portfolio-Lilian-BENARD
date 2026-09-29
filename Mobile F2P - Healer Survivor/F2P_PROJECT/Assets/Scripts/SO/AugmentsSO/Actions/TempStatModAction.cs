using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using Managers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class TempStatModAction : AugmentAction
    {
        [SerializeField] private StatType _StatType;
        [SerializeField] private ModifierType _ModifierType;
        [SerializeField] private float _Value;
        [SerializeField] private float _Duration;

        private struct BuffData
        {
            public StatModifier modifier;
            public Coroutine coroutine;
        }

        private readonly Dictionary<PlayerStatsManager, BuffData> _ActiveBuffs = new Dictionary<PlayerStatsManager, BuffData>();
        private IAugmentable _Player;

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            _Player = pPlayer;
            List<Transform> lTargets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);

            foreach (Transform lTarget in lTargets)
            {
                PlayerStatsManager lStats = (lTarget == pPlayer.transform)
                    ? pPlayer.statsManager
                    : lTarget.GetComponentInParent<Character>()?.statsManager;

                if (lStats == null) continue;

                if (_ActiveBuffs.TryGetValue(lStats, out BuffData lExisting))
                {
                    pPlayer.StopCoroutine(lExisting.coroutine);
                    lExisting.coroutine = pPlayer.StartCoroutine(RemoveAfterDelay(lStats));
                    _ActiveBuffs[lStats] = lExisting;
                }
                else
                {
                    StatModifier lModifier = new StatModifier
                    {
                        statType = _StatType,
                        modifierType = _ModifierType,
                        value = _Value
                    };
                    lStats.AddModifier(lModifier);
                    _ActiveBuffs[lStats] = new BuffData
                    {
                        modifier = lModifier,
                        coroutine = pPlayer.StartCoroutine(RemoveAfterDelay(lStats))
                    };
                }
            }
        }

        private IEnumerator RemoveAfterDelay(PlayerStatsManager pStats)
        {
            yield return new WaitForSeconds(_Duration);
            if (_ActiveBuffs.TryGetValue(pStats, out BuffData lData))
            {
                pStats.RemoveModifier(lData.modifier);
                _ActiveBuffs.Remove(pStats);
            }
        }

        public override void OnRemove(IAugmentable pPlayer)
        {
            foreach (KeyValuePair<PlayerStatsManager, BuffData> lEntry in _ActiveBuffs)
            {
                if (lEntry.Value.coroutine != null)
                    pPlayer.StopCoroutine(lEntry.Value.coroutine);
                if (lEntry.Key != null)
                    lEntry.Key.RemoveModifier(lEntry.Value.modifier);
            }
            _ActiveBuffs.Clear();
        }

        public override string GetDescription()
        {
            string lPrefix = _ModifierType == ModifierType.Multiplicative ? "x" : "+";
            return $"{lPrefix}{_Value} {_StatType} for {_Duration}s";
        }
    }
}