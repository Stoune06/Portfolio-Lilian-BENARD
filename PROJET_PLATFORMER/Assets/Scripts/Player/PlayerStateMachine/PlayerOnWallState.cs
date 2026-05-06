using Tooling;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Player
{
    public class PlayerOnWallState : PlayerState
    {
        public int WallJumpCount = 0;

        public PlayerOnWallState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            SetPhysic();
            InputManager.onJump += Jump;
            m_Player.customBody.OnWallExited += OnFall;
            WallJumpCount++;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Special Features/Wall Jump/MC_WallJump_HitWall");
        }

        public override void Exit()
        {
            base.Exit();
            InputManager.onJump -= Jump;
            m_Player.customBody.OnWallExited -= OnFall;
            m_Player.transform.localScale = new Vector3(m_Player.facingDirection, m_Player.transform.localScale.y, m_Player.transform.localScale.z);
        }

        private void Jump()
        {
            m_Player.SetVelocityX(m_Player.facingDirection * m_PlayerStats.wallImpulsion);
            m_Player.isWallJumping = true;
            m_StateMachine.ChangeState(m_Player.jumpState);
        }

        private void OnFall()
        {
            m_StateMachine.ChangeState(m_Player.fallState);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (m_Player.customBody.isOnGround) m_StateMachine.ChangeState(m_Player.landState);

            float lAxisY = InputManager.InputStick.GetAxisY();
            if(lAxisY < -m_PlayerStats.DeadZoneGravityMultiplier) 
                m_Player.SetVelocityY(-m_PlayerStats.OnWallVelocityY * m_PlayerStats.WallJumpForceGravityMultiplier);
            else m_Player.SetVelocityY(-m_PlayerStats.OnWallVelocityY);
        }

        private void SetPhysic()
        {
            m_Player.SetVelocityY(-m_PlayerStats.OnWallVelocityY);
            m_Player.SetVelocityX(0);
        }
    }
}

