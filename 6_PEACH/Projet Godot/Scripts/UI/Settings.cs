using Godot;
using System;

// Author : Sara Astuti-Aucher

namespace Com.IsartDigital.Kinisi.Menu
{
	public partial class Settings : Control
	{
		#region // ----- Singleton ----- \\

		static private Settings instance;

		static public Settings GetInstance()
		{
			if (instance == null) instance = new Settings();
			return instance;
		}

        #endregion

        [Export] private Button back;
        [Export] private Button frenchFlag;
        [Export] private Button englishFlag;
        [Export] private Button muteOverall;
        [Export] private Button muteMusic;
        [Export] private Button muteSfx;
        [Export] private Button unmuteOverall;
        [Export] private Button unmuteMusic;
        [Export] private Button unmuteSfx;
        [Export] private HSlider sliderOverall;
        [Export] private HSlider sliderMusic;
        [Export] private HSlider sliderSfx;

        private const string PATH_PARALLAX = "Menu/ParallaxBackground/Label";


        public override void _Ready()
		{
			#region // ----- Singleton ----- \\

			if (instance != null)
			{
				GD.Print(Name + " Instance already exist, destroying the last added.");
				QueueFree();
				return;
			}

			instance = this;

			#endregion

			base._Ready();
			back.Pressed += BackPressed;
            frenchFlag.Pressed += FrenchFlagPressed;
            englishFlag.Pressed += EnglishFlagPressed;
            muteOverall.Pressed += MuteOverallPressed;
            muteMusic.Pressed += MuteMusicPressed;
            muteSfx.Pressed += MuteSfxPressed;
            unmuteOverall.Pressed += UnmuteOverallPressed;
            unmuteMusic.Pressed += UnmuteMusicPressed;
            unmuteSfx.Pressed += UnmuteSfxPressed;
            sliderOverall.ValueChanged += SliderOverallValueChanged;
            sliderMusic.ValueChanged += SliderMusicValueChanged;
            sliderSfx.ValueChanged += SliderSfxValueChanged;
            sliderOverall.Value = 0.2f;
            sliderMusic.Value = 0.7;
            sliderSfx.Value = 1;
		}

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);
		}

		private void BackPressed()
		{
            AudioManager.PlaySound("Click", true);
            Menu.GetInstance().buttonsMenu.Visible = true;
            Visible = false;
            GetTree().Root.GetChild(0).GetNode<Label>(PATH_PARALLAX).Visible = true;
        }
        private void FrenchFlagPressed()
        {
            AudioManager.PlaySound("Click", true);
            TranslationServer.SetLocale("fr");
        }
        private void EnglishFlagPressed()
        {
            AudioManager.PlaySound("Click", true);
            TranslationServer.SetLocale("en");
        }
        private void MuteOverallPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(0, false);
            muteOverall.Visible = false;
            unmuteOverall.Visible = true;
        }
        private void MuteMusicPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(1, false);
            muteMusic.Visible = false;
            unmuteMusic.Visible = true;
        }
        private void MuteSfxPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(2, false);
            muteSfx.Visible = false;
            unmuteSfx.Visible = true;
        }
        private void UnmuteOverallPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(0, true);
            muteOverall.Visible = true;
            unmuteOverall.Visible = false;
        }
        private void UnmuteMusicPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(1, true);
            muteMusic.Visible = true;
            unmuteMusic.Visible = false;
        }
        private void UnmuteSfxPressed()
        {
            AudioManager.PlaySound("Click", true);
            AudioServer.SetBusMute(2, true);
            muteSfx.Visible = true;
            unmuteSfx.Visible = false;
        }
        private void SliderOverallValueChanged(double pValue)
        {
            AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb((float)pValue));
        }
        private void SliderMusicValueChanged(double pValue)
        {
            AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb((float)pValue));

        }
        private void SliderSfxValueChanged(double pValue)
        {
            AudioServer.SetBusVolumeDb(2, Mathf.LinearToDb((float)pValue));

        }

        protected override void Dispose(bool pDisposing)
		{
			#region // ----- Singleton ----- \\

			if (pDisposing && instance == this) instance = null;

			#endregion

			base.Dispose(pDisposing);
		}
	}
}
