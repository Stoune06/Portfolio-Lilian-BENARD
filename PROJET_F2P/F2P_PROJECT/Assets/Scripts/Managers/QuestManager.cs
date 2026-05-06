using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Com.IsartDigital.HealerSurvivor.Data;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using Newtonsoft.Json;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class QuestManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public List<QuestProgress> ActiveQuests {get; private set; } = new List<QuestProgress>();
        
        private string _SavePath => Path.Combine(Application.persistentDataPath, Utils.QUEST_JSON_FILE_NAME);

        public static QuestManager Instance { get; private set; }

        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            #endregion
            
            Debug.Log($"Le chemin est là : {_SavePath}");
        }

        private void Start()
        {
            TimeManager.Instance.OnWeeklyReset += ResetWeeklyQuests;
            InitializeQuests();
        }

        private void InitializeQuests()
        {
            ActiveQuests.Clear();

            if (File.Exists(_SavePath))
            {
                string lJson = File.ReadAllText(_SavePath);
                ActiveQuests = JsonConvert.DeserializeObject<List<QuestProgress>>(lJson);

                foreach (QuestProgress lQuest in ActiveQuests)
                {
                    string lPath = Utils.QUEST_DATA_SO_FILE_REF + lQuest.questID;
                    lQuest.data = Resources.Load<QuestData>(lPath);
                }
            }
            else
                GenerateNewQuests();
        }
        
        private void GenerateNewQuests()
        {
            QuestData[] lAll = Resources.LoadAll<QuestData>(Utils.QUEST_DATA_SO_FILE_REF);
            IEnumerable<QuestData> lSelection = lAll.OrderBy(pX => Guid.NewGuid()).Take(3);

            foreach (QuestData lData in lSelection)
            {
                ActiveQuests.Add(new QuestProgress { 
                    data = lData, 
                    questID = lData.name,
                    currentAmount = 0, 
                    isRedeemed = false 
                });
            }
            SaveAllQuests();
        }

        private void ResetWeeklyQuests()
        {
            Debug.Log("Nouvelle semaine ! Génération de nouvelles quêtes...");
            if (File.Exists(_SavePath)) 
                File.Delete(_SavePath);
            
            GenerateNewQuests();
        }

        // CALL WHEN WE WANT PROGRESS IN A QUEST
        public void UpdateProgress(EQuestType pType, int pAmount)
        {
            foreach (QuestProgress lQuest in ActiveQuests)
            {
                if (lQuest.data.questType != pType || lQuest.IsComplete) 
                    continue;
                lQuest.currentAmount += pAmount;
                
                if (lQuest.currentAmount > lQuest.data.goalAmount)
                    lQuest.currentAmount = lQuest.data.goalAmount;

                Debug.Log($"Progression quête {lQuest.data.questName}: {lQuest.currentAmount}/{lQuest.data.goalAmount}");
            }

            SaveAllQuests();
        }

        public void ClaimReward(QuestProgress pQuest)
        {
            if (!pQuest.IsComplete || pQuest.isRedeemed) 
                return;
    
            CurrencyManager.Instance.AddCurrency(pQuest.data.rewardAmount, pQuest.data.rewardType);
            pQuest.isRedeemed = true;
            SaveAllQuests();
        }
        
        private void SaveAllQuests()
        {
            string lJson = JsonConvert.SerializeObject(ActiveQuests, Formatting.Indented);
            File.WriteAllText(_SavePath, lJson);
        }
    }
}