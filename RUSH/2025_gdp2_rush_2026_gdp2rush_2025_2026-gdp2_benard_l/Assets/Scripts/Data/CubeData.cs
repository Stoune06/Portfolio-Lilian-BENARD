using UnityEngine;

[CreateAssetMenu(fileName = "CubeData", menuName = "Scriptable Objects/CubeData")]
public class CubeData : ScriptableObject
{
    [Header("Visuel")]
    public GameObject prefab;
    public Color color = Color.white;

    [Header("Statistiques")]
    [Min(1)] public int actionsPerTick = 1;
}
