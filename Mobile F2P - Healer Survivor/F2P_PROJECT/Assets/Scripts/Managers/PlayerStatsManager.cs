using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
namespace Managers
{
    public class PlayerStatsManager
    {
        private Dictionary<StatType, List<StatModifier>> _PlayerModifiers = new Dictionary<StatType, List<StatModifier>>();

        public IReadOnlyDictionary<StatType, List<StatModifier>> PlayerModifiers => _PlayerModifiers;

        public PlayerStatsManager()
        {
            foreach (StatType lStat in Enum.GetValues(typeof(StatType)))
            {
                _PlayerModifiers[lStat] = new List<StatModifier>();
            }
        }

        public void AddModifier(StatModifier pModifier)
        {
            _PlayerModifiers[pModifier.statType].Add(pModifier);
        }

        public void RemoveModifier(StatModifier pModifier)
        {
            _PlayerModifiers[pModifier.statType].Remove(pModifier);
        }

        public float GetFinalValue(StatType pStatType, float pBaseValue)
        {
            float lFinalValue = pBaseValue;
            List<StatModifier> lModifiers = _PlayerModifiers[pStatType];
            foreach (StatModifier lModifier in lModifiers)
            {
                if (lModifier.modifierType == ModifierType.Additive)
                    lFinalValue += lModifier.value;
            }
            foreach (StatModifier lModifier in lModifiers)
            {
                if (lModifier.modifierType == ModifierType.Multiplicative)
                    lFinalValue *= lModifier.value;
            }
            return lFinalValue;
        }
    }
}
