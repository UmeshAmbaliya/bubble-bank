// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The game configuration type. It stores the general settings of the game.
	/// </summary>
	[Serializable]
	[CreateAssetMenu(fileName = "GameConfiguration", menuName = "Bubble Shooter Kit/Game configuration", order = 0)]
	public class GameConfiguration : ScriptableObject
	{
		public int DefaultBubbleScore;
		
		public int MaxLives;
		public int TimeToNextLife;
		public int LivesRefillCost;

		public int InitialCoins;

		public int SuperAimBoosterAmount;
		public int SuperAimBoosterPrice;
		public int RainbowBubbleBoosterAmount;
		public int RainbowBubbleBoosterPrice;
		public int HorizontalBombBoosterAmount;
		public int HorizontalBombBoosterPrice;
		public int CircleBombBoosterAmount;
		public int CircleBombBoosterPrice;

		public int NumExtraBubbles;
		public int ExtraBubblesCost;

		public int RewardedAdCoins;
		public List<IapItem> IapItems;
	}
}
