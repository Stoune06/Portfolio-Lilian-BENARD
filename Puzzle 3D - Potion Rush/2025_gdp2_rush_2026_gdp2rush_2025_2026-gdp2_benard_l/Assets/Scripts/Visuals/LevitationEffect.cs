using UnityEngine;

public class LevitationEffect : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float _Amplitude = 0.1f; // Hauteur du flottement
    [SerializeField] private float _Frequency = 1f;   // Vitesse du flottement
    [SerializeField] private float _RandomOffset = 0f; // Pour qu'elles ne bougent pas toutes en même temps

    private Vector3 _StartPos;

    private void Start()
    {
        _StartPos = transform.localPosition;
        // On ajoute un décalage aléatoire si on veut, ou on le définit manuellement
        _RandomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        float newY = _StartPos.y + Mathf.Sin(Time.time * _Frequency + _RandomOffset) * _Amplitude;
        transform.localPosition = new Vector3(_StartPos.x, newY, _StartPos.z);
    }
}