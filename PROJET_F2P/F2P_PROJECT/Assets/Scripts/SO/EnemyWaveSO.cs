using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.SO
{
    [CreateAssetMenu(fileName = Utils.WAVE_FILE_NAME, menuName = Utils.ENEMY_WAVE_MENU_NAME)]
    public class EnemyWaveSO : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private int _NumberOfEnemyToSpawn;
        [SerializeField] private float _SpawnInterval = .5f;
        [SerializeField] private int _EnemiesPerSubWave = 5;
        [SerializeField] private float _SubWaveInterval = 3f;
        [SerializeField] private EType _TypeOfEnemy;

        [SerializeField] private EPosition _StartPosition;
        [SerializeField] private EPosition _EndPosition;

        [SerializeField, HideInInspector] private Sprite _WaveIllustration;

        public int NumberOfEnemyToSpawn => _NumberOfEnemyToSpawn;
        public float SpawnInterval => _SpawnInterval;
        public int EnemiesPerSubWave => _EnemiesPerSubWave;
        public float SubWaveInterval => _SubWaveInterval;
        public EType TypeOfEnemy => _TypeOfEnemy;
        public EPosition StartPosition => _StartPosition;
        public EPosition EndPosition => _EndPosition;
        public Sprite WaveIllustration => _WaveIllustration;
    }
}