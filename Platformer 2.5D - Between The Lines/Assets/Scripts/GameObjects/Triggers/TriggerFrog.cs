using UnityEngine;
using Platformer.Tools;
using Tooling;

namespace Platformer.Areas
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class TriggerFrog : MonoBehaviour
    {
        public const string IDLE_FROG_1 = "Idle_Frog_1";
        public const string IDLE_FROG_2 = "Idle_Frog_2";
        public const string IDLE_FROG_3 = "Idle_Frog_3";

        public const string UP_FROG = "Up_Frog";
        public const string JUMP_FROG = "Jump_Frog";

        private string[] _FrogAnimationArray = new string[4] { IDLE_FROG_1, IDLE_FROG_2, IDLE_FROG_3, UP_FROG};


        [Header("Animation")]
        [SerializeField, Range(2f, 15f)] private float _MinToIdle = 2f;
        [SerializeField, Range(2.1f, 15f)] private float _MaxToIdle = 2f;

        private float _TimeToIdle = 2f;
        private float _CurrentTime;

        private Animator _AnimationFrog;
        private SpriteRenderer _SpriteRenderer;
        [SerializeField, ReadOnly] private bool _IsTriggered = false, _IsForced = false;

        [Header("Trigger")]
        [SerializeField] private LayerMask _Layers;
        [SerializeField] private float _RadiusJump = 2f;
        [SerializeField] private Vector3 _JumpDistance = new Vector3(6f, 0f);

        [SerializeField] private TweenProperty _FadeOutProperty;

        private void Awake()
        {
            _AnimationFrog = GetComponent<Animator>();
            _SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _IsTriggered = CircleCollision();
            if (_IsTriggered || _IsForced) return;
            if (_CurrentTime >= _TimeToIdle)
            {
                CustomTools.Timer(_TimeToIdle, PlayRandomAnimation);
                _CurrentTime = 0f;
                SetRandomTrigger();
                SetRandomTime();
            }
            else _CurrentTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, Player.Player.Instance.transform.position.z);
        }

        private bool CircleCollision()
        {
            Collider2D lCollider = Physics2D.OverlapCircle(transform.position, _RadiusJump, _Layers);
            if (lCollider == null) return false;

            if (Vector3.Distance(lCollider.transform.position, transform.position) <= _RadiusJump)
            {
                _AnimationFrog.Play(JUMP_FROG);

                transform.MoveTo((transform.position + _JumpDistance * transform.parent.transform.localScale.x) , _FadeOutProperty).finished += () => Destroy(gameObject);
                Tween lTween = new Tween(_FadeOutProperty);
                _IsForced = true;

                lTween.Start(_SpriteRenderer, nameof(SpriteRenderer.color), new Color(1f, 1f, 1f, 0f));
            }
            return true;
        }

        private void SetRandomTime() => _TimeToIdle = Random.Range(_MinToIdle, _MaxToIdle);

        private void PlayRandomAnimation() => _AnimationFrog.Play(_FrogAnimationArray[Random.Range(0, _FrogAnimationArray.Length)]);
        private void SetRandomTrigger() => _AnimationFrog.SetTrigger(_FrogAnimationArray[Random.Range(0, _FrogAnimationArray.Length)]);
    }
}