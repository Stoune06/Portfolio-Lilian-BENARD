using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class GridManager : Manager
	{
		#region Singleton
		static private GridManager instance;
		private GridManager() { }

		static public GridManager GetInstance()
		{
			if(instance == null) instance = new GridManager();
			return instance;
		}

        #endregion

        [Export] private SubViewport gridRender;
        [Export] private Sprite2D grid;
        [Export] private Camera2D camera;
        [Export] private ColorRect border;

		[Export] private AnimatedSprite2D topBorder, bottomBorder, leftBorder, rightBorder , globalTopBorder, globalBottomBorder, globalLeftBorder, globalRightBorder;

        public List<List<Target>> targetList = new List<List<Target>>(); // Different list of target to not move them with the others
		public List<List<Movable>> movableObjectsOnGrid = new List<List<Movable>>();
		public List<List<Tile>> tilesGrid = new List<List<Tile>>();

		public int maxX { get; private set; } = 0;
		public int maxY { get; private set; } = 0;

		public Tile currentTile;
        public Tile movingToTile;
        private Vector2I mouseIndex;
		public Vector2 tileSize;
		public Vector2 origin = new Vector2();
		private Vector2 screenSize = new Vector2();

		private ScreenManager screenManager;
		private GameManager gameManager;
		private InputManager inputManager;

		public int targetNum;
		private float tileSizeFactor = 0.8f;
		private float viewportPosFactor = 0.5f;
		
		
        public override void Init()
        {
            base.Init();
			screenManager = ScreenManager.GetInstance();
			gameManager = GameManager.GetInstance();
			inputManager = InputManager.GetInstance();
			inputManager.MoveToClick += TurnGreenTween;
        }
        public override void _Ready()
		{
			#region Singleton
			if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(GridManager) + "instance already exists, destroying the last added");
				return;
			}

			instance = this;
			#endregion
		}
        public void TurnGreenTween(Vector2 pPosition)
        {
            if (InputManager.canPlay && currentTile != null && currentTile.cadre != null && GodotObject.IsInstanceValid(currentTile.cadre) && GodotObject.IsInstanceValid(currentTile))
            {
                currentTile.cadre.Modulate = currentTile.greenColor;
			if (movingToTile != null && movingToTile.isGreen)
				{
					movingToTile.cadre.Modulate = movingToTile.baseColor;
					movingToTile.cadre.Visible = false;
					movingToTile.isGreen = false;
                }
				currentTile.isGreen = true;
				movingToTile = currentTile;
	
            }

        }
       
        public void GenerateGrid(int pX, int pY)
		{
			maxX = pX-1; maxY = pY-1;
            for (int x  = 0; x < pX; x++)
			{
				movableObjectsOnGrid.Add(new List<Movable>());
				for(int y = 0; y < pY; y++)
				{
					movableObjectsOnGrid[x].Add(null);
				}
			}
		}

		public Movable GetObject(Vector2 pIndex) // Give the reference of the objects placed on the given index
		{
			return movableObjectsOnGrid[(int)pIndex.X][(int)pIndex.Y];
		}

		public bool IsOnGrid(Vector2 pPosition)
		{
			return (pPosition.X <= maxX && pPosition.X >= 0 && pPosition.Y <= maxY && pPosition.Y >= 0);

        }

		public Vector2I GetIndexOnGrid(GameObject pObject)
		{
			int lLength = movableObjectsOnGrid.Count;
			for(int x = lLength - 1; x >= 0; x--)
			{
				for(int y = movableObjectsOnGrid[x].Count - 1; y >= 0; y--)
				{
					if (movableObjectsOnGrid[x][y] == pObject) return new Vector2I(x, y);
				}
			}
			GD.PrintErr("Object not found on the grid");
			return Vector2I.Zero;
		}

		public Vector2I GetIndexOnGrid(Tile pTile)
		{
			int lLength = tilesGrid.Count;
            for (int x = lLength - 1; x >= 0; x--)
            {
                for (int y = tilesGrid[x].Count - 1; y >= 0; y--)
                {
                    if (tilesGrid[y][x] == pTile) return new Vector2I(x, y);
                }
            }
            GD.PrintErr("Object not found on the grid");
            return Vector2I.Zero;
		}

		public Vector2I GetIndexOnGrid(Vector2 pPosition)
		{
			int lXPos = Mathf.RoundToInt((pPosition.X - origin.X) / tileSize.X);
			int lYPos = Mathf.RoundToInt((pPosition.Y - origin.Y) / tileSize.Y);
            return new Vector2I(lXPos,lYPos);
		}

		public Vector2 GetPosition(Vector2 pIndex)
		{
			return (pIndex * tileSize + origin);
		}

		public Vector2 GetPosition(int pX, int pY)
		{
			return new Vector2(pX, pY) * tileSize + origin;
		}

        /// <summary>
        /// Update the origin position depending on the tileSize and the screenSize
        /// </summary>
        public void UpdateOrigin()
		{
			float lGridWidth = (maxX + 1) * tileSize.X;
			float lGridHeight = (maxY + 1) * tileSize.Y;

			origin = new Vector2((screenManager.screenSize.X - lGridWidth) / 2f, (screenManager.screenSize.Y - lGridHeight + tileSize.Y * 0.4f) / 2f);
        }

		/// <summary>
		/// Update all previous positions of the movables on the grid
		/// </summary>
		public void UpdatePreviousPositions()
		{
			int lLength = movableObjectsOnGrid.Count;
			for (int x = 0; x < lLength; x++)
			{
				for (int y = 0; y < movableObjectsOnGrid[x].Count; y++)
				{
					if (movableObjectsOnGrid[x][y] != null)
					{
						movableObjectsOnGrid[x][y].previousIndex = new Vector2(x, y);
					}
                    
                }
			}
		}

		/// <summary>
		/// Return all movables object forward pPos in the direction pDirection
		/// </summary>
		public List<Movable> GetMovablesForward(Vector2 pPos, Vector2 pDirection)
		{
            List<Movable> lFoundMovables = new List<Movable>();

            Vector2 lNextPos = pPos + pDirection;
			
			while (lNextPos.X >= 0 && lNextPos.X <= maxX && lNextPos.Y >= 0 && lNextPos.Y <= maxY)
			{
                lFoundMovables.Add(GetObject(lNextPos));
				lNextPos += pDirection;
			}

			return lFoundMovables;
        }

		/// <summary>
		/// Update the viewport renderer's position and size with the tilesize and the origin
		/// </summary>
		public void UpdateViewport()
		{

            float lXWitdth = (maxX+1) * tileSize.X;
            float lYWitdth = (maxY+1) * tileSize.Y;

            gridRender.Size = (Vector2I)new Vector2(lXWitdth, lYWitdth + tileSize.Y * 0.4f);

			grid.GlobalPosition = new Vector2((screenManager.screenSize.X - lXWitdth) / 2f, (screenManager.screenSize.Y - lYWitdth - tileSize.Y) / 2f);

			leftBorder.GlobalPosition = globalLeftBorder.GlobalPosition = grid.GlobalPosition + Vector2.Up * tileSize.Y/2f + Vector2.Right * tileSize/2f;
			leftBorder.Scale = globalLeftBorder.Scale = Vector2.One * (maxX / 6f);

            topBorder.GlobalPosition = globalTopBorder.GlobalPosition = grid.GlobalPosition + Vector2.Left * tileSize.X/2f + Vector2.Up * tileSize.Y/2f;
            topBorder.Scale = globalTopBorder.Scale = Vector2.One * (maxX / 6f);

            rightBorder.GlobalPosition  = globalRightBorder.GlobalPosition = grid.GlobalPosition + new Vector2(lXWitdth - tileSize.X/2f,lYWitdth) + Vector2.Down * tileSize.Y/2f;
            rightBorder.Scale = globalRightBorder.Scale = Vector2.One * (maxX / 6f);

            bottomBorder.GlobalPosition = globalBottomBorder.GlobalPosition = grid.GlobalPosition + new Vector2(0,lYWitdth) + Vector2.Left * tileSize.X / 2f;
            bottomBorder.Scale = globalBottomBorder.Scale = Vector2.One * (maxX / 6f);

            camera.Offset = new Vector2((screenManager.screenSize.X - lXWitdth) / 2f, (screenManager.screenSize.Y - lYWitdth) / 2f - tileSize.Y * 0.4f);
        }

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
