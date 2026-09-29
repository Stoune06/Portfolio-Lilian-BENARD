using Godot;
using System;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
    public partial class HoldType : IngredientScene
    {
        [Export] protected ColorRect renderer;
        [Export] private PackedScene liquidParcilesScene;
        [Export] private float incrementInterval = 0.05f;
        [Export] private float maxRotation = -45;
        [Export] private float minRotationToIncrement = -20;

        private CpuParticles2D liquidParticles;
        private float frameCount = 0;
        private float frameCountReverse = 0;
        private float timeSinceLastIncrement = 0;
        private float rotationTime = 1;
        private bool isSoundPlaying = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            renderer.PivotOffset = renderer.Size / 2f;
            liquidParticles = liquidParcilesScene.Instantiate<CpuParticles2D>();
            renderer.AddChild(liquidParticles);
            liquidParticles.Color = contentColor;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (InputManager.isTouching && canPlay)
            {
                if (renderer.RotationDegrees >= maxRotation)
                {
                    frameCount+= (float)delta;
                    renderer.Rotation = (float)Mathf.DegToRad(Mathf.Lerp(renderer.RotationDegrees, maxRotation, frameCount / rotationTime));
                    if (frameCount >= rotationTime) frameCount = 0;
                }
                if (timeSinceLastIncrement >= incrementInterval && renderer.RotationDegrees <= minRotationToIncrement)
                {
                    timeSinceLastIncrement = 0;
                    liquidParticles.Emitting = true;
                    GameManager.GetInstance().count += 1;
                    AudioManager.PlaySound(SoundsList.TAC, true);
                    HUDManager.GetInstance().gameHUD.UpdateGCount(GameManager.GetInstance().count);
                }
                else timeSinceLastIncrement += (float)delta;

            }
            else if (renderer.RotationDegrees <= 0 && !InputManager.isTouching)
            {
                AudioManager.PlaySound(SoundsList.FILLING, false);
                isSoundPlaying = false;
                liquidParticles.Emitting = false;
                if (frameCount != 0) frameCount = 0;
                frameCountReverse += (float)delta;
                renderer.Rotation = (float)Mathf.DegToRad(Mathf.Lerp(renderer.RotationDegrees, 0, frameCountReverse / rotationTime));
                if (frameCountReverse >= rotationTime) frameCountReverse = 0;
            }
        }
    }
}
