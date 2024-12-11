// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	[Serializable]
	[CreateAssetMenu(fileName = "LevelInfo", menuName = "Bubble Shooter Kit/Level", order = 1)]
	public class LevelInfo : ScriptableObject
	{
		public int Number;
		public int Rows;
		public int Columns;
		public int NumBubbles;
		public int Star1Score;
		public int Star2Score;
		public int Star3Score;
		public Sprite BackgroundSprite;
		public List<LevelRow> Tiles;
		public List<ColorBubbleType> AvailableColors;
		public List<LevelGoal> Goals = new List<LevelGoal>();
		public bool IsSuperAimAvailable;
		public bool IsRainbowBombAvailable;
		public bool IsHorizontalBombAvailable;
		public bool IsCircleBombAvailable;

		public void Initialize()
		{
			AvailableColors = new List<ColorBubbleType>();
			foreach (var value in Enum.GetValues(typeof(ColorBubbleType)))
			{
				AvailableColors.Add((ColorBubbleType)value);
			}

			IsSuperAimAvailable = true;
			IsRainbowBombAvailable = true;
			IsHorizontalBombAvailable = true;
			IsCircleBombAvailable = true;
		}
	}
}
