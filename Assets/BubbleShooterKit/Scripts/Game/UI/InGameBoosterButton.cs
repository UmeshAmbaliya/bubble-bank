// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class InGameBoosterButton : MonoBehaviour
	{
		[SerializeField]
		private PurchasableBoosterBubbleType boosterBubbleType = PurchasableBoosterBubbleType.HorizontalBomb;
		
		[SerializeField]
		private Image normalBackgroundImage = null;
		
		[SerializeField]
		private Image lockedBackgroundImage = null;

		[SerializeField]
		private Image boosterImage = null;

		[SerializeField]
		private Image moreImage = null;
		
		[SerializeField]
		private Image amountImage = null;

		[SerializeField]
		private TextMeshProUGUI amountText = null;

		private GameScreen gameScreen;
		private PlayerBubbles playerBubbles;

		private bool isAvailable;
		
		public void Initialize(GameScreen screen, PlayerBubbles bubbles, Sprite boosterSprite, int amount, bool available)
		{
			gameScreen = screen;
			playerBubbles = bubbles;
					
			isAvailable = available;
			this.gameObject.SetActive(false);
			return;
			if (isAvailable)
			{
				boosterImage.sprite = boosterSprite;
				normalBackgroundImage.gameObject.SetActive(true);
				lockedBackgroundImage.gameObject.SetActive(false);
				if (amount > 0)
				{
					amountText.text = PlayerPrefs.GetInt($"num_boosters_{(int) boosterBubbleType}").ToString();
					amountImage.gameObject.SetActive(true);
					amountText.gameObject.SetActive(true);
					moreImage.gameObject.SetActive(false);
				}
				else
				{
					amountImage.gameObject.SetActive(false);
					amountText.gameObject.SetActive(false);
					moreImage.gameObject.SetActive(true);
				}
			}
			else
			{
				normalBackgroundImage.gameObject.SetActive(false);
				lockedBackgroundImage.gameObject.SetActive(true);
				boosterImage.gameObject.SetActive(false);
				moreImage.gameObject.SetActive(false);
				amountImage.gameObject.SetActive(false);
			}
		}

		public void OnButtonPressed()
		{
			if (!isAvailable)
				return;

			if (playerBubbles.IsPlayingEndGameSequence())
				return;
			
			var amount = PlayerPrefs.GetInt($"num_boosters_{(int)boosterBubbleType}");
			if (amount > 0)
			{
				if (!playerBubbles.IsSpecialBubbleActive)
				{
					amountImage.gameObject.SetActive(true);
					moreImage.gameObject.SetActive(false);
					gameScreen.ApplyBooster(boosterBubbleType);
					amount -= 1;
					if (amount == 0)
					{
						amountImage.gameObject.SetActive(false);
						amountText.gameObject.SetActive(false);
						moreImage.gameObject.SetActive(true);
					}

					PlayerPrefs.SetInt($"num_boosters_{(int) boosterBubbleType}", amount);
					amountText.text = amount.ToString();
				}
			}
			else
			{
				amountImage.gameObject.SetActive(false);
				amountText.gameObject.SetActive(false);
				moreImage.gameObject.SetActive(true);
				gameScreen.OpenPopup<BuyBoostersPopup>("Popups/BuyBoostersPopup",
					popup => { popup.Initialize(boosterBubbleType, this); });
			}
		}

		public void UpdateAmount(int newAmount)
		{
			if (newAmount > 0)
			{
				amountImage.gameObject.SetActive(true);
				moreImage.gameObject.SetActive(false);
				amountText.gameObject.SetActive(true);
				amountText.text = newAmount.ToString();
			}
			else
			{
				amountImage.gameObject.SetActive(false);
				amountText.gameObject.SetActive(false);
				moreImage.gameObject.SetActive(true);
			}
		}
	}
}
