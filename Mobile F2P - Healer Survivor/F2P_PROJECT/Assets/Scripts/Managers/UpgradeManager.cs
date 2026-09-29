using System;
using System.Collections.Generic;
using System.Linq;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Inputs;
using Com.IsartDigital.HealerSurvivor.Menus;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Managers
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField]
        private UpgradeCard _UpgradeCardPrefab;
        [SerializeField]
        private int UpgradesPerLevelUp = 2;
        [SerializeField] 
        private InputManager _InputManager;
        
        private List<AugmentSO> _Augments = new List<AugmentSO>();
        private List<AugmentSO> _UsedAugments = new List<AugmentSO>();
        private Player _Player;
        private int _PendingLevelUps;

        private void Start()
        {
            UpgradeCard.onUpgradeSelected += OnAugmentSelected;
            UpgradeCard.onCardsAnimatedOut += OnCardsAnimatedOut;
            _Augments = Resources.LoadAll<AugmentSO>("Upgrades").ToList();
            _Player = Player.instance;
            _Player.OnLevelUp += OnLevelUp;
        }

        private void OnLevelUp()
        {
            Debug.Log("OnLevelUp");
            _PendingLevelUps++;
            if (_PendingLevelUps == 1)
                ShowRandomUpgrades(UpgradesPerLevelUp);
        }

        private void ShowRandomUpgrades(int pNAugments)
        {
            if (_Augments.Count < pNAugments) _Augments = Resources.LoadAll<AugmentSO>("Upgrades").ToList();
            if (_Augments.Count == 0) return;
            int lIndex;
            for (int i = _Augments.Count - 1; i > 0; i--)
            {
                lIndex = Random.Range(0, _Augments.Count-1);
                (_Augments[i], _Augments[lIndex]) = (_Augments[lIndex], _Augments[i]);
            }
            int lCount = Mathf.Min(pNAugments, _Augments.Count);
            for (int j = 0; j < lCount; j++)
            {
                UpgradeCard lUpgradeCard = Instantiate(_UpgradeCardPrefab, transform);
                lUpgradeCard.UpdateCard(_Augments[j]);
                lUpgradeCard.SetCardIndex(j);
            }
            _InputManager.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
        
        private void OnAugmentSelected(AugmentSO pAugment)
        {
            _Augments.Remove(pAugment);
            _UsedAugments.Add(pAugment);
            _Player.NewAugment(pAugment);
            _PendingLevelUps--;
        }

        private void OnCardsAnimatedOut()
        {
            if (_PendingLevelUps > 0)
            {
                ShowRandomUpgrades(UpgradesPerLevelUp);
            }
            else
            {
                _InputManager.gameObject.SetActive(true);
                Time.timeScale = 1;
            }
        }

        private void OnDestroy()
        {
            UpgradeCard.onUpgradeSelected -= OnAugmentSelected;
            UpgradeCard.onCardsAnimatedOut -= OnCardsAnimatedOut;
            _Player.OnLevelUp -= OnLevelUp;
        }
    }
}
