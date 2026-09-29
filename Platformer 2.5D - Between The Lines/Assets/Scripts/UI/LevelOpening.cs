using FMOD.Studio;
using FMODUnity;
using Platformer.Managers;
using UnityEngine;

//Author : Noé SALES
namespace Platformer
{
    public class LevelOpening : MonoBehaviour
    {
        private bool _Opening = false;

        private float _ElpasedTime = 0f;

        [SerializeField] private float _Duration = 1f;
        [SerializeField] private RectTransform[] _Images;

        private Vector2 _StartSize;
        private Vector2 _EndSize;

        private GameManager _GameManager;

        private string musicLvl1Path = "event:/Music/MUS_lvl1";
        public static EventInstance musicInstance1;
        public static EventInstance musicInstance2;
        private string musicLvl2Path = "event:/Music/MUS_lvl2";
        private static bool music1Playing = false;
        private static bool music2Playing = false;

        private string ambLvl1Path = "event:/AMB/AMB_Lvl1";
        public static EventInstance ambInstance1;
        private static bool ambLvl1Playing = false;
        private string ambLvl2Path = "event:/AMB/AMB_Lvl2";
        public static EventInstance ambInstance2;
        private static bool ambLvl2Playing = false;

        private void Awake()
        {
            foreach (RectTransform lImage in _Images)
            {
                lImage.gameObject.SetActive(true);
            }
            _GameManager = GameManager.Instance;
        }

        public void OpenTransitionScreen()
        {
            _StartSize = _Images[0].sizeDelta;
            _EndSize = new Vector2(_Images[0].sizeDelta.x, 0);

            _Opening = true;
            _GameManager.SetTransitionScreenState(false);
        }

        void Update()
        {
            if (!_Opening) return;

            _ElpasedTime += Time.deltaTime;

            if (_ElpasedTime < _Duration)
            {
                foreach (RectTransform lImage in _Images)
                {
                    lImage.sizeDelta = Vector2.Lerp(_StartSize, _EndSize, _ElpasedTime / _Duration);
                }

                return;
            }

            PlayLevelMusic();

            _Opening = false;
            foreach (RectTransform lImage in _Images) lImage.sizeDelta = _EndSize;
            Destroy(gameObject);
        }

        private void PlayLevelMusic()
        {
            if (_GameManager.GetCurrentData().currentLevel == 0 && !music1Playing && !ambLvl1Playing)
            {
                musicInstance1 = RuntimeManager.CreateInstance(musicLvl1Path);
                musicInstance1.start();
                ambInstance1 = RuntimeManager.CreateInstance(ambLvl1Path);
                ambInstance1.start();
                music1Playing = true;
                ambLvl1Playing = true;
            }
            else if (_GameManager.GetCurrentData().currentLevel == 1 && !music2Playing)
            {
                musicInstance2 = RuntimeManager.CreateInstance(musicLvl2Path);
                musicInstance2.start();
                ambInstance2 = RuntimeManager.CreateInstance(ambLvl2Path);
                ambInstance2.start();
                music2Playing = true;
                ambLvl2Playing = true;
            }

        }
    }
}

