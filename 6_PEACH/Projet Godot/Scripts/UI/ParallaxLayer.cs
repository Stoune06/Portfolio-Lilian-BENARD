using Godot;
using System;

// Author : Hector Rosier

namespace Com.IsartDigital.ProjectName {
	
	public partial class ParallaxLayer : Godot.ParallaxLayer
	{
		[Export] Node2D topSprite;
		[Export] Node2D bottomSprite;
		[Export] bool isLogin = true;
		Vector2 viewportSize;
		ColorRect colorRect;
		public override void _Ready()
		{
            GetViewport().SizeChanged += OnViewportResized;
			OnViewportResized();
        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			MotionOffset += Vector2.Right * 20 * lDelta;
			if (colorRect != null)
			{
				topSprite.GlobalPosition = new Vector2(-1000, colorRect.GlobalPosition.Y + 64);
				bottomSprite.GlobalPosition = new Vector2(-1000, colorRect.GlobalPosition.Y - 64);
				GD.Print("parallax");
            }
		}

		private void OnViewportResized()
		{
			if(!isLogin)
			{
				colorRect = (ColorRect)GetParent().GetParent();
			}
			viewportSize = GetViewportRect().Size;
			if (topSprite != null && bottomSprite != null)
            {
				topSprite.GlobalPosition = new Vector2(-1000, 64);
				bottomSprite.GlobalPosition = new Vector2(-1000, viewportSize.Y - 64);
            }
		}


        protected override void Dispose(bool pDisposing)
		{

		}
	}
}
