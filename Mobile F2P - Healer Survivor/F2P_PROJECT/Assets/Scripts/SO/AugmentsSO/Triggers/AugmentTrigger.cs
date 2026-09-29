using System;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Triggers
{
    [Serializable]
    public abstract class AugmentTrigger
    {
        [HideInInspector]
        public Transform lastTarget;
        
        protected IAugmentable m_Player;
        protected Action m_Callback;

        public float lastAmount;

        public void Init(IAugmentable pPlayer, Action pCallback)
        {
            m_Player = pPlayer;
            m_Callback = pCallback;
        }
        
        public abstract void Subscribe();
        public abstract void Unsubscribe();

    }
}
