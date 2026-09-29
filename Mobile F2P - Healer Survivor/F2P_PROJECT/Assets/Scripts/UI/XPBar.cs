using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.HealerSurvivor.Menus
{
    public class XPBar : MonoBehaviour
    {
        [SerializeField] private Slider _Slider;
        [SerializeField] private Text _LevelText;

        private Player _Player;

        private void Start()
        {
            _Player = Player.instance;
            _Player.OnLevelUp += UpdateBar;
            UpdateBar();
        }

        private void Update()
        {
            _Slider.value = _Player.currentXP / _Player.xpToNextLevel;
        }

        private void UpdateBar()
        {
            _LevelText.text = _Player.currentLevel.ToString();
            _Slider.value = 0;
        }

        private void OnDestroy()
        {
            if (_Player != null) _Player.OnLevelUp -= UpdateBar;
        }
    }
}