using Tooling;
using UnityEngine;

namespace Platformer.Player
{
    public class PlayerFallState : PlayerInAirState
    {
        private float _JumpBufferTimer;
        private bool _JumpReleasedDuringBuffer;
        private bool _TimerStarted;

        public PlayerFallState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            m_VerticalVelocity = m_Player.velocity.y;
            _JumpBufferTimer = 0;
            _TimerStarted = false;
            InputManager.onJump += TestJump;
        }

        public override void Exit()
        {
            base.Exit();
            InputManager.onJump -= TestJump;
            m_Player.coyoteTimer = false;
            m_Player.isFastFalling = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (_TimerStarted) _JumpBufferTimer += Time.deltaTime;
            
            if (m_Player.customBody.isOnGround)
            {
                m_VerticalVelocity = 0;
                if (_TimerStarted && _JumpBufferTimer <= m_PlayerStats.jumpBufferTime) m_StateMachine.ChangeState(m_Player.jumpState);
                else m_StateMachine.ChangeState(m_Player.landState);
            }
        }

        public override void PhysicsUpdate()
        {
            if(m_Player.isFastFalling) m_VerticalVelocity += m_PlayerStats.Gravity * m_PlayerStats.gravityOnReleaseMultiplier * m_PlayerStats.fastFallGravityMultiplier * Time.fixedDeltaTime;
            else m_VerticalVelocity += m_PlayerStats.Gravity * m_PlayerStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            m_VerticalVelocity = Mathf.Clamp(m_VerticalVelocity, -m_PlayerStats.maxFallSpeed, 0);
            base.PhysicsUpdate();
            
        }
        public void TestJump()
        {
            if(Time.time - m_StartTime <= m_PlayerStats.jumpCoyoteTime && m_Player.coyoteTimer) m_StateMachine.ChangeState(m_Player.jumpState);
            _TimerStarted = true;
        }
    }
}

