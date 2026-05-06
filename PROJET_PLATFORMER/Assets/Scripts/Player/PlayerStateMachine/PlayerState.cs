using Tooling;
using UnityEngine;

namespace Platformer.Player
{
    public class PlayerState
    {
        protected Player m_Player;
        protected PlayerStateMachine m_StateMachine;
        protected PlayerStats m_PlayerStats;
        protected Vector2 m_MovementDirection;

        protected float m_StartTime;

        private string _AnimationName;

        public PlayerState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName)
        {
            m_Player = pPlayer;
            m_StateMachine = pStateMachine;
            m_PlayerStats = pPlayerStats;
            _AnimationName = pAnimationName;
        }

        public virtual void Enter()
        {
            m_StartTime = Time.time;
            m_Player.playerAnimator.SetBool(_AnimationName, true);
            //Debug.Log(_AnimationName);
        }

        public virtual void Exit()
        {
            if (m_Player != null && m_Player.playerAnimator != null)
            {
                m_Player.playerAnimator.SetBool(_AnimationName, false);
            }
        }

        public virtual void LogicUpdate()
        {
            m_MovementDirection = InputManager.MovementDirection;
        }

        public virtual void PhysicsUpdate()
        {
        }


    }
}