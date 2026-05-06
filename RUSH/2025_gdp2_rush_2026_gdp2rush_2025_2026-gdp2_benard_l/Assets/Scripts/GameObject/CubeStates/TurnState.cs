using TMPro;
using UnityEngine;

public class TurnState : CubeState
{
    protected Vector3 m_TurnDirection;
    private Vector3 _Origin;


    public TurnState(Vector3 pTurnDirection, Vector3 pOrigin)
    {
        m_TurnDirection = pTurnDirection;
        _Origin = pOrigin;
    }

    public override void SetMode(Cube pCube)
    {
        pCube.movementDirection = m_TurnDirection;
        pCube.TryMove();
    }
}
