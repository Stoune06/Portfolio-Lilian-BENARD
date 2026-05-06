using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;

public class TilesPlacer : MonoBehaviour
{
    [SerializeField]
    private string _GroundTag = "Ground";

    [SerializeField]
    private string _ActionTag = "Action";

    [Header("Effects")]
    [SerializeField]
    private ParticleSystem _DustEffectPrefab;
    [SerializeField]
    private float _DustSpawnHeightOffset = 0.1f;

    private RaycastHit _CubeHit;
    private Vector3 _Position;
    private Vector3 _PreviousPosition;

    private static Tile _SelectedTile;
    private static Tile _TileToRemove;

    private bool _CanPlace;
    private bool _CanRemove;
    private static bool _IsReady = false;

    public static bool IsDragging = false;

    private static List<int> _NTilePlaced = new List<int>();

    private static List<Tile> _PlacedTiles = new List<Tile>();

    private static int _CurrentIndex = 0;

    public static event Action<List<int>> onTilePlaced;

    private const float RAYCAST_DISTANCE = 0.9f;
    private const float RAYCAST_OFFSET = 0.15f;
    private const int LAYER_IGNORE_RAYCAST = 2;
    private const int LAYER_DEFAULT = 0;
    private const float CUBE_HEIGHT_MULTIPLIER = 0.5f;

