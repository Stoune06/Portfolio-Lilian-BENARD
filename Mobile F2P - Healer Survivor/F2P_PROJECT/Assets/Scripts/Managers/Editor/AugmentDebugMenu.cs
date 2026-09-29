using System.Collections.Generic;
using System.Linq;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.EditorTools
{
    public class AugmentDebugWindow : EditorWindow
    {
        private List<AugmentSO> _Augments;
        private List<AttackStatsSO> _AttackStats;
        private Vector2 _Scroll;

        [MenuItem("Tools/Augments/Open Picker")]
        private static void Open()
        {
            AugmentDebugWindow lWindow = GetWindow<AugmentDebugWindow>("Augment Picker");
            lWindow.minSize = new Vector2(280, 260);
            lWindow.Refresh();
        }

        [MenuItem("Tools/Augments/Give Random Augment &g")]
        private static void GiveRandom()
        {
            if (!EnsurePlaying()) return;

            List<AugmentSO> lAugments = Resources.LoadAll<AugmentSO>("Upgrades").ToList();
            if (lAugments.Count == 0)
            {
                Debug.LogWarning("[Augments] No AugmentSO found in Resources/Upgrades.");
                return;
            }

            AugmentSO lPick = lAugments[Random.Range(0, lAugments.Count)];
            Player.instance.NewAugment(lPick);
            Debug.Log($"[Augments] Applied random augment: {lPick.name}");
        }

        [MenuItem("Tools/Augments/Trigger Level Up &l")]
        private static void TriggerLevelUp()
        {
            if (!EnsurePlaying()) return;
            Player.instance.DebugLevelUp();
            Debug.Log("[Augments] Triggered level-up (opens the upgrade card UI).");
        }

        private void Refresh()
        {
            _Augments = Resources.LoadAll<AugmentSO>("Upgrades").ToList();
            _AttackStats = AssetDatabase.FindAssets("t:AttackStatsSO")
                .Select(pGuid => AssetDatabase.LoadAssetAtPath<AttackStatsSO>(AssetDatabase.GUIDToAssetPath(pGuid)))
                .Where(pAsset => pAsset != null)
                .ToList();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to apply augments.", MessageType.Info);
                return;
            }

            if (Player.instance == null)
            {
                EditorGUILayout.HelpBox("Player.instance is null (not spawned yet).", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Refresh Lists")) Refresh();
            if (_Augments == null || _AttackStats == null) Refresh();

            _Scroll = EditorGUILayout.BeginScrollView(_Scroll);

            GUILayout.Space(6);
            EditorGUILayout.LabelField($"Augments available ({_Augments.Count})", EditorStyles.boldLabel);
            foreach (AugmentSO lAugment in _Augments)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(lAugment.name, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Give", GUILayout.Width(60)))
                {
                    Player.instance.NewAugment(lAugment);
                    Debug.Log($"[Augments] Applied: {lAugment.name}");
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField($"Change Player Attack Type ({_AttackStats.Count})", EditorStyles.boldLabel);
            foreach (AttackStatsSO lStats in _AttackStats)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(lStats.name, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                {
                    AttackType lAttackType = Player.instance.attackType;
                    if (lAttackType == null)
                    {
                        Debug.LogWarning("[Augments] Player has no AttackType component.");
                    }
                    else
                    {
                        lAttackType.SetAttackStats(lStats);
                        Debug.Log($"[Augments] Player attack type switched to: {lStats.name}");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private static bool EnsurePlaying()
        {
            if (Application.isPlaying && Player.instance != null) return true;
            Debug.LogWarning("[Augments] Enter Play Mode with a Player in the scene first.");
            return false;
        }
    }
}
