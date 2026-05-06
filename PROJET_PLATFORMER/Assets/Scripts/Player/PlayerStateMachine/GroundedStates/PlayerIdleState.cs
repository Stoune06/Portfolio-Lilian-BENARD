using UnityEngine;

namespace Platformer.Player
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats,  string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats,  pAnimationName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(m_Player.velocity.x != 0)
            {
                if (Mathf.Abs(m_Player.velocity.x) <= 0.01f)
                {
                    m_Player.SetVelocityX(0);
                }
                else
                {
                    m_Player.SetVelocityX(Mathf.Lerp(m_Player.velocity.x, 0f, m_PlayerStats.groundDeceleration * Time.fixedDeltaTime));
                }
                
            }
            m_Player.playerAnimator.SetFloat("speed", Mathf.Abs(m_Player.velocity.x) / m_PlayerStats.maxRunSpeed);
            if (m_MovementDirection.x != 0) m_StateMachine.ChangeState(m_Player.moveState);

        }

        public override void Exit()
        {
            base.Exit();
            //m_Player._Renderer.localRotation = Quaternion.Euler(0f, m_Player.facingDirection * 90f, 0f);
        }
    }

}
