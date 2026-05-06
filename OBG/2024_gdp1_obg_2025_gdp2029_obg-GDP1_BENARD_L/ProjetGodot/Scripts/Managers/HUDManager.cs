using Godot;
using System;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class HUDManager : Control
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
        private PackedScene winScreenScene = GD.Load<PackedScene>("res://Scenes/UI/WinScreen.tscn");
        [Export] public MainMenu mainMenu;
		[Export] public InGameHUD gameHUD;
		[Export] public TransitionScreen transitionScreen;
		[Export] public RecipeList recipeList;
		[Export] private WinScreen winScreen;
		[Export] public UI endScreen;


		[Export] private TextureProgressBar timerPogress;
		[Export] private Label gCount;
		[Export] private Label scoreLabel;
		[Export] private Button playButton;
		[Export] private Label tapToContinue;
		[Export] private Button continueButton;
		[Export] private Label personCountLabel;
		[Export] private HBoxContainer personCount;

		public string gCountSuffix = "g";
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
			Size = GetViewportRect().Size;
			playButton.Pressed += OnPlayButtonPressed;
			continueButton.Pressed += OnContinue;
            WinScreen lWinScreen = winScreenScene.Instantiate<WinScreen>();
            AddChild(lWinScreen);
            lWinScreen.Visible = false;
			winScreen = lWinScreen;
        }

		override public void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}

		public void DisplayInGameUI()
		{
			gameHUD.Visible = true;
        }


		public async void OnPlayButtonPressed()
		{
			transitionScreen.TransitionIn();
			await(ToSignal(transitionScreen.transitionPlayer, "animationfinished"));
			GameManager.GetInstance().OnGameStart();
            transitionScreen.TransitionOut();
        }

		public void UpdatePersonCount(int pPersonCount)
		{
			personCountLabel.Text = pPersonCount.ToString();
		}

		public void WinScreen(int pTotalCount, int pFinalScore, int pTotalGoal, int pNPerson)
		{
			winScreen.totalCount = pTotalCount;
			winScreen.finalScore = pFinalScore;
			winScreen.totalGoal = pTotalGoal;
			winScreen.nPlate = pNPerson;
			winScreen.Display();
			gameHUD.Visible = false;
		}

		public async void OnContinue()
		{
			if(LevelManager.GetInstance().currentLevelIndex +1 >= LevelManager.GetInstance().allLevels.Length)
			{
				winScreen.Visible = false;
				endScreen.Visible = true;
				return;
			}
            transitionScreen.TransitionIn();
            await(ToSignal(transitionScreen.transitionPlayer, "animation_finished"));
            winScreen.Visible = false;
            DisplayInGameUI();
			recipeList.GlobalPosition = Vector2.Zero;
            transitionScreen.TransitionOut();
            GameManager.GetInstance().Continue();
            await (ToSignal(transitionScreen.transitionPlayer, "animation_finished"));
			recipeList.animationPlayer.Play("transition");
			await ToSignal(recipeList.animationPlayer, "animation_finished");
			GameManager.GetInstance().Start();
            AudioManager.StopAllSounds();
        }

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
