// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Assertions;

namespace BubbleShooterKit
{
	/// <summary>
	/// Static utility class that provides easy access to the current
	/// scene's sound system.
	/// </summary>
	public static class SoundPlayer
	{
		private static SoundSystem soundSystem;
		
		public static void Initialize()
		{
			soundSystem = Object.FindObjectOfType<SoundSystem>();
			Assert.IsNotNull(soundSystem);
		}

		public static void PlaySoundFx(string soundName)
		{
			soundSystem.PlaySoundFx(soundName);
		}
		
        public static void SetSoundEnabled(bool soundEnabled)
        {
	        soundSystem.SetSoundEnabled(soundEnabled);
        }

        public static void SetMusicEnabled(bool musicEnabled)
        {
	        soundSystem.SetMusicEnabled(musicEnabled);
        }
	}
}
