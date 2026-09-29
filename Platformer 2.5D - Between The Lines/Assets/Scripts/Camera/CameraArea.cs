using UnityEngine;

//Author : Sales Noé
namespace Platformer.GameCamera
{
    public class CameraArea : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private CameraParameter _AreaParameter;

        private const string CAMERA_TAG = "PlayerCamera";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(CAMERA_TAG) || !collision.TryGetComponent(out PlayerCamera lCamera)) return;
            lCamera.AddToQueue(_AreaParameter); //Add camera parameter
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag(CAMERA_TAG) || !collision.TryGetComponent(out PlayerCamera lCamera)) return;
            lCamera.RemoveFromQueue(_AreaParameter); //Remove camera parameter
        }
    }

    [System.Serializable]
    public struct CameraParameter
    {
        public float CameraZoom;
        public Vector2 CameraOffset;
        public AnimationCurve LerpCurve;
        public float Duration;
        public int Priority;
        public bool Loop;
    }
}

