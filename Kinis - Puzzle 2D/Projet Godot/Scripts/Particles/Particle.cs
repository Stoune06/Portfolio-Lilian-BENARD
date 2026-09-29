using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class Particle : GpuParticles2D
	{
		[Export] public ParticleProcessMaterial particleProcessMaterial;
        protected GridManager gridManager;



        public override void _Ready()
        {
            base._Ready();
            gridManager = GridManager.GetInstance();
        }

    }
}
