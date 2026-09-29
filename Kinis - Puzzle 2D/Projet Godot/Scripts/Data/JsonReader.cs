using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Godot;

namespace Com.IsartDigital.ProjectName
{
    public static class JsonReader
    {
        #region PATH
        private const string LEVELSFILEPATH = "res://Assets/Levels/Levels.json";
        private const string EMPTY = "Empty";
        private const string WALL = "Wall";
        private const string BOX = "Box";
        private const string PLAYER = "Player";
        private const string UNKNOWN = "Unknown";
        private const string PAR = "par";
        private const string LAYOUT = "layout";
        #endregion
        //Get the Json and transform the Data of each level to levelData
        public static List<LevelData> LoadLevels()
        {
            if (!FileAccess.FileExists(LEVELSFILEPATH))
                return new();

            using var lFile = FileAccess.Open(LEVELSFILEPATH, FileAccess.ModeFlags.Read);
            string lJsonText = lFile.GetAsText();

            var lJsonResult = Json.ParseString(lJsonText);
            if (lJsonResult.Obj is not Godot.Collections.Array lLevelsArray)
            {
                return new();
            }

            var lLevels = new List<LevelData>();

            foreach (var lLevel in lLevelsArray)
            {
                if (lLevel.Obj is not Godot.Collections.Dictionary lLevelDict)
                    continue;

                var lLayoutArray = lLevelDict.TryGetValue(LAYOUT, out var lLayoutRaw) &&
                   lLayoutRaw.Obj is Godot.Collections.Array lLayoutArrayGodot
                   ? lLayoutArrayGodot.Select(item => item.AsString()).ToArray()
                   : Array.Empty<string>();


                var lLevelData = new LevelData
                {
                    layout = lLayoutArray,
                    par = lLevelDict[PAR].AsInt32(),
                    author = (string)lLevelDict["Author"]
                };
                GD.Print(lLevelData.author);

                lLevels.Add(lLevelData);
            }

            return lLevels;
        }
        //Translate the data made in the previous function and make it in spawnable object understable for our Spawner script
        public static (List<List<string>>, List<List<bool>>) ConvertLayoutToObjects(string[] pLayout)
        {
            var lTileCase = new Dictionary<char, (string, bool)>
            {
                {' ', (EMPTY, false)},
                {'#', (WALL, false)},
                {'$', (BOX, false)},
                {'.', (EMPTY, true)},
                {'@', (PLAYER, false)},
                {'*', (BOX, true)},
                {'%', (WALL, true)}
            };
            var lObjectGrid = new List<List<string>>(pLayout.Length);
            var lTargetGrid = new List<List<bool>>(pLayout.Length);

            foreach (string lLine in pLayout)
            {
                List<string> lRowObjects = new(lLine.Length);
                List<bool> lRowTarget = new(lLine.Length);

                foreach (char lTile in lLine)
                {
                    if (lTileCase.TryGetValue(lTile, out var spawnable))
                    {
                        lRowObjects.Add(spawnable.Item1);
                        lRowTarget.Add(spawnable.Item2);
                    }
                    else
                    {
                        lRowObjects.Add(UNKNOWN);
                        lRowTarget.Add(false);
                    }
                }
                lObjectGrid.Add(lRowObjects);
                lTargetGrid.Add(lRowTarget);
            }

            return (lObjectGrid, lTargetGrid);
        }
    }
}
