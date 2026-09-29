using UnityEngine;

namespace Platformer.Player
{
    public class PlayerMoveState : PlayerGroundedState
    {
        private float _MoveVelocity;
        public PlayerMoveState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats,  string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            m_Player.playerAnimator.SetFloat("speed", 1f);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(m_MovementDirection.x == 0) m_StateMachine.ChangeState(m_Player.idleState);
            m_Player.CheckIfShouldFlip(m_MovementDirection.x);
            m_Player.playerAnimator.SetFloat("speed", Mathf.Abs(m_MovementDirection.x));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //Debug.Log("MouvementDirection : " + m_MovementDirection.x);
            _MoveVelocity = Mathf.Lerp(m_Player.velocity.x, m_PlayerStats.maxRunSpeed * m_MovementDirection.x, m_PlayerStats.groundAcceleration * Time.fixedDeltaTime);
            m_Player.SetVelocityX(_MoveVelocity);
        }
    }

}