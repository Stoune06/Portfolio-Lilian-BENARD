using Godot;
using System;
using System.Drawing;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class LoadingScreen : ColorRect
	{
        [Export] public Sprite2D leftBand, rightBand;
        public override void _Ready()
        {
            base._Ready();
            GlobalPosition += Vector2.Left * 50;
            Size = GetWindow().Size + Vector2.Right * 100;
            GlobalPosition = new Vector2(10, -Size.Y);

            //rightBand.GlobalPosition = new Vector2(30, -rightBand.Texture.GetHeight()/2f);
            //leftBand.GlobalPosition = new Vector2(Size.X - 30, -leftBand.Texture.GetHeight() / 2f);

            rightBand.GlobalPosition = new Vector2(Size.X - 30, -Size.Y - 30);
            leftBand.GlobalPosition = new Vector2(105, -Size.Y - 30);
        }
    }
}
