using Com.IsartDigital.Kinisi;
using Com.IsartDigital.Kinisi.Login;
using Com.IsartDigital.Kinisi.Menu;
using Godot;
using System;
using System.Threading.Tasks;
using Com.IsartDigital.ProjectName;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

// Author : Hector Rosier

namespace Com.IsartDigital.ProjectName
{

    public partial class WinScreen : CanvasLayer
    {
        [Export] private Button menu;
        [Export] private Button next;
        [Export] private PackedScene sceneMenu;
        [Export] private ColorRect background;
        [Export] private AnimatedSprite2D flameOne;
        [Export] private AnimatedSprite2D flameTwo;
        [Export] private AnimatedSprite2D flameThree;
        [Export] private AnimatedSprite2D bigFlame;
        [Export] private GpuParticles2D flameParticles;
        private Tween flameTween;
        private List<AnimatedSprite2D> flames;

        private Timer timer = new();
        private Timer timer2 = new();
        private Timer timer3 = new();
        private Timer timer4 = new();
        private Timer timer5 = new();
        private Timer timer6 = new();
        GameManager gameManager;
        HUDManager hudManager;
        private const string PATH_GAME_CONTAINER = "SubViewport/GameContainer";
        private const string PATH_PARALLAX = "Menu/ParallaxBackground";

        public override void _Ready()
        {
            menu.ButtonDown += MenuButtonDown;
            next.ButtonDown += NextButtonDown;
            InputManager.canPlay = false;
            hudManager = HUDManager.GetInstance();
            gameManager = GameManager.GetInstance();
            flames = new List<AnimatedSprite2D>() { flameOne, flameTwo, flameThree };
        }

        public void ShowScore(int pStarsNum)
        {
            AudioManager.PlaySound("PreWin",true);
            AnimatedSprite2D lCurrentFlame;
            flameTween = CreateTween();

            AddChild(timer2);
            timer2.WaitTime = 0.8;
            timer2.Autostart = false;
            timer2.Timeout += Timer2Timeout;
            timer2.Start();

            AddChild(timer3);
            timer3.WaitTime = 1.2;
            timer3.Autostart = false;
            timer3.Timeout += Timer3Timeout;
            timer3.Start();

            AddChild(timer4);
            timer4.WaitTime = 1.9;
            timer4.Autostart = false;
            timer4.Timeout += Timer4Timeout;
            timer4.Start();

            AddChild(timer5);
            timer5.WaitTime = 2.4;
            timer5.Autostart = false;
            timer5.Timeout += Timer5_Timeout;
            timer5.Start();

            AddChild(timer6);
            timer6.WaitTime = 3;
            timer6.Autostart = false;
            timer6.Timeout += Timer6_Timeout;
            timer6.Start();

            for (int i = 0; i < pStarsNum; i++)
            {
                lCurrentFlame = flames[i];
                Vector2 lEndScale = lCurrentFlame.Scale;
                lCurrentFlame.Scale = Vector2.Zero;
                lCurrentFlame.Visible = true;
                lCurrentFlame.Play();
                flameTween.TweenProperty(lCurrentFlame, TweenProp.SCALE, Vector2.One * 0.6f, 0.4f);
                flameTween.TweenProperty(lCurrentFlame, TweenProp.SCALE, lEndScale, 0.2f).Finished += gameManager.screenShake3.Start;
                AudioManager.PlaySound("Flame1",true);
                
            }
            if (pStarsNum == 3)
            {
                Vector2 lFinalScale = bigFlame.Scale;
                bigFlame.Scale = Vector2.Zero;
                bigFlame.Visible = true;
                bigFlame.Play();

                for (int i = 0;i < pStarsNum; i++)
                {
                    Vector2 lNextScale = lFinalScale * ((float)(i + 1) / 3);
                    lCurrentFlame = flames[i];
                    flameTween.TweenProperty(lCurrentFlame, TweenProp.POSITION, bigFlame.Position, 0.4f);
                    flameTween.SetParallel().TweenProperty(lCurrentFlame, TweenProp.SCALE, Vector2.Zero, 0.5f);
                    flameTween.SetParallel().TweenProperty(bigFlame, TweenProp.SCALE, lNextScale*1.3f, 0.2f);
                    flameTween.SetParallel().TweenProperty(flameParticles, "explosiveness", 1, 0.2f).From(0);
                    flameTween.SetParallel().TweenProperty(flameParticles, "visible", true, 0.2f).From(false).Finished += gameManager.screenShake.Start;
                    flameTween.SetParallel().TweenProperty(background, TweenProp.MODULATE, Colors.Red, 0.2f).From(background.Modulate);
                    flameTween.SetParallel(false);
                    flameTween.TweenProperty(bigFlame, TweenProp.SCALE, lNextScale, 0.1f);
                }
                flameTween.SetParallel().TweenProperty(flameParticles, "explosiveness", 0, 0.2f);
                AddChild(timer);
                timer.WaitTime = 3;
                timer.Autostart = false;
                timer.Timeout += LTimerTimeout1;
                timer.Start();
            }
        }

