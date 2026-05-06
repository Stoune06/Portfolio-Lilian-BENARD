using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Inputs;
using Com.IsartDigital.HealerSurvivor.Juiciness;
using Com.IsartDigital.HealerSurvivor.Menus;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    public class FTUEManager : MonoBehaviour
    {
        public const string PLAYER_PREFS_KEY = "FTUE_Step";
        public const int FTUE_RUN_COUNT = 3;

        public static bool HasCompletedAllRuns => PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0) >= FTUE_RUN_COUNT;

        public static FTUEManager Instance { get; private set; }

        // Debug overlay reads these to diagnose stuck FTUE state on mobile.
        public int CurrentStep => _CurrentStep;
        public int RemainingArrows => _RemainingArrows;
        public bool IsRoutineRunning => _MainCoroutine != null;
        public int SpawnedAlliesCount => _SpawnedAllies.Count;

        [Serializable]
        private class FTUERunConfig
        {
            public string Label;
            public GameObject[] Allies;
            public WaveBuilderSO[] Waves;
        }

        [Header("Ally Approach (shared across runs)")]
        [SerializeField, Range(0f, 1f)] private float _AllyHpPercentAtSpawn = 0.1f;
        [SerializeField] private float _AllyRadiusAroundPlayer = 3f;
        [SerializeField] private EPosition _AlliesApproachFromDirection = EPosition.TOP;
        [SerializeField] private float _AllyApproachOffMapRadius = 15f;
        [SerializeField] private float _AllyApproachSpeed = 6f;
        [SerializeField] private float _AllyArrivalThreshold = 0.1f;

        [Header("FTUE Runs (must be FTUE_RUN_COUNT entries)")]
        [SerializeField] private FTUERunConfig[] _Runs = new FTUERunConfig[FTUE_RUN_COUNT];

        [Header("Arrow")]
        [SerializeField] private GameObject _FTUEArrowPrefab;

        [Header("Scene References")]
        [SerializeField] private InputManager _InputManager;
        [SerializeField] private WaveManager _WaveManager;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Header("Debug")]
        [SerializeField] private KeyCode _DebugSkipKey = KeyCode.F10;
#endif

        private readonly List<Character> _SpawnedAllies = new List<Character>();
        private readonly List<FTUEArrow> _SpawnedArrows = new List<FTUEArrow>();
        private int _RemainingArrows;
        private bool _AlreadyPlayed;
        private Coroutine _MainCoroutine;
        private int _CurrentStep;

        private void Awake()
        {
            Instance = this;
            // TODO: migrate to the project database once it lands; PlayerPrefs is temporary.
            if (HasCompletedAllRuns)
            {
                _AlreadyPlayed = true;
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            GameManager.Instance.onGameStart += HandleGameStart;
            GameManager.Instance.onLeaveGame += HandleLeaveGame;
        }

        // Called on every return to main menu. If all runs are done we skip (FTUEManager is effectively retired);
        // otherwise we reset transient state so the next Play can restart cleanly — either replaying the same
        // mid-run (quit) or moving to the next run (normal completion).
        // Progress is saved to PlayerPrefs only at the end of a successful run, so quitting mid-run naturally
        // replays that step.
        private void HandleLeaveGame()
        {
            if (_AlreadyPlayed || HasCompletedAllRuns) return;

            if (_MainCoroutine != null)
            {
                StopCoroutine(_MainCoroutine);
                _MainCoroutine = null;
            }

            FTUEArrow.OnArrowCompleted -= OnArrowCompleted;

            for (int i = 0; i < _SpawnedArrows.Count; i++)
                if (_SpawnedArrows[i] != null) Destroy(_SpawnedArrows[i].gameObject);
            _SpawnedArrows.Clear();

            // Allies already destroyed by GameManager.CleanupRun; just drop our now-stale references.
            _SpawnedAllies.Clear();

            _RemainingArrows = 0;

            GameManager.Instance.IsWaveSpawningBlocked = false;
        }

        private void HandleGameStart()
        {
            if (HasCompletedAllRuns)
            {
                _AlreadyPlayed = true;
                return;
            }

            // Guard against onGameStart firing twice in the same run (has happened via menu flow bugs).
            if (_MainCoroutine != null) return;

            _CurrentStep = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0);
            if (_CurrentStep < 0 || _CurrentStep >= _Runs.Length)
            {
                _AlreadyPlayed = true;
                return;
            }

            GameManager.Instance.IsWaveSpawningBlocked = true;

            FTUEArrow.OnArrowCompleted += OnArrowCompleted;

            _MainCoroutine = StartCoroutine(FTUERoutine(_CurrentStep));
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            if (Input.GetKeyDown(_DebugSkipKey))
                SkipFTUE();
        }
