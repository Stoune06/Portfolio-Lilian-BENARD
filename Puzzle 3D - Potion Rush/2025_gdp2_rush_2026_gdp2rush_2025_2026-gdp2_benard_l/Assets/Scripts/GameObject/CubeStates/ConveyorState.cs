using UnityEngine;

public class ConveyorState : CubeState
{
    private Vector3 _FromPos, _ToPos;
    private Vector3 _Direction;

    private int _TickElapsed = 0;
    private int _TickToWait  = 2;
    public ConveyorState(Vector3 pDirection)
    {
        _Direction = pDirection;
    }

    public override void SetMode(Cube pCube)
    {
        if(pCube.CheckWall(_Direction)) pCube.SetMode(pCube._VoidState);
        _FromPos = pCube.transform.position;
        _ToPos = pCube.transform.position + _Direction * Cube.cubeSide;
    }

    public override void DoActionMode(Cube pCube)
    {
        if(_TickElapsed == 1)pCube.transform.position = Vector3.Lerp(_FromPos, _ToPos,pCube.ratio);
    }

    public override void OnTick(Cube pCube)
    {
        _TickElapsed += 1;
        if( _TickElapsed > _TickToWait)
        {
            pCube.CheckCollision();
            _TickElapsed = 0;
        }
    }
}
