using Godot;
using System;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class GameManager : Node2D
	{
		#region Singleton
		static private GameManager instance;
		private GameManager() { }

		static public GameManager GetInstance()
		{
			if(instance == null) instance = new GameManager();
			return instance;
		}

		#endregion
		public int count = 0;
		public int goal;

		private int score = 0;
        private int totalCount = 0;
		private int totalGoal = 0;

        const int PERFECT_TOLERANCE = 5;
		const int VERY_GOOD_TOLERANCE = 10;
		const int GOOD_TOLERANCE = 20;
		const int BAD_TOLERANCE = 30;

		[Export] public Timer timer;
		private HUDManager hudManager;
		private LevelManager levelManager;
		private InGameHUD inGameHUD;
		public bool isPlaying = false;
		public override void _Ready()
		{
			#region Singleton
			if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(GameManager) + "instance already exists, destroying the last added");
				return;
			}

			instance = this;
			#endregion
			timer.Timeout += OnTimerTimeout;
			hudManager = HUDManager.GetInstance();
			levelManager = LevelManager.GetInstance();
			inGameHUD = hudManager.gameHUD;
		}

		override public void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			if(isPlaying)inGameHUD.UpdateProgressTimer(timer);
		}



		public void CheckAccuracy()
		{
			int lScore = 0;
			if (IsInRange(count, goal, PERFECT_TOLERANCE)) score += lScore = 5;
			else if (IsInRange(count, goal, VERY_GOOD_TOLERANCE)) score += lScore = 4;
			else if (IsInRange(count, goal, GOOD_TOLERANCE)) score += lScore = 3;
			else if (IsInRange(count, goal, BAD_TOLERANCE)) score += lScore = 2;
			else score += lScore = 1;
			count = 0;
            AccuracyIndicator.CreateAccuracyIndicator(Colors.White, lScore - 1, Vector2.Zero, inGameHUD);//Couleur provisoire
        }

		private int CheckStarNumber()
		{
			float lAverage = (float)score / levelManager.currentLevel.ingredientsList.Length;
			score = 0;
			return Mathf.RoundToInt(lAverage);
		}

		private void OnTimerTimeout()
		{
			totalGoal += goal;
			totalCount += count;
			CheckAccuracy();
			inGameHUD.UpdateGCount(count);
			levelManager.currentLevel.SwitchIngredient(levelManager.currentLevel.currentIngredientIndex+1);
		}

		public void OnGameStart()
		{
			totalCount = 0;
            levelManager.LoadLevel(levelManager.currentLevelIndex, this);
            inGameHUD.UpdateGCount(count);
			inGameHUD.UpdatePersonCount(levelManager.currentLevel.personNumber);
            timer.WaitTime = levelManager.currentLevel.time;
            
        }

		public void Start()
		{
            isPlaying = true;
            timer.Start();
        }

		public void GameOver()
		{
			int lNumber = CheckStarNumber();
            hudManager.WinScreen(totalCount,lNumber,totalGoal, levelManager.currentLevel.personNumber);
			totalGoal = 0;
			isPlaying = false;
			levelManager.currentLevel.Visible = false;
			timer.Stop();
		}

		public void Continue()
		{
			totalCount = 0;
            inGameHUD.UpdatePersonCount(levelManager.currentLevel.personNumber);
            levelManager.SwitchLevel(levelManager.currentLevelIndex + 1, this);
			timer.WaitTime = levelManager.currentLevel.time;
		}

		public bool IsInRange(int pCount, int pGoal, int pTolerance)
		{
			return pCount <= pGoal + pGoal * (pTolerance / 100f) && pCount >= goal - goal * (pTolerance / 100f) ;		
		}

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
