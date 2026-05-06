
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AgentInfoCreator
{
    public List<InfluencePoint> influencePoints;
    public List<AttackType> attackType;
    public List<XP> xp;

    public AIAgent ai;
    public GameObject gameObject;

    public AgentInfoCreator(AIAgent pAgent)
    {
        ai = pAgent;
        gameObject = pAgent.gameObject;
        FillLists();
    }

    public void AddInfluencePoint()
    {
        AddCompToSelected<InfluencePoint>();
    }

    public void RemoveInfluencePoint()
    {
        int lLength = influencePoints.Count - 1;

        for (int i = lLength; i >= 0; i--)
        {
            if (influencePoints[i] is not InfluenceCharacter)
            {
                Object.DestroyImmediate(influencePoints[i], true);
                EditorUtility.SetDirty(gameObject);
                AssetDatabase.SaveAssets();

                FillLists();
            return;
            }
        }
    }

    private void FillLists()
    {
        influencePoints = new List<InfluencePoint>(gameObject.GetComponentsInChildren<InfluencePoint>());
        attackType = new List<AttackType>(gameObject.GetComponentsInChildren<AttackType>());
        xp = new List<XP>(gameObject.GetComponentsInChildren<XP>());
    }

    public void AddCompToSelected<T>() where T : MonoBehaviour
    {
        if (gameObject != null)
        {
            Undo.AddComponent<T>(gameObject);
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssets();
        }

        FillLists();
    }
}