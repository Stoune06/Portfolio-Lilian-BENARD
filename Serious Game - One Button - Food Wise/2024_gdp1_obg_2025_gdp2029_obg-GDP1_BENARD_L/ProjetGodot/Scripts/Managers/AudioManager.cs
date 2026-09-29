using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

//author Lilian Benard

namespace Com.IsartDigital.ProjectName
{
	public partial class AudioManager : Node
	{
		#region Singleton
		static private AudioManager instance;
		private AudioManager() { }

		static public AudioManager GetInstance()
		{
			if(instance == null) instance = new AudioManager();
			return instance;
		}

		#endregion

		public static Dictionary<string,AudioStreamPlayer> sounds = new Dictionary<string,AudioStreamPlayer>();
		public static List<AudioStreamPlayer> soundsList = new List<AudioStreamPlayer>();
		private static List<AudioStreamPlayer> playingSounds = new List<AudioStreamPlayer>();
		public override void _Ready()
		{
			#region Singleton
			if (instance != null)
			{
				QueueFree();
				GD.Print(nameof(AudioManager) + "instance already exists, destroying the last added");
				return;
			}

			instance = this;
			#endregion
			foreach(Node lSound in GetChildren())
			{
				if (lSound is AudioStreamPlayer lPlayer)
				{
					sounds[lSound.Name] = lPlayer;
					soundsList.Add(lPlayer);
				}
			}
		}

        public static void PlaySound(string pSound, bool pPlay)
        {
			AudioStreamPlayer lSoundPlayer = sounds[pSound];
            if (pPlay)
            {
                if (lSoundPlayer.Playing) lSoundPlayer.Stop();
                lSoundPlayer.Play();
				lSoundPlayer.Finished += () => playingSounds.Remove(lSoundPlayer);
				playingSounds.Add(lSoundPlayer);
            }
            else
            {
                if (lSoundPlayer.Playing) lSoundPlayer.Stop();
				playingSounds.Remove(lSoundPlayer);
            }
        }

		public static void StopAllSounds()
		{
			foreach (AudioStreamPlayer lSound in playingSounds) lSound.Stop();
			playingSounds.Clear();
		}
		
		public static void UpdateVolume(float pVolume)
		{
            foreach(AudioStreamPlayer lSound in soundsList) lSound.VolumeDb = pVolume;
        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}
	}
}
