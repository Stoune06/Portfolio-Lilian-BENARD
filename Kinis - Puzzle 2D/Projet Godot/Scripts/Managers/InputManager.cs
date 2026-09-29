using Com.IsartDigital.Kinisi;
using Godot;
using System;

// Author : Sara Astuti-Aucher & Louis Robin

namespace Com.IsartDigital.ProjectName {
	
	public partial class InputManager : Manager
	{
        #region Singleton
        static private InputManager instance;
        private InputManager() { }

        static public InputManager GetInstance()
        {
            if (instance == null) instance = new InputManager();
            return instance;
        }

        #endregion
        private const string PATH_ACTION_UP = "Up";
        private const string PATH_ACTION_DOWN = "Down";
        private const string PATH_ACTION_LEFT = "Left";
        private const string PATH_ACTION_RIGHT = "Right";
        private const string PATH_ACTION_CLICK = "Click";
        private const string PATH_ACTION_SF = "SF";

        public static bool canPlay = false;
        private bool isChoosingDirection = false;

        public Vector2 lastDirection = Vector2.Up;

        [Signal] public delegate void InputDirectionPressedEventHandler(Vector2 pDirection);
        [Signal] public delegate void MoveToClickEventHandler(Vector2 pPosition);
        [Signal] public delegate void InputSFEventHandler(Vector2 pDirection);

        public override void _Ready()
		{
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(InputManager) + "instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion
            base._Ready();
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

        public override void _Input(InputEvent pEvent)
        {
            base._Input(pEvent);

            if (pEvent is InputEventKey)
            {
                if (canPlay)
                {
                    if (Input.IsActionJustPressed(PATH_ACTION_SF))
                    {
                        isChoosingDirection = true;
                        GameManager.GetInstance().SetChargeMode(isChoosingDirection);

                    }
                    Vector2 lDirection = Vector2.Zero;
                    if (Input.IsActionJustPressed(PATH_ACTION_UP))
                    {
                        lDirection = Vector2.Up;
                    }
                    else if (Input.IsActionJustPressed(PATH_ACTION_DOWN))
                    {
                        lDirection = Vector2.Down;
                    }
                    else if (Input.IsActionJustPressed(PATH_ACTION_RIGHT))
                    {
                        lDirection = Vector2.Right;
                    }
                    else if (Input.IsActionJustPressed(PATH_ACTION_LEFT))
                    {
                        lDirection = Vector2.Left;
                    }
                    if (lDirection != Vector2.Zero)
                    {
                        lastDirection = lDirection;
                        if (!isChoosingDirection) EmitSignal(SignalName.InputDirectionPressed, lastDirection);
                        else 
                        {
                            EmitSignal(SignalName.InputSF, lastDirection);
                            GameManager.GetInstance().playerParticle.Position = Player.GetInstance().Position + new Vector2(40,0);
                            GameManager.GetInstance().playerParticle.Emitting = true;
                        }
                    }
                }               
                if (Input.IsActionJustReleased(PATH_ACTION_SF) && isChoosingDirection)
                {
                    
                    isChoosingDirection = false;
                    GameManager.GetInstance().SetChargeMode(isChoosingDirection);
                }
            }

            else if (canPlay && pEvent is InputEventMouseButton && Input.IsActionJustReleased(PATH_ACTION_CLICK))
            {
                EmitSignal(SignalName.MoveToClick, GetViewport().GetMousePosition());
                
            }

        }

        public override void _UnhandledInput(InputEvent @event) // provisoire
        {
            base._UnhandledInput(@event);
            if (@event is InputEventKey lInput && lInput.IsPressed())
            {
                //if (lInput.Keycode == Key.Space) Tile.TileSpawnAnimation();
            }
        }

        
                

        protected override void Dispose(bool pDisposing)
		{
            instance = null;
            base.Dispose(pDisposing);
        }
	}
}
