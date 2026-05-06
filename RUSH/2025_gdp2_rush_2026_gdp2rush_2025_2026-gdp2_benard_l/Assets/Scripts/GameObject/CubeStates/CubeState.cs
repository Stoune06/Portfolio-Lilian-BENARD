using UnityEngine;

public abstract class CubeState 
{
    public virtual void SetMode(Cube pCube) { }
    public virtual void DoActionMode(Cube pCube) { }
    public virtual void OnTick(Cube pCube) { }
}

