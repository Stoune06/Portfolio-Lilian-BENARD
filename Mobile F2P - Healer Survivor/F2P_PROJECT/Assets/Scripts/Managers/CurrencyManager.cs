using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class CurrencyManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public int Gold { get; private set; }
        public int Gems { get; private set; }
        
        public event Action<ECurrencyType, int> OnCurrencyChanged;
        
        public static CurrencyManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
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
        }

        public bool TrySpend(int pAmount, ECurrencyType pType) 
        {
            bool lSuccess = pType switch
            {
                ECurrencyType.GOLD => Gold >= pAmount,
                ECurrencyType.GEMS => Gems >= pAmount,
                _ => false,
            };

            if (!lSuccess) 
                return false;
            
            switch (pType)
            {
                case ECurrencyType.GOLD: 
                    Gold -= pAmount; break;
                case ECurrencyType.GEMS: 
                    Gems -= pAmount; break;
            }

            int lNewValue = (pType == ECurrencyType.GOLD) ? Gold : Gems;
            OnCurrencyChanged?.Invoke(pType, lNewValue);
    
            return true; 
        }
    

        public void AddCurrency(int pAmount, ECurrencyType pType)
        {
            switch (pType)
            {
                case ECurrencyType.GOLD: 
                    Gold += pAmount; 
                    break;
                case ECurrencyType.GEMS:
                    Gems += pAmount; 
                    break;
            }

            OnCurrencyChanged?.Invoke(pType, pType == ECurrencyType.GOLD ? Gold : Gems);
        }
    }
}