    public static TilesPlacer instance {  get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static void Init()
    {
        if (TilesInventoryManager.availableTiles == null) return;
        
        foreach (InventoryEntry lTile in TilesInventoryManager.availableTiles)
        {
            _NTilePlaced.Add(0);
        }
        _CurrentIndex = 0;
        _SelectedTile = Instantiate(TilesData.GetPrefabToLoad(TilesInventoryManager.availableTiles[_CurrentIndex]), Vector3.zero ,TilesData.GetDirection(TilesInventoryManager.availableTiles[_CurrentIndex]));
        _SelectedTile.inventoryIndex = _CurrentIndex;
        instance.AttachVisuals(_SelectedTile.gameObject);
        _IsReady = true;

        LayerUtils.SetLayerRecursively(_SelectedTile.gameObject, LAYER_IGNORE_RAYCAST);
        onTilePlaced?.Invoke(_NTilePlaced);
    }

    private void Start()
    {
        InputManager.onTilePlace += HandleLeftClick;
        ReflectionPanel.onTileButtonPressed += ChangeSelectedTile;
        DragService.onDragStarted += OnDragStarted;
        DragService.onDragEnded += OnDragEnded;
    }

    private void Update()
    {
        if (!_IsReady || GameStateManager.isActionPhase)
        {
            if(_SelectedTile) _SelectedTile.gameObject.SetActive(false);
        } 
        else TileRaycast();
    }

    private void TileRaycast()
    {
        Ray lRay = GameManager.instance.gameCamera.ScreenPointToRay(Input.mousePosition);
        int lLayerMask = ~(1 << 2);

        if (Physics.Raycast(lRay, out _CubeHit, 1000000, lLayerMask))
        {
            GameObject lHitObject = _CubeHit.collider.gameObject;

            if (lHitObject.CompareTag(_GroundTag))
            {
                HandleGroundPlacement(lHitObject);
            }
            else if (lHitObject.CompareTag(_ActionTag))
            {
                HandleExistingTile(lHitObject);
            }
            else
            {
                HandleVoidOrObstacle(lRay);
            }
        }
        else
        {
            HandleVoidOrObstacle(lRay);
        }

        CheckQuantityLimit();
    }

    private void HandleGroundPlacement(GameObject pGround)
    {
        _Position = pGround.transform.position + Vector3.up * Cube.cubeSide * CUBE_HEIGHT_MULTIPLIER;

        if (CheckCollision(pGround.transform.position))
        {
            _SelectedTile.gameObject.SetActive(false);
            _CanPlace = false;
        }
        else
        {
            if (!_SelectedTile.gameObject.activeSelf) _SelectedTile.gameObject.SetActive(true);

            if (_Position != _PreviousPosition)
            {
                _SelectedTile.transform.position = _Position;
                _PreviousPosition = _SelectedTile.transform.position;
            }
            _CanPlace = true;
            _CanRemove = false;
        }
    }

    private void HandleExistingTile(GameObject pTileObject)
    {
        _TileToRemove = pTileObject.GetComponent<Tile>();

        if (_PlacedTiles.Contains(_TileToRemove))
        {
            _SelectedTile.gameObject.SetActive(false);
            _CanRemove = true;
            _CanPlace = false;
        }
        else
        {
            _CanRemove = false;
            _CanPlace = false;
            _SelectedTile.gameObject.SetActive(false);
        }
    }

    private void HandleVoidOrObstacle(Ray pRay)
    {
        if (IsDragging)
        {
            FollowMouseInVoid(pRay);
        }
        else
        {
            _SelectedTile.gameObject.SetActive(false);
        }

        _CanPlace = false;
        _CanRemove = false;
    }

    private void HandleLeftClick()
    {
        if (_CanPlace)
        {
            PlaceTile();
        }
        else if (_CanRemove)
        {
            RemoveTile();
        }
    }

    private void CheckQuantityLimit()
    {
        if (TilesInventoryManager.availableTiles[_CurrentIndex].quantity <= _NTilePlaced[_CurrentIndex])
        {
            _SelectedTile.gameObject.SetActive(false);
            _CanPlace = false;
        }
    }


    private void RemoveTile()
    {
        if (!_CanRemove || _TileToRemove == null) return;

        _PlacedTiles.Remove(_TileToRemove);
        _NTilePlaced[_TileToRemove.inventoryIndex] -= 1;

        onTilePlaced?.Invoke(_NTilePlaced);

        TileDissolve dissolveEffect = _TileToRemove.GetComponent<TileDissolve>();
        if (dissolveEffect == null) dissolveEffect = _TileToRemove.gameObject.AddComponent<TileDissolve>();

        dissolveEffect.StartDissolve(() =>
        {
        });
    }

    private void ChangeSelectedTile(int pIndex)
    {
        if (_SelectedTile != null)
        {
            Destroy(_SelectedTile.gameObject);
        }
        
        _CurrentIndex = pIndex;
        _SelectedTile = Instantiate(TilesData.GetPrefabToLoad(TilesInventoryManager.availableTiles[_CurrentIndex]), _Position, TilesData.GetDirection(TilesInventoryManager.availableTiles[_CurrentIndex]));
        _SelectedTile.inventoryIndex = _CurrentIndex;
        LayerUtils.SetLayerRecursively(_SelectedTile.gameObject, LAYER_IGNORE_RAYCAST);
        AttachVisuals(_SelectedTile.gameObject);
    }

    private void PlaceTile()
    {
        if (!_CanPlace || !CanPlaceTile(_CurrentIndex))
        {
            return;
        }

        _NTilePlaced[_CurrentIndex] += 1;
        _PlacedTiles.Add(_SelectedTile);
        LayerUtils.SetLayerRecursively(_SelectedTile.gameObject, LAYER_DEFAULT);
        TileGhostVisuals lVisuals = _SelectedTile.GetComponent<TileGhostVisuals>();
        lVisuals.TriggerPlacementAnimation();

        if (TilesInventoryManager.availableTiles[_CurrentIndex].quantity <= _NTilePlaced[_CurrentIndex])
        {
            _CurrentIndex = (_CurrentIndex < TilesInventoryManager.availableTiles.Count - 1) ? _CurrentIndex + 1 : 0;
        }

        CreateNewSelectedTile();
        onTilePlaced?.Invoke(_NTilePlaced);
        SpawnDustEffect(_Position);
    }

    private bool CanPlaceTile(int pIndex)
    {
        return TilesInventoryManager.availableTiles[pIndex].quantity > _NTilePlaced[pIndex];
    }

    private void CreateNewSelectedTile()
    {
        _SelectedTile = Instantiate(TilesData.GetPrefabToLoad(TilesInventoryManager.availableTiles[_CurrentIndex]), _Position, TilesData.GetDirection(TilesInventoryManager.availableTiles[_CurrentIndex]));
        _SelectedTile.inventoryIndex = _CurrentIndex;
        LayerUtils.SetLayerRecursively(_SelectedTile.gameObject, LAYER_IGNORE_RAYCAST);
        AttachVisuals(_SelectedTile.gameObject);
    }

    private bool CheckCollision(Vector3 pPosition)
    {
        _SelectedTile.gameObject.SetActive(false);
        Vector3 lOrigin = pPosition - Vector3.up * Cube.cubeSide * CUBE_HEIGHT_MULTIPLIER + Vector3.up * RAYCAST_OFFSET;
        return Physics.Raycast(lOrigin, Vector3.up, out _CubeHit, RAYCAST_DISTANCE);
    }

    public static void SetPauseMod()
    {
        _IsReady = false;
    }

    public static void SetResumeMod()
    {
        _IsReady = true;
    }

    public static void ClearAllTiles()
    {
        for(int i = _PlacedTiles.Count - 1; i >= 0; i--)
        {
            Destroy(_PlacedTiles[i].gameObject);
            _PlacedTiles.RemoveAt(i);
        }
        _NTilePlaced.Clear();
        _IsReady = false;
    }

    public void StartDrag(int pIndex)
    {
        IsDragging = true;
        ChangeSelectedTile(pIndex);
        if (_SelectedTile != null)
        {
            _SelectedTile.gameObject.SetActive(true);
            TileRaycast();
        }
    }

    public void EndDrag()
    {
        IsDragging = false;
        PlaceTile();
    }


    private void OnDragStarted(int pIndex)
    {
        GameStateManager.canRotateCamera = false;
        StartDrag(pIndex);
    }

    private void OnDragEnded()
    {
        GameStateManager.canRotateCamera = true;
        EndDrag();
    }

    private void SpawnDustEffect(Vector3 pPos)
    {
        Instantiate(_DustEffectPrefab,pPos,Quaternion.identity);
    }

    private void FollowMouseInVoid(Ray pRay)
    {
        float lHeight = Cube.cubeSide * CUBE_HEIGHT_MULTIPLIER;
        Plane lPlane = new Plane(Vector3.up, new Vector3(0, lHeight, 0));

        if (lPlane.Raycast(pRay, out float lDistance))
        {
            Vector3 lTargetPos = pRay.GetPoint(lDistance);
            _SelectedTile.transform.position = lTargetPos;
            if (!_SelectedTile.gameObject.activeSelf) _SelectedTile.gameObject.SetActive(true);
        }
    }

    private void AttachVisuals(GameObject pTileObj)
    {
        // On v�rifie si le script existe d�j�, sinon on l'ajoute
        if (pTileObj.GetComponent<TileGhostVisuals>() == null)
        {
            pTileObj.AddComponent<TileGhostVisuals>();
        }
    }


}
