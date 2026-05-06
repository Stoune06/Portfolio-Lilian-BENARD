using Godot;
using System;
using static Godot.TextServer;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class WallFallParticles : Particle
	{

        Vector3 direction;
        Movable parentMovable;
        public override void _Ready()
        {
            base._Ready();
            parentMovable = (Movable)GetParent().GetParent();
            particleProcessMaterial.EmissionBoxExtents = new Vector3(gridManager.tileSize.X / 2, gridManager.tileSize.Y / 2, 0);
            Vector2 lOppositeDirection = -(parentMovable.startPosition - parentMovable.targetPosition);
            direction = new Vector3(lOppositeDirection.X, lOppositeDirection.Y, 0);
            particleProcessMaterial.Gravity = direction;
        }
    }
}
