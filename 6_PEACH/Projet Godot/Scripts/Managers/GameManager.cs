using Com.IsartDigital.Kinisi.Login;
using Com.IsartDigital.Kinisi.Menu;
using Com.IsartDigital.Utils.Effects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Com.IsartDigital.Kinisi;

//author Lilian Benard & Louis Robin

namespace Com.IsartDigital.ProjectName
{
	public partial class GameManager : Manager
	{
		#region Singleton
		static private GameManager instance;
		private GameManager() { }

		static public GameManager GetInstance()
		{
			if(instance == null) instance = new GameManager();
			return instance;
		}

		#endregion
		[Export] private Manager[] managers;
		[Export] public Node2D gameContainer;
		[Export] private PackedScene sceneLogin;
		[Export] private PackedScene sceneMenu; //Can be erased when the login is reactivated
        [Export] private PackedScene sceneTitleCard;
        [Export] private PackedScene sceneWinScreen;
        [Export] public Shaker screenShake; //added by Hector
        [Export] public Shaker screenShake2;
        [Export] public Shaker screenShake3;
        [Export] public Vector2 tileSize = new Vector2(120f, 120f);
        [Export] public Camera2D globalCamera;
        [Export] public GpuParticles2D playerParticle;

        private ShaderMaterial furyMod;
        [Export] private ColorRect shader;
        [Export] private Sprite2D viewportBackground;
        [Export] private LoadingScreen transitionRect;

        public Player player;
        private GridManager gridManager;
        private InputManager inputManager;
        private HUDManager hudManager;
        public const float OBJECT_MOVE_TIME = 0.2f;
        public static bool isPlaying = false;

        private bool fastMode = false; // Enable it in order to skip directly to the fastLevel
        private int fastLevel = 1;

        private static Stack<List<List<Movable>>> undoStack = new Stack<List<List<Movable>>>();
        private static Stack<List<List<Movable>>> redoStack = new Stack<List<List<Movable>>>();

        public static int currentPar = 0;
        public static int stepsNum = 0;
        public static int starsNum = 0;
        public static int score;

        public static int activatedTargetsNum = 0;

        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(GameManager) + "instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
            CheckAllManagersReady();
            gridManager = GridManager.GetInstance();
            inputManager = InputManager.GetInstance();
            hudManager = HUDManager.GetInstance();

            TitleCard lTitleCard = sceneTitleCard.Instantiate<TitleCard>();
            AddChild(lTitleCard);

            //AddLogin();
            furyMod = (ShaderMaterial)shader.Material;
        }

        public override void _Process(double delta)
        {
            Vector2 lPlayerPostion = default;
            if (player != null && isPlaying)
            {
                lPlayerPostion = globalCamera.GetViewport().GetFinalTransform().AffineInverse() * (player.marker.GlobalPosition + 100 * Vector2.Up);
                //GD.Print("Shader player_screen_pos = ", lPlayerPostion);
                furyMod.SetShaderParameter("player_screen_pos", lPlayerPostion);
                float time = Time.GetTicksMsec() / 1000f;
                furyMod.SetShaderParameter("time", time);

            }

            if (viewportBackground != null && viewportBackground.GlobalPosition != Vector2.Zero) viewportBackground.GlobalPosition = Vector2.Zero;
            
        }

        public void AddLogin()
        {
            Login lLogin = sceneLogin.Instantiate() as Login;
            AddChild(lLogin);
            AudioManager.PlaySound("Loading",true);
        }

