using Godot;
using System;

// Author : Sara Astuti-Aucher

namespace Com.IsartDigital.ProjectName {
	
	public partial class TitleCard : Control
	{
        [Export] private TextureRect isartLogo;
        private float isartLogoTime = 2f;
        public override void _Ready()
		{
            base._Ready();
            CustomMinimumSize = GetWindow().Size;
            isartLogo.Show();

            Tween lTween = CreateTween().Chain();
            lTween.TweenProperty(isartLogo, "modulate", Colors.White, isartLogoTime * 0.5f).From(Colors.Transparent);
            lTween.TweenProperty(isartLogo, "modulate", Colors.Transparent, isartLogoTime * 0.5f);
            lTween.Finished += LTweenFinished;
            lTween.Play();
        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}
        private void LTweenFinished()
        {
            GameManager.GetInstance().AddLogin();
            QueueFree();
        }
    }
}
