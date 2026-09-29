using System;
using UnityEditor;
using UnityEngine;

public class OnTargetState : CubeState
{
    private float _TickElapsed = 0;
    private Target _Target;

    public OnTargetState(Target pTarget)
    {
        _Target = pTarget;
    }

    public override void SetMode(Cube pCube)
    {
        if( _Target.targetColor != pCube.color )
        {
            pCube.TryMove();
            return;
        }
        base.SetMode(pCube);
        CubeAnimation(pCube);
    }

    public override void OnTick(Cube pCube)
    {
        _TickElapsed++;
        base.OnTick(pCube);
        CubeAnimation(pCube);
    }

    private void CubeAnimation(Cube pCube)
    {
        //TODO
        
        if (_TickElapsed >= 1)
        {
            pCube.DestroyCube();
            _Target.TargetReached();
            _TickElapsed = 0;
            AudioSignals.TriggerSound(AudioClipsEnum.TargetReached, pCube.transform.position);
            return;
        }
        
    }
}