        /// <summary>
        /// Update physics postion depending on the movableOnGrid list
        /// </summary>
        public void UpdatePositions()
        {
            int lLength = gridManager.movableObjectsOnGrid.Count;
            Vector2 lMovement;
            Vector2 lIndex;
            PackedScene lPackedScene = default;
            Movable lNewMovable;
            for (int x = 0; x < lLength; x++)
            {
                for (int y = 0; y < gridManager.movableObjectsOnGrid[x].Count; y++)
                {
                    if (gridManager.movableObjectsOnGrid[x][y] is Movable lMovableObject)
                    {
                        lIndex = new Vector2(x, y);

                        if (lIndex == lMovableObject.previousIndex)
                        {
                            if (lMovableObject is Player lPlayer) lPlayer.lastDistanceTraveled = 0; 
                            continue;
                        }

                        lMovement = lIndex - lMovableObject.previousIndex;
                        if (lMovement.LengthSquared() == 1 || lMovableObject is Player) lMovableObject.MoveObject(gridManager.GetPosition(lIndex), OBJECT_MOVE_TIME); // If distance between the previous pos and the next pos is equal to 1 simple travel

                        else //else create a new movable and delete it to simulate a teleportation to the other side
                        {
                            if (lMovableObject is Wall)
                            {
                                lPackedScene = Spawner.wallFactory;
                            }
                            else if (lMovableObject is Box)
                            {
                                lPackedScene = Spawner.boxFactory;
                            }

                            lIndex = lMovableObject.previousIndex - lMovement.Normalized();
                            lNewMovable = (Movable)Spawner.SpawnGameObject(lPackedScene, lMovableObject.Position, gridManager.tileSize, y + 1);
                            lNewMovable.MoveObject(gridManager.GetPosition(lIndex), OBJECT_MOVE_TIME);
                            lNewMovable.toDelete = true;
                            lNewMovable.ZIndex = (int)(lIndex - lMovement.Normalized()).Y;

                            lIndex = new Vector2(x, y);
                            lMovableObject.Position = gridManager.GetPosition(lIndex + lMovement.Normalized());
                            lMovableObject.MoveObject(gridManager.GetPosition(lIndex), OBJECT_MOVE_TIME);
                            lMovableObject.previousIndex = lIndex;
                            
                        }
                     gridManager.movableObjectsOnGrid[x][y].ZIndex = y + 1;
                        
                    }
                }
            }
            gridManager.UpdatePreviousPositions();
        }

        /// <summary>
        /// Move all movables on the grid in pDirection teleporting some movables to the next side
        /// </summary>
        public void MoveAllObjectsOnGrid(Vector2 pDirection)
        {
            Vector2I lPlayerIndex = gridManager.GetIndexOnGrid(player);
            Vector2I lPlayerLastIndex = (Vector2I)player.previousIndex;


            gridManager.UpdatePreviousPositions();
            screenShake2.Start();

            int lLength = gridManager.movableObjectsOnGrid.Count;
            List<List<Movable>> lMovables = gridManager.movableObjectsOnGrid;

            if (pDirection.Y != 0)
            {
                for (int x = 0; x < lLength; x++)
                {
                    for (int y = 1; y < lMovables[x].Count; y++)
                    {
                        if (pDirection.Y > 0) (lMovables[x][0], lMovables[x][y]) = (lMovables[x][y], lMovables[x][0]);
                        else (lMovables[x][lMovables[x].Count - 1], lMovables[x][lMovables[x].Count - y - 1]) = (lMovables[x][lMovables[x].Count - y - 1], lMovables[x][lMovables[x].Count - 1]);
                    }
                }
            }
            else for (int x = 1; x < lLength; x++)
                {
                    if (pDirection.X > 0) (lMovables[0], lMovables[x]) = (lMovables[x], lMovables[0]);
                    else (lMovables[lMovables.Count - 1], lMovables[lMovables.Count - x - 1]) = (lMovables[lMovables.Count - x - 1], lMovables[lMovables.Count - 1]);
                }

            lPlayerIndex = gridManager.GetIndexOnGrid(player);
            lMovables[lPlayerIndex.X][lPlayerIndex.Y] = null;
            lMovables[lPlayerLastIndex.X][lPlayerLastIndex.Y] = player;

            UpdatePositions();
        }

        public void MoveObjectOnGrid(Movable pObject, Vector2 pDirection)
        {
            pObject.previousIndex = gridManager.GetIndexOnGrid(pObject);
            int lXIndex = gridManager.GetIndexOnGrid(pObject).X;
            int lYIndex = gridManager.GetIndexOnGrid(pObject).Y;
            Vector2 lNextPos = gridManager.GetIndexOnGrid(pObject) + pDirection;

            if (!gridManager.IsOnGrid(lNextPos) || gridManager.movableObjectsOnGrid[(int)lNextPos.X][(int)lNextPos.Y] != null) return;
            (gridManager.movableObjectsOnGrid[(int)lNextPos.X][(int)lNextPos.Y], gridManager.movableObjectsOnGrid[lXIndex][lYIndex]) = (gridManager.movableObjectsOnGrid[lXIndex][lYIndex], gridManager.movableObjectsOnGrid[(int)lNextPos.X][(int)lNextPos.Y]);
            if (pObject is Player)
            {
                stepsNum++;
                hudManager.UpdateStep(stepsNum);
                redoStack.Clear();
                player.lastDirection = pDirection;
            }

            UpdatePositions();
        }

