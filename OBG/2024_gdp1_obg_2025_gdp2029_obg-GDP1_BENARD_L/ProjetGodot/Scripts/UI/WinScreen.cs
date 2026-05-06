using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class WinScreen : UI
	{
		
		[Export] private HBoxContainer personContainer;
		[Export] private PersonWinScreen pot;
		[Export] private Label starNumber;
		[Export] private TextureButton continueButton;
		[Export] private RichTextLabel label;
		[Export] private AnimationPlayer animationPlayer;

		private List<PersonWinScreen> allPersons = new List<PersonWinScreen>();

		const string STAR_NUMBER_LABEL_SUFFIX = "/5";

		public float totalCount;
		private float restingFood = 0;
		private float potTarget;

        public int totalGoal = 1;
		private float individualGoal = 0;

		private bool isFilling = false;
		private int index = 0;

		private bool alreadyDisplay = false;

		private float personTarget = 0;
		private const float ANIMATION_DURATION = 1f;
		private const float SCALE_ANIMATION_DURATION = 0.5f;

		public int finalScore;
		public int nPlate;

		private PersonWinScreen currentPerson;
		public override void _Ready()
		{
			//PersonPositioning(3);
			
        }

		private void PersonPositioning(int pNPerson)
		{
			PersonWinScreen lPerson;
			float lAngle;
			Vector2 lPosition;
			for(int i = 0; i < pNPerson; i++)
			{
				lAngle = 2* Mathf.Pi * ((float)i /(pNPerson));
				lPosition = Polar2Cartesian(500,lAngle - Mathf.Pi /2f);
				lPerson = PersonWinScreen.CreatePerson(lPosition + (pot.GlobalPosition + pot.Size/2f) , this);
				allPersons.Add(lPerson);
			}
		}

		public void Display()
		{
            if(!alreadyDisplay)continueButton.Pressed += HUDManager.GetInstance().OnContinue;

			for (int i = allPersons.Count - 1; i >= 0; i--) allPersons[i].QueueFree();
			allPersons.Clear();
			PersonPositioning(nPlate);
			label.Visible = false;
			starNumber.Visible = false;
			index = 0;
            pot.SetCutoff(1);
            Visible = true;
			restingFood = potTarget = totalCount;
            individualGoal = totalGoal / (float)allPersons.Count;
			Fill(index);
			alreadyDisplay = true;
        }

		private void ShowStarNumberLabel()
		{
			label.Visible = true;
			if (finalScore == 5) label.Text = "[center][rainbow][wave][font_size=80][color=black]EXCELLENT!";
			else if(finalScore ==4) label.Text = "[center][wave][font_size=50][color=black]TRES BIEN!";
			else if(finalScore ==3) label.Text = "[center][font_size=80][color=black]BIEN!";
			else if(finalScore == 2) label.Text = "[center][tornado][font_size=80][color=black]MAUVAIS...";
			else label.Text = "[center][shake][font_size=50][color=black]TRES MAUVAIS...";
			animationPlayer.Play("perfect");
            starNumber.Text = finalScore + STAR_NUMBER_LABEL_SUFFIX;
            starNumber.PivotOffset = starNumber.Size / 2f;
            starNumber.Scale *= 1.5f;
			
			Tween lTween = CreateTween();
			lTween.TweenProperty(starNumber, TweenProperties.SCALE, Vector2.One, SCALE_ANIMATION_DURATION);
			//lTween.SetParallel().TweenProperty(starNumber, "visible", true, SCALE_ANIMATION_DURATION);
		}

        private void Fill(int pIndex)
		{
			if (pIndex > allPersons.Count - 1 || restingFood <= 0)
			{
				ShowStarNumberLabel();
				return;
			}
            GD.Print("Commencer a fill person numero" + pIndex);
            currentPerson = allPersons[pIndex];
			Tween lTween = CreateTween();
			lTween.TweenProperty(currentPerson, TweenProperties.SCALE, Vector2.One * 1.1f, SCALE_ANIMATION_DURATION);

			//if (restingFood <= 0) return;
			if(individualGoal >= restingFood) currentPerson.TweenCutoff(0, restingFood / individualGoal, ANIMATION_DURATION);
            else currentPerson.TweenCutoff(0f, 1f, ANIMATION_DURATION);
            potTarget -= individualGoal;
			
			pot.TweenCutoff(restingFood/totalCount,potTarget / totalCount,ANIMATION_DURATION, true);
			AudioManager.PlaySound(SoundsList.FILLING, true);
			restingFood = potTarget;
            lTween.TweenProperty(currentPerson, TweenProperties.SCALE, Vector2.One, SCALE_ANIMATION_DURATION).SetDelay(ANIMATION_DURATION).Finished += () => Fill(++index);
		}

		private Vector2 Polar2Cartesian(float pRadius, float pAngle)
		{
			return new Vector2(Mathf.Cos(pAngle),Mathf.Sin(pAngle)) * pRadius;
		}
    }
   
}
