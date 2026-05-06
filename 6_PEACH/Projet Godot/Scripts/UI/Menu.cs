using Com.IsartDigital.Kinisi.Login;
using Com.IsartDigital.ProjectName;
using Com.IsartDigital.Utils.Effects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.IsartDigital.Kinisi.Menu
{
    public partial class Menu : CanvasLayer
    {
        #region Singleton

        static private Menu instance;

        private Menu() { }

        static public Menu GetInstance()
        {
            if (instance == null) instance = new Menu();
            return instance;
        }

        #endregion

        [Export] public ButtonsMenu buttonsMenu;
        [Export] private Control settings;
        [Export] private Control buttonsLevels;
        [Export] private PackedScene sceneScore;
        [Export] private PackedScene scenePause;
        [Export] private Shaker shaker;
        [Export] private AnimationPlayer intro;
        [Export] private GpuParticles2D explo;
        [Export] private GpuParticles2D Fall;
        [Export] private ParallaxBackground parallaxBack;
        [Export] private Label kinisi;
        [Export] private PackedScene[] levelScenes;
        public static List<PackedScene> sceneLevels;

        private int[] levelNumbers;
        public static int levelIndex;
        public static int levelCount;

        private const string PLAY = "Play";
        private const string SETTINGS = "Settings";
        private const string SCORE = "Score";
        private const string QUIT = "Quit";
        private const string BACK = "Back";
        private const string INPUT_UP = "ui_up";
        private const string INPUT_DOWN = "ui_down";
        private const string UNLOCKED_ALL = "UnlockLevels";

        private bool soundBool = true;
        private Timer timer = new();
        private Timer timer2 = new();
        HUDManager hudManager = HUDManager.GetInstance();

        public override void _Ready()
        {
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Menu) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            LoadSceneLevels();
            AddChild(timer2);
            timer2.OneShot = true;
            timer2.Autostart = true;
            timer2.WaitTime = 0.25;
            timer2.Start();
            timer2.Timeout += Timer2_Timeout;

            intro.Play();
            AudioManager.PlaySound("Menu", true);
            AudioManager.PlaySound("Wind2", true);

            AddChild(timer);
            timer.OneShot = true;
            timer.WaitTime = 0.8;
            timer.Timeout += LTimerTimeout;
            timer.Start();
            
            Fall.Emitting = true;

            levelNumbers = new int[sceneLevels.Count];
            for (int i = 0; i < sceneLevels.Count; i++) levelNumbers[i] = i + 1;

            foreach (Button lButton in buttonsMenu.GetChildren()) lButton.Pressed += () => MenuButtons(lButton);
            foreach (Button lButton in buttonsLevels.GetChildren().OfType<Button>()) lButton.Pressed += () => LevelsButtons(lButton);
        }

        private void Timer2_Timeout()
        {
            AudioManager.PlaySound("Dash4", true);
            soundBool = false;
            explo.Emitting = true;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Pause.Pause lPause = scenePause.Instantiate() as Pause.Pause;
                AddChild(lPause);
                GetTree().Paused = true;
            }

        }

        private void LoadSceneLevels()
        {
            sceneLevels = new(levelScenes);
        }

        private void LTimerTimeout()
        {
            ButtonsMenuAnimation();
            buttonsMenu.Visible = true;
        }

        private void MenuButtons(Button pButton)
        {
            switch (pButton.Name)
            {
                case PLAY:
                    buttonsMenu.Visible = false;
                    buttonsLevels.Visible = true;
                    AudioManager.PlaySound("Click2", true);
                    kinisi.Visible = false;
                    break;
                case SETTINGS:
                    buttonsMenu.Visible = false;
                    settings.Visible = true;
                    kinisi.Visible = false;
                    AudioManager.PlaySound("Click", true);
                    break;
                case SCORE:
                    buttonsMenu.Visible = false;
                    kinisi.Visible = false;
                    Login.LeaderBoard lLeaderboard = sceneScore.Instantiate() as Login.LeaderBoard;
                    AddChild(lLeaderboard);
                    AudioManager.PlaySound("Click", true);
                    break;
                case QUIT:
                    AudioManager.PlaySound("Click", true);
                    GetTree().Quit();
                    break;
            }
        }

        private async void LevelsButtons(Button pButton)
        {
            string lName = pButton.Name;
            if (lName == BACK)
            {
                AudioManager.PlaySound("Click", true);
                buttonsLevels.Visible = false;
                buttonsMenu.Visible = true;
                kinisi.Visible = true;
                DataManager.SaveScore(GameManager.score);
                return;
            }
            if (lName == UNLOCKED_ALL)
            {
                AudioManager.PlaySound("Click", true);
                DataManager.GetInstance().UnlockAllLevels();
                return;
            }
            if (sceneLevels == null || sceneLevels.Count == 0) return;
            if (int.TryParse(new string(pButton.Text.Where(char.IsDigit).ToArray()), out int lParsedLevelNumber))
            {
                Tween lTween = GameManager.GetInstance().StartTransition();
                await ToSignal(lTween, "finished");
                int lParsedIndex = lParsedLevelNumber - 1;

                if (
                    lParsedIndex >= 0 && lParsedIndex < sceneLevels.Count &&
                    lParsedIndex < DataManager.LevelsGrid.Count &&
                    DataManager.GetInstance().IsLevelUnlocked(lParsedLevelNumber))
                {
                    parallaxBack.Layer = -1;
                    AudioManager.PlaySound("Click", true);
                    levelIndex = lParsedIndex;
                    GameManager.currentPar = DataManager.Levels[levelIndex].par;
                    Node2D lLevel = sceneLevels[lParsedIndex].Instantiate() as Node2D;
                    GetParent().GetNode<Node2D>("SubViewport/GameContainer").AddChild(lLevel);
                    Spawner.SpawnGameObjects(DataManager.LevelsGrid[lParsedIndex], lLevel, GameManager.GetInstance().tileSize);
                    Spawner.SpawnTargets(DataManager.LevelsTarget[lParsedIndex], lLevel);
                    hudManager.UpdateBackground();
                    hudManager.CurrentBackground = hudManager.CurrentSprites[lParsedIndex];
                    hudManager.ShowBackground();
                    hudManager.SetVisible(true);
                    hudManager.UpdateRealizedBy(DataManager.Levels[lParsedIndex].author);
                    hudManager.UpdatePar(GameManager.currentPar);
                    hudManager.UpdateStep(GameManager.stepsNum);
                    AudioManager.PlaySound("Menu", false);
                    AudioManager.PlaySound("Lvl1", true);
                    AudioManager.PlaySound("Wind", true);
                    Visible = false;
                    lTween = GameManager.GetInstance().EndTransition();
                    await ToSignal(lTween, "finished");
                    Tile.TileSpawnAnimation();
                    GameManager.isPlaying = true;
                }
            }
        }

        private void ButtonsMenuAnimation()
        {
            Tween lTween = CreateTween();
            Vector2 lPosition;
            float lDelay = 0;
            float lRotationOffset = 5;
            foreach (Button lButton in buttonsMenu.buttons)
            {
                lButton.RotationDegrees += (float)GD.RandRange(0, lRotationOffset);
                lRotationOffset = -lRotationOffset;
                lPosition = lButton.GlobalPosition;
                lButton.GlobalPosition += Vector2.Up * 1500;
                lTween.SetParallel().TweenProperty(lButton, "position", lPosition, 0.8f).SetDelay(lDelay).Finished += shaker.Start;
                lDelay += 0.4f;
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}