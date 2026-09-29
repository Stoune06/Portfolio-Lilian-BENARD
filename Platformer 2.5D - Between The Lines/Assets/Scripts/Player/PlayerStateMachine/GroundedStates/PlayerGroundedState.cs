using Tooling;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Platformer.Player
{
    public class PlayerGroundedState : PlayerState
    {
        private float _CoyoteTimer = 0f;

        public PlayerGroundedState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }
        

        public override void Enter()
        {
            base.Enter();
            InputManager.onJump += Jump;
            _CoyoteTimer = 0f;
            if(m_Player.onWallState.WallJumpCount != 0) m_Player.onWallState.WallJumpCount = 0;
        }

        public override void Exit()
        {
            base.Exit();
            InputManager.onJump -= Jump;

        }
        private void Jump()
        {
            m_StateMachine.ChangeState(m_Player.jumpState);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(!m_Player.customBody.isOnGround)
            {
                m_Player.coyoteTimer = true;
                m_StateMachine.ChangeState(m_Player.fallState);
            }
            else if(_CoyoteTimer !=  0) _CoyoteTimer = 0;
            
        }
    }
}

