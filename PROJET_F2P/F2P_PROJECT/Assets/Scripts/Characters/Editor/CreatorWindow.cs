using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatorWindow : EditorWindow
{
    private const string WINDOW_NAME = "Creator";
    private readonly string[] _TabLabels = { "Influence", "Weapon", "Stats", "Card" };
    private readonly string[] _TypeLabels = { "Ally", "Enemy", "All" };

    private int _SelectedTab;
    private int _SelectedTypeTab;

    private Ally[] _AlliesList;
    private Enemy[] _EnemiesList;
    private AIAgent[] _All;

    private AIAgent _Target;
    private AgentInfoCreator _AgentInfoCreator;

    private string _Filter = "";
    private Vector2 _ScrollPos;

    private const float SEPARATOR_SIZE = 8f;

    private Dictionary<Component, Editor> _CachedEditors = new Dictionary<Component, Editor>();

    [MenuItem("Tools/" + WINDOW_NAME)]
    public static void ShoWindow()
    {
        CreatorWindow lWindow = GetWindow<CreatorWindow>();
        lWindow.titleContent = new GUIContent(WINDOW_NAME);
        lWindow.ResetTarget();
    }

    private void OnGUI()
    {
        Rect lRect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(lRect, new Color(0.12f, 0.12f, 0.12f, 0.4f));
        if (_Target == null) GUILayout.Label("Sélectionnez une IA sur la scène ou dans la liste ci-dessous pour modifier ses propriétés.");
        if (_Target == null) DrawAIList();
        else DrawTab();
        EditorGUILayout.EndVertical();
    }

    private void DrawAIList()
    {
        _SelectedTypeTab = GUILayout.Toolbar(_SelectedTypeTab, _TypeLabels);
        EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth));
        EditorGUILayout.Space(5f);
        EditorGUILayout.LabelField(_TypeLabels[_SelectedTypeTab]);

        _Filter = EditorGUILayout.TextField("Filtrer par nom :", _Filter);

        _ScrollPos = GUILayout.BeginScrollView(_ScrollPos, "box");

        AIAgent[] lAgents;
        if (_SelectedTypeTab == 0) lAgents = _AlliesList;
        else if (_SelectedTypeTab == 1) lAgents = _EnemiesList;
        else lAgents = _All;
        
        for (int i = 0; i < lAgents.Length; i++)
        {
            if (!lAgents[i].gameObject.name.Contains(_Filter)) continue;

            if (GUILayout.Button(lAgents[i].gameObject.name))
            {
                _Target = lAgents[i];
                ClearCachedEditors();
                _AgentInfoCreator = new AgentInfoCreator(_Target);
            }
                
        }

        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }


    private void DrawTab()
    {
        GUILayout.Space(10);

        GUIStyle lCenteredStyle = new GUIStyle(EditorStyles.label);
        lCenteredStyle.alignment = TextAnchor.MiddleCenter;
        lCenteredStyle.fontSize = 18;
        lCenteredStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField(_Target.gameObject.name, lCenteredStyle);

        GUILayout.Space(10);

        _SelectedTab = GUILayout.Toolbar(_SelectedTab, _TabLabels);

        GUILayout.Space(10);

        _ScrollPos = GUILayout.BeginScrollView(_ScrollPos, "box");

        DrawSeparator();

        switch (_SelectedTab)
        {
            case 0:
                DrawInfluence(); break;
            case 1:
                DrawWeapon(); break;
            case 2:
                DrawStatsInfo(); break;
        }
        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        Rect lLineRect = EditorGUILayout.GetControlRect(false, 1);

        GUILayout.Space(10f);


        EditorGUI.DrawRect(lLineRect, new Color(0.5f, 0.5f, 0.5f, 0.75f));

   

        if (GUILayout.Button("QUIT", GUILayout.Height(30)))
        {
            ResetTarget();
        }

        GUILayout.Space(10);
    }

    private void DrawStatsInfo()
    {
        EditorGUILayout.LabelField("Stats & XP", EditorStyles.boldLabel);

        DrawComponentInspector(_AgentInfoCreator.ai);

        if (_AgentInfoCreator.xp != null)
        {
            for (int lI = 0; lI < _AgentInfoCreator.xp.Count; lI++)
            {
                DrawComponentInspector(_AgentInfoCreator.xp[lI]);
            }
        }
    }

    private void DrawWeapon()
    {
        EditorGUILayout.LabelField("Weapon & Attack Types", EditorStyles.boldLabel);

        if (_AgentInfoCreator.attackType != null)
        {
            for (int lI = 0; lI < _AgentInfoCreator.attackType.Count; lI++)
            {
                DrawComponentInspector(_AgentInfoCreator.attackType[lI]);
            }
        }
    }

    private void DrawInfluence()
    {
        EditorGUILayout.LabelField("Influence Map Points", EditorStyles.boldLabel);

        if (_AgentInfoCreator.influencePoints != null)
        {
            for (int lI = 0; lI < _AgentInfoCreator.influencePoints.Count; lI++)
            {
                DrawComponentInspector(_AgentInfoCreator.influencePoints[lI]);
            }
        }

        GUILayout.Space(15f);

        if (GUILayout.Button("ENLEVER INFLUENCE")) _AgentInfoCreator.RemoveInfluencePoint();
        if (GUILayout.Button("AJOUTER INFLUENCE")) _AgentInfoCreator.AddInfluencePoint();
    }

    private void DrawComponentInspector(Component pTarget)
    {
        if (pTarget == null) return;

        Rect lRect = EditorGUILayout.BeginVertical(GUI.skin.box);

        
        GUI.color = Color.white;

        if (!_CachedEditors.TryGetValue(pTarget, out Editor lEditor) || lEditor == null)
        {
            lEditor = Editor.CreateEditor(pTarget);
            _CachedEditors[pTarget] = lEditor;
        }

        lEditor.OnInspectorGUI();

        GUI.color = Color.white;

        EditorGUI.DrawRect(lRect, new Color(0.8f, 0.8f, 0.8f, 0.12f));

        EditorGUILayout.EndVertical();

        DrawSeparator();
    }

    private void DrawSeparator()
    {
        Rect lRect = EditorGUILayout.BeginVertical();
        lRect.y += SEPARATOR_SIZE * 0.5f * 0.8f;
        lRect.height -= SEPARATOR_SIZE * 0.8f;

        EditorGUI.DrawRect(lRect, new Color(0.8f, 0.8f, 0.8f, 1f));
        GUILayout.Space(SEPARATOR_SIZE);
        EditorGUILayout.EndVertical();
    }

    private void ResetTarget()
    {
        _Target = null;
        GetEnemies();
        GetAllies();
        GetAll();
        ClearCachedEditors();
    }

    private void GetEnemies()
    {
        _EnemiesList = GetAssetsWithComponent<Enemy>();
    }

    private void GetAllies()
    {
        _AlliesList = GetAssetsWithComponent<Ally>();
    }
    private void GetAll()
    {
        _All = GetAssetsWithComponent<AIAgent>();
    }

    private T[] GetAssetsWithComponent<T>() where T : MonoBehaviour
    {
        string[] lGuids = AssetDatabase.FindAssets("t:Prefab");
        List<T> lResults = new List<T>();

        string lPath;
        GameObject lGo;

        for (int lI = 0; lI < lGuids.Length; lI++)
        {
            lPath = AssetDatabase.GUIDToAssetPath(lGuids[lI]);
            lGo = AssetDatabase.LoadAssetAtPath<GameObject>(lPath);

            if (lGo != null)
            {
                T lComp = lGo.GetComponent<T>();
                if (lComp != null)
                {
                    lResults.Add(lComp);
                }
            }
        }

        return lResults.ToArray();
    }

    private void ClearCachedEditors()
    {
        foreach (Editor lEditor in _CachedEditors.Values)
        {
            if (lEditor != null) DestroyImmediate(lEditor);
        }
        _CachedEditors.Clear();
    }
}
