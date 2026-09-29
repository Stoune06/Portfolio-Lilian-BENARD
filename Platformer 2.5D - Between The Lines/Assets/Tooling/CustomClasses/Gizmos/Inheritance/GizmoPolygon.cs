#if UNITY_EDITOR
using Tooling.GizmoShapes.Properties;
using UnityEngine;

namespace Tooling.GizmoShapes
{
    internal class GizmoCube : GizmoShape
    {
        [SerializeField] public CompactSerializedProperty<Vector3> size;

        public override void Init(Transform pTransform)
        {
            size.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            size.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            Vector3 lSize = size.useSerialized ? size.serializedProperty.vector3Value : size.property;
            m_Action = () => Gizmos.DrawCube(pTransform.position, lSize);
        }
    }

    internal class GizmoWireCube : GizmoCube
    {
        protected override void SetAction(Transform pTransform)
        {
            Vector3 lSize = size.useSerialized ? size.serializedProperty.vector3Value : size.property;
            m_Action = () => Gizmos.DrawWireCube(pTransform.position, lSize);
        }
    }

    internal class GizmoFrustum : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<float> fov;
        [SerializeField] internal CompactSerializedProperty<float> maxRange;
        [SerializeField] internal CompactSerializedProperty<float> minRange;
        [SerializeField] internal CompactSerializedProperty<float> aspect;

        public override void Init(Transform pTransform)
        {
            fov.Init();
            maxRange.Init();
            minRange.Init();
            aspect.Init();

            Update(pTransform);
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            fov.UpdateSerializedProperty();
            maxRange.UpdateSerializedProperty();
            minRange.UpdateSerializedProperty();
            aspect.UpdateSerializedProperty();

            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            float lFov = fov.useSerialized ? fov.serializedProperty.floatValue : fov.property;
            float lMaxRange = maxRange.useSerialized ? maxRange.serializedProperty.floatValue : maxRange.property;
            float lMinRange = minRange.useSerialized ? minRange.serializedProperty.floatValue : minRange.property;
            float lAspect = aspect.useSerialized ? aspect.serializedProperty.floatValue : aspect.property;

            m_Action = () => Gizmos.DrawFrustum(pTransform.position, lFov, lMaxRange, lMinRange, lAspect);
        }
    }

    internal class GizmoLine : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<Vector3> to;

        public override void Init(Transform pTransform)
        {
            to.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            to.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            Vector3 lTo = to.useSerialized ? to.serializedProperty.vector3Value : to.property;
            m_Action = () => Gizmos.DrawLine(pTransform.position, lTo);
        }
    }

    internal class GizmoMesh : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<Mesh> mesh;
        [SerializeField] internal CompactSerializedProperty<int> subMeshIndex;
        [SerializeField] internal CompactSerializedProperty<Quaternion> rotation;
        [SerializeField] internal CompactSerializedProperty<Vector3> scale;

        public override void Init(Transform pTransform)
        {
            mesh.Init();
            subMeshIndex.Init();
            rotation.Init();
            scale.Init();

            Update(pTransform);
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            mesh.UpdateSerializedProperty();
            subMeshIndex.UpdateSerializedProperty();
            rotation.UpdateSerializedProperty();
            scale.UpdateSerializedProperty();

            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            Mesh lMesh = mesh.useSerialized ? (Mesh)mesh.serializedProperty.objectReferenceValue : mesh.property;
            int lSubMeshIndex = subMeshIndex.useSerialized ? subMeshIndex.serializedProperty.intValue : subMeshIndex.property;
            Quaternion lRotation = rotation.useSerialized ? rotation.serializedProperty.quaternionValue : rotation.property;
            Vector3 lScale = scale.useSerialized ? scale.serializedProperty.vector3Value : scale.property;

            m_Action = () => Gizmos.DrawMesh(lMesh, lSubMeshIndex, pTransform.position, lRotation, lScale);
        }
    }

    internal class GizmoWireMesh : GizmoMesh
    {
        protected override void SetAction(Transform pTransform)
        {
            Mesh lMesh = mesh.useSerialized ? (Mesh)mesh.serializedProperty.objectReferenceValue : mesh.property;
            int lSubMeshIndex = subMeshIndex.useSerialized ? subMeshIndex.serializedProperty.intValue : subMeshIndex.property;
            Quaternion lRotation = rotation.useSerialized ? rotation.serializedProperty.quaternionValue : rotation.property;
            Vector3 lScale = scale.useSerialized ? scale.serializedProperty.vector3Value : scale.property;

            m_Action = () => Gizmos.DrawMesh(lMesh, lSubMeshIndex, pTransform.position, lRotation, lScale);
        }
    }

    internal class GizmoRay : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<Vector3> direction;

        public override void Init(Transform pTransform)
        {
            direction.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            direction.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            Vector3 lDirection = direction.useSerialized ? direction.serializedProperty.vector3Value : direction.property;
            m_Action = () => Gizmos.DrawRay(pTransform.position, lDirection);
        }
    }

    internal class GizmoSphere : GizmoShape
    {
        [SerializeField] public CompactSerializedProperty<float> radius;

        public override void Init(Transform pTransform)
        {
            radius.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            radius.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            float lSize = radius.useSerialized ? radius.serializedProperty.floatValue : radius.property;
            m_Action = () => Gizmos.DrawSphere(pTransform.position, lSize);
        }
    }

    internal class GizmoWireSphere : GizmoSphere
    {
        protected override void SetAction(Transform pTransform)
        {
            float lSize = radius.useSerialized ? radius.serializedProperty.floatValue : radius.property;
            m_Action = () => Gizmos.DrawWireSphere(pTransform.position, lSize);
        }
    }

    internal class GizmoGUITexture : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<Rect> screenRect;
        [SerializeField] internal CompactSerializedProperty<Texture> texture;

        public override void Init(Transform pTransform)
        {
            screenRect.Init();
            texture.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            screenRect.UpdateSerializedProperty();
            texture.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            Rect lScreenRect = screenRect.useSerialized ? screenRect.serializedProperty.rectValue : screenRect.property;
            Texture lTexture = texture.useSerialized ? (Texture)texture.serializedProperty.objectReferenceValue : texture.property;
            m_Action = () => Gizmos.DrawGUITexture(lScreenRect, lTexture);
        }
    }

    internal class GizmoIcon : GizmoShape
    {
        [SerializeField] internal CompactSerializedProperty<string> name;
        [SerializeField] internal CompactSerializedProperty<bool> allowScaling;

        public override void Init(Transform pTransform)
        {
            name.Init();
            allowScaling.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            name.UpdateSerializedProperty();
            allowScaling.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform)
        {
            string lName = name.useSerialized ? name.serializedProperty.stringValue : name.property;
            bool lAllowScaling = allowScaling.useSerialized ? allowScaling.serializedProperty.boolValue : allowScaling.property;
            m_Action = () => Gizmos.DrawIcon(pTransform.position, lName, lAllowScaling);
        }
    }
}
#endif