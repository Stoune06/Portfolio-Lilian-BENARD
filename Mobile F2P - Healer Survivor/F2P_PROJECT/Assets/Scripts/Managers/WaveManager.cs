using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.SO;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Menus;
using Com.IsartDigital.HealerSurvivor.Other;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class WaveManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [HideInInspector] public List<Enemy> allEnemies = new List<Enemy>();
        [SerializeField] private List<WaveBuilderSO> _WaveBuilders;
        [SerializeField] private GameObject _GameContainer;
        [SerializeField] private float _RadiusSpawn = 50f;

        private float _TwoPi => Mathf.PI * 2f;

        private GameManager _GameManager => GameManager.Instance;

        private bool _IsLastWaveCalled;
        private bool _IsSpawningWave;
        
        public static WaveManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            #endregion
            
            EnemyFactory.Initialize();
        }

        private Vector3 GetSpawnPosition(EPosition pStart, EPosition pEnd)
        {
            int lCount = System.Enum.GetValues(typeof(EPosition)).Length;
            float lStep = _TwoPi / lCount;
            float lStartAngle = (int)pStart * lStep;
            float lEndAngle = (int)pEnd * lStep;

            if (lEndAngle < lStartAngle)
                lEndAngle += _TwoPi;

            float lAngle = Random.Range(lStartAngle, lEndAngle);

            return PolarToCartesian(lAngle, _RadiusSpawn);
        }

        private void SpawnWave(EnemyWaveSO pWave) => StartCoroutine(SpawnWaveCoroutine(pWave));

        // Exposed for FTUEManager to spawn a scripted tutorial wave without relying on a WaveBuilderSO asset.
        // Returns the IEnumerator directly so callers can yield on it and chain sub-waves sequentially.
        public IEnumerator SpawnCustomWaveRoutine(EType pType, int pCount, float pInterval) => SpawnCustomWaveCoroutine(pType, pCount, pInterval);

        private IEnumerator SpawnCustomWaveCoroutine(EType pType, int pCount, float pInterval)
        {
            WaitForSeconds lWait = new WaitForSeconds(pInterval);

            for (int i = 0; i < pCount; i++)
            {
                float lAngle = Random.Range(0f, _TwoPi);
                Vector3 lPos = PolarToCartesian(lAngle, _RadiusSpawn);
                EnemyFactory.SpawnEnemy(pType, lPos, _GameContainer.transform);

                yield return lWait;
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator SpawnWaveCoroutine(EnemyWaveSO pWave)
        {
            int lTotal = pWave.NumberOfEnemyToSpawn;
            // Max(1, ...) so legacy assets that haven't set EnemiesPerSubWave don't loop forever.
            int lPerBatch = Mathf.Max(1, pWave.EnemiesPerSubWave);
            WaitForSeconds lIntraBatch = new WaitForSeconds(pWave.SpawnInterval);
            WaitForSeconds lBetweenBatches = new WaitForSeconds(pWave.SubWaveInterval);

            int lSpawned = 0;
            while (lSpawned < lTotal)
            {
                int lBatchSize = Mathf.Min(lPerBatch, lTotal - lSpawned);

                for (int i = 0; i < lBatchSize; i++)
                {
                    Vector3 lPos = GetSpawnPosition(pWave.StartPosition, pWave.EndPosition);
                    GameObject lNewEnemy = EnemyFactory.SpawnEnemy(pWave.TypeOfEnemy, lPos, _GameContainer.transform);
                    Enemy lCurrentEnemy = lNewEnemy.GetComponent<Enemy>();

                    if (lCurrentEnemy != null)
                    {
                        allEnemies.Add(lCurrentEnemy);
                        lCurrentEnemy.onDeath += () => CheckWinCondition(lCurrentEnemy);
                    }
                    else
                        Debug.LogError($"L'ennemi spawn de type {pWave.TypeOfEnemy} n'a pas de script Enemy !");

                    lSpawned++;
                    if (i < lBatchSize - 1)
                        yield return lIntraBatch;
                }

                if (lSpawned < lTotal)
                    yield return lBetweenBatches;
            }
        }

        private void OnCallNextWave(int pWaveIndex) => StartCoroutine(CallNextWaveRoutine(pWaveIndex));

        // Public so FTUEManager (or any scripted flow) can play an arbitrary WaveBuilderSO outside of the
        // _WaveBuilders list. Does not touch win condition or index — caller handles end-of-wave logic.
        public IEnumerator PlayWaveBuilderRoutine(WaveBuilderSO pBuilder)
        {
            if (pBuilder == null) yield break;
            _IsSpawningWave = true;
            foreach (EnemyWaveSO lEnemyWave in pBuilder.EnemyToSpawn)
                yield return StartCoroutine(SpawnWaveCoroutine(lEnemyWave));
            _IsSpawningWave = false;
        }

        private IEnumerator CallNextWaveRoutine(int pWaveIndex)
        {
            if (pWaveIndex < 0 || pWaveIndex >= _WaveBuilders.Count)
                yield break;

            if (pWaveIndex == _WaveBuilders.Count - 1)
                _IsLastWaveCalled = true;

            yield return StartCoroutine(PlayWaveBuilderRoutine(_WaveBuilders[pWaveIndex]));

            // Enemies may already all be dead by the time the last sub-wave finishes spawning (small waves, fast kills).
            CheckWinCondition();
        }
        
        private void ClearAllEnemies()
        {
            StopAllCoroutines();

            // Reset flags: StopAllCoroutines kills PlayWaveBuilderRoutine mid-spawn, so its final
            // `_IsSpawningWave = false` never runs. Without this reset, the flag stays true across runs
            // and CheckWinCondition is paralysed forever (no new wave, no win screen on next run).
            _IsSpawningWave = false;
            _IsLastWaveCalled = false;

            for (int i = allEnemies.Count - 1; i >= 0; i--)
                if (allEnemies[i] != null)
                    Destroy(allEnemies[i].gameObject);

            allEnemies.Clear();
        }
        
        private void CheckWinCondition(Enemy pEnemyToRemove = null)
        {
            if (pEnemyToRemove != null)
                allEnemies.Remove(pEnemyToRemove);

            if (_IsSpawningWave || allEnemies.Count > 0)
                return;

            if (_IsLastWaveCalled)
                _GameManager.onLevelFinished?.Invoke(EMenuType.WIN_SCREEN);
            else
                _GameManager.HandleNewWave();
        }

        private Vector3 PolarToCartesian(float pAngle, float pRadius) => new Vector3(Mathf.Cos(pAngle), 0f, Mathf.Sin(pAngle)) * pRadius;

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Handles.color = Color.red;
            Handles.DrawWireDisc(_GameContainer.transform.position, Vector3.up, _RadiusSpawn);
#endif
        }

        private void OnEnable()
        {
            if (_GameManager == null)
                return;

            _GameManager.onLeaveGame += ClearAllEnemies;
            _GameManager.onCallWaveAndIncreaseIndex += OnCallNextWave;
        }

        private void OnDisable()
        {
            if (_GameManager == null)
                return;

            _GameManager.onLeaveGame -= ClearAllEnemies;
            _GameManager.onCallWaveAndIncreaseIndex -= OnCallNextWave;
        }
    }
}