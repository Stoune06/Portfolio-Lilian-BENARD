using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem _ParticleSystem;

    private void Start()
    {
        _ParticleSystem = GetComponent<ParticleSystem>();

        ParticleSystem.MainModule lMain = _ParticleSystem.main;
        if (lMain.loop)
            Debug.LogWarning(gameObject.name + " est en mode Loop. " +"Il ne sera jamais détruit automatiquement.");
    }

    private void Update()
    {
        if (_ParticleSystem == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!_ParticleSystem.IsAlive(true))
            Destroy(gameObject);
    }
}