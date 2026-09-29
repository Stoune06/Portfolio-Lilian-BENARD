using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] 
    private StatesEnum _State = StatesEnum.Void;

    public int inventoryIndex = 0;

    [NonSerialized]
    public CubeState stateToGive;
    protected virtual void Start()
    {
        switch(_State)
        {
            case StatesEnum.Void:
                {
                    stateToGive = new VoidState();
                    return;
                }
            case StatesEnum.Fall:
                {
                    stateToGive = new FallState();
                    return;
                }
            case StatesEnum.Turn:
                {
                    stateToGive = new TurnState(transform.forward, transform.position);
                    return;
                }
            case StatesEnum.Stop:
                {
                    stateToGive = new StopState();
                    return;
                }
            case StatesEnum.Conveyor:
                {
                    stateToGive = new ConveyorState(transform.forward);
                    return;
                }
            case StatesEnum.Switch:
                {
                    return;
                }
        }
    }
}
