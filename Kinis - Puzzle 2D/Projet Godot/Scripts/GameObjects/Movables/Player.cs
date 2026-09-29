using Com.IsartDigital.Kinisi;
using Com.IsartDigital.Utils.Effects;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Author : Sara Astuti-Aucher & Louis Robin

namespace Com.IsartDigital.ProjectName
{
    public partial class Player : Movable
    {
        #region // ----- Singleton ----- \\

        static private Player instance;

        static public Player GetInstance()
        {
            if (instance == null) instance = new Player();
            return instance;
        }

        #endregion
        [Export] private PackedScene particleStepScene;
        private ShaderMaterial flashShader;

        private InputManager inputManager;
        private GridManager gridManager;
        private GameManager gameManager;

        private int pathFindExecutionIndex = 0;
        private const int TIME_BETWEEN_MOVES = (int)(GameManager.OBJECT_MOVE_TIME * 1100);
        private const int SF_DELAY = (int)(GameManager.OBJECT_MOVE_TIME * 1000);

        public int stepsNum = 0;
        public int lastDistanceTraveled = 0;

        private const float STUN_TIME = 0.5f;

        public Vector2 lastDirection = Vector2.Up;
        private bool isFlashing = false;

        public override void _Ready()
        {
            #region // ----- Singleton ----- \\

            if (instance != null)
            {
                GD.Print(Name + " Instance already exist, destroying the last added.");
                QueueFree();
                return;
            }

            instance = this;

            #endregion

            base._Ready();
            inputManager = InputManager.GetInstance();
            gridManager = GridManager.GetInstance();
            gameManager = GameManager.GetInstance();

            gameManager.player = this;
            inputManager.InputDirectionPressed += TryMoveTowards;
            inputManager.MoveToClick += MoveToMouseClick;
            inputManager.InputSF += Dash;
            MovementFinished += OnMovementFinished;
            inputManager.lastDirection = lastDirection = Vector2.Up;
            flashShader = (ShaderMaterial)outRenderer.Material;
        }
        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
            base._Process(lDelta);

        }


        public void TryMoveTowards(Vector2 pDirection) // moves to the direction pDirection if it's possible, if there is a box it pushes it
        {
            if (isMoving) return;
            Vector2 lPos;
            lPos = gridManager.GetIndexOnGrid(this);
            lPos += pDirection;
            StepNoise();
            if (!gridManager.IsOnGrid(lPos) || pDirection == Vector2.Zero) return;
            Particle lParticle = Spawner.SpawnParticle(particleStepScene, marker);
            lParticle.Emitting = true;
            lParticle.Finished += lParticle.QueueFree;
            Movable lObj = gridManager.GetObject(lPos);
            if (!(lObj is Wall))
            {
                SetAnimation(GetWalkAnimationState(pDirection));
                gameManager.SaveLastGridState();
                if (lObj is Box)
                {
                    SetAnimation(GetPushAnimationState(pDirection));
                    gameManager.MoveObjectOnGrid(lObj, pDirection);
                }
                gameManager.MoveObjectOnGrid(this, pDirection);
            }

        }

        public async void MoveToMouseClick(Vector2 pTargetPosition) // void called at every click for the player to move to the click position (written by Louis)
        {
            pathFindExecutionIndex++;
            int lThisExeIndex = pathFindExecutionIndex;
            Vector2 lTargetPosition = pTargetPosition - Vector2.Up * gridManager.tileSize * 0.1f;
            Vector2I lPlayerIndex = gridManager.GetIndexOnGrid(this);
            Vector2I lTargetIndex = (Vector2I)((lTargetPosition - gridManager.origin) / gridManager.tileSize);

            if (lTargetIndex.X >= 0 && lTargetIndex.Y >= 0 && lTargetIndex.X <= gridManager.maxX && lTargetIndex.Y <= gridManager.maxY)
            {
                Movable lClickedObject = gridManager.GetObject(lTargetIndex);
                List<Vector2> lPath = PathFinding.GetPath(gridManager.movableObjectsOnGrid, lPlayerIndex.Y, lPlayerIndex.X, lTargetIndex.Y, lTargetIndex.X, lClickedObject);
                int lLenght = lPath.Count;

                for (int i = lLenght - 1; i >= 0; i--)
                {
                    if (pathFindExecutionIndex != lThisExeIndex) return;
                    lPlayerIndex = gridManager.GetIndexOnGrid(this);
                    TryMoveTowards(new Vector2(lPath[i].Y, lPath[i].X) - lPlayerIndex);
                    await Task.Delay(TIME_BETWEEN_MOVES);
                }
            }
        }

