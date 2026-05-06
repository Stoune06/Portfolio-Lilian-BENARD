using Godot;
using System;

//Author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class TransitionScreen : UI
	{
		[Export] public AnimationPlayer transitionPlayer;

		private const string TRANSITION_IN = "transition_in";
		private const string TRANSITION_OUT = "transition_out";
		private bool alreadyConnected = false;

		public void TransitionIn()
		{
			AudioManager.PlaySound(SoundsList.TRANSITION_IN, true);
			transitionPlayer.CurrentAnimation = TRANSITION_IN;
			transitionPlayer.Play();
			if (!alreadyConnected)
			{
                transitionPlayer.AnimationFinished += TransitionPlayer_AnimationFinished;
				alreadyConnected = true;
            } 
        }

        private void TransitionPlayer_AnimationFinished(StringName animName)
        {
            AudioManager.PlaySound(SoundsList.TRANSITION_IN, false);
        }

        public void TransitionOut()
		{
            transitionPlayer.CurrentAnimation = TRANSITION_OUT;
			transitionPlayer.Play();
			AudioManager.PlaySound(SoundsList.TRANSITION_OUT, true);
        }
	}
}
