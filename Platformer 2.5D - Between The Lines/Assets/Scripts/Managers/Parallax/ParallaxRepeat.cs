using UnityEngine;

namespace Platformer.Managers
{
    [RequireComponent(typeof(Renderer))]
    public class ParallaxRepeat : MonoBehaviour
    {
        [field: SerializeField] public float ParallaxSpeed { get; private set; } = 0.5f;
        private Transform _CameraTransform;

        private Vector3 _LastCameraPosition;
        private Renderer _Renderer;

        void Start()
        {
            _CameraTransform = Camera.main.transform;
            _LastCameraPosition = _CameraTransform.position;
            _Renderer = GetComponent<Renderer>();
        }

        void LateUpdate()
        {
            Vector3 lPosition = _CameraTransform.position - _LastCameraPosition;
            transform.position += lPosition;
            _Renderer.material.mainTextureOffset += (Vector2)(Time.deltaTime * ParallaxSpeed * lPosition);
            _LastCameraPosition = _CameraTransform.position;
        }
    }
}