using Godot;
using System;
using System.Collections.Generic;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class HUDManager : Manager
	{
		#region Singleton
		static private HUDManager instance;
		private HUDManager() { }

		static public HUDManager GetInstance()
		{
			if(instance == null) instance = new HUDManager();
			return instance;
		}

		#endregion
		[Export] private Button undo;
		[Export] private Button redo;
		[Export] private Button reset;

		[Export] private Label stepLabel;
		[Export] private Label realizedBy;
		[Export] private Label parLabel;

		[Export] private Control container;

		public Sprite2D CurrentBackground;
		public List<Sprite2D> CurrentSprites = new List<Sprite2D>();

		private Sprite2D BackGroundLevel1;
        private Sprite2D BackGroundLevel2;
        private Sprite2D BackGroundLevel3;
        private Sprite2D BackGroundLevel4;
        private Sprite2D BackGroundLevel5;
        private Sprite2D BackGroundLevel6;
        private Sprite2D BackGroundLevel7;
        private Sprite2D BackGroundLevel8;

        private const string PAR = "PAR : ";
		private const string REALIZED_BY = "Level Realized By : ";
		private const string STEP = "STEP : ";

		//[Export] Control thisControl;

		GameManager gameManager;
		ScreenManager screenManager;
        public override void Init()
        {
            base.Init();
			//thisControl.Size = ScreenManager.GetInstance().screenSize;
			gameManager = GameManager.GetInstance();
			screenManager = ScreenManager.GetInstance();

			container.Size = screenManager.screenSize;
			undo.Pressed += gameManager.Undo;
			redo.Pressed += gameManager.Redo;
			reset.Pressed += gameManager.Retry;
			BackGroundLevel1 = (Sprite2D)GetNode("Control/Background1");
            BackGroundLevel2 = (Sprite2D)GetNode("Control/Background2");
            BackGroundLevel3 = (Sprite2D)GetNode("Control/Background3");
            BackGroundLevel4 = (Sprite2D)GetNode("Control/Background4");
            BackGroundLevel5 = (Sprite2D)GetNode("Control/Background5");
            BackGroundLevel6 = (Sprite2D)GetNode("Control/Background6");
            BackGroundLevel7 = (Sprite2D)GetNode("Control/Background7");
            BackGroundLevel8 = (Sprite2D)GetNode("Control/Background8");
			CurrentSprites.Add(BackGroundLevel1);
            CurrentSprites.Add(BackGroundLevel2);
            CurrentSprites.Add(BackGroundLevel3);
            CurrentSprites.Add(BackGroundLevel4);
            CurrentSprites.Add(BackGroundLevel5);
            CurrentSprites.Add(BackGroundLevel6);
            CurrentSprites.Add(BackGroundLevel7);
            CurrentSprites.Add(BackGroundLevel8);
        }

        public override void _Ready()
		{
			#region Singleton
			if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(HUDManager) + "instance already exists, destroying the last added");
				return;
			}

			instance = this;
			#endregion
		}

		override public void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		public void SetVisible(bool pVisibility)
		{
			undo.Visible = pVisibility;
			redo.Visible = pVisibility;
			reset.Visible = pVisibility;
		}

		public void UpdateStep(int pStep)
		{
			stepLabel.Text = STEP + pStep;
		}
		public void ShowBackground()
		{
			CurrentBackground.Visible = true;
		}
        public void UpdateBackground()
        {
			BackGroundLevel1.Visible = false;
            BackGroundLevel2.Visible = false;
            BackGroundLevel3.Visible = false;
            BackGroundLevel4.Visible = false;
            BackGroundLevel5.Visible = false;
            BackGroundLevel6.Visible = false;
            BackGroundLevel7.Visible = false;
            BackGroundLevel8.Visible = false;
        }

        public void UpdatePar(int pPar)
		{
			parLabel.Text = PAR + pPar;
		}

		public void UpdateRealizedBy(string pName)
		{
			realizedBy.Text = REALIZED_BY + pName;
		}

		protected override void Dispose(bool pDisposing)
		{
            undo.Pressed -= gameManager.Undo;
            redo.Pressed -= gameManager.Redo;
            reset.Pressed -= gameManager.Retry;
            instance = null;
			base.Dispose(pDisposing);
        }
	}
}