#endif

        private IEnumerator FTUERoutine(int pStep)
        {
            FTUERunConfig lConfig = _Runs[pStep];

            // Fail loudly if the run wasn't configured in the Inspector — otherwise the coroutine would
            // run through instantly and trigger the win screen on scene load.
            if (lConfig == null || lConfig.Allies == null || lConfig.Allies.Length == 0
                || lConfig.Waves == null || lConfig.Waves.Length == 0)
            {
                Debug.LogError($"[FTUE] Run {pStep} is not configured (missing Allies or Waves in Inspector). FTUE aborted, falling back to normal gameplay.");
                // Mark FTUE fully done so the player doesn't stay stuck on an empty map — they'll get the
                // regular wave flow. They can reset via Tools/FTUE/Reset if they fix the config.
                PlayerPrefs.SetInt(PLAYER_PREFS_KEY, FTUE_RUN_COUNT);
                PlayerPrefs.Save();
                _MainCoroutine = null;
                GameManager.Instance.IsWaveSpawningBlocked = false;
                GameManager.Instance.SpawnActiveDeck();
                GameManager.Instance.HandleNewWave();
                yield break;
            }

            // Player.stateMachine is created in Character.Start(); wait one frame so Start order doesn't matter.
            yield return null;

            DisablePlayerControl();

            yield return StartCoroutine(SpawnAndApproachAllies(lConfig.Allies));

            EnablePlayerControl();

            // Arrows only on the first run — they introduce the concept of allies.
            if (pStep == 0)
            {
                SpawnArrows();
                while (_RemainingArrows > 0)
                    yield return null;
            }

            ReactivateAllies();

            if (lConfig.Waves != null)
            {
                foreach (WaveBuilderSO lWave in lConfig.Waves)
                {
                    yield return StartCoroutine(_WaveManager.PlayWaveBuilderRoutine(lWave));
                    // Wait for every enemy to die before moving to the next WaveBuilder.
                    while (_WaveManager.allEnemies.Count > 0)
                        yield return null;
                }
            }

            CompleteCurrentRun();
        }

        private void CompleteCurrentRun()
        {
            int lNextStep = _CurrentStep + 1;
            PlayerPrefs.SetInt(PLAYER_PREFS_KEY, lNextStep);
            PlayerPrefs.Save();

            // Clear the coroutine handle so the next HandleGameStart's double-call guard lets the new run start.
            _MainCoroutine = null;
            GameManager.Instance.IsWaveSpawningBlocked = false;

            // Trigger the win screen so the player is returned to the menu flow naturally.
            // If all runs are done, the next Play will skip FTUE entirely via HasCompletedAllRuns.
            GameManager.Instance.onLevelFinished?.Invoke(EMenuType.WIN_SCREEN);
        }

        private IEnumerator SpawnAndApproachAllies(GameObject[] pAllyPrefabs)
        {
            int lCount = pAllyPrefabs != null ? pAllyPrefabs.Length : 0;
            if (lCount == 0) yield break;

            Vector3 lPlayerPos = Player.instance.transform.position;
            Vector3 lApproachDirection = DirectionFromEPosition(_AlliesApproachFromDirection);
            Vector3 lOffMapCenter = lPlayerPos + lApproachDirection * _AllyApproachOffMapRadius;

            List<Vector3> lTargetPositions = ComputeTargetTriangle(lPlayerPos, _AllyRadiusAroundPlayer, lCount);

            for (int i = 0; i < lCount; i++)
            {
                GameObject lPrefab = pAllyPrefabs[i];
                if (lPrefab == null) continue;

                Vector3 lSpawnPos = lOffMapCenter + new Vector3((i - (lCount - 1) * 0.5f) * 1.5f, 0f, 0f);
                GameObject lAllyGO = Instantiate(lPrefab, lSpawnPos, Quaternion.identity);
                Character lAlly = lAllyGO.GetComponent<Character>();
                _SpawnedAllies.Add(lAlly);
            }

            // Character.Start() runs on the next tick and sets health = maxHealth; overrides must wait one frame.
            yield return null;

            // IdleState subscribes to the player's joystick by design, and MoveState.ChangeVelocity assumes a Player
            // component. Forcing allies into IDLE would trigger MoveState on any joystick input and crash on allies
            // (pPlayer = null). Disabling the Character component freezes their AI entirely until the approach is done.
            for (int i = 0; i < _SpawnedAllies.Count; i++)
            {
                Character lAlly = _SpawnedAllies[i];
                if (lAlly == null) continue;
                lAlly.velocity = Vector3.zero;
                lAlly.rigidBody.maxLinearVelocity = 0f;
                lAlly.health = lAlly.targetMaxHealth * _AllyHpPercentAtSpawn;
                lAlly.enabled = false;
            }

            bool lAllArrived = false;
            while (!lAllArrived)
            {
                lAllArrived = true;
                for (int i = 0; i < _SpawnedAllies.Count; i++)
                {
                    Character lAlly = _SpawnedAllies[i];
                    if (lAlly == null) continue;

                    Vector3 lTarget = lTargetPositions[i];
                    lAlly.transform.position = Vector3.MoveTowards(lAlly.transform.position, lTarget, _AllyApproachSpeed * Time.deltaTime);

                    Vector3 lFlatDelta = new Vector3(lTarget.x - lAlly.transform.position.x, 0f, lTarget.z - lAlly.transform.position.z);
                    if (lFlatDelta.sqrMagnitude > 0.001f)
                        lAlly.transform.rotation = Quaternion.LookRotation(lFlatDelta.normalized);

                    if (Vector3.Distance(lAlly.transform.position, lTarget) > _AllyArrivalThreshold)
                        lAllArrived = false;
                }
                yield return null;
            }
        }

        private void ReactivateAllies()
        {
            for (int i = 0; i < _SpawnedAllies.Count; i++)
            {
                Character lAlly = _SpawnedAllies[i];
                if (lAlly == null) continue;
                lAlly.enabled = true;

                //tempo
                lAlly.rigidBody.maxLinearVelocity = 100f;
            }
        }

        private void SpawnArrows()
        {
            _RemainingArrows = 0;
            foreach (Character lAlly in _SpawnedAllies)
            {
                if (lAlly == null)
                    continue;

                GameObject lArrowGO = Instantiate(_FTUEArrowPrefab, lAlly.transform.position, Quaternion.identity);
                FTUEArrow lArrow = lArrowGO.GetComponent<FTUEArrow>();
                lArrow.Initialize(lAlly);
                _SpawnedArrows.Add(lArrow);
                _RemainingArrows++;
            }
        }

        private void OnArrowCompleted(FTUEArrow pArrow)
        {
            if (_SpawnedArrows.Remove(pArrow))
                _RemainingArrows--;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Skip all remaining FTUE runs at once (dev convenience).
        private void SkipFTUE()
        {
            if (_MainCoroutine != null) StopCoroutine(_MainCoroutine);

            for (int i = 0; i < _SpawnedArrows.Count; i++)
                if (_SpawnedArrows[i] != null) Destroy(_SpawnedArrows[i].gameObject);
            _SpawnedArrows.Clear();

            for (int i = 0; i < _SpawnedAllies.Count; i++)
                if (_SpawnedAllies[i] != null) Destroy(_SpawnedAllies[i].gameObject);
            _SpawnedAllies.Clear();

            PlayerPrefs.SetInt(PLAYER_PREFS_KEY, FTUE_RUN_COUNT);
            PlayerPrefs.Save();

            if (Time.timeScale == 0f) Time.timeScale = 1f;
            if (_InputManager != null) _InputManager.gameObject.SetActive(true);

            _MainCoroutine = null;
            GameManager.Instance.IsWaveSpawningBlocked = false;
            GameManager.Instance.HandleNewWave();
            CleanupAndDestroy();
        }
#endif

        private void CleanupAndDestroy()
        {
            FTUEArrow.OnArrowCompleted -= OnArrowCompleted;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onGameStart -= HandleGameStart;
                GameManager.Instance.onLeaveGame -= HandleLeaveGame;
            }

            FTUEArrow.OnArrowCompleted -= OnArrowCompleted;
        }

        private void DisablePlayerControl()
        {
            // Disabling InputManager alone is sufficient: no joystick events fire, so the Player stays in IdleState
            // (its initial state) and can't transition to Move. Calling InitState here would re-run IdleState.OnEnter
            // and duplicate the joystick subscription, leaving the state machine in a leaky state.
            if (_InputManager != null) _InputManager.gameObject.SetActive(false);
            if (Player.instance != null) Player.instance.velocity = Vector3.zero;
        }

        private void EnablePlayerControl()
        {
            if (_InputManager != null) _InputManager.gameObject.SetActive(true);
        }

        private static Vector3 DirectionFromEPosition(EPosition pPosition)
        {
            float lAngle = (int)pPosition * (Mathf.PI * 2f / 8f);
            return new Vector3(Mathf.Cos(lAngle), 0f, Mathf.Sin(lAngle));
        }

        private static List<Vector3> ComputeTargetTriangle(Vector3 pCenter, float pRadius, int pCount)
        {
            List<Vector3> lPositions = new List<Vector3>(pCount);
            float lStep = Mathf.PI * 2f / Mathf.Max(1, pCount);
            for (int i = 0; i < pCount; i++)
            {
                float lAngle = Mathf.PI / 2f + i * lStep;
                lPositions.Add(pCenter + new Vector3(Mathf.Cos(lAngle), 0f, Mathf.Sin(lAngle)) * pRadius);
            }
            return lPositions;
        }
    }
}
