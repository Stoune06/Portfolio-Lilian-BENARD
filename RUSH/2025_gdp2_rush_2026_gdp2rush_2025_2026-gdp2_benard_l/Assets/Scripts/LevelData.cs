using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public GameObject levelPrefab;
    public List<InventoryEntry> levelInventory = new List<InventoryEntry>();
    public int YBound = 0;
    public float previewScale = 0.01f;

    [Header("Potion Visuals")]
    public Color PotionColorTop = Color.blue;
    public Color PotionColorBottom = Color.cyan;
}
