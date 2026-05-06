using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    
    public abstract class BaseState
    {
        protected readonly Character m_Character;
        protected readonly Animator m_Animator;

        protected BaseState(Character pCharacter, Animator pAnimator)
        {
            m_Character = pCharacter;
            m_Animator = pAnimator;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}