using System;
using UnityEngine;

public class Target : Tile
{
    public static event Action onTargetReached;
    public Color targetColor;

    [Header("VFX")]
    [SerializeField]
    private ParticleSystem _WinVFXPrefab;

    protected override void Start()
    {
        stateToGive = new OnTargetState(this);
        targetColor = GetComponentInChildren<MeshRenderer>().material.color;
    }


    private void PlayWinVFX()
    {
        if (_WinVFXPrefab != null)
        {
            ParticleSystem lVfxInstance = Instantiate(_WinVFXPrefab, transform.position, Quaternion.identity);
            var lMainModule = lVfxInstance.main;
            lMainModule.startColor = targetColor;
            lVfxInstance.Play();
            Destroy(lVfxInstance.gameObject, 2f);
        }
    }
    public void TargetReached()
    {
        onTargetReached?.Invoke();

        PlayWinVFX();
    }

    private void TargetAnimation()
    {
        //TODO
    }
}
