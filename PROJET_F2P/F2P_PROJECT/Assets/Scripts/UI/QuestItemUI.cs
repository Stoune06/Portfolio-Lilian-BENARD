using System;
using Com.IsartDigital.HealerSurvivor.Data;
using Com.IsartDigital.HealerSurvivor.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class QuestItemUI : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _TitleText;
        [SerializeField] private TextMeshProUGUI _ProgressText;
        [SerializeField] private Image _ProgressBar;
        [SerializeField] private Button _ClaimButton;

        private QuestProgress _Quest;
        
        private void Start() => _ClaimButton.onClick.AddListener(OnClaimClick);

        public void Setup(QuestProgress pQuest)
        {
            _Quest = pQuest;
            UpdateUI();
        }

        public void UpdateUI()
        {
            _TitleText.text = _Quest.data.questName;
            _ProgressText.text = $"{_Quest.currentAmount} / {_Quest.data.goalAmount}";
            _ProgressBar.fillAmount = (float)_Quest.currentAmount / _Quest.data.goalAmount;
            _ClaimButton.interactable = _Quest.IsComplete && !_Quest.isRedeemed;
            if (_Quest.isRedeemed) 
                _ProgressText.text = "REDEEMED !";
        }

        public void OnClaimClick()
        {
            QuestManager.Instance.ClaimReward(_Quest);
            UpdateUI();
        }

        private void OnDisable() => _ClaimButton.onClick.RemoveAllListeners();
        
    }
}