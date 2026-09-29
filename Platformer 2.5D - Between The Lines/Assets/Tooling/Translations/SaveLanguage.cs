using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling
{
    [DefaultExecutionOrder(-1)]
    public class SaveLanguage : ScriptableObject
    {
        public const string RESOURCES_PATH = "Languages/LanguageSave";
        public const string ASSET_PATH = "Assets/Resources/" + RESOURCES_PATH + ".asset";
        [SerializeField, HideInInspector] public string[] languages = new string[2] { "English", "Francais" };

        public int CurrentVoiceLanguage 
        { 
            get => _CurrentVoiceLanguage; 
            set { _CurrentVoiceLanguage = Mathf.Clamp(value, 0, languages.Length - 1);}
        }
        [SerializeField, HideInInspector] private int _CurrentVoiceLanguage = 0;

        public int CurrentLanguage
        { 
            get => _CurrentLanguage; 
            set { _CurrentLanguage = Mathf.Clamp(value, 0, languages.Length - 1);}
        }
        [SerializeField, HideInInspector] private int _CurrentLanguage = 0;

        public static SaveLanguage GetInstance() => Resources.Load<SaveLanguage>(RESOURCES_PATH);

        public string GetCurrentLanguageAsString() => languages[_CurrentLanguage];
        public string GetCurrentVoiceLanguageAsString() => languages[_CurrentVoiceLanguage];
    }
}