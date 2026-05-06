using System.Collections.Generic;
using UnityEngine;

public class SwitchState : TurnState
{
    private static List<SwitchState> _AllSwitch = new List<SwitchState>();

    private float _AngleRotation = 90f;
    private SwitchTile _OwnerTile;

    public SwitchState(SwitchTile pTile, Vector3 pTurnDirection, Vector3 pOrigin) : base(pTurnDirection, pOrigin)
    {
        _OwnerTile = pTile;
    }

    public override void SetMode(Cube pCube)
    {
        m_TurnDirection = Quaternion.AngleAxis(_AngleRotation,Vector3.up) * pCube.movementDirection;
        base.SetMode(pCube);
        _AngleRotation *= -1;
        _OwnerTile.OnSwitchActivated();
    }

    public static void ResetRotations()
    {
        foreach(SwitchState lSwitch in _AllSwitch)
        {
            lSwitch._AngleRotation = 90f;
        }
    }
}
