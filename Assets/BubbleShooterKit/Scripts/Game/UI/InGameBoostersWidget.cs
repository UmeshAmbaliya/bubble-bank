// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	public class InGameBoostersWidget : MonoBehaviour
	{
		[SerializeField]
		private GameScreen gameScreen = null;
		
		[SerializeField]
		private List<Sprite> boosterSprites = null;
			
		[SerializeField]
		private List<InGameBoosterButton> buttons = null;

		public void Initialize(GameConfiguration gameConfig, LevelInfo levelInfo)
		{
			buttons[0].Initialize(gameScreen, gameScreen.PlayerBubbles, boosterSprites[0], PlayerPrefs.GetInt("num_boosters_0"), levelInfo.IsSuperAimAvailable);
			buttons[1].Initialize(gameScreen, gameScreen.PlayerBubbles, boosterSprites[1], PlayerPrefs.GetInt("num_boosters_1"), levelInfo.IsRainbowBombAvailable);
			buttons[2].Initialize(gameScreen, gameScreen.PlayerBubbles, boosterSprites[2], PlayerPrefs.GetInt("num_boosters_2"), levelInfo.IsHorizontalBombAvailable);
			buttons[3].Initialize(gameScreen, gameScreen.PlayerBubbles, boosterSprites[3], PlayerPrefs.GetInt("num_boosters_3"), levelInfo.IsCircleBombAvailable);
		}
	}
}
