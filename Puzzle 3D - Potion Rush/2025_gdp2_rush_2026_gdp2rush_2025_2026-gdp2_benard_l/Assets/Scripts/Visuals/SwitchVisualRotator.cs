using UnityEngine;

public class SwitchVisualRotator : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float _RotationSpeed = 50f;
    [SerializeField] private string _PropertyName = "_Rotation"; // Créez cette propriété Float dans votre Shader Graph !
    [SerializeField] private Renderer _Renderer;

    private float _CurrentAngle = 0f;
    private int _PropertyID;

    private void Awake()
    {
        if (_Renderer == null) _Renderer = GetComponent<Renderer>();
        _PropertyID = Shader.PropertyToID(_PropertyName);
    }

    private void Update()
    {
        // On incrémente l'angle (indépendant des FPS)
        _CurrentAngle += _RotationSpeed * Time.deltaTime;
        _CurrentAngle %= 360f; // On garde une valeur propre

        // On envoie au shader
        if (_Renderer != null)
        {
            _Renderer.material.SetFloat(_PropertyID, _CurrentAngle);
        }
    }

    // C'est cette fonction qu'on appellera pour inverser le sens
    public void InvertDirection()
    {
        _RotationSpeed *= -1;
    }
}