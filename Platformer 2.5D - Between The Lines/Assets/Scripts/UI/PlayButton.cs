using UnityEngine;
using UnityEngine.UI;

//Author : Noé SALES
namespace Platformer
{
    public class PlayButton : MonoBehaviour
    {
        public static event System.Action OnPlay;
        private Button _Button;

        private void OnEnable()
        {
            _Button = GetComponent<Button>();
            _Button.onClick.AddListener(() => OnPlay?.Invoke());
        }
        
        private void OnDisable()
        {
            _Button.onClick.RemoveAllListeners();
        }
    }
}