        private void Timer6_Timeout()
        {
            AudioManager.PlaySound("Flame2", true);
            timer6.QueueFree();
        }

        private void Timer5_Timeout()
        {
            AudioManager.PlaySound("Flame1", true);
            timer5.QueueFree();
        }

        private void Timer4Timeout()
        {
            AudioManager.PlaySound("Flame4", true);
            timer4.QueueFree();
        }

        private void Timer3Timeout()
        {
            AudioManager.PlaySound("Flame3", true);
            timer3.QueueFree();
        }

        private void Timer2Timeout()
        {
            AudioManager.PlaySound("Flame2", true);
            timer2.QueueFree();
        }

        private void LTimerTimeout1()
        {
            AudioManager.PlaySound("Flame5",true);
            timer.QueueFree();
        }

        private void NextButtonDown()
        {
            DeleteLevel();

            AudioManager.PlaySound("Click", true);
            Timer lTimer = new Timer();
            AddChild(lTimer);
            lTimer.Autostart = true;
            lTimer.WaitTime = 0.2f;
            lTimer.Timeout += LTimerTimeout;
            lTimer.Start();
            
        }

        private void LTimerTimeout()
        {
            NextLevel();

        }

        private void LTimer_Timeout()
        {
            throw new NotImplementedException();
        }

        private void NextLevel()
        {
            

            Menu.levelIndex++;
            Menu.levelCount++;

            Node2D lNextLevel = Menu.sceneLevels[Menu.levelIndex].Instantiate() as Node2D;
            GameManager.currentPar = DataManager.Levels[Menu.levelIndex].par;

            GetParent().GetNode<Node2D>("SubViewport/GameContainer").AddChild(lNextLevel);

            Spawner.SpawnGameObjects(DataManager.LevelsGrid[Menu.levelIndex], lNextLevel, GameManager.GetInstance().tileSize);
            Spawner.SpawnTargets(DataManager.LevelsTarget[Menu.levelIndex], lNextLevel);

            hudManager.UpdateBackground();
            hudManager.CurrentBackground = hudManager.CurrentSprites[Menu.levelIndex];
            hudManager.ShowBackground();
            hudManager.UpdatePar(GameManager.currentPar);
            hudManager.UpdateStep(GameManager.stepsNum);
            hudManager.UpdateRealizedBy(DataManager.Levels[Menu.levelIndex].author);

            Tile.TileSpawnAnimation();
            GameManager.isPlaying = true;
            QueueFree();
        }

        private void MenuButtonDown()
        {
            //Menu lMenu = sceneMenu.Instantiate() as Menu;
            //GetParent().AddChild(lMenu);
            Menu.GetInstance().Visible = true;
            AudioManager.PlaySound("Click", true);

            DeleteLevel();

            GetTree().Root.GetChild(0).GetNode<ParallaxBackground>(PATH_PARALLAX).Layer = 0;

            QueueFree();
        }

        private void DeleteLevel()
        {
            GameManager.isPlaying = false;
            DataManager.GetInstance().UnlockNextLevel();
            GetParent().GetNode<Node2D>(PATH_GAME_CONTAINER).GetChild(0).QueueFree();
            Tile.allTiles.Clear();
            Movable.allMovables.Clear();
            GridManager.GetInstance().movableObjectsOnGrid.Clear();
            GridManager.GetInstance().targetList.Clear();
            GameManager.ResetLevelData();
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;

        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}