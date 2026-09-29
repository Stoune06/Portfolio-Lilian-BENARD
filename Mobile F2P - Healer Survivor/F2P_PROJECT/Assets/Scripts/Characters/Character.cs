using System;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using Managers;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    [RequireComponent(typeof(AttackType), typeof(Rigidbody))]
    public class Character : MonoBehaviour, ITarget, IStatProvider, IKnockbackable
    {
        public static int allyAliveCount;

        public CharacterStatsSO baseStats;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public Vector3 velocity;
        public StateMachine stateMachine;
        public Animator animator;

        public AttackType attackType;

        public ParticleSystem dustParticles;
        public float health;

        public float targetHealth => health;

        public float targetMaxHealth => baseStats.maxHealth;
        
        public static event Action<Transform> OnAllyDied;
        public static event Action<Transform> OnAllySpawn;
        public static event Action<Transform> OnKill;

        public Rigidbody rigidBody;

        public PlayerStatsManager statsManager { get; private set; } = new PlayerStatsManager();

        protected void InvokeAllySpawn(Transform pAlly) => OnAllySpawn?.Invoke(pAlly);
        protected void InvokeAllyDied(Transform pAlly) => OnAllyDied?.Invoke(pAlly);
        protected void InvokeKill(Transform pEnemy) => OnKill?.Invoke(pEnemy);

        private float _ImortatiltyTimer = 0f;
        private const float IMORTALITY_DURATION = 0.1f;
        
        public virtual void TakeDamage(float pAmount)
        {
            if (pAmount >= 0f && _ImortatiltyTimer <= 0f)
            {
                _ImortatiltyTimer += IMORTALITY_DURATION;
                health -= pAmount;
            }
            else if (pAmount < 0) health -= pAmount;

            if (health <= 0)
                Kill();
        }

        public void TakeKnockback(Vector3 pDirection)
        {
            velocity += pDirection;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        protected virtual void Start()
        {
            Init();
            if (gameObject.layer == LayerMask.NameToLayer("Ally")) allyAliveCount++;
            health = baseStats.maxHealth;

            rigidBody = GetComponent<Rigidbody>();
        }

        protected virtual void OnDestroy()
        {
            if (gameObject.layer == LayerMask.NameToLayer("Ally")) allyAliveCount--;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        protected virtual void Update()
        {
            stateMachine.UpdateLogic();
            if (_ImortatiltyTimer > 0f) _ImortatiltyTimer -= Time.deltaTime;
        }

        protected virtual void FixedUpdate()
        {
            stateMachine.FixedUpdateLogic();

            rigidBody.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);

            if (velocity.sqrMagnitude > 0.01f)
            {
                Quaternion lTargetRotation = Quaternion.LookRotation(velocity.normalized);
                rigidBody.MoveRotation(lTargetRotation);
            }
        }

        protected virtual void Kill()
        {
            Destroy(gameObject);
        }


        private void Init()
        {
            stateMachine = new StateMachine(this, animator);
            attackType = GetComponent<AttackType>();
        }

        public virtual float GetStat(StatType pType)
        {
            float lBase = pType switch
            {
                StatType.MoveSpeed => baseStats.moveSpeed,
                StatType.MaxHP => baseStats.maxHealth,
                StatType.Damage => baseStats.AttackDamageFactor,
                StatType.AttackSpeed => baseStats.attackSpeedFactor,
                StatType.Range => baseStats.additionalRange,
                StatType.PickupRange => baseStats.pickupRange,
                _ => 1f
            };
            return statsManager.GetFinalValue(pType, lBase);
        }

        public void Heal(float pAmount)
        {
            health = Mathf.Min(health + pAmount, baseStats.maxHealth);
        }
    }
}