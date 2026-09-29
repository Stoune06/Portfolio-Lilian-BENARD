using UnityEngine;

public class PotionFillController : MonoBehaviour
{
    [Header("Cible Visuelle")]
    [Tooltip("Le Renderer de la fiole")]
    [SerializeField] private Renderer _PotionRenderer;

    [Tooltip("Si votre objet a plusieurs matériaux (ex: 0=Verre, 1=Liquide), indiquez l'index du liquide ici.")]
    [SerializeField] private int _MaterialIndex = 0; 
    
    [Tooltip("Nom de la propriété dans le Shader Graph (ex: _FillAmount)")]
    [SerializeField] private string _FillPropertyName = "_FillAmount";

    [Header("Réglages")]
    [SerializeField] private float _Smoothness = 2f;

    private int _FillPropertyID;
    private float _ValueEmpty = 10f;
    private float _ValueFull = -10f;

    private float _TargetFill;
    private float _CurrentFill;
    private Material _LiquidMaterialInstance;

    private int _TotalCubesInLevel = 0;
    private int _CubesReachedCount = 0;

    private void Awake()
    {
        _FillPropertyID = Shader.PropertyToID(_FillPropertyName);
        
        if (_PotionRenderer == null) _PotionRenderer = GetComponent<Renderer>();

        if (_PotionRenderer != null)
        {
            Material[] mats = _PotionRenderer.materials;
            if (_MaterialIndex >= 0 && _MaterialIndex < mats.Length)
            {
                _LiquidMaterialInstance = mats[_MaterialIndex];
            }
            else
            {
                _LiquidMaterialInstance = mats[0];
            }
        }
    }

    private void Start()
    {
        RecalculateObjectives();

        _CurrentFill = _ValueEmpty;
        UpdateMaterial(_CurrentFill);

        Target.onTargetReached += OnOneCubeArrived;
        LevelManager.onCurrentLevelChanged += OnLevelChanged;

        InputManager.onResetLevel += RecalculateObjectives;
        InputManager.onResetLevel += () => StartCoroutine(RecalculateDelayed());
        if (LevelManager.currentLevel != null)
        {
            UpdatePotionColors(LevelManager.currentLevel);
        }
    }

    private void OnLevelChanged(LevelData level)
    {
        RecalculateObjectives();
        StartCoroutine(RecalculateDelayed());
        UpdatePotionColors(level);
    }

    private void OnDestroy()
    {
        Target.onTargetReached -= OnOneCubeArrived;
        InputManager.onResetLevel -= RecalculateObjectives;
        LevelManager.onCurrentLevelChanged -= OnLevelChanged;
    }

    private void UpdatePotionColors(LevelData level)
    {
        CubeLiquidVisual visual = GetComponent<CubeLiquidVisual>();

        if (visual != null)
        {
            visual.SetLiquidGradient(level.PotionColorTop, level.PotionColorBottom);
        }
    }

    private void Update()
    {
        if (Mathf.Abs(_CurrentFill - _TargetFill) > 0.01f)
        {
            _CurrentFill = Mathf.Lerp(_CurrentFill, _TargetFill, Time.deltaTime * _Smoothness);
            UpdateMaterial(_CurrentFill);
        }
    }

    private System.Collections.IEnumerator RecalculateDelayed()
    {
        // Attend une frame : Unity aura le temps de supprimer les vieux objets
        yield return null;

        RecalculateObjectives();
    }

    private void UpdateMaterial(float amount)
    {
        if (_LiquidMaterialInstance != null)
        {
            _LiquidMaterialInstance.SetFloat(_FillPropertyID, amount);
        }
    }

    private void RecalculateObjectives()
    {
        _CubesReachedCount = 0;
        _TotalCubesInLevel = 0;
        
        _TargetFill = _ValueEmpty; 

        SpawnPoint[] spawners = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            _TotalCubesInLevel += spawner.GetTotalCubesToSpawn();
        }
        
        if (_TotalCubesInLevel == 0) _TotalCubesInLevel = 1; 
    }

    private void OnOneCubeArrived()
    {
        _CubesReachedCount++;

        float ratio = (float)_CubesReachedCount / _TotalCubesInLevel;
        ratio = Mathf.Clamp01(ratio);
        _TargetFill = Mathf.Lerp(_ValueEmpty, _ValueFull, ratio);
    }
}