using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Header("Réglages")]
    [Tooltip("Distance par rapport au centre du parent (pour éviter le Z-fighting)")]
    [SerializeField] private float _OffsetDistance = 0.6f;

    void LateUpdate()
    {
        Camera lTargetCam = GetTargetCamera();

        if (lTargetCam != null)
        {
            transform.rotation = lTargetCam.transform.rotation;
            if (transform.parent != null)
            {
                Vector3 lDirectionToCamera = (lTargetCam.transform.position - transform.parent.position).normalized;
                transform.position = transform.parent.position + (lDirectionToCamera * _OffsetDistance);
            }
        }
    }

    private Camera GetTargetCamera()
    {
        if (GameManager.instance != null && GameManager.instance.gameCamera != null)
        {
            return GameManager.instance.gameCamera;
        }
        return Camera.main;
    }
}