        private async void Dash(Vector2 pDirection) //Special Feature, if there is a wall in the direction pDirection, dashes to the wall and call a void to push the whole level
        {
            if (isMoving) return;
            lastDirection = pDirection;
            SetAnimation(GetSFAnimationState(pDirection));
            List<Movable> lMovablesForward = gridManager.GetMovablesForward(gridManager.GetIndexOnGrid(this), pDirection);
            bool lIsWallForward = false;
            bool lIsBoxForward = false;

            DashSFX();

            int lNumTileToMove = 0;
            foreach (Movable lMovable in lMovablesForward)
            {
                if (lMovable is Wall)
                {
                    if (lNumTileToMove > 0) lIsWallForward = true;
                    break;
                }
                if (lMovable is Box)
                {
                    Vector2I lBoxMoveIndex = gridManager.GetIndexOnGrid(lMovable) + (Vector2I)pDirection;
                    if (gridManager.IsOnGrid(lBoxMoveIndex) && gridManager.movableObjectsOnGrid[lBoxMoveIndex.X][lBoxMoveIndex.Y] == null) lIsBoxForward = true;
                    break;
                }
                lNumTileToMove++;
            }
            if (lNumTileToMove > 0 || lIsBoxForward) gameManager.SaveLastGridState();

            for (int i = 0; i < lNumTileToMove; i++)
            {
                gameManager.MoveObjectOnGrid(this, pDirection);
            }
            await Task.Delay(SF_DELAY);
            FlashWhiteOnWallHit();
            FreezeFrame(0.06f);
            if (lIsWallForward) gameManager.MoveAllObjectsOnGrid(pDirection);
            else if (lIsBoxForward) gameManager.MoveObjectOnGrid(gridManager.GetObject(gridManager.GetIndexOnGrid(this) + pDirection), pDirection); 
            else Stun(pDirection);
            Tween lTween = CreateTween();
            lTween.TweenProperty(gameManager.globalCamera,"position",-pDirection * 20,0.2);
            lTween.TweenProperty(gameManager.globalCamera, "position", Vector2.Zero, 0.1);
        }


        private void Stun(Vector2 pDirection)
        {
            SetAnimation(GetStunAnimationState(pDirection));
            InputManager.canPlay = false;
            Timer lTimer = new Timer();
            AddChild(lTimer);
            lTimer.OneShot = true;
            lTimer.WaitTime = STUN_TIME;
            lTimer.Timeout += () => InputManager.canPlay = true;
            lTimer.Timeout += lTimer.QueueFree;
            lTimer.Start();

        }

        async void FreezeFrame(float pDuration)
        {
            Engine.TimeScale = 0.0001f;
            Modulate = Colors.White;
            await Task.Delay((int)(pDuration * 1000));
            Engine.TimeScale = 1.0f;
        }


        public override void MoveObject(Vector2 pEndPosition, float pDuration = 0.2f)
        {
            base.MoveObject(pEndPosition, pDuration);
            Vector2I lDistance = gridManager.GetIndexOnGrid(targetPosition) - gridManager.GetIndexOnGrid(startPosition);
            lastDistanceTraveled = Mathf.Abs(lDistance.X) + Mathf.Abs(lDistance.Y);
        }

