using Com.IsartDigital.Kinisi;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Louis Robin 

namespace Com.IsartDigital.ProjectName
{
	public partial class Movable : GameObject
    {
        [Signal] public delegate void MovementFinishedEventHandler();

        public static List<Movable> allMovables = new List<Movable>();
        public static PackedScene fallParticlesScene = (PackedScene)ResourceLoader.Load("res://Scenes/SFX/Particles/WallFallParticles.tscn");

        private static float landingDuration = 0.5f;

        public bool isMoving = false;
        public bool toDelete = false;

        private float moveDuration;
        private float moveTimer;

        public Vector2 startPosition;
        public Vector2 targetPosition;
        public Vector2 previousIndex;


        private const float BASE_RADIUS = 40;
        public override void _Ready()
        {
            base._Ready();
            allMovables.Add(this);
            if (!GameManager.isPlaying)  Position = new Vector2(endPos.X, yOffset); 
            else Position = endPos;

            MovementFinished += StopMove;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
            if (isMoving)
            {
                moveTimer += lDelta;
                float lMoveFactor = moveTimer /moveDuration;

                if (lMoveFactor >= 1f)
                {
                    lMoveFactor = 1f;
                    EmitSignal(SignalName.MovementFinished);
                }

                Position = startPosition.Lerp(targetPosition, lMoveFactor);
            }
        }

        public virtual void MoveObject(Vector2 pEndPosition, float pDuration = 0.2f)
        {
            moveTimer = 0f;
            startPosition = Position;
            targetPosition = pEndPosition;
            moveDuration = pDuration;

            isMoving = true;
        }

        protected virtual void StopMove()
        {
            isMoving = false;
            if (toDelete) QueueFree();
        }

        public static void MovablesSpawnAnimation()
        {
            
            Tween lTween = GameManager.GetInstance().CreateTween();
            foreach (Movable lMovable in allMovables)
            {
                if(lMovable is Player)
                {
                    lMovable.SetAnimation(FALL_STATE); 
                }
                if (lMovable is Box lBox) lTween.Finished += lBox.CheckActivation;
                lTween.Parallel().TweenProperty(lMovable, TweenProp.POSITION, lMovable.endPos, landingDuration).SetDelay(GD.Randf()).Finished += lMovable.Landing;
            }
            lTween.Finished += () => InputManager.canPlay = true;
        }

        private void Landing()
        {
            if (GD.Randf() < 0.5) AudioManager.PlaySound("SpawnWall", true);
            else AudioManager.PlaySound("SpawnWall2", true);
            Particle lParticle = Spawner.SpawnParticle(fallParticlesScene, marker);
            //lParticle.particleProcessMaterial.EmissionSphereRadius = Mathf.Clamp((BASE_RADIUS * (-GridManager.GetInstance().maxX + 10)),0,300);
            //lParticle.Scale = Vector2.One * (2 - 1f/ GridManager.GetInstance().maxX);
            lParticle.Emitting = true;
            gameManager.screenShake2.Start();
        }
    }
}
