// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Assertions;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class contains the logic associated to the in-game pause popup.
    /// </summary>
	public class PausePopup : Popup
	{
		[SerializeField]
		private AnimatedButton musicButton = null;
		
		[SerializeField]
		private AnimatedButton soundButton = null;

		protected override void Awake()
		{
			base.Awake();
			Assert.IsNotNull(musicButton);
			Assert.IsNotNull(soundButton);
		}

		protected override void Start()
		{
			base.Start();
			var currentMusic = PlayerPrefs.GetInt("music_enabled");
			if (currentMusic == 0)
				musicButton.GetComponent<SpriteSwapper>().SwapSprite();
			var currentSound = PlayerPrefs.GetInt("sound_enabled");
			if (currentSound == 0)
				soundButton.GetComponent<SpriteSwapper>().SwapSprite();
		}

		public void OnContinueButtonPressed()
		{
			var gameScreen = ParentScreen as GameScreen;
			if (gameScreen != null)
			{
                Debug.LogError("UnlockInput 8");
                gameScreen.UnlockInput();
			}
			Close();
		}

		public void OnRestartButtonPressed()
		{
			ParentScreen.OpenPopup<ConfirmationPopup>("Popups/ConfirmationPopup", popup =>
			{
				popup.SetInfo("Do you really want to restart the game?", "(You will lose a life)", () =>
				{
					var gameScreen = ParentScreen as GameScreen;
					if (gameScreen != null)
					{
                        Debug.LogError("UnlockInput 9");
                        gameScreen.UnlockInput();
						gameScreen.PenalizePlayer();
						gameScreen.GameLogic.RestartGame();
					}
					
					popup.Close();
					Close();
				});
			});
		}

		public void OnQuitButtonPressed()
		{
			ParentScreen.OpenPopup<ConfirmationPopup>("Popups/ConfirmationPopup", popup =>
			{
				popup.SetInfo("Do you really want to quit the game?", "(You will lose a life)", () =>
				{
					var gameScreen = ParentScreen as GameScreen;
					if (gameScreen != null)
						gameScreen.PenalizePlayer();
					
					GetComponent<ScreenTransition>().PerformTransition();
				});
			});
		}

		public void OnMusicButtonPressed()
		{
			var currentMusic = PlayerPrefs.GetInt("music_enabled");
			currentMusic = 1 - currentMusic;	
            SoundPlayer.SetMusicEnabled(currentMusic == 1);
            PlayerPrefs.SetInt("music_enabled", currentMusic);
		}

		public void OnSoundButtonPressed()
		{
			var currentSound = PlayerPrefs.GetInt("sound_enabled");
			currentSound = 1 - currentSound;	
            SoundPlayer.SetSoundEnabled(currentSound == 1);
            PlayerPrefs.SetInt("sound_enabled", currentSound);
		}
	}
}
