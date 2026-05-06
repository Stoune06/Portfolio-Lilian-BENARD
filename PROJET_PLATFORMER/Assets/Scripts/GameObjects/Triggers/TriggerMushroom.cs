using UnityEngine;
using Platformer.Tools;
using Tooling;

namespace Platformer.Areas
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class TriggerMushroom : MonoBehaviour
    {
        public const string IDLE_BIRD = "Idle_Bird";

        public const string TAKE_OFF_BIRD = "Glow_Mushroom";

        private Animator _Animation;
        private SpriteRenderer _SpriteRenderer;

        [Header("Trigger")]
        [SerializeField] private LayerMask _Layers;
        [SerializeField] private float _RadiusTakeOff = 2f;
        private bool _WaitTrigger = true;

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

        private void CircleCollision()
        {
            if (_WaitTrigger)
            {
                Collider2D lCollider = Physics2D.OverlapCircle(transform.position, _RadiusTakeOff, _Layers);
                if (lCollider == null) return;

                if (Vector3.Distance(lCollider.transform.position, transform.position) <= _RadiusTakeOff)
                {
                    _Animation.Play(TAKE_OFF_BIRD);
                    _WaitTrigger = false;

                    //TODO MS MUSHROOM GLOW SOUND
                }
            }
        }
    }
}