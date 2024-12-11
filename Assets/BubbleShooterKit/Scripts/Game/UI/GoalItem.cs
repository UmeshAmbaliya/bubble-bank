// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class GoalItem : MonoBehaviour
	{
		[SerializeField]
		private Image bubbleImage = null;
		
		[SerializeField]
		private TextMeshProUGUI amountText = null;

		public void Initialize(Sprite sprite, int amount)
		{
			bubbleImage.sprite = sprite;
			amountText.text = amount.ToString();
		}
	}
}
