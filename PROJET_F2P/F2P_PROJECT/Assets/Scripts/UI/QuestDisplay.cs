using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Data;
using Com.IsartDigital.HealerSurvivor.Manager;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class QuestDisplay : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private GameObject _QuestPrefab;
        [SerializeField] private Transform _Container;

        private void Start()
        {
            RefreshWindow();
        }

        public void RefreshWindow()
        {
            foreach (Transform child in _Container) 
                Destroy(child.gameObject);

            List<QuestProgress> lActiveQuests = QuestManager.Instance.ActiveQuests;

            foreach (QuestProgress lQuest in lActiveQuests)
            {
                GameObject lObj = Instantiate(_QuestPrefab, _Container);
                QuestItemUI lItemScript = lObj.GetComponent<QuestItemUI>();

                if (lItemScript != null)
                    lItemScript.Setup(lQuest);
            }
        }
    }
}