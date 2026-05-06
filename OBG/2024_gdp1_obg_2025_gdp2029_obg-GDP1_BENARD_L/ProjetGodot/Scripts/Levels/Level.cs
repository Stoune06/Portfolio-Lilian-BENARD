using Godot;
using System;
using System.Collections.Generic;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class Level : Node2D
	{
		[Export] protected Control marker2DPositions; 

		[Export] public PackedScene[] ingredientsList;
		[Export] protected PackedScene[] recipientsList;

		[Export] public int personNumber = 1;
		[Export] public float time = 10;
		[Export] protected Marker2D ingredientPos;
		[Export] protected Marker2D recipientPos;

		protected IngredientScene currentIngredient;
		protected Recipient currentRecipient;

		public int currentIngredientIndex = 0;
		protected int currentRecipientIndex = 0;

        public override void _Ready()
        {
            base._Ready();
			HUDManager.GetInstance().gCountSuffix = "g";

            marker2DPositions.Size = GetViewportRect().Size;
			//HUDManager.GetInstance().UpdatePersonCount(personNumber);

			currentIngredient = IngredientScene.LoadIngredient(ingredientsList[currentIngredientIndex], this, personNumber,ingredientPos.GlobalPosition);
			currentRecipient = Recipient.LoadRecipient(recipientsList[currentRecipientIndex], this, recipientPos.GlobalPosition);
            currentRecipient.AddNewContent(currentIngredient.contentColor);
            if (currentRecipient is Stove lStove && currentIngredient is SimpleTapType lSimpleTapType) lSimpleTapType.bakingPosition = lStove.bakingPosition;
        }

        public virtual void SwitchIngredient(int pIndex)
        {
			AudioManager.StopAllSounds();
			currentIngredient.IngredientTransitionOut();
            if (pIndex >= ingredientsList.Length)
            {
				GameManager.GetInstance().GameOver();
				//LevelManager.GetInstance().SwitchLevel(1, GameManager.GetInstance());
                return;
            }
            currentIngredient = IngredientScene.LoadIngredient(ingredientsList[pIndex], this, personNumber ,ingredientPos.GlobalPosition);
			currentIngredientIndex = pIndex;
			currentRecipient.nIngredients--;

			if(currentRecipient.nIngredients <= 0) SwitchRecipient();
			currentRecipient.AddNewContent(currentIngredient.contentColor);
		}

		protected void SwitchRecipient()
		{
			if (recipientsList.Length < ++currentRecipientIndex) return;
			currentRecipient.QueueFree();
			currentRecipient = Recipient.LoadRecipient(recipientsList[currentRecipientIndex], this, recipientPos.GlobalPosition);
			if(currentRecipient is Stove lStove && currentIngredient is SimpleTapType lSimpleTapType) lSimpleTapType.bakingPosition = lStove.bakingPosition;
		}

	}
}
