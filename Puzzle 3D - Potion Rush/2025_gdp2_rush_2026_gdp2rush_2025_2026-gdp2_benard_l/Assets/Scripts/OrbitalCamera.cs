using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{

    [Header("Pivot")]
    [SerializeField] private Vector3 _Pivot = Vector3.zero; 

    [Header("Distance")]
    [SerializeField] private float _Radius = 5f; 
    [SerializeField] private float _MinRadius = 2f;
    [SerializeField] private float _MaxRadius = 10f;
    [SerializeField] private float _ZoomSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float _MouseRotationSpeed = 3f;
    [SerializeField] private float _KeyRotationSpeed = 15f;
    [SerializeField] private float _MinTheta = 5f;   
    [SerializeField] private float _MaxTheta = 85f;

    private float _Theta; 
    private float _Phi;   

    private void Start()
    {

        Vector3 lOffset = transform.position - _Pivot;
        _Radius = lOffset.magnitude;
        _Theta = Mathf.Asin(lOffset.y / _Radius) * Mathf.Rad2Deg;
        _Phi = Mathf.Atan2(lOffset.x, lOffset.z) * Mathf.Rad2Deg;

        UpdateCameraPosition();

        InputManager.onCameraMove += HandleKeyboardMove;
        InputManager.onCameraRotate += HandleMouseRotate;
        InputManager.onCameraZoom += HandleZoom;
    }

    private void Update()
    {
        UpdateCameraPosition();
    }

    private void HandleKeyboardMove(float pHonrizontalAxe, float pVerticalAxe)
    {
        _Phi -= pHonrizontalAxe * _KeyRotationSpeed  * Time.deltaTime;
        _Theta += pVerticalAxe * _KeyRotationSpeed * Time.deltaTime;

        ClampAndApply();
    }

    private void HandleMouseRotate(float pMouseX, float pMouseY)
    {
        _Phi += pMouseX * _MouseRotationSpeed;
        _Theta -= pMouseY * _MouseRotationSpeed;

        ClampAndApply();
    }


    private void HandleZoom(float pScroll)
    {
        _Radius -= pScroll * _ZoomSpeed;
        _Radius = Mathf.Clamp(_Radius, _MinRadius, _MaxRadius);

        UpdateCameraPosition();
    }

    private void ClampAndApply()
    {
        _Theta = Mathf.Clamp(_Theta, _MinTheta, _MaxTheta);
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        float lRadTheta = Mathf.Deg2Rad * _Theta;
        float lRadPhi = Mathf.Deg2Rad * _Phi;

        Vector3 lOffset = new Vector3(
            _Radius * Mathf.Sin(lRadPhi) * Mathf.Cos(lRadTheta),
            _Radius * Mathf.Sin(lRadTheta),
            _Radius * Mathf.Cos(lRadPhi) * Mathf.Cos(lRadTheta)
        );

        transform.position = _Pivot + lOffset;
        transform.LookAt(_Pivot);
    }
}
