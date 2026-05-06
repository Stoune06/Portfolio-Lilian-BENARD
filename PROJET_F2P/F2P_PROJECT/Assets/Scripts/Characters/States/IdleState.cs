using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Inputs;
using UnityEngine;

// Author : Florian MAJCHER & Lilian BENARD - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    
    public class IdleState : BaseState
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public IdleState(Character pCharacter, Animator pAnim) : base(pCharacter, pAnim) {}

        public override void OnEnter()
        {
            base.OnEnter();
            InputManager.OnJoystickMove += ChangeToMove;
        }

        public override void Update()
        {
            base.Update();
            if (m_Character.velocity.magnitude > 0.01f)
            {
                m_Character.velocity = Vector3.Lerp(m_Character.velocity, Vector3.zero, Time.deltaTime * m_Character.baseStats.deceleration);
            }
            else if(m_Character.velocity.magnitude > 0)
            {
                m_Character.velocity = Vector3.zero;
            }
        }


        public override void OnExit()
        {
            base.OnExit();
            InputManager.OnJoystickMove -= ChangeToMove;
        }

        private void ChangeToMove(Vector2 pVelocity)
        {
            m_Character.stateMachine.ChangeState(EState.MOVE);
        }
    }
}