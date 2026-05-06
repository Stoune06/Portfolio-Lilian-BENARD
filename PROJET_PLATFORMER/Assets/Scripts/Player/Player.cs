using Platformer.Managers;
using Platformer.Platforms;
using Tooling;
using UnityEngine;
using UnityEngine.VFX;

namespace Platformer.Player
{
    public class Player : Singleton<Player>
    {
        #region States Variables
        [SerializeField]
        private PlayerStats _Stats;
        public PlayerStateMachine stateMachine {  get; private set; }

        public Animator playerAnimator { get; private set; }

        public PlayerMoveState moveState { get; private set; }
        public PlayerIdleState idleState { get; private set; }
        public PlayerJumpState jumpState { get; private set; }
        public PlayerLandState landState { get; private set; }
        public PlayerFallState fallState { get; private set; }
        public PlayerOnWallState onWallState { get; private set; }
        public PlayerDeathState deathState { get; private set; }
        #endregion

        #region Components
        public CustomBody2D customBody { get; private set; }
        [HideInInspector] public BoxCollider2D boxCollider;
        public VFX_Manager VfxManager { get; private set; }

        public GameObject Outline;
        public VisualEffect[] TrailVFX;
        [Header("DeathObjects")]
        public DeathCamera DeathCamera;
        public SkinnedMeshRenderer PlayerRenderer;
        public AnimationCurve DissolveCurve;
        public VisualEffect DissolveVFX;
        public VisualEffect RespawnVFX;
        [HideInInspector] public Material DissolveMaterial;
        [Space(30)]

        #endregion

        #region Other Variables

        [SerializeField]
        public LayerMask _RaycastFootstepMask;
        public static System.Action OnPlayerDeath;
        public static System.Action<int> OnPlayerRespawn;

        public Vector2 velocity{get; private set;}
        public int facingDirection { get; private set; } = 1;
        public bool isFastFalling;
        public bool isWallJumping;
        public bool coyoteTimer;
        private float _BaseRotation;
        #endregion

        #region Unity Functions
        public override void Awake()
        {
            base.Awake();
            //_BaseRotation = _Renderer.transform.localScale.y;
            stateMachine = new PlayerStateMachine();
            customBody = GetComponent<CustomBody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            VfxManager = GetComponentInChildren<VFX_Manager>();
            DissolveMaterial = PlayerRenderer.material;
            moveState = new PlayerMoveState(this, stateMachine, _Stats, "grounded");
            idleState = new PlayerIdleState(this, stateMachine, _Stats, "grounded");
            jumpState = new PlayerJumpState(this, stateMachine, _Stats, "jump");
            landState = new PlayerLandState(this, stateMachine, _Stats, "land");
            fallState = new PlayerFallState(this, stateMachine, _Stats, "fall");
            onWallState = new PlayerOnWallState(this, stateMachine, _Stats, "onWall");
            deathState = new PlayerDeathState(this, stateMachine, _Stats, "death");
        }

        private void Start()
        {
            playerAnimator = GetComponent<Animator>();

            if(customBody.isOnGround) stateMachine.Initialize(idleState);
            else stateMachine.Initialize(fallState);

            customBody.OnCollisionEnter += DestroyPlatformDetection;
        }

        private void Update()
        {
            stateMachine.currentState.LogicUpdate();
            customBody.MoveAndSlide(velocity * Time.deltaTime);
            if (InputManager.MovementDirection.x > 0.1f || InputManager.MovementDirection.x < -0.1f 
                || stateMachine.currentState == jumpState || stateMachine.currentState == fallState) EmitTrail(true);
            else EmitTrail(false);
        }

        private void FixedUpdate()
        {
            stateMachine.currentState.PhysicsUpdate();

        }
        #endregion

        #region Set Functions
        public void SetVelocityX(float pVelocityX)
        {
            velocity = new Vector2(pVelocityX, velocity.y);
        }

        public void SetVelocityY(float pVelocityY)
        {
            velocity = new Vector2(velocity.x, pVelocityY);
        }
        public void EmitTrail(bool pBool)
        {
            foreach (VisualEffect VFX in TrailVFX) VFX.SetBool("EmitParticles", pBool);
        }
        #endregion

        #region Check Functions
        public void CheckIfShouldFlip(float pInputX)
        {
            if (pInputX == 0) return;

            if ((pInputX > 0 && facingDirection == -1) || (pInputX < 0 && facingDirection == 1))
            {
                Flip();
            }
        }
        #endregion

        #region Other Functions
        private void Flip()
        {
            facingDirection *= -1;
            DissolveMaterial.SetFloat("_FlipSign", facingDirection);
            transform.localScale = new Vector3(facingDirection,transform.localScale.y,transform.localScale.z);
        }

        public void FlipToDirection(Vector2 pDirection)
        {
            if (pDirection.x < 0)
            {
                facingDirection = -1;
            }
            else if (pDirection.x > 0)
            {
                facingDirection = 1;
            }
            transform.localScale = new Vector3(facingDirection,transform.localScale.y,transform.localScale.z);
        }
        private void DestroyPlatformDetection(Collider2D pCollider)
        {
            if (pCollider.TryGetComponent(out DestroyablePlatform pPlatform)) pPlatform.DestroyPlatform();
        }
        public void Die()
        {
            stateMachine.ChangeState(deathState);
        }
        public void PlayFootstep()
        {
            string lEventPath = "event:/SFX/MC/Movements/Footstep_Grass";

            RaycastHit2D lHit = Physics2D.Raycast(transform.position, Vector2.down, 2f, _RaycastFootstepMask);
            if (lHit.collider != null)
            {
                switch (lHit.collider.tag)
                {
                    
                     case "Grass": lEventPath = "event:/SFX/MC/Movements/Footstep_Grass"; break;
                     case "Stone": lEventPath = "event:/SFX/MC/Movements/Footstep_Stone"; break;
                     case "Paint":  lEventPath = "event:/SFX/MC/Movements/Footstep_Paint"; break;
                }
            }

            FMODUnity.RuntimeManager.PlayOneShot(lEventPath);
        }
        private void OnDestroy()
        {
            base.OnDestroy();
            InputManager.onJump -= jumpState.Enter;
            InputManager.onJump -= fallState.TestJump;
        }
        #endregion
    }
}

