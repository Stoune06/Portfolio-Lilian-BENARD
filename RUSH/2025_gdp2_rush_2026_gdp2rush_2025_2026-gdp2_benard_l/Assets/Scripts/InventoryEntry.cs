using UnityEngine;

[System.Serializable]
public struct InventoryEntry
{
    public TileType tileType;
    [SerializeField]
    public TileDirection direction;

    public int quantity;
}