using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField]
    private float _RaycastOffset = 0.4f;

    [SerializeField]
    public int actionsPerTick = 1;

    private int _CurrentActionIndex = 0;

    [SerializeField]
    private GameObject _ScribbleEffect;

    [SerializeField]
    public string groundTag = "Ground";

    [SerializeField]
    private string _ActionTag = "Action";

    [SerializeField]
    private string _LevelTileTag = "LevelTile";

    [SerializeField]
    private string _CubeTag = "Cube";

    [SerializeField]
    private string _SpawnPointTag = "Spawner";

    [Header("Animation & Feeling")]
    public AnimationCurve standardMoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve startMoveCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, -0.2f), new Keyframe(1, 1));
    public AnimationCurve blockedMoveCurve = new AnimationCurve(
         new Keyframe(0, 0),
         new Keyframe(0.2f, -0.2f),   // Bascule arri�re (IDENTIQUE au startMove)
         new Keyframe(0.3f, -0.15f),  // D�but de l'�lan vers l'avant (M�me pente que startMove)
         new Keyframe(0.31f, 0),      // CLAC ! Mur. (Retour instantan� � 0)
         new Keyframe(1, 0)           // Reste coll� au mur
     );

    [HideInInspector] public bool isContinuousMove = false;


    public Color color;

    private CubeState _currentBehaviour;

    public CubeState _VoidState, _FallState;

    public MoveState moveState;

    public CollisionState collisionState;

    public Vector3 movementDirection = Vector3.zero;

    private float _CurrentGroundY;

    public static float cubeSide = 1f;

    private RaycastHit _Hit;
    public float raycastDistance;

    public bool justTeleported;

    public static event Action<Cube> onCollisionEvent;
    public static event Action<Cube> onOutOfLevel;

    private CubeLiquidVisual _Visual;

    private Collider _Collider;

    public float ratio
    {
        get
        {
            float lGlobalRatio = TickManager.GetRatioBetweenTicks();
            float lTotalProgress = lGlobalRatio * actionsPerTick;
            return Mathf.Clamp01(lTotalProgress - _CurrentActionIndex);
        }
    }

    private void Awake()
    {
        _Visual = GetComponent<CubeLiquidVisual>();
        _Collider = GetComponent<Collider>();
    }

    private void Start()
    {
        CubeManager.RegisterCube(this);
        if(movementDirection == Vector3.zero) movementDirection = transform.forward;

        collisionState = new CollisionState();
        moveState = new MoveState();
        _VoidState = new VoidState();
        _FallState = new FallState();

        SetMode(moveState);
        raycastDistance = cubeSide * 0.5f + _RaycastOffset;

        TickManager.onTick += OnTickEvent;
        SetMode(new VoidState());
    }

    private void Update()
    {
        float lProgressInTick = TickManager.GetRatioBetweenTicks() * actionsPerTick;
        if ((int)lProgressInTick > _CurrentActionIndex && _CurrentActionIndex < actionsPerTick - 1)
        {
            _CurrentActionIndex++;
            SetActionLogic();
        }
        _currentBehaviour?.DoActionMode(this);
    }

    private void OnTickEvent()
    {
        _CurrentActionIndex = 0;
        SetActionLogic();
    }

    public void SetColliderState(bool pEnabled)
    {
        if (_Collider != null)
        {
            _Collider.enabled = pEnabled;
        }
    }

    public void SetMode(CubeState pNewState)
    {
        if (pNewState is MoveState && _currentBehaviour is MoveState)
        {
            isContinuousMove = true;
        }
        else if (pNewState is MoveState)
        {
            isContinuousMove = false;
        }
        if (_currentBehaviour != pNewState) _currentBehaviour = pNewState;
        _currentBehaviour?.SetMode(this);
    }

    public void SetColor(Color pColor)
    {
        color = pColor;
        _Visual.SetLiquidColor(color);
    }

    private void SetActionLogic()
    {
        if (_currentBehaviour != _VoidState &&
            _currentBehaviour != collisionState &&
            !(_currentBehaviour is ConveyorState) &&
            !(_currentBehaviour is TeleportState))
        {
            CheckCollision();
        }
        _currentBehaviour?.OnTick(this);
    }

    public void CheckCollision()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _Hit, raycastDistance))
        {
            GameObject lCollided = _Hit.collider.gameObject;
            
            if(lCollided.CompareTag(_ActionTag) || lCollided.CompareTag(_LevelTileTag) || lCollided.CompareTag(_SpawnPointTag)) moveState.yFloorPos = lCollided.transform.position.y-0.5f;
            else moveState.yFloorPos = lCollided.transform.position.y;

            if ((lCollided.CompareTag(_ActionTag) || lCollided.CompareTag(_LevelTileTag)) && !justTeleported)
            {
                Tile lTile = lCollided.GetComponent<Tile>();
                SetMode(lTile.stateToGive);
                return;
            }
            else if (CheckWall(movementDirection))
            {
                collisionState.waitingDuration = 3;
                SetMode(collisionState);
            }
            else if (lCollided.CompareTag(groundTag) || justTeleported || lCollided.CompareTag(_SpawnPointTag))
            {
                if (justTeleported) moveState.yFloorPos = lCollided.transform.position.y - 0.5f * cubeSide;
                SetMode(moveState);
                justTeleported = false;
            }
            
        }
        else
        {
            SetMode(_FallState);
        }
    }

    public bool CheckWall(Vector3 pDirection)
    {
        if (Physics.Raycast(transform.position, pDirection, out _Hit, raycastDistance))
        {
            GameObject lCollided = _Hit.collider.gameObject;
            return lCollided.CompareTag(groundTag);
        }
        return false;
    }

    public void TryMove(int pWaitingDuration = 3)
    {
        if (CheckWall(movementDirection))
        {
            collisionState.waitingDuration = pWaitingDuration;
            SetMode(collisionState);
        }
        else
        {
            SetMode(moveState);
        }
    }

        public void OutOfLevelTrigger()
    {
        onOutOfLevel.Invoke(this);
        SetMode(_VoidState);
        _ScribbleEffect.SetActive(true);
        Debug.Log("out of level");
    }

    private void OnTriggerEnter(Collider pOther)
    {
        if(pOther.CompareTag(_CubeTag))
        {
            AudioSignals.TriggerSound(AudioClipsEnum.CubeHitOtherCube, transform.position);
            onCollisionEvent.Invoke(this);
            SetMode(_VoidState);
            _ScribbleEffect.SetActive(true);
            Debug.Log("collision");
        }
    }

    public void DestroyCube()
    {
        CubeManager.UnregisterCube(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        TickManager.onTick -= OnTickEvent;
        Destroy(this);
    }
}
