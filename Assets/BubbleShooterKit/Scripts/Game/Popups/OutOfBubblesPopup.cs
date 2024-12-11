// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class contains the logic associated to the popup that is displayed
    /// to the user when he runs out of bubbles during a game.
    /// </summary>
	public class OutOfBubblesPopup : Popup
	{
		public GameConfiguration GameConfig;
		public CoinsSystem CoinsSystem;
		
		[SerializeField]
		private TextMeshProUGUI numBubblesText = null;
		
		[SerializeField]
		private TextMeshProUGUI questionText = null;
		
		[SerializeField]
		private TextMeshProUGUI priceText = null;
		
		[SerializeField]
		private TextMeshProUGUI priceBorderText = null;

		private GameScreen gameScreen;

		protected override void Awake()
		{
			base.Awake();
			Assert.IsNotNull(numBubblesText);
			Assert.IsNotNull(questionText);
			Assert.IsNotNull(priceText);
			Assert.IsNotNull(priceBorderText);
		}

		public void SetInfo(GameScreen screen)
		{
			gameScreen = screen;
			numBubblesText.text = $"+ {GameConfig.NumExtraBubbles}";
			questionText.text = $"Add {GameConfig.NumExtraBubbles} bubbles to continue?";
			priceText.text = GameConfig.ExtraBubblesCost.ToString();
			priceBorderText.text = priceText.text;
		}
		
		public void OnQuitButtonPressed()
		{
			Close();
			gameScreen.StartCoroutine(gameScreen.OpenLosePopupAsync());
		}
		
		public void OnBuyButtonPressed()
		{
			var numCoins = PlayerPrefs.GetInt("num_coins");
			var cost = GameConfig.ExtraBubblesCost;
			if (numCoins >= cost)
			{
				CoinsSystem.SpendCoins(cost);
				//coinParticles.Play();
				//SoundManager.instance.PlaySound("CoinsPopButton");
				Close();
				gameScreen.GameLogic.ContinueGame();
			}
			else
			{
				//SoundManager.instance.PlaySound("Button");
				gameScreen.OpenCoinsPopup();
			}
		}
	}
}