        public void SaveLastGridState()
        {
            List<List<Movable>> lMovableObjects = new List<List<Movable>>();
            foreach (List<Movable> lList in gridManager.movableObjectsOnGrid) lMovableObjects.Add(new List<Movable>(lList));

            undoStack.Push(lMovableObjects);
        }

        public void Undo() //Moves all the movables to where they were in the last grid state
        {
            if (undoStack.Count > 0)
            {
                redoStack.Push(gridManager.movableObjectsOnGrid);
                gridManager.UpdatePreviousPositions();
                gridManager.movableObjectsOnGrid = undoStack.Pop();

                UpdatePositions();
                stepsNum -= player.lastDistanceTraveled;
                hudManager.UpdateStep(stepsNum);
            }
        }

        public void Redo() //Moves all the movables to where they were in the last undo grid state
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(gridManager.movableObjectsOnGrid);
                gridManager.UpdatePreviousPositions();
                gridManager.movableObjectsOnGrid = redoStack.Pop();

                UpdatePositions();
                stepsNum += player.lastDistanceTraveled;
                hudManager.UpdateStep(stepsNum);
            }
        }

        public void Retry()
        {
            if (undoStack.Count > 0)
            {
                gridManager.movableObjectsOnGrid = undoStack.Last();
                UpdatePositions();
                ResetLevelData();
                hudManager.UpdateStep(stepsNum);
            }
        }

        public static void ResetLevelData()
        {
            undoStack.Clear();
            redoStack.Clear();  

            stepsNum = 0;
            starsNum = 1;
            activatedTargetsNum = 0;
        }

        public void CheckTargetsActivation() //To check if the Targets are activated by boxes or not
        {
            if (gridManager.targetNum == activatedTargetsNum)
            {
                WinScreen lWinScreen = (WinScreen) sceneWinScreen.Instantiate();
                AddChild(lWinScreen);
                Win();
                lWinScreen.ShowScore(starsNum);
            }
        }

        private void Win()
        {
            if (stepsNum <= currentPar)
            {
                starsNum = 3;
                score += 5000 - stepsNum;
            }
            else if (stepsNum < currentPar * 1.5f)
            {
                starsNum = 2;
                score += 2000 - stepsNum;
            }
            else
            {
                starsNum = 1;
                score += 1000 - stepsNum;
            }
        }

        public void ToggleChargeMode(bool isCharging)
        {
            furyMod.SetShaderParameter("is_charge_mode", isCharging);
            GD.Print(furyMod.GetShaderParameter("is_charge_mode"));
        }

        public void SetChargeMode(bool isCharging)
        {
            // Cree un nouveau tween
            Tween lTween = GetTree().CreateTween();
            float target = isCharging ? 1.0f : 0.0f;
            float duration = isCharging ? 0.2f : 0.6f; // fondu plus lent en sortie

            lTween.TweenProperty(
                furyMod,
                "shader_parameter/fade_amount",
                target,
                duration
            );
        }

        public Tween StartTransition()
        {
            Tween lTween = CreateTween();
            GD.Print("Start Transition");
            transitionRect.rightBand.Visible = transitionRect.leftBand.Visible = true;
            transitionRect.GlobalPosition = new Vector2(-10, transitionRect.GlobalPosition.Y);

            
            lTween.TweenProperty(transitionRect, "position", new Vector2(transitionRect.GlobalPosition.X, transitionRect.GlobalPosition.Y + transitionRect.Size.Y),0.5d);
            return lTween;
        }

        public Tween EndTransition()
        {
            Tween lTween = CreateTween();
            GD.Print("End Transition");
            transitionRect.GlobalPosition = Vector2.Zero;
            transitionRect.Size = GetWindow().Size + 100 * Vector2.Right;
            lTween.TweenProperty(transitionRect, "position", new Vector2(transitionRect.GlobalPosition.X,-transitionRect.Size.Y), 1d).Finished += () => transitionRect.rightBand.Visible = transitionRect.leftBand.Visible = false;
            return lTween;
        }

            private async void CheckAllManagersReady()
        {
            // wait after each manager to be ready inside the Tree
            while (!AreAllManagersReady())
            {
                await ToSignal(GetTree(), "process_frame");
            }

            // call each Init function of every manager
            foreach (Manager manager in managers)
            {
                manager.Init();
            }
        }

        private bool AreAllManagersReady()
        {
            foreach (Manager manager in managers)
            {
                if (!manager.IsInsideTree())
                {
                    return false;
                }
            }

            return true;
        }
        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
