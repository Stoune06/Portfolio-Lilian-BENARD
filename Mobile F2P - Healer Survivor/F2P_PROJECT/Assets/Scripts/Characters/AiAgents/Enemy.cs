using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.Other;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    public class Enemy : AIAgent
    {
        [SerializeField]
        private float _XPAmount = 1; 
        [SerializeField] private int _MinCoins = 1;
        [SerializeField] private int _MaxCoins = 3;

        public Action onDeath;

        private Coin _CoinPrefab;
        private XP _XPPrefab;

        private void Awake()
        {
            _XPPrefab = Resources.Load<XP>("XPOrb");
            _CoinPrefab = Resources.Load<Coin>("Coin");
        }

        protected override void Kill()
        {
            // Notify wave progression FIRST so any exception in quests/drops below can't leave this enemy
            // stuck in allEnemies and block CheckWinCondition forever (seen as "no new wave, no win screen"
            // on mobile where a subtle null ref in a drop kills the rest of the method).
            if (WaveManager.Instance != null) WaveManager.Instance.allEnemies.Remove(this);
            onDeath?.Invoke();
            InvokeKill(transform);

            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateProgress(EQuestType.KILL_ENEMIES, 1);

            DropXP();

            int lAmount = UnityEngine.Random.Range(_MinCoins, _MaxCoins + 1);
            for (int i = 0; i < lAmount; i++)
                DropCoin();

            base.Kill();
        }
        
        private void DropCoin()
        {
            Coin lCoin = Instantiate(_CoinPrefab, transform.position, Quaternion.identity);
    
            if (lCoin.TryGetComponent(out Coin coinScript))
                coinScript.Launch();
        }

        private void DropXP()
        {
            XP lXP = Instantiate(_XPPrefab, transform.position, Quaternion.identity);
            lXP._XpValue =  _XPAmount;
        }
    }
}
