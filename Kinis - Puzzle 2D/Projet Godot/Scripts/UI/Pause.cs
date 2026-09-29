using Com.IsartDigital.Kinisi.Menu;
using Com.IsartDigital.ProjectName;
using Godot;
using System;

// Author : Hector Rosier

namespace Com.IsartDigital.Kinisi.Pause{ 

    public partial class Pause : CanvasLayer
	{
		[Export] private Button resume;
		[Export] private Button menu;
        private InputManager inputManager;

        private const string GAME_CONTAINER = "Main/SubViewport/GameContainer";
        private const string PATH_PARALLAX = "Menu/ParallaxBackground";


        public override void _Ready()
		{
            resume.ButtonDown += ResumeButtonDown;
            menu.ButtonDown += MenuButtonDown;
            inputManager = InputManager.GetInstance();
		}

        private void MenuButtonDown()
        {
            AudioManager.PlaySound("Click", true);
            GetParent<Menu.Menu>().Visible = true;
            GetTree().Root.GetNode<Node2D>(GAME_CONTAINER).GetChild(0).QueueFree();
            GetTree().Paused = false;
            
            Tile.allTiles.Clear();
            Movable.allMovables.Clear();
            GridManager.GetInstance().movableObjectsOnGrid.Clear();
            GridManager.GetInstance().targetList.Clear();
            GridManager.GetInstance().tilesGrid.Clear();
            InputManager.canPlay = false;
            GameManager.ResetLevelData();
            GameManager.isPlaying = false;

            AudioManager.PlaySound("Lvl1", false);
            AudioManager.PlaySound("Wind", false);
            AudioManager.PlaySound("Menu", true);

            GetTree().Root.GetChild(0).GetNode<ParallaxBackground>(PATH_PARALLAX).Layer = 0;

            QueueFree();

        }

        private void ResumeButtonDown()
        {
            AudioManager.PlaySound("Click", true);
            GetTree().Paused = false;
            QueueFree();
        }

    }
}
