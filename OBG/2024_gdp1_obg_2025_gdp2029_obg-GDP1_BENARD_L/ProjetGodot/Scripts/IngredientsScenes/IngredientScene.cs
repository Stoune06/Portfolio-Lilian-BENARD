using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
    public partial class IngredientScene : Node2D
    {
        
        [Export] public int goal = 100;
        [Export] public Color contentColor;
        protected bool canPlay = true;

        public override void _Ready()
        {
            base._Ready();
        }
        public static IngredientScene LoadIngredient(PackedScene pIngredientScene, Node pContainer, int pNPerson, Vector2 pPos = default)
        {
            IngredientScene lIngredient = pIngredientScene.Instantiate<IngredientScene>();
            pContainer.AddChild(lIngredient);
            lIngredient.IngredientTransitionIn(pPos);
            GameManager.GetInstance().goal = lIngredient.goal * pNPerson;
            return lIngredient;
        }

        public void IngredientTransitionIn(Vector2 pPos)
        {
            canPlay = true;
            GlobalPosition = pPos + GetViewportRect().Size * Vector2.Right;
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, "position", pPos, 1f);
        }

        public void IngredientTransitionOut()
        {
            AudioManager.PlaySound(SoundsList.SWITCH_INGREDIENT, true);
            canPlay = false;
            Tween lTween = CreateTween().SetEase(Tween.EaseType.InOut);
            lTween.TweenProperty(this, "position", GlobalPosition + GetViewportRect().Size * Vector2.Left, 1f).Finished += QueueFree;
        }
    }
}
