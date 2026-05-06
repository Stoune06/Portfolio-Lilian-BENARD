using Com.IsartDigital.Kinisi;
using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class Wall : Movable
	{
        private static PackedScene dustScene = (PackedScene)ResourceLoader.Load("res://Scenes/SFX/Particles/WallDeplacementParticles.tscn");
        private Particle dustParticle;

        public override void _Ready()
        {
            base._Ready();
            RandomNumberGenerator rand = new RandomNumberGenerator();
            rand.Randomize();
            outRenderer.Frame = rand.RandiRange(0, 4);
        }

        public override void MoveObject(Vector2 pEndPosition, float pDuration)
        {
            base.MoveObject(pEndPosition, pDuration);
            if(dustParticle == null)
            {
                dustParticle = Spawner.SpawnParticle(dustScene, marker);
                dustParticle.Rotation = Position.AngleTo(pEndPosition);
            }
            dustParticle.Emitting = true;
            dustParticle.ZIndex = ZIndex - 10;

        }

        protected override void StopMove()
        {
            base.StopMove();
            if (dustParticle != null) dustParticle.Emitting = false;
        }

    }
}
