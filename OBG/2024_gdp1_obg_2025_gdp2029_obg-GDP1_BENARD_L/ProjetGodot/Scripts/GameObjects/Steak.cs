using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class Steak : SimpleTapType
	{
        private static PackedScene cookingParticles = GD.Load<PackedScene>("res://Scenes/SFX/CookingParticles.tscn");

        [Export] private Polygon2D basePolygon;
        private Vector2[] polygonData;

        [Export] private Vector2 rectSize;
        [Export] private Color rectStartColor;
        [Export] private Color rectEndColor;
        private float cookingDuration = 3f;

        protected override void DoAction(float pDelta)
        {
            base.DoAction(pDelta);
            if (bakingPosition.Length < index) return;
            Tween lTween = CreateTween();
            Polygon2D lColorRect = CreateSteak();
            lTween.TweenProperty(lColorRect, "position", bakingPosition[index++].GlobalPosition, 1d).Finished += () => CreateParticles(lColorRect, Vector2.Zero);
            
            lTween.TweenProperty(lColorRect,"color",rectEndColor,cookingDuration);

        }

        private static void CreateParticles(Node pContainer, Vector2 pPosition)
        {
            CpuParticles2D lParticles = cookingParticles.Instantiate<CpuParticles2D>();
            pContainer.AddChild(lParticles);
            lParticles.Emitting = true;
            lParticles.Position = pPosition;
        }

        private Polygon2D CreateSteak()
        {
            Polygon2D lPolygon = (Polygon2D)basePolygon.Duplicate();
            lPolygon.GlobalPosition = GlobalPosition;
            lPolygon.Color = rectStartColor;
            lPolygon.Scale *= 0.6f;
            lPolygon.RotationDegrees = GD.Randf() * 360;
            LevelManager.GetInstance().currentLevel.AddChild(lPolygon);
            return lPolygon;
        }
    }
}
