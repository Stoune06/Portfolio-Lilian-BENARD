using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    [RequireComponent(typeof(AttackType))]
    public class AttackManager : MonoBehaviour
    {
        private AttackType[] _AttackTypes;

        [SerializeField] private GameObject[] _Warnings;

        private Animator _Animator;

        public static event Action<Transform> OnAttackEvent;
        public static event Action<Transform> OnHealEvent;

        private void OnEnable()
        {
            _AttackTypes = GetComponents<AttackType>();
            _Animator = GetComponentInChildren<Animator>();

            foreach (AttackType lAttack in _AttackTypes)
            {
                lAttack.HitInfoEvent += SetDamage;

                if (_Animator == null) continue;

                lAttack.onAttack += LaunchAttack;
                lAttack.onPrepareAttack += PrepareAttack;
                lAttack.onCancelAttack += CancelAttack;
            }
        }

        private void LaunchAttack()
        {
            UpdateAnimationSpeed();
            _Animator.SetTrigger("Attack");
            SetTriggerVisibility(true);
        }
        private void PrepareAttack()
        {
            UpdateAnimationSpeed();
            _Animator.SetTrigger("Prepare");
            SetTriggerVisibility(true);
        }

        private void CancelAttack()
        {
            UpdateAnimationSpeed();
            _Animator.SetTrigger("Cancel");
            SetTriggerVisibility(false);
        }

        private void SetDamage(HitInfoStruct pStruct)
        {
            if (pStruct.target == null) return;

            pStruct.target.TakeDamage(pStruct.damage);

            IKnockbackable lKnockbackable;
            if (pStruct.hitTransform.TryGetComponent<IKnockbackable>(out lKnockbackable))
            {
                Vector3 lKnockBack = (pStruct.hitTransform.position - pStruct.origin).normalized * pStruct.knockBack;
                lKnockBack.y = 0f;
                lKnockbackable.TakeKnockback(lKnockBack);
            }

            if (pStruct.damage >= 0) OnAttackEvent?.Invoke(pStruct.hitTransform);
            else
            {
                OnHealEvent?.Invoke(pStruct.hitTransform);
                if (Player.instance != null && gameObject == Player.instance.gameObject)
                    Player.instance.InvokeOnHealAlly(-pStruct.damage, pStruct.hitTransform);
            }
        }

        private void FixedUpdate()
        {
            if(_Animator == null) return;

            AnimatorStateInfo stateInfo = _Animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("attack") && stateInfo.normalizedTime >= 0.5f)
            {
                SetTriggerVisibility(false);
            }


            if (stateInfo.IsName("Idle"))
            {
                SetTriggerVisibility(false);
            }


            if (stateInfo.IsName("Prepare"))
            {
                SetTriggerVisibility(true);
            }


        }

        private void SetTriggerVisibility(bool pVisibility)
        {
            if (_Warnings == null) return;
            foreach(GameObject lGo in _Warnings)
            {
                lGo.SetActive(pVisibility);
            }
        }

        private void UpdateAnimationSpeed()
        {
            if (_Animator == null) return;

            foreach (AttackType lAttack in _AttackTypes)
            {
                _Animator.SetFloat("AttackSpeed", lAttack.attackAnimationSpeed);
                _Animator.SetFloat("PrepareFactor", lAttack.prepareAttackAnimationSpeed);
            }
        }

        private void OnDisable()
        {
            if (_AttackTypes == null) return;

            foreach (AttackType lAttack in _AttackTypes)
            {   
                lAttack.HitInfoEvent -= SetDamage;

                if (_Animator == null) continue;

                lAttack.onAttack -= LaunchAttack;
                lAttack.onPrepareAttack -= PrepareAttack;
                lAttack.onCancelAttack -= CancelAttack;
            }
        }
    }
}