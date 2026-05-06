using System;
using Tooling;
using Unity.VisualScripting;
using UnityEngine;

namespace Platformer.Player
{
    /// <summary>
    /// TestScript, <c>DO NOT USE</c>
    /// </summary>
    [Obsolete]
    public class DebugPlayer : MonoBehaviour
    {
        private CustomBody2D _Body2D;
        private Action DoAction;

        [field: SerializeField] public Vector2 _JumpVelocity = 50f * Vector2.up;

        [SerializeField] InputStick _InputStick;
        public float velocity = 5f;
        public float fallVelocity = 5f;


        private void Awake()
        {
            _Body2D = GetComponent<CustomBody2D>();
        }

        void Start()
        {
            Language.CurrentLanguage = 1;

            //_Body2D.WhileOnWall = () => { DoAction = WallJump; };
            _Body2D.OnGround = () => { Debug.Log("OnGround"); };
            _Body2D.OnCeiling = () => { Debug.Log("onCeiling"); };//
            _Body2D.OnGroundExited = () => { Debug.Log("Onfall"); };//
            //_Body2D.OnFall = () => { DoAction = null; Debug.Log("OnFall"); };
            //_Body2D.WhileFalling = () => { Debug.Log("Falling"); };
        }


        private void Update()
        {
            _Body2D.MoveAndSlide(Vector3.down * fallVelocity * Time.deltaTime);
            Move();
        }

        private void Move()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _Body2D.MoveAndSlide(Vector3.right * velocity * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                _Body2D.MoveAndSlide(Vector3.left * velocity * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Space) && _Body2D.isOnGround)
            {
                _Body2D.MoveAndSlide(_JumpVelocity);
            }
        }
    }
}