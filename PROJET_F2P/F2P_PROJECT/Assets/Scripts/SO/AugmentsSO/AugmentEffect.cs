using System.Collections.Generic;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO
{
    [System.Serializable]
    public abstract class AugmentEffect
    {
        [SerializeReference]
        public List<AugmentCondition> conditions = new List<AugmentCondition>();

        protected virtual bool AreConditionsMet(IAugmentable pPlayer)
        {
            foreach (AugmentCondition lCondition in conditions)
            {
                if (lCondition == null) continue;
                if (!lCondition.IsMet(pPlayer)) return false;
            }
            return true;
        }
        
        public abstract void Apply(IAugmentable pPlayer);

        public abstract void Remove(IAugmentable pPlayer);

        public abstract string GetDescription();
    }
}
