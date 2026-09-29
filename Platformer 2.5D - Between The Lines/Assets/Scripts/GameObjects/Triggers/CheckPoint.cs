using Platformer.Managers;
using Platformer.Utils;
using UnityEngine;

//Author : Sales Noé
namespace Platformer.Areas
{
    public class CheckPoint : Area_System
    {
        public static event System.Action CheckPointGetEvent;

        [SerializeField] private Animator _Animator;
        [SerializeField] private GameObject _VFX;
        [SerializeField] private float _VFXOffset;
        [SerializeField] private bool _RecallPlatformsOnCheck = true;

        private DataManager _DataManager;
        private GameManager _GameManager;

        private int _Index = 0;
        private bool _AlreadyCheck = false;

        private new void Start()
        {
            _DataManager = DataManager.Instance;
            _GameManager = GameManager.Instance;
        }

        protected override void OnCollision()
        {
            if (_AlreadyCheck) { return; }

            if(_RecallPlatformsOnCheck) CheckPointGetEvent?.Invoke();
            SaveGameState();
            
            ActiveCheckPoint();
        }

        private async void SaveGameState()
        {
            if (_GameManager == null) return;

            SaveData lCurrentSave = _GameManager.GetCurrentData();
            lCurrentSave.lastCheckpoint = _Index;
            _GameManager.SetCurrentData(lCurrentSave);
            if (_DataManager != null && !GameManager.IsOffline)
            {
                await _DataManager.SaveData(lCurrentSave);
                Debug.Log("OnlineSave");
            }
        }

        public void ActiveCheckPoint()
        {
            _Animator.SetTrigger("Activation");
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/INTERRACTIONS/Checkpoint", transform.position);

            GameObject lVFX = Instantiate(_VFX);
            lVFX.transform.SetParent(transform);
            lVFX.transform.position = gameObject.transform.position + Vector3.up * _VFXOffset;
            Vibration.Vibrate(Vibration.PredefinedEffect.TICK);
            _AlreadyCheck = true;
        }

        public void SetIndex(int pIndex) => _Index = pIndex;
    }
}