using UnityEngine;
using Platformer.Tools;
using Tooling;

namespace Platformer.Areas
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class TriggerBird : MonoBehaviour
    {
        public const string IDLE_BIRD = "Idle_Bird";

        public const string TAKE_OFF_BIRD = "TakeOff_Bird";

        private Animator _Animation;
        private SpriteRenderer _SpriteRenderer;

        [Header("Trigger")]
        [SerializeField] private LayerMask _Layers;
        [SerializeField] private float _RadiusTakeOff = 2f;
        [SerializeField] private Vector3 _JumpDistance = new Vector3(6f, 0f);

        [SerializeField] private TweenProperty _FadeOutProperty;

        private void Awake()
        {
            _Animation = GetComponent<Animator>();
            _SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            CircleCollision();
            transform.position = new Vector3(transform.position.x, transform.position.y, Player.Player.Instance.transform.position.z);
        }

        private bool CircleCollision()
        {
            Collider2D lCollider = Physics2D.OverlapCircle(transform.position, _RadiusTakeOff, _Layers);
            if (lCollider == null) return false;

            if (Vector3.Distance(lCollider.transform.position, transform.position) <= _RadiusTakeOff)
            {
                _Animation.Play(TAKE_OFF_BIRD);

                transform.MoveTo((transform.position + _JumpDistance * transform.parent.transform.localScale.x) * transform.localScale.x, _FadeOutProperty).finished += () => Destroy(gameObject);
                Tween lTween = new Tween(_FadeOutProperty);
                lTween.Start(_SpriteRenderer, nameof(SpriteRenderer.color), new Color(1f, 1f, 1f, 0f));
            }
            return true;
        }
    }
}