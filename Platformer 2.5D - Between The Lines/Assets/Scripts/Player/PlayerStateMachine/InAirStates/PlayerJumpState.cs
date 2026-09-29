using Tooling;
using Unity.VisualScripting;
using UnityEngine;

namespace Platformer.Player
{
    public class PlayerJumpState : PlayerInAirState
    {
        private bool _isPastApexThreshold;
        private bool _IsCut;
        private float _ApexPoint;
        private float _TimePastApexThreshold;

        private float _Gravity;
        private float _InitialVelocity;
        public PlayerJumpState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }
        

        public override void Enter()
        {
            base.Enter();
            _IsCut = false;
            m_Player.VfxManager.PlayJumpParticles();
            m_Player.customBody.OnWallFall += SwitchToWallJump;
            InputManager.onJumpReleased += JumpButtonReleased;
            m_Player.customBody.OnCeiling += FastFalling;
            if (m_Player.isWallJumping)
            {
                _InitialVelocity =  m_PlayerStats.InitialWallJumpVelocity;
                _Gravity = m_PlayerStats.WallJumpGravity;
            }
            else
            {
                _InitialVelocity = m_PlayerStats.InitialJumpVelocity;
                _Gravity = m_PlayerStats.Gravity;
            }
            m_VerticalVelocity = _InitialVelocity;
            m_Player.SetVelocityY(m_VerticalVelocity);
            
            _TimePastApexThreshold = 0f;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Movements/Jump");
        }
        
        public override void Exit()
        {
            base.Exit();
            InputManager.onJumpReleased -= JumpButtonReleased;
            m_Player.customBody.OnWallFall -= SwitchToWallJump;
            m_Player.customBody.OnCeiling -= FastFalling;
            m_Player.isWallJumping = false;
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (m_Player.isWallJumping) m_MovementDirection = m_Player.velocity.normalized;
        }



        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            _ApexPoint = Mathf.InverseLerp(_InitialVelocity, 0f, m_VerticalVelocity);
            if(_IsCut) TestJumpCut();
            
            if (_ApexPoint > m_PlayerStats.apexThreshold)
            {
                if (!_isPastApexThreshold && m_VerticalVelocity >= 0.01f)
                {
                    _isPastApexThreshold = true;
                    _TimePastApexThreshold = 0f;
                }

                else
                {
                    _TimePastApexThreshold += Time.fixedDeltaTime;
                    if (_TimePastApexThreshold < m_PlayerStats.apexHangTime && !m_Player.isWallJumping)
                    {
                        m_VerticalVelocity = 0f;
                    }
                    else
                    {
                        m_VerticalVelocity = -0.01f;
                        m_StateMachine.ChangeState(m_Player.fallState);
                    }
                }
            }
            else
            {
                //Debug.Log("Gravity");
                m_VerticalVelocity += _Gravity * Time.fixedDeltaTime;
            }
        }

        private void JumpButtonReleased()// Jump cut
        {
            if(!_IsCut) _IsCut = true;
        }

        private void TestJumpCut()
        {
            if (m_VerticalVelocity > 0 && Time.time - m_StartTime > m_PlayerStats.timeForUpwardsCancel)
            {
                m_VerticalVelocity *= 0.5f;
                _isPastApexThreshold = true;
                _TimePastApexThreshold = m_PlayerStats.apexHangTime + 1;
                _IsCut = false;
            }
        }
        
        private void FastFalling()
        {
            m_StateMachine.ChangeState(m_Player.fallState);
            m_Player.isFastFalling = true;
        }


    }
}

