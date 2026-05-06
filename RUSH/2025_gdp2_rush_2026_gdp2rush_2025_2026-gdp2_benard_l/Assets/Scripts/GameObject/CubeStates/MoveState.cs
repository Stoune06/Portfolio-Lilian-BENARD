using UnityEngine;

public class MoveState : CubeState
{
    private Vector3 _FromPos, _Pivot;
    private Quaternion _FromRotation, _ToRotation;

    public float yFloorPos;

    private bool _HasDisabledCollider = false;
    public override void SetMode(Cube pCube)
    {
        _FromPos = pCube.transform.position;
        _FromPos.y = yFloorPos + Cube.cubeSide;
        _FromRotation = pCube.transform.rotation;
        float lCubeSide = Cube.cubeSide;

        _Pivot = _FromPos + pCube.movementDirection * (lCubeSide / 2f) + Vector3.down * (lCubeSide / 2f);
        _Pivot = new Vector3(_Pivot.x , yFloorPos + Cube.cubeSide * 0.5f, _Pivot.z);
        _ToRotation = Quaternion.AngleAxis(90f, Vector3.Cross(Vector3.up, pCube.movementDirection)) * _FromRotation;


        AudioSignals.TriggerSound(AudioClipsEnum.CubeMove,_FromPos);

        if (!pCube.isContinuousMove)
        {
            pCube.SetColliderState(false);
            _HasDisabledCollider = true;
        }
        else
        {
            _HasDisabledCollider = false;
            pCube.SetColliderState(true);
        }
    }

    public override void DoActionMode(Cube pCube)
    {
        AnimationCurve currentCurve = pCube.isContinuousMove ? pCube.standardMoveCurve : pCube.startMoveCurve;
        float evaluatedRatio = currentCurve.Evaluate(pCube.ratio);

        if (_HasDisabledCollider && pCube.ratio > 0.3f)
        {
            pCube.SetColliderState(true);
            _HasDisabledCollider = false;
        }
        float lAngle = Mathf.PI * 0.5f * evaluatedRatio;
        Vector3 lAxis = Vector3.Cross(Vector3.up, pCube.movementDirection);

        pCube.transform.position = RotateAroundPivot(_FromPos, lAxis, lAngle, _Pivot); 
        pCube.transform.rotation = Quaternion.LerpUnclamped(_FromRotation, _ToRotation, evaluatedRatio);
    }

    public override void OnTick(Cube pCube)
    {
        pCube.SetColliderState(true);
        _HasDisabledCollider = false;
        base.OnTick(pCube);
    }

    private Vector3 RotateAroundPivot(Vector3 pX, Vector3 pAxis, float pAngle, Vector3 pPivot)
    {
        pAxis = pAxis.normalized;
        Vector3 lX = pX - pPivot;
        return pPivot + (lX
            + Mathf.Sin(pAngle) * Vector3.Cross(pAxis, lX)
            + (1 - Mathf.Cos(pAngle)) * Vector3.Cross(pAxis, Vector3.Cross(pAxis, lX)));
    }
}

