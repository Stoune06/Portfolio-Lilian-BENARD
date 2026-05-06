using UnityEngine;

public class TeleportState : CubeState
{
    private int _TickCount = 0;
    private Teleporter _Target;
    private Teleporter _Origin;

    // Animation
    private int _NumberOfSpins = 2;
    private AnimationCurve _ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // État
    private Vector3 _StartPos;
    private Quaternion _StartRot;

    public TeleportState(Teleporter pOrigin, Teleporter pTarget)
    {
        _Target = pTarget;
        _Origin = pOrigin;
    }

    public override void SetMode(Cube pCube)
    {
        _TickCount = -1;

        _StartPos = pCube.transform.position;
        _StartRot = pCube.transform.rotation;

        AudioSignals.TriggerSound(AudioClipsEnum.Teleport, pCube.transform.position);
    }

    public override void DoActionMode(Cube pCube)
    {
        float ratio = pCube.ratio;
        float spinAngle = 360f * _NumberOfSpins * ratio;

        if (_TickCount == 0)
        {
            float currentScale = Mathf.Lerp(1f, 0f, _ScaleCurve.Evaluate(ratio));
            pCube.transform.localScale = Vector3.one * currentScale;
            pCube.transform.rotation = Quaternion.AngleAxis(spinAngle, Vector3.up) * _StartRot;
            pCube.transform.position = _StartPos;
        }

        else if (_TickCount == 1)
        {
            float currentScale = Mathf.Lerp(0f, 1f, _ScaleCurve.Evaluate(ratio));
            pCube.transform.localScale = Vector3.one * currentScale;
            pCube.transform.rotation = Quaternion.AngleAxis(spinAngle, Vector3.up) * _StartRot;
            pCube.transform.position = _Target.transform.position + Vector3.up * Cube.cubeSide * 0.5f;
        }
    }

    public override void OnTick(Cube pCube)
    {
        _TickCount++;
        if (_TickCount == 1)
        {
            pCube.transform.position = _Target.transform.position + Vector3.up * Cube.cubeSide * 0.5f;

            pCube.justTeleported = true;
            AudioSignals.TriggerSound(AudioClipsEnum.Teleport, pCube.transform.position);
        }
        else if (_TickCount >= 2)
        {
            pCube.transform.localScale = Vector3.one;
            pCube.transform.rotation = _StartRot;

            pCube.transform.position = _Target.transform.position + Vector3.up * Cube.cubeSide * 0.5f;
            pCube.justTeleported = true;
            pCube.CheckCollision();
            _TickCount = 0;
        }
    }
}