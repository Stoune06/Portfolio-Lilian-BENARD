using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Manager.EditorTools
{
    public static class FTUEDebugMenu
    {
        private const string PLAYER_PREFS_KEY = FTUEManager.PLAYER_PREFS_KEY;
        private const int FTUE_RUN_COUNT = FTUEManager.FTUE_RUN_COUNT;

        [MenuItem("Tools/FTUE/Reset FTUE (step 0)")]
        private static void ResetFTUEFlag()
        {
            PlayerPrefs.DeleteKey(PLAYER_PREFS_KEY);
            PlayerPrefs.Save();
            Debug.Log($"[FTUE] '{PLAYER_PREFS_KEY}' reset. FTUE restarts at run 1 on next scene load.");
        }

        [MenuItem("Tools/FTUE/Mark All Runs As Played")]
        private static void MarkFTUEAsPlayed()
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_KEY, FTUE_RUN_COUNT);
            PlayerPrefs.Save();
            Debug.Log($"[FTUE] '{PLAYER_PREFS_KEY}' set to {FTUE_RUN_COUNT}. FTUE fully skipped on next scene load.");
        }

        [MenuItem("Tools/FTUE/Set Step 1 (skip run 1)")]
        private static void SetStepOne()
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log($"[FTUE] '{PLAYER_PREFS_KEY}' = 1. Next Play starts run 2.");
        }

        [MenuItem("Tools/FTUE/Set Step 2 (skip runs 1-2)")]
        private static void SetStepTwo()
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_KEY, 2);
            PlayerPrefs.Save();
            Debug.Log($"[FTUE] '{PLAYER_PREFS_KEY}' = 2. Next Play starts run 3.");
        }

        [MenuItem("Tools/FTUE/Log FTUE State")]
        private static void LogFTUEFlag()
        {
            int lValue = PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 0);
            string lDesc = lValue >= FTUE_RUN_COUNT
                ? "all runs completed, FTUE will skip"
                : $"run {lValue + 1} will play next";
            Debug.Log($"[FTUE] '{PLAYER_PREFS_KEY}' = {lValue} ({lDesc}).");
        }
    }
}
