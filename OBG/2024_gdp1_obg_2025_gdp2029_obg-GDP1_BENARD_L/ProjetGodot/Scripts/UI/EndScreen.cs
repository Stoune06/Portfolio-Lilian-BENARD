using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class EndScreen : UI
	{
		private void Menu()
		{
			LevelManager.GetInstance().currentLevelIndex = 0;
			Visible = false;
			HUDManager.GetInstance().mainMenu.Visible = true;
			HUDManager.GetInstance().mainMenu.richTextAnimation.Play("RESET");
            HUDManager.GetInstance().mainMenu.StartAnimation();
		}
	}
}
