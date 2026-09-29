using static Editors.History.Utils;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Editors.History
{
    [InitializeOnLoad]
    public static class RadialTabMenu
    {
        private const float PI2 = Mathf.PI * 2f;
        private static float _Radius = 80f;

        private static bool _IsOpen = false;
        private static bool _WasNotOpenLastFrame = true;

        private static Vector2 _Center;
        private static int _SelectedIndex = 0;

        private static Dictionary<string, List<NamePath>> _FolderToTabHolder = new Dictionary<string, List<NamePath>>();

        //static RadialTabMenu()
        //{
        //    SceneView.duringSceneGui += OnSceneGUI;
        //}

        //static void OnSceneGUI(SceneView pScene)
        //{
        //    Event e = Event.current;
        //    SetTabBagsPaths();
        //    if (e.keyCode == KeyCode.Space)
        //    {
        //        if (e.type == EventType.KeyDown)
        //        {
        //            _IsOpen = true;
        //            if (_WasNotOpenLastFrame)
        //            {
        //                _WasNotOpenLastFrame = false;
        //                _Center = e.mousePosition;
        //            }
        //            e.Use();
        //        }
        //        else if (e.type == EventType.KeyUp)
        //        {
        //            _IsOpen = false;
        //            _WasNotOpenLastFrame = true;
        //            e.Use();
        //        }
        //    }
        //    else if (!_IsOpen)
        //        return;

        //    Handles.BeginGUI();
        //    DrawRadialMenu(e.mousePosition);
        //    Handles.EndGUI();

        //    pScene.Repaint();
        //}

        //static void DrawRadialMenu(Vector2 pMousePos)
        //{
        //    Vector2 lPosition = pMousePos - _Center;
        //    float lAngle = Mathf.Atan2(lPosition.y, lPosition.x);
        //    _SelectedIndex = Mathf.FloorToInt(lAngle / PI2 * _FolderToTabHolder.Count);

        //    string lName;
        //    Rect lRect;

        //    for (int i = 0; i < _FolderToTabHolder.Count; i++)
        //    {
        //        lName = _FolderToTabHolder.ElementAt(i).Key;
        //        lAngle = (PI2 / _FolderToTabHolder.Count) * i;
        //        lPosition = _Center + new Vector2(Mathf.Cos(lAngle), Mathf.Sin(lAngle)) * _Radius;
        //        lRect = new Rect(lPosition.x - 30f, lPosition.y - 10f, lName.Length * CHAR_SIZE, 20f);

        //        if (i == _SelectedIndex) //TODO CLEAN
        //        {
        //            GUI.backgroundColor = Color.cyan;
        //            DrawFinalRadialMenu(lRect, _FolderToTabHolder[lName]);
        //            GUI.backgroundColor = Color.white;
        //        }
        //        GUI.Button(lRect, lName);
        //    }
        //}

        //static void DrawFinalRadialMenu(Rect pRect, List<NamePath> pList)
        //{
        //    GUI.backgroundColor = Color.red;
        //    float lAngle;
        //    Vector2 lPosition;
        //    Rect lRect;

        //    for (int i = 0; i < pList.Count; i++)
        //    {
        //        lAngle = (PI2 / pList.Count) * i;
        //        lPosition = pRect.center + new Vector2(Mathf.Cos(lAngle), -Mathf.Sin(lAngle)) * _Radius;
        //        lRect = new Rect(lPosition.x - 30f, lPosition.y - 10f, pList[i].name.Length * CHAR_SIZE,20f);

        //        if(GUI.Button(lRect, pList[i].name))
        //        {
        //            Pressed(pList[i].path);
        //        }

        //    }

        //    GUI.backgroundColor = Color.white;
        //}

        //static void SetTabBagsPaths()
        //{
        //    _FolderToTabHolder.Clear();
        //    string[] lPathArray = Directory.GetDirectories(TabBagWindow.LastInstance.Parameter.savePath);
        //    int pCharacterToIgnore = TabBagWindow.LastInstance.Parameter.savePath.Length;

        //    NamePath lDuo;
        //    string lBagPath;
        //    string lBagName;

        //    for (int i = 0; i < lPathArray.Length; i++)
        //    {
        //        lBagPath = lPathArray[i];
        //        lBagName = Path.GetFileName(lBagPath);
        //        _FolderToTabHolder.Add(lBagName, new List<NamePath>());

        //        foreach (string item in Directory.GetDirectories(lBagPath))
        //        {
        //            lDuo = new NamePath(Path.GetFileName(item), item);
        //            _FolderToTabHolder[lBagName].Add(lDuo);
        //        }
        //    }
        //}

        //static void Pressed(string pPath)
        //{
        //    TabBagWindow.LastInstance.SetDataBag(pPath);
        //}
    }

    internal struct NamePath
    {
        public string name;
        public string path;

        public NamePath(string pName, string pPath)
        {
            name = pName;
            path = pPath;
        }
    }
}