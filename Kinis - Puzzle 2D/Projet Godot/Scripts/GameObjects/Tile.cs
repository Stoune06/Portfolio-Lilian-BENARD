using Com.IsartDigital.Kinisi;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName {
	
	public partial class Tile : GameObject
    {
        public static List<Tile> allTiles = new List<Tile>();

        public Godot.Color baseColor;
        public Godot.Color greenColor;
        [Export] private int frameCount = 3;
        private static float animationDelay = 0.1f;
        private static float delayIncrement = 0.4f;
        private static float fallDuration = 1.5f;
        public Sprite2D cadre;
        Vector2 mousePos;
        public bool isAlreadyPlaying = false;
        public bool isGreen = false;
        public Tween cadreTween;

        RandomNumberGenerator rand = new RandomNumberGenerator();
        
        public override void _Ready()
        {
            rand.Randomize();
            base._Ready();
			allTiles.Add(this);
            cadre = (Sprite2D)GetNode("AnimatedSprite2D/Sprite2D");
            outRenderer.Frame = rand.RandiRange(0, frameCount);
            baseColor = cadre.Modulate;
            greenColor = new Godot.Color(0, 1, 0, 1);
            
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (isGreen)
            {
                Visible = true;
            }
        }

        public static void TileSpawnAnimation()
        {
            //gameManager.screenShake.Start();
            //Tween lTween = gameManager.CreateTween();
            //lTween.Finished += TileSpawnAnimationFinished;
            //float lDelay = animationDelay;
            //int lBaseZindex = Spawner.BASE_TILE_ZINDEX;
            //AudioManager.PlaySound("Spawn", true);
            //foreach (Tile lTile in allTiles)
            //{
            //    lTile.Position += new Vector2(0, ScreenManager.GetInstance().screenSize.Y + yOffset);
            //    if (lTile.ZIndex == lBaseZindex)
            //    {
            //        lDelay += delayIncrement;
            //        lTween.Parallel().TweenProperty(lTile, "position", lTile.originPos, fallDuration).SetDelay(lDelay).Finished += gameManager.screenShake3.Start;
            //    }
            //    else lTween.Parallel().TweenProperty(lTile, "position", lTile.originPos, fallDuration).SetDelay(lDelay);
            //}
            //foreach (Movable lMovable in Movable.allMovables)
            //{
            //    if (lMovable is Player) continue;
            //    lMovable.Position -= new Vector2(0, ScreenManager.GetInstance().screenSize.Y + yOffset);
            //}
            foreach(Tile lTile in allTiles)
            {
                //lTile.GlobalPosition = lTile.endPos;
            }
            TileSpawnAnimationFinished();

        }

        private void TileSFX()
        {
            //switch (GD.RandRange(0, 2))
            //{
            //    case 0:
            //        AudioManager.GetInstance().GetNode<AudioStreamPlayer>("SpawnTile").PitchScale = 1.0f;
            //        break;
            //    case 1:
            //        AudioManager.GetInstance().GetNode<AudioStreamPlayer>("SpawnTile").PitchScale = 1.4f;
            //        break;
            //    case 2:
            //        AudioManager.GetInstance().GetNode<AudioStreamPlayer>("SpawnTile").PitchScale = 0.65f;
            //        break;
            //}

            //AudioManager.PlaySound("SpawnTile",true);
        }

        /*public void TileSelected()
        {
            cadreTween = gameManager.CreateTween();
            if (!isAlreadyPlaying && !isGreen)
            {
                cadreTween.Stop();
                cadreTween.Kill();
                return;
                
            }        
            else
            {
                cadreTween.Finished += TileSelected;
            }

            cadreTween.TweenProperty(cadre, "scale", new Vector2(0.8f, 0.8f), 0.5f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
            cadreTween.Parallel().TweenProperty(cadre ,"modulate:a",0.75f,0.5f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
            cadreTween.TweenProperty(cadre, "scale", new Vector2(1.874f,1.574f), 0.5f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
            cadreTween.Parallel().TweenProperty(cadre, "modulate:a", 0.5f, 0.5f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

        }
        */
        public override void _Notification(int what)
        {
            base._Notification(what);
            if (what== NotificationPredelete && cadreTween != null)
            {
                cadreTween.Stop();
                QueueFree();
            }
  
        }
        public static void TileSpawnAnimationFinished()
        {

            Movable.MovablesSpawnAnimation();
            gameManager.screenShake.Stop();
        }

    }
}
