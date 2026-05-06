using System;
using System.Collections.Generic;
using UnityEngine;

public static class TilesInventoryManager
{
    private static List<InventoryEntry> _AvailableTiles;

    public static List<InventoryEntry> availableTiles
    {
        get => _AvailableTiles;
        set
        {
            _AvailableTiles = value;
            onAvailableTilesChanged?.Invoke(value);
        }
    }

    public static event Action<List<InventoryEntry>> onAvailableTilesChanged;
}

