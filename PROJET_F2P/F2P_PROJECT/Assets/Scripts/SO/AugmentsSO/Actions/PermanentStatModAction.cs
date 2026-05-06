using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using Managers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class PermanentStatModAction : AugmentAction
    {
        [SerializeField] private StatType _StatType;
        [SerializeField] private ModifierType _ModifierType;
        [SerializeField] private float _Value;

        private List<StatModifier> _Modifiers = new List<StatModifier>();
        private List<PlayerStatsManager> _AffectedManagers = new List<PlayerStatsManager>();

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            StatModifier lMod = new StatModifier
                {
                    statType = _StatType,
                    modifierType = _ModifierType,
                    value = _Value
                };
            _Modifiers.Add(lMod);

            List<Transform> lTargets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            foreach (Transform lTarget in lTargets)
            {
                PlayerStatsManager lStats = (lTarget == pPlayer.transform)
                    ? pPlayer.statsManager
                    : lTarget.GetComponentInParent<Character>()?.statsManager;
                if (lStats != null)
                {
                    lStats.AddModifier(lMod);
                    if (!_AffectedManagers.Contains(lStats))
                        _AffectedManagers.Add(lStats);
                }
            }
        }

        public override void OnRemove(IAugmentable pPlayer)
        {
            foreach (PlayerStatsManager lManager in _AffectedManagers)
            {
                if (lManager == null) continue;
                foreach (StatModifier lMod in _Modifiers) lManager.RemoveModifier(lMod);
            }
            _Modifiers.Clear();
            _AffectedManagers.Clear();
        }

        public override string GetDescription()
        {
            string lPrefix = _ModifierType == ModifierType.Multiplicative ? "x" : "+"; 
            return $"{lPrefix}{_Value}{_StatType} (permanent, stacks)";
    }
}

}
