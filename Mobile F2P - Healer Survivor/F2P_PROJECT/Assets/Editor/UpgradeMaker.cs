using System;
using System.Collections.Generic;
using System.Reflection;
using Com.IsartDigital.HealerSurvivor.SO;
using Com.IsartDigital.HealerSurvivor.SO.Actions;
using Com.IsartDigital.HealerSurvivor.SO.Conditions;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Tools
{
    public class UpgradeMaker : EditorWindow
    {
        private string _AugmentName;
        private string _Description;
        private Sprite _Icon;

        private List<AugmentEffect> _AugmentEffects = new List<AugmentEffect>();
        private int _SelectedEffectIndex;
        private Dictionary<AugmentEffect, int> _SelectedConditionIndex = new Dictionary<AugmentEffect, int>();
        private Vector2 _ScrollPosition;

        private static readonly Type[] _EffectTypes = new Type[]
        {
            typeof(ComposableAugmentEffect),
            typeof(StatModifierEffect),
            typeof(OverhealEffect)
        };

        private static readonly string[] _EffectNames = new string[]
        {
            "Composable",
            "Stat Modifier",
            "Overheal"
        };

        private static readonly Type[] _TriggerTypes = new Type[]
        {
            typeof(OnKillTrigger),
            typeof(OnAllyDeathTrigger),
            typeof(OnHealAllyTrigger),
            typeof(OnHealCastTrigger),
            typeof(OnXPPickupTrigger),
            typeof(OnWaveStartTrigger),
            typeof(TimerTrigger)
        };

        private static readonly string[] _TriggerNames = new string[]
        {
            "On Kill",
            "On Ally Death",
            "On Heal Ally",
            "On Heal Cast",
            "On XP Pickup",
            "On Wave Start",
            "Timer"
        };

        private static readonly Type[] _ActionTypes = new Type[]
        {
            typeof(HealAction),
            typeof(TempStatModAction),
            typeof(HealPercentAction),
            typeof(PermanentStatModAction),
            typeof(ApplyHoTAction),
            typeof(DamageAreaAction),
            typeof(SpawnPrefabAction),
            typeof(ChangeAttackTypeAction)
        };

        private static readonly string[] _ActionNames = new string[]
        {
            "Heal",
            "Temp Stat Modifier",
            "Heal Percent",
            "Permanent Stat Modifier",
            "Apply HoT",
            "Damage Area",
            "Spawn Prefab",
            "Change Attack Type"
        };

        private Dictionary<AugmentEffect, int> _SelectedTriggerIndex = new Dictionary<AugmentEffect, int>();
        private Dictionary<AugmentEffect, int> _SelectedActionIndex = new Dictionary<AugmentEffect, int>();

        private Dictionary<AugmentSO, bool> _EditFoldoutState = new Dictionary<AugmentSO, bool>();
        private Dictionary<AugmentSO, int> _EditSelectedEffectIndex = new Dictionary<AugmentSO, int>();

        private static readonly Type[] _ConditionTypes = new Type[]
        {
            typeof(HPThresholdCondition),
            typeof(AllyCountCondition),
            typeof(WaveCondition),
            typeof(StatThresholdCondition),
            typeof(IsStationaryCondition)
        };

        private static readonly string[] _ConditionNames = new string[]
        {
            "HP Threshold",
            "Ally Count",
            "Wave",
            "Stat Threshold",
            "Is Stationary"
        };

        [MenuItem("Tools/Augment Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow<UpgradeMaker>();
            window.Show();
        }

        private void OnGUI()
        {
            _ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);

            EditorGUILayout.LabelField("New Augment", EditorStyles.boldLabel);
            _AugmentName = EditorGUILayout.TextField("Augment Name", _AugmentName);
            _Description = EditorGUILayout.TextField("Description", _Description);
            _Icon = EditorGUILayout.ObjectField("Icon", _Icon, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

            _SelectedEffectIndex = EditorGUILayout.Popup("Effect Type", _SelectedEffectIndex, _EffectNames);
            if (GUILayout.Button("Add Effect"))
            {
                AugmentEffect lNewEffect = (AugmentEffect)Activator.CreateInstance(_EffectTypes[_SelectedEffectIndex]);
                _AugmentEffects.Add(lNewEffect);
            }

            EditorGUILayout.Space();

            for (int i = _AugmentEffects.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_AugmentEffects[i].GetType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    _AugmentEffects.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                DrawEffectFields(_AugmentEffects[i]);

                EditorGUILayout.Space();
                DrawConditionsUI(_AugmentEffects[i]);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            GUI.enabled = !string.IsNullOrEmpty(_AugmentName) && _AugmentEffects.Count > 0;
            if (GUILayout.Button("Create Augment"))
                CreateAugment();
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Existing Augments", EditorStyles.boldLabel);

            AugmentSO[] lAugments = Resources.LoadAll<AugmentSO>("Upgrades");
            foreach (AugmentSO lAugment in lAugments)
            {
                if (lAugment == null) continue;
                DrawExistingAugment(lAugment);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawExistingAugment(AugmentSO pAugment)
        {
            if (!_EditFoldoutState.ContainsKey(pAugment))
                _EditFoldoutState[pAugment] = false;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            _EditFoldoutState[pAugment] = EditorGUILayout.Foldout(
                _EditFoldoutState[pAugment],
                string.IsNullOrEmpty(pAugment.upgradeName) ? pAugment.name : pAugment.upgradeName,
                true);
            GUILayout.FlexibleSpace();
            if (pAugment.icon != null)
                EditorGUILayout.ObjectField(pAugment.icon, typeof(Sprite), false, GUILayout.Width(32), GUILayout.Height(32));
            bool lDeletePressed = GUILayout.Button("Delete", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            if (lDeletePressed)
            {
                if (EditorUtility.DisplayDialog(
                        "Delete Augment",
                        $"Really delete '{pAugment.upgradeName}'?\nThis action cannot be undone.",
                        "Delete", "Cancel"))
                {
                    string lPath = AssetDatabase.GetAssetPath(pAugment);
                    EditorGUILayout.EndVertical();
                    AssetDatabase.DeleteAsset(lPath);
                    AssetDatabase.SaveAssets();
                    GUIUtility.ExitGUI();
                    return;
                }
            }

            if (_EditFoldoutState[pAugment])
            {
                EditorGUI.BeginChangeCheck();
                DrawExistingAugmentBody(pAugment);
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(pAugment);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawExistingAugmentBody(AugmentSO pAugment)
        {
            pAugment.upgradeName = EditorGUILayout.TextField("Name", pAugment.upgradeName);
            pAugment.description = EditorGUILayout.TextField("Description", pAugment.description);
            pAugment.icon = EditorGUILayout.ObjectField("Icon", pAugment.icon, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.miniBoldLabel);

            if (!_EditSelectedEffectIndex.ContainsKey(pAugment))
                _EditSelectedEffectIndex[pAugment] = 0;

            EditorGUILayout.BeginHorizontal();
            _EditSelectedEffectIndex[pAugment] = EditorGUILayout.Popup(
                _EditSelectedEffectIndex[pAugment], _EffectNames);
            if (GUILayout.Button("Add Effect", GUILayout.Width(90)))
            {
                AugmentEffect lNewEffect = (AugmentEffect)Activator.CreateInstance(
                    _EffectTypes[_EditSelectedEffectIndex[pAugment]]);
                if (pAugment.effects == null) pAugment.effects = new List<AugmentEffect>();
                pAugment.effects.Add(lNewEffect);
            }
            EditorGUILayout.EndHorizontal();

            if (pAugment.effects == null) return;

            for (int i = pAugment.effects.Count - 1; i >= 0; i--)
            {
                AugmentEffect lEffect = pAugment.effects[i];
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    lEffect != null ? lEffect.GetType().Name : "<null>",
                    EditorStyles.boldLabel);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    pAugment.effects.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                if (lEffect != null)
                {
                    DrawEffectFields(lEffect);
                    EditorGUILayout.Space();
                    DrawConditionsUI(lEffect);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
        }

        private void DrawConditionsUI(AugmentEffect pEffect)
        {
            if (!_SelectedConditionIndex.ContainsKey(pEffect))
                _SelectedConditionIndex[pEffect] = 0;

            int lCondIndex = _SelectedConditionIndex[pEffect];

            EditorGUILayout.BeginHorizontal();
            _SelectedConditionIndex[pEffect] = EditorGUILayout.Popup(lCondIndex, _ConditionNames);
            if (GUILayout.Button("Add Condition", GUILayout.Width(110)))
            {
                AugmentCondition lNewCondition = (AugmentCondition)Activator.CreateInstance(_ConditionTypes[_SelectedConditionIndex[pEffect]]);
                pEffect.conditions.Add(lNewCondition);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = pEffect.conditions.Count - 1; i >= 0; i--)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(pEffect.conditions[i].GetType().Name, EditorStyles.miniLabel);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    pEffect.conditions.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                DrawObjectFields(pEffect.conditions[i]);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawEffectFields(AugmentEffect pEffect)
        {
            if (pEffect is ComposableAugmentEffect)
            {
                DrawComposableFields(pEffect);
                return;
            }
            DrawObjectFields(pEffect);
        }

        private void DrawComposableFields(AugmentEffect pEffect)
        {
            FieldInfo lTriggerField = pEffect.GetType().GetField("_Trigger", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo lActionsField = pEffect.GetType().GetField("_Actions", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo lCooldownField = pEffect.GetType().GetField("_Cooldown", BindingFlags.NonPublic | BindingFlags.Instance);

            // Cooldown
            float lCooldown = (float)lCooldownField.GetValue(pEffect);
            lCooldownField.SetValue(pEffect, EditorGUILayout.FloatField("Cooldown", lCooldown));

            // Trigger
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Trigger", EditorStyles.miniBoldLabel);

            AugmentTrigger lTrigger = (AugmentTrigger)lTriggerField.GetValue(pEffect);

            if (!_SelectedTriggerIndex.ContainsKey(pEffect))
                _SelectedTriggerIndex[pEffect] = 0;

            EditorGUILayout.BeginHorizontal();
            _SelectedTriggerIndex[pEffect] = EditorGUILayout.Popup(_SelectedTriggerIndex[pEffect], _TriggerNames);
            if (GUILayout.Button("Set Trigger", GUILayout.Width(90)))
            {
                lTrigger = (AugmentTrigger)Activator.CreateInstance(_TriggerTypes[_SelectedTriggerIndex[pEffect]]);
                lTriggerField.SetValue(pEffect, lTrigger);
            }
            EditorGUILayout.EndHorizontal();

            if (lTrigger != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(lTrigger.GetType().Name, EditorStyles.miniLabel);
                DrawObjectFields(lTrigger);
                EditorGUI.indentLevel--;
            }

            // Actions
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Actions", EditorStyles.miniBoldLabel);

            List<AugmentAction> lActions = (List<AugmentAction>)lActionsField.GetValue(pEffect);
            if (lActions == null)
            {
                lActions = new List<AugmentAction>();
                lActionsField.SetValue(pEffect, lActions);
            }

            if (!_SelectedActionIndex.ContainsKey(pEffect))
                _SelectedActionIndex[pEffect] = 0;

            EditorGUILayout.BeginHorizontal();
            _SelectedActionIndex[pEffect] = EditorGUILayout.Popup(_SelectedActionIndex[pEffect], _ActionNames);
            if (GUILayout.Button("Add Action", GUILayout.Width(90)))
            {
                AugmentAction lNewAction = (AugmentAction)Activator.CreateInstance(_ActionTypes[_SelectedActionIndex[pEffect]]);
                lActions.Add(lNewAction);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = lActions.Count - 1; i >= 0; i--)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(lActions[i].GetType().Name, EditorStyles.miniLabel);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    lActions.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                DrawObjectFields(lActions[i]);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawObjectFields(object pObject)
        {
            List<FieldInfo> lFields = new List<FieldInfo>();
            Type lType = pObject.GetType();
            while (lType != null && lType != typeof(object))
            {
                lFields.AddRange(lType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                lType = lType.BaseType;
            }
            foreach (FieldInfo lField in lFields)
            {
                if (lField.GetCustomAttribute<SerializeField>() == null) continue;

                string lName = ObjectNames.NicifyVariableName(lField.Name);

                if (lField.FieldType == typeof(float))
                {
                    float lValue = (float)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.FloatField(lName, lValue));
                }
                else if (lField.FieldType == typeof(int))
                {
                    int lValue = (int)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.IntField(lName, lValue));
                }
                else if (lField.FieldType == typeof(string))
                {
                    string lValue = (string)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.TextField(lName, lValue));
                }
                else if (lField.FieldType == typeof(bool))
                {
                    bool lValue = (bool)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.Toggle(lName, lValue));
                }
                else if (lField.FieldType.IsEnum)
                {
                    System.Enum lValue = (System.Enum)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.EnumPopup(lName, lValue));
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(lField.FieldType))
                {
                    UnityEngine.Object lValue = (UnityEngine.Object)lField.GetValue(pObject);
                    lField.SetValue(pObject, EditorGUILayout.ObjectField(lName, lValue, lField.FieldType, false));
                }
            }
        }

        private void CreateAugment()
        {
            AugmentSO lAugment = CreateInstance<AugmentSO>();
            lAugment.upgradeName = _AugmentName;
            lAugment.description = _Description;
            lAugment.icon = _Icon;
            lAugment.effects = new List<AugmentEffect>(_AugmentEffects);

            AssetDatabase.CreateAsset(lAugment, "Assets/Resources/Upgrades/" + _AugmentName + ".asset");
            AssetDatabase.SaveAssets();

            _AugmentEffects.Clear();
            _AugmentName = "";
            _Description = "";
            _Icon = null;
        }
    }
}