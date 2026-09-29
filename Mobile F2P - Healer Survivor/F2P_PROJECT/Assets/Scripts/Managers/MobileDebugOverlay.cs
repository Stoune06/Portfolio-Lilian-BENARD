using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Menus;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    // Attach to any persistent GameObject. Draws a debug panel top-left + action buttons bottom-left.
    // Remove or disable once the mobile issues are solved.
    public class MobileDebugOverlay : MonoBehaviour
    {
        [SerializeField] private bool _Enabled = true;
        [SerializeField] private int _FontSize = 22;

        private GUIStyle _Style;
        private GUIStyle _ButtonStyle;

        private void OnGUI()
        {
            if (!_Enabled) return;

            EnsureStyles();

            string lText = BuildDebugText();
            float lHeight = _Style.CalcHeight(new GUIContent(lText), 700);

            GUI.color = new Color(0, 0, 0, 0.75f);
            GUI.DrawTexture(new Rect(8, 8, 700, lHeight + 12), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(12, 10, 700, lHeight + 8), lText, _Style);

            DrawButtons();
        }

        private void DrawButtons()
        {
            float lY = Screen.height - 260;

            if (GUI.Button(new Rect(10, lY, 260, 60), "Force WIN_SCREEN", _ButtonStyle))
            {
                MenuManager lMenu = FindFirstObjectByType<MenuManager>(FindObjectsInactive.Include);
                if (lMenu != null) lMenu.ShowMenu(EMenuType.WIN_SCREEN);
            }

            if (GUI.Button(new Rect(10, lY + 70, 260, 60), "Skip all FTUE runs", _ButtonStyle))
            {
                PlayerPrefs.SetInt(FTUEManager.PLAYER_PREFS_KEY, FTUEManager.FTUE_RUN_COUNT);
                PlayerPrefs.Save();
            }

            if (GUI.Button(new Rect(10, lY + 140, 260, 60), "Reset FTUE (step 0)", _ButtonStyle))
            {
                PlayerPrefs.DeleteKey(FTUEManager.PLAYER_PREFS_KEY);
                PlayerPrefs.Save();
            }

            if (GUI.Button(new Rect(10, lY + 210, 260, 40), "Clear LastLog", _ButtonStyle))
            {
                _LastLog = "(cleared)";
            }
        }

        private string BuildDebugText()
        {
            int lStep = PlayerPrefs.GetInt(FTUEManager.PLAYER_PREFS_KEY, 0);
            bool lAllRunsDone = FTUEManager.HasCompletedAllRuns;

            string lWaveBlocked = GameManager.Instance != null ? GameManager.Instance.IsWaveSpawningBlocked.ToString() : "NO GM";
            int lEnemies = WaveManager.Instance != null ? WaveManager.Instance.allEnemies.Count : -1;
            bool lPlayerAlive = Player.instance != null;

            string lFtueRunning = FTUEManager.Instance != null ? FTUEManager.Instance.IsRoutineRunning.ToString() : "NO FTUE";
            int lArrows = FTUEManager.Instance != null ? FTUEManager.Instance.RemainingArrows : -1;
            int lAllies = FTUEManager.Instance != null ? FTUEManager.Instance.SpawnedAlliesCount : -1;

            // Check if WIN_SCREEN is registered in MenuManager
            MenuManager lMenu = FindFirstObjectByType<MenuManager>(FindObjectsInactive.Include);
            string lMenuInfo = lMenu != null ? "MenuManager found" : "NO MenuManager!";

            return $"FTUE_Step={lStep}/{FTUEManager.FTUE_RUN_COUNT}  AllDone={lAllRunsDone}  FtueRunning={lFtueRunning}\n" +
                   $"WaveBlocked={lWaveBlocked}  Enemies={lEnemies}  Arrows={lArrows}  Allies={lAllies}\n" +
                   $"PlayerAlive={lPlayerAlive}  timeScale={Time.timeScale:F2}  {lMenuInfo}\n" +
                   $"LastLog: {_LastLog}";
        }

        private void EnsureStyles()
        {
            if (_Style == null)
            {
                _Style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = _FontSize,
                    normal = { textColor = Color.yellow },
                    padding = new RectOffset(6, 6, 2, 2),
                };
            }
            if (_ButtonStyle == null)
            {
                _ButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = _FontSize,
                };
            }
        }

        private static string _LastLog = "(none)";

        private void OnEnable() => Application.logMessageReceived += OnLog;
        private void OnDisable() => Application.logMessageReceived -= OnLog;

        private static void OnLog(string pCondition, string pStackTrace, LogType pType)
        {
            if (pType == LogType.Error || pType == LogType.Exception || pType == LogType.Warning)
                _LastLog = $"[{pType}] {pCondition}";
        }
    }
}
