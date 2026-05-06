using Com.IsartDigital.Kinisi.Login;
using Com.IsartDigital.Kinisi.Menu;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Com.IsartDigital.ProjectName
{
    public partial class DataManager : Manager
    {
        #region Singleton
        static private DataManager instance;
        private DataManager() { }

        static public DataManager GetInstance()
        {
            if (instance == null) instance = new DataManager();
            return instance;
        }
        #endregion

        public static List<LevelData> Levels { get; private set; } = new List<LevelData>();
        public static List<List<List<string>>> LevelsGrid { get; private set; }  = new List<List<List<string>>>();
        public static List<List<List<bool>>> LevelsTarget { get; private set; }  = new List<List<List<bool>>>();

        private const string LEVELS = "levels";
        private const int COUNT = 2;
        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(DataManager) + " instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
            LoadLevels(JsonReader.LoadLevels());
            CheckScore();
        }

        public override void Init() { }

        //Manage the JsonReader and get what it produced
        public void LoadLevels(List<LevelData> pLevels)
        {
            Levels = pLevels;
            foreach (LevelData lLevel in Levels)
            {
                var (lObjectGrid, lTargetGrid) = JsonReader.ConvertLayoutToObjects(lLevel.layout);
                LevelsGrid.Add(lObjectGrid);
                LevelsTarget.Add(lTargetGrid);
            }
        }

        //Create a LevelIndex usable for PAR
        public static LevelData GetLevel(int pIndex)
        {
            return pIndex >= 0 && pIndex < Levels.Count ? Levels[pIndex] : null;
        }

        //Get the current User data
        private Godot.Collections.Dictionary<string, Dictionary> LoadUserData()
        {
            if (FileAccess.FileExists(NewUserAccount.SAVE_FILE))
            {
                var lFile = FileAccess.OpenEncryptedWithPass(NewUserAccount.SAVE_FILE, FileAccess.ModeFlags.Read, NewUserAccount.ENCRYPTION_PASS);
                string lJsonString = lFile.GetAsText();
                lFile.Close();

                Json lJson = new();
                lJson.Parse(lJsonString);
                return lJson.Data.AsGodotDictionary<string, Dictionary>();
            }
            return null;
        }

        //Change the Current Data 
        private void SaveUserData(Godot.Collections.Dictionary<string, Dictionary> pLData)
        {
            string lUpdatedJsonString = Json.Stringify(pLData);
            var lFile = FileAccess.OpenEncryptedWithPass(NewUserAccount.SAVE_FILE, FileAccess.ModeFlags.Write, NewUserAccount.ENCRYPTION_PASS);
            lFile.StoreLine(lUpdatedJsonString);
            lFile.Close();
        }

        public static void SaveScore(int score) //Added by Hector
        {
            using var file = FileAccess.Open("user://score.save", FileAccess.ModeFlags.Write);
            file.Store32((uint)score);
        }

        private void CheckScore() //Added by Hector
        {
            if (FileAccess.FileExists("user://score.save"))
            {
                using var file = FileAccess.Open("user://score.save", FileAccess.ModeFlags.Read);
                GameManager.score = (int)file.Get32();
            }
        }

        public void UnlockNextLevel()
        {
            var lData = LoadUserData();
            if (lData != null && lData.TryGetValue(Login.savedUsername, out var lUserInfo))
            {
                var lLevels = (Godot.Collections.Dictionary<string, bool>)lUserInfo[LEVELS];
                var lCompletedLevels = lLevels.Where(x => x.Value).Select(x => x.Key).ToList();
                var lNextLevelIndex = lCompletedLevels.LastOrDefault();

                if (lNextLevelIndex != null)
                {
                    lLevels[(Menu.levelIndex+COUNT).ToString()] = true;
                    SaveUserData(lData);
                }
            }
        }


        public bool IsLevelUnlocked(int pLevelNumber)
        {
            var lData = LoadUserData();
            if (lData != null && lData.TryGetValue(Login.savedUsername, out var lUserInfo))
            {
                var lLevels = (Godot.Collections.Dictionary<string, bool>)lUserInfo[LEVELS];
                return lLevels.ContainsKey(pLevelNumber.ToString()) && lLevels[pLevelNumber.ToString()];
            }
            return false;
        }

        public void UnlockAllLevels()
        {
            var lData = LoadUserData();
            if (lData != null && lData.TryGetValue(Login.savedUsername, out var lUserInfo))
            {
                var lLevels = (Godot.Collections.Dictionary<string, bool>)lUserInfo[LEVELS];
                bool lAllLevelsUnlocked = lLevels.Values.All(lValue => lValue);

                //So it wont lock the first level
                foreach (var lKey in lLevels.Keys.ToList())
                {
                    lLevels[lKey] = lAllLevelsUnlocked ? (lKey == "1" ? true : false) : true;
                }

                SaveUserData(lData);
            }
        }
    }
}
