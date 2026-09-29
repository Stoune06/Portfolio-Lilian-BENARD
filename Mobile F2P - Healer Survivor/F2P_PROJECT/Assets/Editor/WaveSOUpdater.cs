using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEditor;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Tools
{

    public class WaveSOUpdater : Editor
    {
        private const string WAVE_CIRCLE_VARIABLE = "_WaveIllustration";

        [MenuItem(Utils.TOOL_UPDATE_ILLUSTRATIONS)]
        public static void UpdateAllWaves()
        {
            string[] lGuids = AssetDatabase.FindAssets(Utils.GUIDS_ENEMY_WAVE_SO);
            Sprite lNewSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Utils.CIRCLE_PATH);

            if (lNewSprite == null)
            {
                Debug.LogError(Utils.ERR_SPRITE_CIRCLE);
                return;
            }

            foreach (string lGuid in lGuids)
            {
                string lPath = AssetDatabase.GUIDToAssetPath(lGuid);
                EnemyWaveSO lSo = AssetDatabase.LoadAssetAtPath<EnemyWaveSO>(lPath);

                if (lSo != null)
                {
                    SerializedObject lSerializedSo = new SerializedObject(lSo);
                    lSerializedSo.FindProperty(WAVE_CIRCLE_VARIABLE).objectReferenceValue = lNewSprite;
                    lSerializedSo.ApplyModifiedProperties();

                    EditorUtility.SetDirty(lSo);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Mise à jour terminée pour {lGuids.Length} vagues !");
        }
    }
}