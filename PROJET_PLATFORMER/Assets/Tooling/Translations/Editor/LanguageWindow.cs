using System.Collections.Generic;
using Tooling;
using UnityEditor;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Editors.Translation
{
    public class LanguageWindow : EditorWindow
    {
        private const string PROPERTY_LANGUAGES = "_Languages";
        private const float SPACE = 5f;

        private SaveLanguage _SaveLanguage;
        private SerializedObject _SaveObject;
        private SerializedProperty _SavePropertyArray;

        [MenuItem("Tooling/Language")]
        public static LanguageWindow Open()
        {
            LanguageWindow lWindow = GetWindow<LanguageWindow>();
            lWindow.titleContent = new GUIContent("Language", (Texture2D)EditorGUIUtility.IconContent("InputField Icon").image);
            return lWindow;
        }


        private void CreateGUI()
        {
            CheckIfScriptableExist();
        }

        private void OnGUI()
        {
            GUILayout.Space(SPACE);

            GUILayout.Label("TRANSLATION", EditorStyles.boldLabel);
            ChooseLanguage();
            ChooseVoiceLanguage();
            _SaveObject.ApplyModifiedProperties();

            GUILayout.Space(SPACE);
            GUILayout.Label("LANGUAGES LIST", EditorStyles.boldLabel);
            ManageLanguages();
        }

        private void ManageLanguages()
        {
            EditorGUILayout.PropertyField(_SavePropertyArray, new GUIContent("Languages List"));
        }

        private void ChooseLanguage()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Languages");
            GUILayout.FlexibleSpace();
            _SaveLanguage.CurrentLanguage = EditorGUILayout.Popup(_SaveLanguage.CurrentLanguage, _SaveLanguage.languages);

            GUILayout.EndHorizontal();
        }

        private void ChooseVoiceLanguage()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Voice Languages");
            GUILayout.FlexibleSpace();
            _SaveLanguage.CurrentVoiceLanguage = EditorGUILayout.Popup(_SaveLanguage.CurrentVoiceLanguage, _SaveLanguage.languages);

            GUILayout.EndHorizontal();
        }

        private void CheckIfScriptableExist()
        {
            _SaveLanguage = AssetDatabase.LoadAssetAtPath<SaveLanguage>(SaveLanguage.ASSET_PATH);

            if (_SaveLanguage == null)
            {
                //_SaveLanguage = ScriptableObject.CreateInstance<SaveLanguage>();
            }

            _SaveObject = new SerializedObject(_SaveLanguage);
            _SavePropertyArray = _SaveObject.FindProperty(PROPERTY_LANGUAGES);

        }
    }
}