using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.Menus;
using Com.IsartDigital.HealerSurvivor.SO;
using MagicPigGames;
using Managers;
using UnityEngine;

// Author : Florian MAJCHER & Lilian BENARD - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    
    public class Player : Character, IAugmentable
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField]
        private float _BaseXP = 10f;
        [SerializeField]
        private float _XpIncrement = 1.5f;

        [Header("UI")]
        [SerializeField]
        private ProgressBar _HealProgressBar;
        [SerializeField]
        private ProgressBar _XpProgressBar;
        
        [SerializeField] private float _StepInterval = 0.4f;
        [SerializeField] private AudioClip _FootStepSound;
        private bool _IsPlayingFootsteps;
        private Coroutine _FootstepCoroutine;

        private float _LastHealProgress = -1f;
        private float _LastXpProgress = -1f;
        
        public static Player instance;
        public List<AugmentSO> augments { get; private set; }  = new List<AugmentSO>();
        public float currentXP;
        public int currentLevel = 1;

        public float xpToNextLevel => _BaseXP + Mathf.Pow(_XpIncrement, currentLevel - 1);

        public new float health => base.health;
        public bool isStationary => velocity.sqrMagnitude < 0.01f;
        
        private GameManager _GameManager => GameManager.Instance;
        
        #region Events

        public event Action OnLevelUp;
        public event Action<AugmentSO> OnAugmentApplied;
        public event Action<float, Transform> OnHealAlly;
        public event Action<int> OnWaveStart;
        public event Action OnHealCast;
        public event Action OnXPPickup;

        #endregion

        public void DebugLevelUp() => OnLevelUp?.Invoke();
        public void InvokeWaveStart(int pWaveNumber) => OnWaveStart?.Invoke(pWaveNumber);
        public void InvokeHealCast() => OnHealCast?.Invoke();
        public void InvokeOnHealAlly(float pAmount, Transform pAlly) => OnHealAlly?.Invoke(pAmount, pAlly);

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            if (instance == null) 
                instance = this;
        }

        protected override void Start()
        {
            base.Start();
            stateMachine.InitState(EState.IDLE);
            InvokeAllySpawn(transform);
        }

        protected override void Update()
        {
            base.Update();
            AttackType[] lAttackTypes = GetComponentsInChildren<AttackType>();
            foreach (AttackType lAttack in lAttackTypes)
            {
                lAttack.TryAttack();
            }
            UpdateProgressBars();
            
            HandleFootsteps();
        }
        
        private void HandleFootsteps()
        {
            switch (isStationary)
            {
                case false when _FootstepCoroutine == null:
                    _FootstepCoroutine = StartCoroutine(FootstepRoutine());
                    break;
                case true when _FootstepCoroutine != null:
                    StopCoroutine(_FootstepCoroutine);
                    _FootstepCoroutine = null;
                    break;
            }
        }

        private IEnumerator FootstepRoutine()
        {
            _IsPlayingFootsteps = true;

            while (!isStationary)
            {
                SoundManager.Instance.PlaySound(_FootStepSound, transform.position);
        
                yield return new WaitForSeconds(_StepInterval);
            }

            _IsPlayingFootsteps = false;
        }

        private void UpdateProgressBars()
        {
            if (_HealProgressBar != null)
            {
                float lMaxHealth = targetMaxHealth;
                float lProgress = lMaxHealth > 0f ? Mathf.Clamp01(health / lMaxHealth) : 0f;
                if (!Mathf.Approximately(lProgress, _LastHealProgress))
                {
                    _HealProgressBar.SetProgress(lProgress);
                    _LastHealProgress = lProgress;
                }
            }

            if (_XpProgressBar != null)
            {
                float lMaxXp = xpToNextLevel;
                float lProgress = lMaxXp > 0f ? Mathf.Clamp01(currentXP / lMaxXp) : 0f;
                if (!Mathf.Approximately(lProgress, _LastXpProgress))
                {
                    _XpProgressBar.SetProgress(lProgress);
                    _LastXpProgress = lProgress;
                }
            }
        }

        public void NewAugment(AugmentSO pAugment)
        {
            augments.Add(pAugment);
            foreach (AugmentEffect effect in pAugment.effects)
            {
                effect.Apply(this);
            }
            OnAugmentApplied?.Invoke(pAugment);
        }

        public void RemoveAugment(AugmentSO pAugment)
        {
            if (!augments.Remove(pAugment)) return;
            foreach (AugmentEffect effect in pAugment.effects)
            {
                effect.Remove(this);
            }
        }
        
        public void AddXp(float pValue)
        {
            currentXP += pValue;
            OnXPPickup?.Invoke();
            while (currentXP >= xpToNextLevel)
            {
                currentXP -= xpToNextLevel;
                currentLevel++;
                OnLevelUp?.Invoke();
            }
        }

        private void OnTriggerEnter(Collider pOther)
        {
            if(!pOther.CompareTag((Utils.TAG_COIN)))
                return;
            
            if (pOther.TryGetComponent(out Coin lCoin))
                lCoin.StartAttraction(transform);
            
            QuestManager.Instance.UpdateProgress(EQuestType.COLLECT_GOLD, 1);
            _GameManager.onGainingCoins?.Invoke();
        }

        protected override void Kill()
        {
            base.Kill();
            _GameManager.onGameOver?.Invoke(EMenuType.GAME_OVER);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            InvokeAllyDied(transform);
            instance = null;
        }
    }
}