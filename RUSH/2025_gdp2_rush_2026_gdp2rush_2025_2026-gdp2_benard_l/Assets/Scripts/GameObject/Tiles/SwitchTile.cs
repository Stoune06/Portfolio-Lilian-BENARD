using UnityEngine;

public class SwitchTile : Tile
{
    [Header("Liaison Visuelle")]
    [SerializeField] private SwitchVisualRotator _VisualRotator;

    protected override void Start()
    {
        stateToGive = new SwitchState(this, transform.forward, transform.position);

        // Sécurité
        if (_VisualRotator == null) _VisualRotator = GetComponentInChildren<SwitchVisualRotator>();
    }

    // Cette fonction sera appelée par le SwitchState
    public void OnSwitchActivated()
    {
        if (_VisualRotator != null)
        {
            _VisualRotator.InvertDirection();
        }
    }
}