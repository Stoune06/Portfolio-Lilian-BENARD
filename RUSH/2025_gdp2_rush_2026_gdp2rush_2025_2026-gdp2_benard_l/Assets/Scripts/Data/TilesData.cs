using System.Linq.Expressions;
using UnityEngine;

public static class TilesData
{
        public static Tile GetPrefabToLoad(InventoryEntry pTile)
        {
            switch(pTile.tileType)
            {
                case TileType.Arrow:
                {
                    return Resources.Load<Tile>("Prefabs/Tiles/Arrow");
                }
                case TileType.Stop:
                    {
                        return Resources.Load<Tile>("Prefabs/Tiles/Stopper");
                    }
                case TileType.Switch:
                    {
                        return Resources.Load<Tile>("Prefabs/Tiles/Turnstile");
                    }
                case TileType.Conveyor:
                    {
                        return Resources.Load<Tile>("Prefabs/Tiles/Conveyor");
                    }
                default: return null;
        }
    }

    public static Quaternion GetDirection(InventoryEntry pTile)
    {
        switch(pTile.direction)
        {
            case TileDirection.Up:
                {
                    return Quaternion.AngleAxis(0,Vector3.up);
                }
            case TileDirection.Down:
                    {
                        return Quaternion.AngleAxis(180, Vector3.up);
                    }
            case TileDirection.Left:
                {
                    return Quaternion.AngleAxis(270, Vector3.up);
                }
            case TileDirection.Right:
                {
                    return Quaternion.AngleAxis(90, Vector3.up);
                }
            default: return Quaternion.identity;
        }
    }
}
