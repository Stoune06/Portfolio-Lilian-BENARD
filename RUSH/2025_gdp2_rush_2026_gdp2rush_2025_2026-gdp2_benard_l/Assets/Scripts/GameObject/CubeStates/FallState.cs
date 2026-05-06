using UnityEngine;

public class FallState : CubeState
{
    private Vector3 _FromPosition, _ToPosition;
    public static float levelYLimit;

    public override void SetMode(Cube pCube)
    {
        if (pCube.transform.position.y <= -levelYLimit)
        {
            pCube.OutOfLevelTrigger();
            Debug.Log(-levelYLimit);
        }
        _FromPosition = pCube.transform.position;
        _ToPosition = _FromPosition + Vector3.down;
    }

    public override void DoActionMode(Cube pCube)
    {
        pCube.transform.position = Vector3.Lerp(_FromPosition, _ToPosition, pCube.ratio);
    }
}
