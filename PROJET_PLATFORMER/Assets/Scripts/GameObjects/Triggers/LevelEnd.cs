using UnityEngine;

//Author : Noé SALES
using Platformer.Managers;

namespace Platformer.Areas
{
    public class LevelEnd : Area_System
    {
        protected override void OnCollision()
        {
            GameManager.Instance.StartLevelTransition();
            m_Collider.enabled = false;
        }
    }
}

