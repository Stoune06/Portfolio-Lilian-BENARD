using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting;
using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting.Sort;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.Enemies
{
    public class AiMoveState : BaseAIState
    {
        private const float DESTINATION_UPDATE_INTERVAL = 0.5f;
        private const float MIN_DESTINATION_CHANGE_SQR = 1.5f * 1.5f;
        private const float DESTINATION_DETECTION_RANGE = 20f;
        private const float RANDOM_POS_RADIUS = 5f;

        private Vector3 _TargetPosition;
        private Vector3 _Direction;
        private float _TimeSinceLastUpdate;

        public AiMoveState(Character pCharacter, Animator pAnimator) : base(pCharacter, pAnimator) { }

        public override void OnEnter()
        {
            base.OnEnter();
            _TimeSinceLastUpdate = DESTINATION_UPDATE_INTERVAL;
            _TargetPosition = m_InfluenceCharacter.personality == null ? GetRandomPosition() : InfluenceManager.GetBestDestination(m_InfluenceCharacter);
            _TimeSinceLastUpdate = 0f;
        }

        public override void Update()
        {
            base.Update();
            if (m_Character.attackType.CheckRange(m_Character.baseStats.attackRangeHysteresis))
                m_Character.stateMachine.ChangeState(EState.ATTACK);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _TimeSinceLastUpdate += Time.fixedDeltaTime;
            if (_TimeSinceLastUpdate >= DESTINATION_UPDATE_INTERVAL)
            {
                _TimeSinceLastUpdate = 0f;
                Vector3 lNewTarget;

                if (m_InfluenceCharacter.personality == null)
                {
                    Transform lGridTransform = InfluenceGrid.Get(m_Character.transform.position).transform;
                    Transform lTarget = GetClosestTransform(lGridTransform.position, GetPriorityLayers());

                    if (lTarget == null)
                        lTarget = GetClosestTransform(lGridTransform.position, GetFocusLayers());

                    lNewTarget = (lTarget != null) ? lTarget.position : GetRandomPosition();
                }
                else
                {
                    Vector3 lBaseDestination = InfluenceManager.GetBestDestination(m_InfluenceCharacter);
                    Transform lTarget = GetClosestTransform(lBaseDestination, GetPriorityLayers());

                    if (lTarget == null)
                        lTarget = GetClosestTransform(lBaseDestination, GetFocusLayers());

                    lNewTarget = (lTarget != null) ? lTarget.position : lBaseDestination;
                }

                if ((_TargetPosition - lNewTarget).sqrMagnitude > MIN_DESTINATION_CHANGE_SQR)
                {
                    _TargetPosition = lNewTarget;
                }
            }

            _Direction = _TargetPosition - m_Character.transform.position;

            m_InfluenceCharacter.targetVelocity = _Direction.magnitude >= 0.5f
                ? new Vector3(_Direction.x, 0, _Direction.z).normalized * m_Character.baseStats.moveSpeed
                : Vector3.zero;

            m_Character.velocity = Vector3.Lerp(m_Character.velocity, m_InfluenceCharacter.targetVelocity, Time.fixedDeltaTime * m_Character.baseStats.acceleration);
        }

        private Transform GetClosestTransform(Vector3 pCenter, LayerMask pLayer)
        {
            if (TargetScan.CheckSphere(pCenter, DESTINATION_DETECTION_RANGE, pLayer))
            {
                return TargetScan.GetBest(TargetSorting.GetSortingFunc(SortTargetEnum.closest)).transform;
            }
            return null;
        }

        private Vector3 GetRandomPosition()
        {
            Vector2 lRandomCircle = Random.insideUnitCircle * RANDOM_POS_RADIUS;
            return m_Character.transform.position + new Vector3(lRandomCircle.x, 0, lRandomCircle.y);
        }

        private LayerMask GetPriorityLayers()
        {
            return m_InfluenceCharacter.priorityFocusLayer;
        }

        private LayerMask GetFocusLayers()
        {
            return m_Character.attackType.focusLayer;
        }
    }
}