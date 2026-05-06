using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Inputs;
using UnityEngine;

// Author : Florian MAJCHER & Lilian BENARD- Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{

    public class MoveState : BaseState
    {
        private Player pPlayer;
        private float _Speed;
        private bool _HasJoystickInput;
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public MoveState(Character pCharacter, Animator pAnim) : base(pCharacter, pAnim) { }

        public override void OnEnter()
        {
            base.OnEnter();

            if(m_Character != null && m_Character.GetComponent<Player>()) pPlayer = m_Character.GetComponent<Player>();

            _HasJoystickInput = true;
            m_Character.dustParticles.Play();
            InputManager.OnJoystickMove +=  ChangeVelocity;
            InputManager.OnTouchRemove += ChangeToIdleState;
        }
        

        public override void Update()
        {
            base.Update();
            if (!_HasJoystickInput && m_Character.velocity.sqrMagnitude < 0.01f)
                m_Character.stateMachine.ChangeState(EState.IDLE);
        }

        public override void OnExit()
        {
            base.OnExit();
            //TODO : Fix particles pausing game + can't control joystick
            m_Character.dustParticles.Stop();
            InputManager.OnJoystickMove -=  ChangeVelocity;
            InputManager.OnTouchRemove -= ChangeToIdleState;
        }

        private void ChangeToIdleState()
        {
            m_Character.stateMachine.ChangeState(EState.IDLE);
        }

        private void ChangeVelocity(Vector2 pVelocity)
        {
            _HasJoystickInput = pVelocity.sqrMagnitude > 0.0001f;
            m_Character.velocity = Vector3.Lerp(m_Character.velocity,
                    new Vector3(pVelocity.x, 0, pVelocity.y) * pPlayer.GetStat(StatType.MoveSpeed),
                    Time.deltaTime *
                    m_Character.baseStats.acceleration);
        }
    }
}