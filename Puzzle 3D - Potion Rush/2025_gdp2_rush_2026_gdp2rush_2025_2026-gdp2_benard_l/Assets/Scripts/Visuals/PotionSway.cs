using UnityEngine;

public class PotionSway : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float _AngleMax = 15f;
    [SerializeField] private float _Vitesse = 2f;

    private Quaternion _RotationInitiale;
    private float _ElapsedTime = 0;

    private void Start()
    {
        _RotationInitiale = transform.localRotation;
    }

    private void Update()
    {
        _ElapsedTime += Time.deltaTime;
        float angleZ = Mathf.Sin(_ElapsedTime * _Vitesse) * _AngleMax;
        transform.localRotation = _RotationInitiale * Quaternion.Euler(0, 0, angleZ);
    }
}