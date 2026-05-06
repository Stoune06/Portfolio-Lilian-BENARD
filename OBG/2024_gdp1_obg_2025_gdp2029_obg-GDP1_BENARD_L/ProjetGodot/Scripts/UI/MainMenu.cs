using Godot;
using System;
using System.Collections.Generic;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class MainMenu : UI
	{
		[Export] private Label gameTitle;

		[Export] private TextureButton playButton;
		[Export] private TextureButton settingsButton;
		[Export] private TextureButton quitButton;
        [Export] public AnimationPlayer richTextAnimation;
		[Export] private TextureRect settings;

        private List<TextureButton> buttonList;
		private bool isFTUE = true;

		const float BUTTON_ANIMATION_DURATION = 0.3f;
		public override void _Ready()
		{
			base._Ready();
			buttonList = new List<TextureButton> { playButton, settingsButton, quitButton };
            playButton.Pressed += OnPlayButtonPressed;
            settingsButton.Pressed += OnSettingsButtonPressed;
            quitButton.Pressed += OnQuitButtonPressed;
			StartAnimation();
		}

        private void OnQuitButtonPressed()
        {
			GetTree().Quit();
        }

        private void OnSettingsButtonPressed()
        {
            settings.Visible = true;
        }

		private void QuitSettingsTab()
		{
			settings.Visible = false;
		}

		public void ChangeVolume(float pVolume)
		{
			GD.Print(pVolume / 100f);
			AudioManager.UpdateVolume(pVolume);
		}

        private async void OnPlayButtonPressed()
        {
			TransitionScreen lTransitionScreen = HUDManager.GetInstance().transitionScreen;
			RecipeList lRecipeList = HUDManager.GetInstance().recipeList;
            lTransitionScreen.TransitionIn();
            await (ToSignal(lTransitionScreen.transitionPlayer, "animation_finished"));

            if (isFTUE)
            {
                richTextAnimation.Play("textapparition");
                await ToSignal(richTextAnimation, "animation_finished");
                isFTUE = false;
            }
            Visible = false;
			lTransitionScreen.TransitionOut();
            lRecipeList.animationPlayer.Play("transition");
            await ToSignal(lTransitionScreen.transitionPlayer, "animation_finished");
            AudioManager.StopAllSounds();
            GameManager.GetInstance().OnGameStart();
            HUDManager.GetInstance().DisplayInGameUI();
            await (ToSignal(lRecipeList.animationPlayer, "animation_finished"));
            //await (ToSignal(lTransitionScreen.transitionPlayer, "animation_finished"));
            GameManager.GetInstance().Start();
        }

		public void StartAnimation()
		{
			List<Vector2> lPositions = new List<Vector2>();
			foreach (TextureButton lButton in buttonList)
			{
				lButton.Scale = Vector2.Zero;
				lButton.PivotOffset = new Vector2(lButton.Size.X / 2f, lButton.PivotOffset.Y);
			}
			gameTitle.Scale *= 4;
			gameTitle.PivotOffset = gameTitle.Size / 2f;

			Tween lTween = CreateTween();
			lTween.TweenProperty(gameTitle, "scale", Vector2.One, BUTTON_ANIMATION_DURATION);
			lTween.SetTrans(Tween.TransitionType.Elastic);
			lTween.TweenProperty(playButton, "scale", Vector2.One, BUTTON_ANIMATION_DURATION).SetDelay(BUTTON_ANIMATION_DURATION);
			lTween.SetParallel().TweenProperty(settingsButton, "scale", Vector2.One, BUTTON_ANIMATION_DURATION).SetDelay(BUTTON_ANIMATION_DURATION + 0.1f);
			lTween.TweenProperty(quitButton, "scale", Vector2.One, BUTTON_ANIMATION_DURATION).SetDelay(BUTTON_ANIMATION_DURATION +0.2f);
		}

        override public void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
		}
	}
}
