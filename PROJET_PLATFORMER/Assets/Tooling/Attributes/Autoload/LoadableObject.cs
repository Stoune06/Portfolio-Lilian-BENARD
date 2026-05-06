using UnityObject = UnityEngine.Object;
using System;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Attributes
{
    /// <summary>
    /// Used to store <see cref="GameObject"/> in the AutoLoad Window
    /// </summary>
    [Serializable]
    public class LoadableObject : ILoadable
    {
        [SerializeField] public UnityObject gameObject = default;
        public LoadType LoadType { get => loadType; set => loadType = value; }
        [SerializeField] public LoadType loadType;
        public LoadableObject(UnityObject pGameObject)
        {
            gameObject = pGameObject;
        }

        public static int operator &(LoadableObject pLoadable, LoadType pLoadType) => ((int)pLoadable.loadType & (int)pLoadType);
        public static int operator &(LoadableObject pLoadable, LoadableObject pLoadableA) => ((int)pLoadable.loadType & (int)pLoadableA.loadType);

        public UnityObject Instantiate() => UnityObject.Instantiate(gameObject);
    }
}