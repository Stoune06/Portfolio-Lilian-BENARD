using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class RecipeList : UI
	{
		[Export] public AnimationPlayer animationPlayer;
		[Export] private RichTextLabel[] labelList;
        private int index = 1;
        public override void _Ready()
        {
            base._Ready();
			GlobalPosition += GetViewportRect().Size.X * Vector2.Right;
            animationPlayer.AnimationFinished += AnimationPlayerAnimationFinished;
        }

        private void AnimationPlayerAnimationFinished(StringName animName)
        {
            if(index < labelList.Length)labelList[index].Visible = true;
            index++;
        }
    }
}
