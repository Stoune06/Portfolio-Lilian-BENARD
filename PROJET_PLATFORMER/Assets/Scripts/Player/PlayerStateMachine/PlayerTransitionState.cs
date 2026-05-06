using Platformer.Managers;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Player
{
    public class PlayerTransitionState : PlayerIdleState
    {
        public PlayerTransitionState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            m_Player.SetVelocityX(0);
            m_Player.SetVelocityY(0);
        }

        public override void LogicUpdate() { }


        public override void Exit()
        {
            base.Exit();
        }
    }
}

