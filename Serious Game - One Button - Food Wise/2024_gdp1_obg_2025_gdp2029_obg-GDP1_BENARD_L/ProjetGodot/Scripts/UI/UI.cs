using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class UI : Control
	{
        protected HUDManager hudManager;
        protected GameManager gameManager;
        public override void _Ready()
        {
            base._Ready();
            Size = GetViewportRect().Size;
        }
    }
}
