using System.Collections.Generic;
using UnityEngine;
using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Language
    {
        //public static string[] Languages { get => _SaveLanguage.languages; set => _SaveLanguage.languages = value; }
        //public static int currentLanguage { get => _SaveLanguage.CurrentLanguage; set => _SaveLanguage.CurrentLanguage = value; }
        //public static int currentVoiceLanguage { get => _SaveLanguage.CurrentVoiceLanguage; set => _SaveLanguage.CurrentVoiceLanguage = value; }

        public static string[] Languages = new string[2] { "English", "French" };
        public static int CurrentLanguage { get => _CurrentLanguage; set { _CurrentLanguage = value; TranslateAll(value); } }
        private static int _CurrentLanguage = 0;

        public static int CurrentVoiceLanguage { get => _CurrentVoiceLanguage; set { _CurrentVoiceLanguage = value; TranslateAllVoices(value); } }
        private static int _CurrentVoiceLanguage = 0;

        private static List<BaseTranslable> _TranslableVoiceArray = new List<BaseTranslable>();
        private static List<BaseTranslable> _TranslableArray = new List<BaseTranslable>();

        private static SaveLanguage _SaveLanguage;
        private static bool _WaitInitialization = true;

        public static event Action OnLanguageChanged;
        public static event Action OnVoiceLanguageChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (_WaitInitialization)
            {
                _SaveLanguage = SaveLanguage.GetInstance();
                _WaitInitialization = false;
            }
        }

        internal static void AddTranslatable(BaseTranslable pTranslatable)
        {
            if (pTranslatable.IsVoice() && !_TranslableVoiceArray.Contains(pTranslatable))
                _TranslableVoiceArray.Add(pTranslatable);
            else if(!_TranslableArray.Contains(pTranslatable))
                _TranslableArray.Add(pTranslatable);
        }

        public static void Translate() => TranslateAll(CurrentLanguage);

        public static void TranslateAll(int pIndex)
        {
            TranslateAll(pIndex, _TranslableArray);
            OnLanguageChanged?.Invoke();
        }

        public static void Translate(string pLanguage) => TranslateAll(Array.IndexOf(Languages, pLanguage));

        public static void TranslateVoices() => TranslateAll(CurrentVoiceLanguage);

        public static void TranslateAllVoices(int pIndex)
        {
            TranslateAll(pIndex, _TranslableVoiceArray);
            OnVoiceLanguageChanged?.Invoke();
        }

        private static void TranslateAll(int pIndex, List<BaseTranslable> pList)
        {
            int lIteration = pList.Count - 1;
            for (int i = lIteration; i >= 0; i--)
            {
                try
                {
                    pList[i].SetTranslation(pIndex);
                }
                catch
                {
                    pList.RemoveAt(i);
                }
            }
        }

        public static void TranslateVoices(string pLanguage) => TranslateAllVoices(Array.IndexOf(Languages, pLanguage));
    }
}