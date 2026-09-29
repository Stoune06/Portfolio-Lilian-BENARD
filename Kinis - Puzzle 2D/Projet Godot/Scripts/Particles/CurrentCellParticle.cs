using Godot;
using System;

// Author : Guillaume Julia

namespace Com.IsartDigital.ProjectName
{
	public partial class CurrentCellParticle : Node2D 
	{
		[Export] private GpuParticles2D CurrentCellParticles;
        private Vector2 MousePosition;
       

        public override void _Ready()
        {
            base._Ready();
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            MousePosition = GetGlobalMousePosition();
            PlayParticles();
        }
        private void PlayParticles()
        {
            if (GridManager.GetInstance().GetIndexOnGrid(MousePosition).X >= 0 && GridManager.GetInstance().GetIndexOnGrid(MousePosition).X <= 5 && GridManager.GetInstance().GetIndexOnGrid(MousePosition).Y <= 0 && GridManager.GetInstance().GetIndexOnGrid(MousePosition).Y <= 6)
            {
                GlobalPosition = GridManager.GetInstance().movableObjectsOnGrid[GridManager.GetInstance().GetIndexOnGrid(MousePosition).X][GridManager.GetInstance().GetIndexOnGrid(MousePosition).Y].GlobalPosition;
               
            }
            GD.Print(GridManager.GetInstance().GetIndexOnGrid(MousePosition));
            
        }
    }
}
