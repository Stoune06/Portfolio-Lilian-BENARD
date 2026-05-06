using Godot;
using System;
using System.Collections.Generic;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
    public partial class AccuracyIndicator : Label
    {
        [Export] private AnimationPlayer animationPlayer;
        private static PackedScene indicatorScene = GD.Load<PackedScene>("res://Scenes/UI/AcurracyIndicator.tscn");

        public static string[] allTexts = new string[]
        {"VERY BAD","BAD","GOOD","VERY GOOD","PERFECT" };

        public override void _Ready()
        {
            animationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public static void CreateAccuracyIndicator(Color pColor, int pIndex, Vector2 pPosition, Node pContainer)
        {
            AccuracyIndicator lIndicator = indicatorScene.Instantiate<AccuracyIndicator>();
            lIndicator.Text = allTexts[pIndex];
            lIndicator.Modulate = pColor;
            lIndicator.GlobalPosition = pPosition;
            pContainer.AddChild(lIndicator);
        }

        private void OnAnimationFinished(StringName animName)
        {
            QueueFree();
        }
    }
}
