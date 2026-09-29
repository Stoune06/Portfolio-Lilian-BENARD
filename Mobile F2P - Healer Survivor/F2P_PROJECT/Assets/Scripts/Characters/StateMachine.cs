using Com.IsartDigital.HealerSurvivor.Enum;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay.Enemies;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    
    public class StateMachine
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private BaseState _CurrentState;
        private readonly Dictionary<EState, BaseState> _States;
        

        public StateMachine(Character pCharacter, Animator pAnim)
        {
            _States = new Dictionary<EState, BaseState>()
            {
                { EState.IDLE, new IdleState(pCharacter, pAnim)},
                { EState.MOVE, new MoveState(pCharacter, pAnim)},
                { EState.ATTACK, new AttackState(pCharacter, pAnim)}, 
                {EState.AIMOVE, new AiMoveState(pCharacter, pAnim)},
            };
        }

        public void InitState(EState pStartingState)
        {
            _CurrentState = _States[pStartingState];
            _CurrentState.OnEnter();
        }

        public void ChangeState(EState pNewState)
        {
            if (_CurrentState == _States[pNewState]) 
                return;
            _CurrentState.OnExit();

            _CurrentState = _States[pNewState];
            _CurrentState.OnEnter();
        }

        public bool CheckCurrentState(EState pStateToCheck) => _CurrentState == _States[pStateToCheck];
        
        public void UpdateLogic() => _CurrentState?.Update();
        
        public void FixedUpdateLogic() => _CurrentState?.FixedUpdate();
        
    }
}