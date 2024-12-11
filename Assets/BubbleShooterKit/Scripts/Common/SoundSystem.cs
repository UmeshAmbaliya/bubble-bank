// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The sound system handles the sound pool of the current scene.
	/// </summary>
	public class SoundSystem : MonoBehaviour
	{
		public List<SoundCollection> Collections;

		private ObjectPool soundFxPool;
		private readonly Dictionary<string, AudioClip> nameToSound = new Dictionary<string, AudioClip>();

		private void Awake()
		{
			if (!PlayerPrefs.HasKey("sound_enabled"))
				PlayerPrefs.SetInt("sound_enabled", 1);
			
			if (!PlayerPrefs.HasKey("music_enabled"))
				PlayerPrefs.SetInt("music_enabled", 1);
			
			soundFxPool = GetComponent<ObjectPool>();
			foreach (var collection in Collections)
				foreach (var sound in collection.Sounds)
					nameToSound.Add(sound.name, sound);
		}

		private void Start()
		{
			soundFxPool.Initialize();
		}

		public void PlaySoundFx(string soundName)
		{
			var clip = nameToSound[soundName];
			if (clip != null)
				PlaySoundFx(clip);
		}

		private void PlaySoundFx(AudioClip clip)
		{

			var sound = 1;// DataHandler.inst.Sound;//PlayerPrefs.GetInt("sound_enabled");
			if (sound == 1 && clip != null)
                soundFxPool.GetObject().GetComponent<SoundFx>().Play(clip);
		}
		
        public void SetSoundEnabled(bool soundEnabled)
        {
            PlayerPrefs.SetInt("sound_enabled", soundEnabled ? 1 : 0);
        }

        public void SetMusicEnabled(bool musicEnabled)
        {
            PlayerPrefs.SetInt("music_enabled", musicEnabled ? 1 : 0);
			var bgMusic = FindObjectOfType<BackgroundMusic>();
	        if (bgMusic != null)
	            bgMusic.GetComponent<AudioSource>().mute = !musicEnabled;
        }
	}
}
