using UnityEngine;

public class CollisionState : CubeState
{
    public float waitingDuration = 3f;
    private float _TickElapsed = 0;
    private Vector3 _StartPosition;

    public override void SetMode(Cube pCube)
    {
        _TickElapsed = 0;
        _StartPosition = pCube.transform.position;
        AudioSignals.TriggerSound(AudioClipsEnum.CubeHitWall, pCube.transform.position);
    }

    public override void DoActionMode(Cube pCube)
    {
        // L'animation se joue pendant le tout premier Tick (le choc)
        if (_TickElapsed == 1)
        {
            float animValue = pCube.blockedMoveCurve.Evaluate(pCube.ratio);
            Vector3 offset = pCube.movementDirection * (animValue * Cube.cubeSide);
            pCube.transform.position = _StartPosition + offset;
        }
        else
        {
            pCube.transform.position = _StartPosition;
        }
    }

    public override void OnTick(Cube pCube)
    {
        _TickElapsed += 1;

        // --- CORRECTION : Utilisation de >= pour ne pas attendre un tour de trop ---
        if (_TickElapsed >= waitingDuration)
        {
            Vector3 lRight = Quaternion.AngleAxis(90, Vector3.up) * pCube.movementDirection;

            if (Physics.Raycast(pCube.transform.position, lRight, out RaycastHit lHit, pCube.raycastDistance)
                && lHit.collider.gameObject.CompareTag(pCube.groundTag))
            {
                pCube.movementDirection = -pCube.movementDirection;
            }
            else
            {
                pCube.movementDirection = lRight;
            }

            pCube.SetMode(pCube.moveState);
            _TickElapsed = 0;
            pCube.justTeleported = false;
        }
    }
}