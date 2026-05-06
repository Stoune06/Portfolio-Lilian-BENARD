using UnityEngine;

namespace Platformer.Player
{
    public class PlayerStateMachine
    {
        public PlayerState currentState { get; private set; }

        public void Initialize(PlayerState pStartingState)
        {
            currentState = pStartingState;
            currentState.Enter();
        }

        public void ChangeState(PlayerState pNewState)
        {
            currentState.Exit();
            currentState = pNewState;
            currentState.Enter();
            
        }
    }
}

