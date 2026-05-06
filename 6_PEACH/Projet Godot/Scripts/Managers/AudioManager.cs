using Com.IsartDigital.ProjectName;
using Godot;
using System;
using System.Collections.Generic;

// Author : Hector Rosier

namespace Com.IsartDigital.Kinisi
{
	public partial class AudioManager : Manager
	{
        #region Singleton

        static private AudioManager instance;

        private AudioManager() { }

        static public AudioManager GetInstance()
        {
            if (instance == null) instance = new AudioManager();
            return instance;
        }

        #endregion

        public static Dictionary<string, AudioStreamPlayer> soundsPlayer = new();

        public override void _Ready()
		{
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(AudioManager) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            foreach (AudioStreamPlayer lSounds in GetChildren()) soundsPlayer[lSounds.Name] = lSounds;
            GetNode<AudioStreamPlayer>("Lvl1").Finished += AudioManagerFinished;
            GetNode<AudioStreamPlayer>("Lvl2").Finished += AudioManagerFinished2;
            GetNode<AudioStreamPlayer>("Lvl3").Finished += AudioManagerFinished3;
        }

        private void AudioManagerFinished3()
        {
            PlaySound("Lvl1",true);

        }

        private void AudioManagerFinished2()
        {
            PlaySound("Lvl3",true);
        }

        private void AudioManagerFinished()
        {
            PlaySound("Lvl2",true);
        }

        public static void PlaySound(string pSound, bool pPlay)
        {
            if (pPlay == true && soundsPlayer.TryGetValue(pSound, out AudioStreamPlayer lSoundPlayer))
            {
                if (lSoundPlayer.Playing) lSoundPlayer.Stop();
                lSoundPlayer.Play();
            }
            else if (pPlay != true && soundsPlayer.TryGetValue(pSound, out lSoundPlayer))
            {
                if (lSoundPlayer.Playing) lSoundPlayer.Stop();
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
