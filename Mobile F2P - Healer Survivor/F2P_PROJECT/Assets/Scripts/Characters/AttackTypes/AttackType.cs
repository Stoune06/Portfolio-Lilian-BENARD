using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting;
using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting.Sort;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;
using System.Collections;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    public class AttackType : MonoBehaviour
    {
        public event Action<HitInfoStruct> HitInfoEvent;

        public event Action onPrepareAttack;
        public event Action onCancelAttack;
        public event Action onAttack;

        public float attackAnimationSpeed => (_StatProvider != null && _AttackStats.baseAttackSpeed  != 0 ? (_StatProvider.GetStat(StatType.AttackSpeed) * _AttackStats.baseAttackSpeed) : 1f / _AttackStats.baseAttackSpeed);
        public float prepareAttackAnimationSpeed => _AttackStats.AttackPreparationTime != 0 ? _AttackStats.AttackPreparationTime / attackAnimationSpeed : 0f;

        public float visibleRange => _Range;

        public LayerMask focusLayer => _AttackLayer;

        [SerializeField] private LayerMask _AttackLayer;
        [SerializeField] private AttackStatsSO _AttackStats;
        [SerializeField] public SortTargetEnum attackPriority = SortTargetEnum.closest;

        public AttackStatsSO currentAttackStats => _AttackStats;

        private IStatProvider _StatProvider;

        private float _Damage => _StatProvider != null ? _StatProvider.GetStat(StatType.Damage) * _AttackStats.baseDamage : 1f;
        private float _Range => _StatProvider != null ? _StatProvider.GetStat(StatType.Range) + _AttackStats.baseRange : 1f;
        private float _AttackSpeed => _StatProvider != null ? _StatProvider.GetStat(StatType.AttackSpeed) * _AttackStats.baseAttackSpeed : 1f;

        private float _ElapsedTime;

        private const float BASE_ATTACK_SPEED = 1f;

        private Func<TargetStruct, TargetStruct, int> _SortingFunc;

        [SerializeField] private bool _IsInDebug = true;

        private Coroutine _AttackCoroutine = null;

        private void Awake()
        {
            _StatProvider = GetComponentInParent<IStatProvider>();
        }

        private void Start()
        {
            _SortingFunc = TargetSorting.GetSortingFunc(attackPriority);
        }

        public bool CheckRange(float pFactor = 1f)
        {
            return TargetScan.CheckInRange(transform.position, _Range * pFactor, _AttackLayer);
        }

        public bool CanAttack()
        {
            return TargetScan.CheckSphere(transform.position, _Range, _AttackLayer);
        }

        public bool TryAttack()
        {
            if (!CanAttack()) return false;

            if (_ElapsedTime < BASE_ATTACK_SPEED) return true;

            if (_AttackStats.AttackPreparationTime == 0) ForceAttack(); 
            else if (_AttackCoroutine == null) _AttackCoroutine = StartCoroutine(AttackCoroutine());

            return true;
        }

        private IEnumerator AttackCoroutine()
        {
            onPrepareAttack?.Invoke();
            while (_ElapsedTime < BASE_ATTACK_SPEED + _AttackStats.AttackPreparationTime)
            {
                _ElapsedTime += Time.deltaTime * _AttackSpeed;
                yield return null;
            }
            if (!CheckRange()) onCancelAttack?.Invoke();
            else ForceAttack();
            
            _AttackCoroutine = null;
        }

        public void ForceAttack()
        {
            onAttack?.Invoke();
            _ElapsedTime = 0f;

            TargetStruct lBestTarget = TargetScan.GetBest(_SortingFunc);

            Vector3 lDirection = (lBestTarget.transform.position - transform.position).normalized;

            if (_StatProvider is Ally) TargetScan.CheckSphere(lDirection, _Range, _AttackLayer);

            if (_AttackStats.paternEnum == ScanPaternEnum.Sphere) EmitMultipleAttack();
            else if (_AttackStats.paternEnum == ScanPaternEnum.Target) EmitAttack(lBestTarget);
            else if (_AttackStats.paternEnum == ScanPaternEnum.CustomObject)
            {
                CustomAttack lAttack;
                if (_AttackStats.isInstanciateOnSelf) lAttack = Instantiate(_AttackStats.customAttack, transform.position, Quaternion.LookRotation(lDirection));
                else lAttack = Instantiate(_AttackStats.customAttack, lBestTarget.transform.position, Quaternion.LookRotation(lDirection));
                lAttack.SetStats(_AttackStats.baseDamage, _AttackStats.knockback, _StatProvider, _AttackLayer, HitInfoEvent);
            }
            else
            {
                DoCorrectScan(lDirection);
                EmitMultipleAttack();
            }
#if UNITY_EDITOR
            if (_IsInDebug)
                DrawCorrect(_AttackStats, transform, _Range, lDirection);
#endif
        }

#if UNITY_EDITOR
        private void DrawCorrect(AttackStatsSO pAttackStats, Transform pTransform, float pRange, Vector3 pDirection)
        {
            switch (pAttackStats.paternEnum)
            {
                case ScanPaternEnum.Sphere:
                    TargetScan.DrawCircle(pTransform.position, pRange);
                    break;
                case ScanPaternEnum.Cone:
                    if (!pAttackStats.toTarget) TargetScan.DrawCone(pTransform.position, transform.forward, pAttackStats.angle, pRange);
                    else TargetScan.DrawCone(pTransform.position, pDirection, pAttackStats.angle, pRange);
                    break;
                case ScanPaternEnum.Box:
                    TargetScan.DrawBox(pTransform.position, pDirection, pRange, pAttackStats.width);
                    break;
                case ScanPaternEnum.Ray:
                    TargetScan.DrawRay(pTransform.position, pDirection, pRange);
                    break;
            }
        }
#endif

        private void EmitAttack(TargetStruct pStruct)
        {
            HitInfoStruct lAttack = new HitInfoStruct(pStruct.transform, pStruct.targetInterface, transform.position, _Damage, _AttackStats.knockback);
            HitInfoEvent?.Invoke(lAttack);
        }

        private void EmitMultipleAttack()
        {
            for (int i = 0; i < TargetScan.targetNumber; i++)
            {
                EmitAttack(TargetScan.targetBuffer[i]);
            }
        }

        private void DoCorrectScan(Vector3 pDirection)
        {
            switch (_AttackStats.paternEnum)
            {
                case ScanPaternEnum.Cone:
                    if (!_AttackStats.toTarget) TargetScan.CheckCone(transform.position, transform.forward, _AttackStats.angle, _Range, _AttackLayer);
                    else TargetScan.CheckCone(transform.position, pDirection, _AttackStats.angle, _Range, _AttackLayer);
                    break;
                case ScanPaternEnum.Box:
                    TargetScan.CheckBox(transform.position, pDirection, _Range, _AttackStats.width, _AttackLayer);
                    break;
                case ScanPaternEnum.Ray:
                    TargetScan.CheckRay(transform.position, pDirection, _Range, _AttackLayer);
                    break;
            }
        }

        public void SetAttackStats(AttackStatsSO pAttackStats)
        {
            _AttackStats = pAttackStats;
        }

        private void Update()
        {
            if (_ElapsedTime < BASE_ATTACK_SPEED)
                _ElapsedTime += Time.deltaTime * _AttackSpeed;
        }

        private void OnDisable()
        {
            if (_AttackCoroutine != null)
            {
                StopCoroutine(_AttackCoroutine);
                _AttackCoroutine = null;
            }
        }

        public void OverrideProviders(IStatProvider pStatProvider, Action<HitInfoStruct> pCanal)
        {
            _StatProvider = pStatProvider;
            HitInfoEvent = pCanal;
        }
    }
}
