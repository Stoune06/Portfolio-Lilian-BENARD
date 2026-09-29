using UnityEngine;

namespace Tooling
{
    /// <summary>
    /// <see cref="Singleton{T}"></see> allows you to create a <c>Singleton</c> of the <typeparamref name="T"/> type simply by inheriting it.
    /// This is not yet compatible with multithreading, <c> only work if <typeparamref name="T"/> inherit from <see cref="MonoBehaviour"></see></c>
    /// </summary>
    [DefaultExecutionOrder(-512), DisallowMultipleComponent, Icon("Assets/Tooling/CustomClasses/Singleton.png")]
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;
        public static bool InstanceExist() => Instance != null;

        public virtual void Awake()
        {
            SetInstance();
        }

        protected void SetInstance()
        {
            if (InstanceExist())
                Destroy(this);
            else
                Instance = this as T;
        }

        public virtual void OnDestroy()
        {
            if (this == Instance) Instance = null;
        }
    }
}