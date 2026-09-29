using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    [RequireComponent(typeof(InfluenceCharacter))]
    public class AIAgent : Character
    {
        override protected void Start()
        {
            base.Start();
            stateMachine.InitState(EState.AIMOVE);
        }
    }
}
