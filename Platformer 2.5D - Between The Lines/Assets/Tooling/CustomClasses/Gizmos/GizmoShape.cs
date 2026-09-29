#if UNITY_EDITOR
using UnityEngine;
using System;
using Tooling.GizmoShapes.Properties;


namespace Tooling.GizmoShapes
{
    [Serializable]
    internal abstract class GizmoShape
    {
        [NonSerialized] public bool ready = false;
        protected Action m_Action;

        public void DrawShape(Color pColor)
        {
            Gizmos.color = pColor;
            m_Action?.Invoke();
        }

        public abstract void Update(Transform pTransform);
        public abstract void Init(Transform pTransform);
        protected abstract void SetAction(Transform pTransform);
    }
}
#endif