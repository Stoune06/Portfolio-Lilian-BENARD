using Platformer.Managers;
using UnityEngine;

//Author : Noé SALES
namespace Platformer
{
    public class CinematicRelay : MonoBehaviour
    {
        public void OnLoadNextLevel()
        {
            if(GameManager.Instance != null) GameManager.Instance.EndLevelTransition();
            if(LoadManager.Instance != null) LoadManager.Instance.LoadNextLevel();
        }
    }
}

