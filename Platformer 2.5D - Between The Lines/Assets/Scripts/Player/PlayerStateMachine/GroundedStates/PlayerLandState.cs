using UnityEngine;

namespace Platformer.Player
{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName) : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            m_Player.SetVelocityY(0);
            m_StateMachine.ChangeState(m_Player.idleState);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Movements/Land_Grass");
        }
        
        public void PlayLandingSound()
        {
            string lEventPath = "event:/SFX/MC/Movements/land_Grass";

            RaycastHit2D lHit = Physics2D.Raycast(m_Player.transform.position, Vector2.down, 2f,m_Player._RaycastFootstepMask);
            if (lHit.collider != null)
            {
                switch (lHit.collider.tag)
                {
                    //Examples
                     case "Grass": lEventPath = "event:/SFX/MC/Movements/Land_Grass"; break;
                     case "Stone": lEventPath = "event:/SFX/MC/Movements/Land_Stone"; break;
                     case "Paint":  lEventPath = "event:/SFX/MC/Movements/Land_Paint"; break;
                }
            }

            FMODUnity.RuntimeManager.PlayOneShot(lEventPath);
        }
    }
}

