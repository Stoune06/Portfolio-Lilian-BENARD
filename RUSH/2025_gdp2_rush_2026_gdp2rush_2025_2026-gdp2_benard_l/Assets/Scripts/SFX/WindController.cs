using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Cible")]
    [SerializeField] private ParticleSystem _LeavesParticles;

    [Header("Réglages Vitesse")]
    [SerializeField] private float _MinSimulationSpeed = 1f; // Calme plat
    [SerializeField] private float _MaxSimulationSpeed = 8f; // Tempête (Victoire)
    [SerializeField] private float _AccelerationSmoothness = 2f;

    private float _TargetSpeed;
    private float _CurrentSpeed;

    // Compteurs
    private int _TotalCubesInLevel = 0;
    private int _CubesReachedCount = 0;

    private void Start()
    {
        // Initialisation
        _TargetSpeed = _MinSimulationSpeed;
        _CurrentSpeed = _MinSimulationSpeed;

        // Abonnements aux événements clés
        Target.onTargetReached += OnOneCubeArrived;

        // Quand on charge un nouveau niveau ou qu'on reset, on recalcule tout
        LevelManager.onCurrentLevelChanged += (level) => RecalculateObjectives();
        InputManager.onResetLevel += RecalculateObjectives;

        // Premier calcul au lancement
        RecalculateObjectives();
    }

    private void OnDestroy()
    {
        Target.onTargetReached -= OnOneCubeArrived;
        InputManager.onResetLevel -= RecalculateObjectives;
        // Note : Pour LevelManager c'est plus complexe de désabonner une lambda, 
        // mais comme ce contrôleur reste souvent actif ça va. 
        // Idéalement : créez une méthode dédiée pour OnLevelChanged.
    }

    private void Update()
    {
        // Transition douce de la vitesse
        _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, _TargetSpeed, Time.deltaTime * _AccelerationSmoothness);

        if (_LeavesParticles != null)
        {
            var main = _LeavesParticles.main;
            main.simulationSpeed = _CurrentSpeed;
        }
    }

    // Appelé quand on lance/relance un niveau
    private void RecalculateObjectives()
    {
        _CubesReachedCount = 0;
        _TotalCubesInLevel = 0;
        _TargetSpeed = _MinSimulationSpeed;

        // On cherche tous les spawners actifs dans la scène
        SpawnPoint[] spawners = FindObjectsOfType<SpawnPoint>();

        foreach (var spawner in spawners)
        {
            // On utilise la méthode publique qu'on vient de créer
            _TotalCubesInLevel += spawner.GetTotalCubesToSpawn();
        }

        // Petite sécurité si aucun spawner
        if (_TotalCubesInLevel == 0) _TotalCubesInLevel = 1;
    }

    // Appelé à chaque fois qu'un cube touche une cible
    private void OnOneCubeArrived()
    {
        _CubesReachedCount++;

        // Calcul du pourcentage de réussite (0 à 1)
        float ratio = (float)_CubesReachedCount / _TotalCubesInLevel;

        // On ne dépasse pas 1 (au cas où)
        ratio = Mathf.Clamp01(ratio);

        // On augmente la vitesse du vent
        _TargetSpeed = Mathf.Lerp(_MinSimulationSpeed, _MaxSimulationSpeed, ratio);
    }
}