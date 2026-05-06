using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class InGameHUD : UI
	{
        [Export] private TextureProgressBar timerPogress;
        [Export] private RichTextLabel gCount;
        [Export] private Label personCountLabel;
        [Export] private HBoxContainer personCount;

        private const float DURATION = 0.1f;
        private float scaleFactor = 1.2f;
        public override void _Ready()
        {
            base._Ready();
            gCount.PivotOffset = gCount.Size / 2f;
        }

        public void UpdateGCount(int pNewCount)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(gCount, TweenProperties.SCALE, Vector2.One * scaleFactor, DURATION);
            lTween.TweenProperty(gCount, TweenProperties.SCALE, Vector2.One, DURATION);
            gCount.Text = "[center][color=black][font_size=200]" + pNewCount + HUDManager.GetInstance().gCountSuffix;
        }

        public void UpdatePersonCount(int pPersonCount)
        {
            personCountLabel.Text = pPersonCount.ToString();
        }

        public void UpdateProgressTimer(Timer pTimer)
        {
            timerPogress.Value = pTimer.TimeLeft * 100 / pTimer.WaitTime;
        }
    }
}
