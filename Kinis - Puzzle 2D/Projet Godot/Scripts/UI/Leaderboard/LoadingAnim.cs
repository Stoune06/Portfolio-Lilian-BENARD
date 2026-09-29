using Com.IsartDigital.Kinisi;
using Godot;
using System;

// Author : Hector Rosier

namespace Com.IsartDigital.ProjectName {
	
	public partial class LoadingAnim : Sprite2D
	{
		public override void _Ready()
		{
            Tween lTween = CreateTween().SetLoops();
            lTween.TweenProperty(this, TweenProp.ROTATION, 15, 10f).FromCurrent();
        }
	}
}
