using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class FTUE : Level
	{
        [Export] public AnimationPlayer richTextAnimation;
        public override void _Ready()
        {
            base._Ready();
            HUDManager.GetInstance().gCountSuffix += "[color=black][font_size=100] /([color=dark_red]50g[/color] x 2)[img=200]res://Assets/utilisateur.png[/img]";
        }

        public override void SwitchIngredient(int pIndex)
        {
            base.SwitchIngredient(pIndex);
            HUDManager.GetInstance().gCountSuffix = "[color=black][font_size=100] g/([color=dark_red]100g[/color] x [color=blue]1,5[/color])";
            HUDManager.GetInstance().gameHUD.UpdateGCount(0);
        }
    }
}
