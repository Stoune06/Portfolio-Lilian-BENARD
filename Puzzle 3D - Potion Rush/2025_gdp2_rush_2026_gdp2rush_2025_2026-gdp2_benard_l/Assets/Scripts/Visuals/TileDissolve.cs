using System;
using System.Collections;
using UnityEngine;

public class TileDissolve : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _Duration = 0.8f;
    [SerializeField] private Renderer _Renderer;

    private static readonly int _DissolveProp = Shader.PropertyToID("_DissolveAmount");

    private void Awake()
    {
        if (_Renderer == null) _Renderer = GetComponentInChildren<Renderer>();
    }

    public void StartDissolve(Action onComplete)
    {
        StartCoroutine(DissolveRoutine(onComplete));
    }

    private IEnumerator DissolveRoutine(Action onComplete)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = false;

        float elapsed = 0f;

        if (_Renderer != null) _Renderer.material.SetFloat(_DissolveProp, 0f);

        while (elapsed < _Duration)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / _Duration;
            if (_Renderer != null)
            {
                _Renderer.material.SetFloat(_DissolveProp, ratio);
            }

            yield return null;
        }
        if (_Renderer != null) _Renderer.material.SetFloat(_DissolveProp, 1f);
        onComplete?.Invoke();
        Destroy(gameObject);
    }
}