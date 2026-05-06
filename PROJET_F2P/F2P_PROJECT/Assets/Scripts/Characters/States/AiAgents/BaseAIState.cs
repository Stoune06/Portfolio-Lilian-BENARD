using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay.Enemies
{
    public class BaseAIState : BaseState
    {
        
        protected InfluenceCharacter m_InfluenceCharacter;

        public BaseAIState(Character pCharacter, Animator pAnimator) : base(pCharacter, pAnimator)
        {
            m_InfluenceCharacter = pCharacter.GetComponent<InfluenceCharacter>();
        }
    }
}