        private void StepNoise()
        {
            switch (GD.RandRange(0, 3))
            {
                case 0:
                    AudioManager.PlaySound("Step", true);
                    break;
                case 1:
                    AudioManager.PlaySound("Step2", true);
                    break;
                case 2:
                    AudioManager.PlaySound("Step3", true);
                    break;
                case 3:
                    AudioManager.PlaySound("Step4", true);
                    break;
            }
        }

        private void DashSFX()
        {
            switch (GD.RandRange(0, 4))
            {
                case 0:
                    AudioManager.PlaySound("Dash", true);
                    break;
                case 1:
                    AudioManager.PlaySound("Dash2", true);
                    break;
                case 2:
                    AudioManager.PlaySound("Dash3", true);
                    break;
                case 3:
                    AudioManager.PlaySound("Dash4", true);
                    break;
                case 4:
                    AudioManager.PlaySound("Dash5", true);
                    break;
            }
        }
        private void OnMovementFinished()
        {
            if (IsAnimationFinished() || WasWalkingAnimation(currentState))
            {
                SetAnimation(GetIdleAnimationState(lastDirection));
            }
        }
        private bool WasWalkingAnimation(string state)
        {
            return state == WALK_UP_STATE || state == WALK_DOWN_STATE ||
                   state == WALK_LEFT_STATE || state == WALK_RIGHT_STATE;
        }

        private bool IsAnimationFinished()
        {
            return !outRenderer.IsPlaying(); // Vérifie si l'animation actuelle est terminée
        }
        private string GetIdleAnimationState(Vector2 direction)
        {
            if (direction == Vector2.Up) return IDLE_UP_STATE; 
            if (direction == Vector2.Down) return IDLE_DOWN_STATE;
            if (direction == Vector2.Left) return IDLE_LEFT_STATE;
            return IDLE_RIGHT_STATE;
        }

        private string GetWalkAnimationState(Vector2 direction)
        {
            if (direction == Vector2.Up) { lastDirection = Vector2.Up; return WALK_UP_STATE; }
            if (direction == Vector2.Down) { lastDirection = Vector2.Down; return WALK_DOWN_STATE; }
            if (direction == Vector2.Left) { lastDirection = Vector2.Left; return WALK_LEFT_STATE; }
            lastDirection = Vector2.Right;
            return WALK_RIGHT_STATE;
        }

        private string GetPushAnimationState(Vector2 direction)
        {
            if (direction == Vector2.Up) return PUSH_UP_STATE;
            if (direction == Vector2.Down) return PUSH_DOWN_STATE;
            if (direction == Vector2.Left) return PUSH_LEFT_STATE;
            return PUSH_RIGHT_STATE;
        }

        private string GetSFAnimationState(Vector2 direction)
        {
            if (direction == Vector2.Up) return SF_UP_STATE;
            if (direction == Vector2.Down) return SF_DOWN_STATE;
            if (direction == Vector2.Left) return SF_LEFT_STATE;
            return SF_RIGHT_STATE;
        }
        private string GetStunAnimationState(Vector2 direction)
        {
            if (direction == Vector2.Up) return STUNNED_UP_STATE;
            if (direction == Vector2.Down) return STUNNED_DOWN_STATE;
            if (direction == Vector2.Left) return STUNNED_LEFT_STATE;
            return STUNNED_RIGHT_STATE;
        }

        public async void FlashWhiteOnWallHit()
        {
            if (isFlashing) return;

            isFlashing = true;
            flashShader.SetShaderParameter("flash_amount", 1.0f);

            await ToSignal(GetTree().CreateTimer(0.08f), "timeout");

            flashShader.SetShaderParameter("flash_amount", 0.0f);
            isFlashing = false;
        }

        protected override void Dispose(bool pDisposing)
        {
            inputManager.InputDirectionPressed -= TryMoveTowards;
            inputManager.MoveToClick -= MoveToMouseClick;
            inputManager.InputSF -= Dash;
            #region // ----- Singleton ----- \\

            if (pDisposing && instance == this) instance = null;

            #endregion

            base.Dispose(pDisposing);
        }
       
    }
}
