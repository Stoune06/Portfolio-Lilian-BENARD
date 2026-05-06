using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

//Author : MERFOUD Kelyan

namespace Tooling.Attributes
{
    [System.Diagnostics.DebuggerStepThrough, DefaultExecutionOrder(-1)]
    internal static class AutoLoadManager
    {
        internal const string AUTO_LOAD = "[AutoLoad]";

        internal static bool _Initialized = false;
        internal static Transform _AutoLoadTransform, _DestructibleAutoLoadTransform;

        internal static Dictionary<Type, bool> _TypeToUnique = new Dictionary<Type, bool>();
        internal static Dictionary<Type, bool> _TypeToDestructibleUnique = new Dictionary<Type, bool>();

        internal static List<UnityObject> _PrefabList = new List<UnityObject>();
        internal static List<UnityObject> _DestructiblePrefabList = new List<UnityObject>();

        internal static Dictionary<MethodInfo, RuntimeAutoLoadType> _MethodInfoToRunType = new Dictionary<MethodInfo, RuntimeAutoLoadType>();
        internal static object[] _RunTimeObjects;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            if (_Initialized) return;

            List<Type> lListType = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies().
                Where(a =>{ try{return a.GetReferencedAssemblies().Any(r => r.FullName == Assembly.GetExecutingAssembly().FullName);}
                    catch{return false;}}))
            {
                foreach (Type lType in item.GetTypes())
                {
                    lListType.Add(lType);
                }
            }

            Type[] lTypeArray = lListType.ToArray();

            SetAutoLoadedMethod(lTypeArray);
            CallMethodFromAttribute(RuntimeAutoLoadType.BeforeAutoLoad);

            SetAutoLoadedClasses(lTypeArray);
            SetAutoLoadedPrefabs();

            if (_TypeToUnique.Count + _PrefabList.Count > 0)
            {
                InstantiateAutoLoadContainer();
                InstantiateAutoLoadScripts(_AutoLoadTransform, _TypeToUnique);
                InstantiateAutoLoadPrefabs(_AutoLoadTransform, _PrefabList);
                _TypeToUnique.Clear();
            }

            InstantiateAllDestructibles();
            if (_TypeToDestructibleUnique.Count + _DestructiblePrefabList.Count > 0)
            {
                SceneManager.sceneLoaded += (pA, pB) =>
                {
                    CallMethodFromAttribute(RuntimeAutoLoadType.BeforeAutoLoad);
                    InstantiateAllDestructibles();
                    CallMethodFromAttribute(RuntimeAutoLoadType.AfterAutoLoad);
                };
            }

            CallMethodFromAttribute(RuntimeAutoLoadType.AfterAutoLoad);
            _Initialized = true;
        }

        private static void InstantiateAutoLoadContainer()
        {
            _AutoLoadTransform = new GameObject($"{AUTO_LOAD} Container").transform;
            GameObject.DontDestroyOnLoad(_AutoLoadTransform);
        }

        private static void InstantiateDestructibleAutoLoadContainer()
        {
            _DestructibleAutoLoadTransform = new GameObject($"{AUTO_LOAD} DestroyOnLoad").transform;
        }

        private static void InstantiateAllDestructibles()
        {
            InstantiateDestructibleAutoLoadContainer();
            InstantiateAutoLoadScripts(_DestructibleAutoLoadTransform, _TypeToDestructibleUnique);
            InstantiateAutoLoadPrefabs(_DestructibleAutoLoadTransform, _DestructiblePrefabList);
        }

        private static void SetAutoLoadedClasses(Type[] pTypeArray)
        {
            Type lMonoType = typeof(MonoBehaviour);
            ILoadable lAutoLoad;
            Type lType;

            for (int i = 0; i < pTypeArray.Length; i++)
            {
                lType = pTypeArray[i];

                if (lType.IsSubclassOf(lMonoType))
                {
                    lAutoLoad = lType.GetCustomAttribute<AutoLoad>();

                    if (lAutoLoad != null && lType.IsClass && !lType.IsAbstract)
                    {
                        if (lAutoLoad.IsDestroyedOnLoad())
                            _TypeToDestructibleUnique.Add(lType, lAutoLoad.IsUniqueGameObject());
                        else
                            _TypeToUnique.Add(lType, lAutoLoad.IsUniqueGameObject());
                    }
                }
            }
        }

        private static void SetAutoLoadedMethod(Type[] pTypeArray)
        {
            MethodInfo[] lMethodInfoArray;
            RuntimeAutoLoadMethod lRuntimeAutoLoad;
            MethodInfo lMethodInfo;
            List<object> lObjectList = new List<object>();


            for (int j = 0; j < pTypeArray.Length; j++)
            {
                lMethodInfoArray = pTypeArray[j].GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                for (int i = 0; i < lMethodInfoArray.Length; i++)
                {
                    lMethodInfo = lMethodInfoArray[i];
                    lRuntimeAutoLoad = lMethodInfo.GetCustomAttribute<RuntimeAutoLoadMethod>();

                    if (lRuntimeAutoLoad != null)
                    {
                        _MethodInfoToRunType.Add(lMethodInfo, lRuntimeAutoLoad.RuntimeAutoLoadType);
                        lObjectList.Add(pTypeArray[j]);
                    }
                }
            }

            _RunTimeObjects = lObjectList.ToArray();
        }

        private static void SetAutoLoadedPrefabs()
        {
            AutoScriptable lScriptable = Resources.Load<AutoScriptable>(AutoScriptable.PATH_AS_RESOURCE);
            if (lScriptable != null)
            {
                LoadableObject lLoadableObject;
                ILoadable lLoadable;

                for (int i = 0; i < lScriptable.LoadableArray.Length; i++)
                {
                    lLoadableObject = lScriptable.LoadableArray[i];
                    lLoadable = lLoadableObject;

                    if (lLoadable.IsActive())
                    {
                        if (lLoadable.IsDestroyedOnLoad()) _DestructiblePrefabList.Add(lLoadableObject.gameObject);
                        else _PrefabList.Add(lLoadableObject.gameObject);
                    }
                }
            }
        }

        private static void InstantiateAutoLoadScripts(Transform pContainer, Dictionary<Type, bool> pDico)
        {
            GameObject lGameObject;

            foreach (Type item in pDico.Keys)
            {
                if (pDico[item])
                {
                    lGameObject = new GameObject(item.Name);
                    lGameObject.transform.SetParent(pContainer.transform);
                }
                else
                {
                    lGameObject = pContainer.gameObject;
                }

                lGameObject.AddComponent(item);
            }
        }

        private static void InstantiateAutoLoadPrefabs(Transform pContainer, List<UnityObject> pList)
        {
            foreach (UnityObject item in pList)
            {
                switch (item)
                {
                    case GameObject lGameObject:
                        GameObject.Instantiate<GameObject>(lGameObject, pContainer);
                        break;
                    case SceneHolder lSceneHolder:
                        SceneHolder.Instantiate(lSceneHolder, LoadSceneMode.Additive);
                        break;
                    default:
                        UnityObject.Instantiate(item);
                        break;
                }
            }
        }

        private static void CallMethodFromAttribute(RuntimeAutoLoadType pRunType)
        {
            MethodInfo lMethodInfo;
            int lIteration = _MethodInfoToRunType.Keys.Count;

            for (int i = 0; i < lIteration; i++)
            {
                lMethodInfo = _MethodInfoToRunType.Keys.ElementAt(i);
                if (_MethodInfoToRunType[lMethodInfo] == pRunType) lMethodInfo.Invoke(_RunTimeObjects[i], null);
            }
        }
    }
}