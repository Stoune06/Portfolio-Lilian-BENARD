using System;
using Tooling;
using UnityEngine;

namespace Platformer.Player
{
    public class PlayerInAirState : PlayerState
    {
        protected float m_HorizontalVelocity;
        protected float m_VerticalVelocity;
        public PlayerInAirState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            m_Player.customBody.OnWallFall += SwitchToWallJump;
            m_HorizontalVelocity = m_Player.velocity.x;
            m_VerticalVelocity = m_Player.velocity.y;
        }

        protected void SwitchToWallJump(Vector2 pNormal)
        {
            if (m_Player.onWallState.WallJumpCount > m_PlayerStats.MaxWallJump) return;

            Vector2 lDirection = pNormal.normalized;

            m_Player.FlipToDirection(lDirection);

            m_StateMachine.ChangeState(m_Player.onWallState);
        }

        public override void Exit()
        {
            base.Exit();
            m_Player.customBody.OnWallFall -= SwitchToWallJump;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (m_MovementDirection.x != 0)
            {
                m_Player.CheckIfShouldFlip(m_MovementDirection.x);
                m_HorizontalVelocity = Mathf.Lerp(m_Player.velocity.x, m_PlayerStats.maxRunSpeed * m_MovementDirection.x, m_PlayerStats.airAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                m_HorizontalVelocity = Mathf.Lerp(m_Player.velocity.x, 0f, m_PlayerStats.airDeceleration * Time.fixedDeltaTime);
            }
            m_Player.SetVelocityX(m_HorizontalVelocity);
            m_Player.SetVelocityY(m_VerticalVelocity);
        }

    }
}

