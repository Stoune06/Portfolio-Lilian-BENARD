using UnityEngine;
using UnityEngine.UI;

//Author : Noé SALES
namespace Platformer
{
    public class MainMenuButton : MonoBehaviour
    {
        public static event System.Action BackToMainMenuEvent;
        private Button _Button;

        private void Start()
        {
            _Button = GetComponent<Button>();
            _Button.onClick.AddListener(() => BackToMainMenuEvent?.Invoke());
        }
        private void OnDisable()
        {
            _Button.onClick.RemoveAllListeners();
        }
    }
}

