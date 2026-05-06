using Com.IsartDigital.Kinisi;
using Godot;
using System;
using System.Collections.Generic;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public static class Spawner
	{
        public static PackedScene tileScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameObjects/Tile.tscn");
        public static PackedScene boxFactory = ResourceLoader.Load<PackedScene>("res://Scenes/GameObjects/Movables/Box.tscn");
        public static PackedScene wallFactory = ResourceLoader.Load<PackedScene>("res://Scenes/GameObjects/Movables/Wall.tscn");
        public static PackedScene targetFactory = ResourceLoader.Load<PackedScene>("res://Scenes/GameObjects/Target.tscn");
        public static PackedScene playerFactory = ResourceLoader.Load<PackedScene>("res://Scenes/GameObjects/Movables/Player.tscn");

        private static GridManager gridManager = GridManager.GetInstance();
        private static GameManager gameManager = GameManager.GetInstance();
        private static ScreenManager screenManager = ScreenManager.GetInstance();
    

        public const int BASE_TILE_ZINDEX = -100;
        public const int BASE_TARGET_ZINDEX = -10;
        private const int BASE_PARTICLES_ZINDEX = -2;
        private const float MAX_GRID_SIZE = 9f;

        public static void SpawnGameObjects(List<List<string>> pData, Node pContainer, Vector2 pSize)
        {
            int lXLength = pData.Count;
            int lYLength = pData[0].Count;
            PackedScene lPackedScene = null;
            gridManager.GenerateGrid(lXLength, lYLength);
            //gridManager.UpdateOrigin();
            SpawnTiles(lXLength, lYLength, pContainer, pSize);
            gridManager.UpdateViewport();

            for (int x = 0; x < lXLength; x++)
            {
                for (int y = 0; y < lYLength; y++)
                {
                    if (pData[x][y] == "Box")
                    {
                        lPackedScene = boxFactory;
                    }
                    else if (pData[x][y] == "Wall")
                    {
                        lPackedScene = wallFactory;
                    }
                    else if (pData[x][y] == "Player") lPackedScene = playerFactory;
                    if (lPackedScene != null)
                    {
                        gridManager.movableObjectsOnGrid[x][y] = (Movable)SpawnGameObject(lPackedScene, gridManager.GetPosition(new Vector2(x, y)), gridManager.tileSize, y + 1, pContainer);
                    }
                    lPackedScene = null;
                }
            }
            gridManager.UpdatePreviousPositions();
        }

        public static GameObject SpawnGameObject(PackedScene pPackedScene, Vector2 pPos, Vector2 pSize = default, int pZIndex = default, Node pContainer = default)
        {
            GameObject lGameObject = pPackedScene.Instantiate<GameObject>();

            if (lGameObject is Movable lMovable)
            {
                lMovable.endPos = pPos;
            }
            else lGameObject.Position = pPos;
            lGameObject.Resize(pSize);
            lGameObject.ZIndex = pZIndex;
            if(pContainer != default) pContainer.AddChild(lGameObject);
            else gameManager.gameContainer.AddChild(lGameObject);
            return lGameObject;
        }

        public static Particle SpawnParticle(PackedScene pPackedScene, Node pContainer , float pScale = default, int pZindex = BASE_PARTICLES_ZINDEX)
        {
            Particle lParticle = pPackedScene.Instantiate<Particle>();
            lParticle.ZIndex = pZindex;
            pContainer.AddChild(lParticle);
            lParticle.particleProcessMaterial.ScaleMin = pScale;
            return lParticle;
        }

        public static void SpawnTargets(List<List<bool>> pData, Node pContainer)
        {
            int lLength = pData.Count;
            int lTargetNum = 0;
            Target lTarget;
            for (int x = 0; x < lLength; x++)
            {
                List<Target> lTargetListY = new List<Target>();
                for (int y = 0; y < pData[x].Count; y++)
                {
                    lTarget = null;
                    if (pData[x][y])
                    {
                        lTarget = (Target)SpawnGameObject(targetFactory, gridManager.GetPosition(new Vector2(x, y)), gridManager.tileSize, BASE_TARGET_ZINDEX + y, pContainer);
                        lTargetNum++;                        
                    }
                    lTargetListY.Add(lTarget);
                }
                gridManager.targetNum = lTargetNum;
                gridManager.targetList.Add(lTargetListY);
            }
        }

        public static void SpawnTiles(int pX, int pY, Node pContainer, Vector2 pTileSize ) //ObjectsSpawner a modifier
        {
            //float lRatio = MAX_GRID_SIZE / (pX* 1.2f);
            //Vector2 lTileSize = pTileSize * lRatio;
            gridManager.tileSize = pTileSize;
            gridManager.UpdateOrigin();
            Tile lTile;

            for (int i = 0; i < pX; i++)
            {
                gridManager.tilesGrid.Add(new List<Tile>());
                for (int y = 0; y < pY; y++)
                {
                    lTile =(Tile)SpawnGameObject(tileScene, gridManager.GetPosition(new Vector2(y, i)), gridManager.tileSize, BASE_TILE_ZINDEX + y, pContainer);
                    gridManager.tilesGrid[i].Add(lTile);
                }
                
            }GD.Print(gridManager.tilesGrid.Count);
        }
    }
    
}
