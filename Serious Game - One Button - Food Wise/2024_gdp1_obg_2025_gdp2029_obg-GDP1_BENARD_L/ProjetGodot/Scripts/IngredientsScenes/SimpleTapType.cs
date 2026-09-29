using Godot;
using System;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class SimpleTapType : IngredientScene
	{
		private bool isAlreadyBaking = false;
		private bool isHolding = false;
        protected int index = 0;
		[Export] protected int increment = 1;

		public Marker2D[] bakingPosition;
        public override void _Ready()
		{
		}

		override public void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			if(InputManager.isTouching && !isHolding)
			{
				DoAction(lDelta);
				isHolding = true;
			}
			else if(!InputManager.isTouching && isHolding)
			{
				isHolding = false;
			}
		}

		protected virtual void DoAction(float pDelta)
		{
			if(!isAlreadyBaking)
			{
				AudioManager.PlaySound(SoundsList.COOK, true);
				isAlreadyBaking =true;
			}
            GameManager.GetInstance().count += increment;
			AudioManager.PlaySound(SoundsList.TAC, true);
            HUDManager.GetInstance().gameHUD.UpdateGCount(GameManager.GetInstance().count);
            GD.Print("Tap"); 
		}
	}
}
