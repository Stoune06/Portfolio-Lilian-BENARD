using UnityEngine;

//Author : Noé SALES
namespace Platformer.Areas
{
    [RequireComponent(typeof(Animator))]
    public class TriggerEvent : Area_System
    {
        [SerializeField] private string _AnimationName = null;
        [SerializeField] private bool _Loop = false;

        private bool _AlreadyActived = false;
        private Animator _Animator = null;
        private int _AnimationHash;

        private new void Start()
        {
            base.Start();
            if (TryGetComponent(out Animator lAnimator))
            {
                _Animator = lAnimator;
                _AnimationHash = Animator.StringToHash(_AnimationName);
            }
        }

        protected override void OnCollision()
        {
            if (!_Loop && _AlreadyActived) return;

            if (_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _AnimationHash) return;

            PlayEvent();
        }

        private void PlayEvent()
        {
            _AlreadyActived = true;
            _Animator.Play(_AnimationHash);
        }
    }
}