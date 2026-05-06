using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Gameplay.Enemies;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    
    public class AttackState : BaseAIState
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public AttackState(Character pCharacter, Animator pAnim) : base(pCharacter, pAnim) { }

        public override void OnEnter()
        {
            base.OnEnter();
            m_Character.velocity = Vector3.zero;
        }

        public override void Update()
        {
            base.Update();
            m_Character.velocity = Vector3.zero;
            if (!m_Character.attackType.TryAttack() && !m_Character.attackType.CheckRange(0))
                m_Character.stateMachine.ChangeState(EState.AIMOVE);
        }
    }
}