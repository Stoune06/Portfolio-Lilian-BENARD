using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class Background : Control
	{
        public override void _Ready()
        {
            base._Ready();
            Size = GetWindow().Size;
        }
    }
}
