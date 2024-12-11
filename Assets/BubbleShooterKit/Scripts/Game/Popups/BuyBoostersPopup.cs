// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class BuyBoostersPopup : Popup
	{
		[SerializeField]
		private GameConfiguration gameConfig = null;

		[SerializeField]
		private CoinsSystem coinsSystem = null;
		
		[SerializeField]
		private List<Sprite> boosterSprites = null;

		[SerializeField]
		private Image boosterImage = null;

		[SerializeField]
		private TextMeshProUGUI boosterNameText = null;
		
		[SerializeField]
		private TextMeshProUGUI boosterAmountText = null;
		
		[SerializeField]
		private TextMeshProUGUI boosterDescriptionText = null;
		
		[SerializeField]
		private TextMeshProUGUI boosterPriceText = null;

		private PurchasableBoosterBubbleType boosterBubbleType;
		private InGameBoosterButton boosterButton;

		protected override void Awake()
		{
			base.Awake();
			Assert.IsNotNull(gameConfig);
			Assert.IsNotNull(boosterImage);
			Assert.IsNotNull(boosterNameText);
			Assert.IsNotNull(boosterAmountText);
			Assert.IsNotNull(boosterDescriptionText);
			Assert.IsNotNull(boosterPriceText);

			OnOpen.AddListener(() =>
			{
				var gameScreen = ParentScreen as GameScreen;
				if (gameScreen != null)
					gameScreen.OpenTopCanvas();
			});
			OnClose.AddListener(() =>
			{
				var gameScreen = ParentScreen as GameScreen;
				if (gameScreen != null)
					gameScreen.CloseTopCanvas();
			});
		}

		public void Initialize(PurchasableBoosterBubbleType bubbleType, InGameBoosterButton button)
		{
			boosterBubbleType = bubbleType;
			boosterButton = button;
			boosterImage.sprite = boosterSprites[(int)boosterBubbleType];
			boosterImage.SetNativeSize();

			switch (boosterBubbleType)
			{
				case PurchasableBoosterBubbleType.SuperAim:
					boosterNameText.text = "Super aim";
					boosterDescriptionText.text = "Lengthens your shooting line";
					boosterAmountText.text = $"x{gameConfig.SuperAimBoosterAmount}";
					boosterPriceText.text = gameConfig.SuperAimBoosterPrice.ToString();
					break;
				
				case PurchasableBoosterBubbleType.RainbowBubble:
					boosterNameText.text = "Rainbow bubble";
					boosterDescriptionText.text = "Matches with any bubble";
					boosterAmountText.text = $"x{gameConfig.RainbowBubbleBoosterAmount}";
					boosterPriceText.text = gameConfig.RainbowBubbleBoosterPrice.ToString();
					break;
				
				case PurchasableBoosterBubbleType.HorizontalBomb:
					boosterNameText.text = "Horizontal bomb";
					boosterDescriptionText.text = "Destroys the entire row touched";
					boosterAmountText.text = $"x{gameConfig.HorizontalBombBoosterAmount}";
					boosterPriceText.text = gameConfig.HorizontalBombBoosterPrice.ToString();
					break;
				
				case PurchasableBoosterBubbleType.CircleBomb:
					boosterNameText.text = "Circle bomb";
					boosterDescriptionText.text = "Destroys the circle around the touched row";
					boosterAmountText.text = $"x{gameConfig.CircleBombBoosterAmount}";
					boosterPriceText.text = gameConfig.CircleBombBoosterPrice.ToString();
					break;
			}
		}
		
		public void OnBuyButtonPressed()
		{
		    var playerPrefsKey = $"num_boosters_{(int)boosterBubbleType}";
		    var numBoosters = PlayerPrefs.GetInt(playerPrefsKey);

		    Close();

		    var gameScreen = ParentScreen as GameScreen;
		    if (gameScreen != null)
		    {
			    var cost = GetBoosterCost(boosterBubbleType);
				if (!PlayerPrefs.HasKey("num_coins"))
				    PlayerPrefs.SetInt("num_coins", gameConfig.InitialCoins);
			    var coins = PlayerPrefs.GetInt("num_coins");
			    if (cost > coins)
			    {
				    var button = boosterButton;
				    gameScreen.OpenPopup<BuyCoinsPopup>("Popups/BuyCoinsPopup",
					    popup =>
					    {
						    popup.OnClose.AddListener(
							    () =>
							    {
								    gameScreen.OpenPopup<BuyBoostersPopup>("Popups/BuyBoostersPopup",
									    buyBoostersPopup => { buyBoostersPopup.Initialize(boosterBubbleType, button); });
							    });
					    });
			    }
			    else
			    {
				    coinsSystem.SpendCoins(cost);
                    SoundPlayer.PlaySoundFx("CoinsPopButton");
				    numBoosters += GetBoosterAmount(boosterBubbleType);
				    PlayerPrefs.SetInt(playerPrefsKey, numBoosters);
				    boosterButton.UpdateAmount(numBoosters);
			    }
		    }
		}
		
		private int GetBoosterAmount(PurchasableBoosterBubbleType bubbleType)
		{
			switch (bubbleType)
			{
				case PurchasableBoosterBubbleType.SuperAim:
					return gameConfig.SuperAimBoosterAmount;
				
				case PurchasableBoosterBubbleType.RainbowBubble:
					return gameConfig.RainbowBubbleBoosterAmount;
				
				case PurchasableBoosterBubbleType.HorizontalBomb:
					return gameConfig.HorizontalBombBoosterAmount;
				
				default:
					return gameConfig.CircleBombBoosterAmount;
			}
		}

		private int GetBoosterCost(PurchasableBoosterBubbleType bubbleType)
		{
			switch (bubbleType)
			{
				case PurchasableBoosterBubbleType.SuperAim:
					return gameConfig.SuperAimBoosterPrice;
				
				case PurchasableBoosterBubbleType.RainbowBubble:
					return gameConfig.RainbowBubbleBoosterPrice;
				
				case PurchasableBoosterBubbleType.HorizontalBomb:
					return gameConfig.HorizontalBombBoosterPrice;
				
				default:
					return gameConfig.CircleBombBoosterPrice;
			}
		}
	}
}
