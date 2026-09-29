using Platformer.Player;
using Player;
using TMPro;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Areas
{
    public class FTUETrigger : Area_System
    {
        [SerializeField] private float _Amplitude = 0.5f;
        [SerializeField] private float _Speed = 2f;
        [SerializeField] private Vector3 _Direction = Vector3.zero;
        [SerializeField] private int _FTUEScreenIndex = 1;
        [SerializeField] private bool _PressedFTUE = false;

        [SerializeField] private GameObject _FTUEScreen;
        [SerializeField] private Transform _FingerToMove;
        [SerializeField] private Transform _CircleToScale;

        private bool _IsActive = false;
        private Vector3 _StartPosition;
        private Vector3 _StartScale;
        

        protected override void OnCollision()
        {
            Debug.Log("FTUE COLLID");
            m_Collider.enabled = false;
            EnableFTUEScreen();
        }

        private void EnableFTUEScreen()
        {
            if (_FingerToMove)
            {
                _StartPosition = _FingerToMove.position;
                _IsActive = true;
            }
            if (_CircleToScale)
            {
                _StartScale = _CircleToScale.localScale;
                _IsActive = true;
            }
            _FTUEScreen.SetActive(true);
        }

        private void Update()
        {
            if(!_IsActive) return;

            float lOffset = Mathf.Sin(Time.time * _Speed) * _Amplitude;

            if (_PressedFTUE) _CircleToScale.localScale = _StartScale + _Direction * lOffset;
            else _FingerToMove.position = _StartPosition + _Direction * lOffset;
        }
    }
}

