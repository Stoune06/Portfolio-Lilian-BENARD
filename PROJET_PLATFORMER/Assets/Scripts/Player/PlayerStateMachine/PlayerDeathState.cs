using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Platformer.Player
{
    public class PlayerDeathState : PlayerState
    {
        private static readonly int _DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
        private static readonly int _EmitParticlesID = Shader.PropertyToID("EmitParticles");
        private static readonly int _DeathAnimationHash = Animator.StringToHash("DeathAnimation");

        private const float _DISSOLVE_START = 1f;

        private float _DissolveDuration = 2f;
        private bool _HasStartedDissolve;
        private Coroutine _ActiveDissolveCoroutine;

        public PlayerDeathState(Player pPlayer, PlayerStateMachine pStateMachine, PlayerStats pPlayerStats, string pAnimationName)
            : base(pPlayer, pStateMachine, pPlayerStats, pAnimationName) { }

        public override void Enter()
        {
            base.Enter();
            Player.OnPlayerDeath?.Invoke();
            _HasStartedDissolve = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/MC_Death");

            m_Player.customBody.enabled = false;
            m_Player.boxCollider.enabled = false;
            m_Player.SetVelocityX(0);
            m_Player.SetVelocityY(0);

            m_Player.Outline.SetActive(false);
            m_Player.DissolveMaterial.SetFloat(_DissolveAmountID, _DISSOLVE_START);

            m_Player.playerAnimator.speed = 1f;
            m_Player.DeathCamera.Init();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (_HasStartedDissolve) return;

            AnimatorStateInfo lStateInfo = m_Player.playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (lStateInfo.shortNameHash == _DeathAnimationHash && lStateInfo.normalizedTime >= 0.5f)
            {
                _HasStartedDissolve = true;
                //m_Player.playerAnimator.speed = 0f;
                m_Player.DissolveVFX.SetBool(_EmitParticlesID, true);
                _ActiveDissolveCoroutine = m_Player.StartCoroutine(DissolveCoroutine(_DissolveDuration));
            }
        }

        private IEnumerator DissolveCoroutine(float pTime, bool pDissolve = true)
        {
            float lElapsedTime = 0f;
            float lDissolveValue = 0;
            float lVfxValue = 0;

            m_Player.DissolveMaterial.SetFloat("_Dissolve", pDissolve ? 1 : 0);

            while (lElapsedTime < pTime)
            {
                lElapsedTime += Time.deltaTime;
                float lT = m_Player.DissolveCurve.Evaluate(lElapsedTime / pTime);

                if (pDissolve)
                {
                    lVfxValue = Mathf.Lerp(0, -1, lElapsedTime / pTime);
                    lDissolveValue = Mathf.Lerp(_DISSOLVE_START, -_DISSOLVE_START, lT);
                }
                else
                {
                    lVfxValue = Mathf.Lerp(-1, 0, lElapsedTime / pTime);
                    lDissolveValue = Mathf.Lerp(-_DISSOLVE_START, _DISSOLVE_START, lT);
                }

                    m_Player.DissolveMaterial.SetFloat(_DissolveAmountID, lDissolveValue);

                if (lT < 0.8f)
                {
                    m_Player.DissolveVFX.SetFloat(_DissolveAmountID, lVfxValue);
                }
                else
                {
                    m_Player.DissolveVFX.SetBool(_EmitParticlesID, false);
                }

                yield return null;
            }

            m_Player.playerAnimator.speed = 1f;
            if(pDissolve) m_StateMachine.ChangeState(m_Player.idleState);
            else m_Player.Outline.SetActive(true);
        }

        public override void Exit()
        {
            base.Exit();

            if (_ActiveDissolveCoroutine != null)
            {
                m_Player.StopCoroutine(_ActiveDissolveCoroutine);
            }

            m_Player.DissolveVFX.SetBool(_EmitParticlesID, false);
            m_Player.DeathCamera.ResetState();

            m_Player.DissolveMaterial.SetFloat(_DissolveAmountID, _DISSOLVE_START);

            Player.OnPlayerRespawn?.Invoke(0);
            m_Player.customBody.enabled = true;
            m_Player.boxCollider.enabled = true;

            m_Player.RespawnVFX.SendEvent("EmitParticles");
            m_Player.StartCoroutine(DissolveCoroutine(1.3f, false));
        }
    }
}