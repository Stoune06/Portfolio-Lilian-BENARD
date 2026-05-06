using UnityEngine;

public class StopState : CubeState
{
    private int _WallCollisionWaitingDuration = 1;
    private float _WaitingDuration = 2f;
    private float _TickElapsed = 0;

    public override void OnTick(Cube pCube)
    {
        _TickElapsed += 1;
        if (_TickElapsed > _WaitingDuration)
        {
            pCube.TryMove(_WallCollisionWaitingDuration);
            _TickElapsed = 0;
        }
    }
}
