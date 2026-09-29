using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Com.IsartDigital.HealerSurvivor.SO;
using Managers;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CustomEditor(typeof(Character), true)]
    public class CharacterDrawer : Editor
    {
        public bool _MoreSettings;
        public bool _DebugInfo;
        public bool _Stats;
        public bool _RuntimeDebug;
        private Editor _StatsEditor;

        private Dictionary<Component, Editor> _CachedEditors = new Dictionary<Component, Editor>();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty lStatProperty = serializedObject.FindProperty("baseStats");
            EditorGUILayout.PropertyField(lStatProperty);
            if(target is Enemy) EditorGUILayout.PropertyField(serializedObject.FindProperty("_XPAmount"));
            else if (target is SwordAlly)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BuffGameObject"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BuffFactor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BuffDuration"));
            }
            else if (target is BerserkerAlly)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BuffGameObject"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BuffFactor"));
            }

            else if (target is Player)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_BaseXP"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_XpIncrement"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_HealProgressBar"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_XpProgressBar"));
                
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_FootStepSound"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_StepInterval"));
            }

            if (lStatProperty.objectReferenceValue != null)
            {
                _Stats = EditorGUILayout.Foldout(_Stats, "Edition", true);
                if (_Stats) 
                { 
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                if (_StatsEditor == null || _StatsEditor.target != lStatProperty.objectReferenceValue)
                {
                    if (_StatsEditor != null) DestroyImmediate(_StatsEditor);
                    _StatsEditor = CreateEditor(lStatProperty.objectReferenceValue);
                }

                _StatsEditor.OnInspectorGUI();

                EditorGUILayout.EndVertical();
            } 
        }

            GUILayout.Space(10);

            _MoreSettings = EditorGUILayout.Foldout(_MoreSettings, "More Informations", true);

            if (_MoreSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dustParticles"));

                _DebugInfo = EditorGUILayout.Foldout(_DebugInfo, "Debug Informations", true);

                if (_DebugInfo)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("velocity"));
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
                DrawRuntimeDebug();
        }

        private void DrawRuntimeDebug()
        {
            Character lCharacter = (Character)target;
            Player lPlayer = lCharacter as Player;

            GUILayout.Space(10);
            _RuntimeDebug = EditorGUILayout.Foldout(_RuntimeDebug, "Runtime Debug", true);
            if (!_RuntimeDebug) return;

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            foreach (StatType lStat in System.Enum.GetValues(typeof(StatType)))
            {
                float lBase = GetBaseStat(lCharacter, lStat);
                float lFinal = lCharacter.GetStat(lStat);

                if (lBase != lFinal)
                    EditorGUILayout.LabelField(lStat.ToString(), $"{lBase} \u2192 {lFinal}");
                else
                    EditorGUILayout.LabelField(lStat.ToString(), lFinal.ToString());

                if (lPlayer != null && lPlayer.statsManager.PlayerModifiers.TryGetValue(lStat, out var lModifiers))
                {
                    EditorGUI.indentLevel++;
                    foreach (StatModifier lMod in lModifiers)
                    {
                        if (lMod.modifierType == ModifierType.Additive)
                            EditorGUILayout.LabelField($"+ Additive: +{lMod.value}");
                        else
                            EditorGUILayout.LabelField($"\u00d7 Multiplicative: \u00d7{lMod.value}");
                    }
                    EditorGUI.indentLevel--;
                }
            }

            AttackType lAttackType = lCharacter.attackType;
            if (lAttackType != null)
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);

                SerializedObject lAtkSO = new SerializedObject(lAttackType);
                SerializedProperty lStatsProp = lAtkSO.FindProperty("_AttackStats");
                if (lStatsProp != null && lStatsProp.objectReferenceValue != null)
                {
                    AttackStatsSO lStats = (AttackStatsSO)lStatsProp.objectReferenceValue;
                    EditorGUILayout.ObjectField("AttackStats SO", lStats, typeof(AttackStatsSO), false);
                    EditorGUILayout.LabelField("Pattern", lStats.paternEnum.ToString());

                    float lDmg = lCharacter.GetStat(StatType.Damage) * lStats.baseDamage;
                    float lRange = lCharacter.GetStat(StatType.Range) + lStats.baseRange;
                    float lSpd = lCharacter.GetStat(StatType.AttackSpeed) * lStats.baseAttackSpeed;
                    EditorGUILayout.LabelField("Final Damage", lDmg.ToString("F2"));
                    EditorGUILayout.LabelField("Final Range", lRange.ToString("F2"));
                    EditorGUILayout.LabelField("Final AtkSpeed", lSpd.ToString("F2"));
                }
            }

            if (lPlayer != null && lPlayer.augments.Count > 0)
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Augments", EditorStyles.boldLabel);
                foreach (AugmentSO lAugment in lPlayer.augments)
                {
                    EditorGUILayout.LabelField("\u2022 " + lAugment.upgradeName);
                }
            }

            EditorGUI.indentLevel--;
            Repaint();
        }

        private float GetBaseStat(Character pCharacter, StatType pType)
        {
            return pType switch
            {
                StatType.MoveSpeed => pCharacter.baseStats.moveSpeed,
                StatType.MaxHP => pCharacter.baseStats.maxHealth,
                StatType.Damage => pCharacter.baseStats.AttackDamageFactor,
                StatType.AttackSpeed => pCharacter.baseStats.attackSpeedFactor,
                StatType.Range => pCharacter.baseStats.additionalRange,
                StatType.PickupRange => pCharacter.baseStats.pickupRange,
                _ => 1f
            };
        }

        private void OnDisable()
        {
            if (_StatsEditor != null)
            {
                DestroyImmediate(_StatsEditor);
            }
        }
    }
}
