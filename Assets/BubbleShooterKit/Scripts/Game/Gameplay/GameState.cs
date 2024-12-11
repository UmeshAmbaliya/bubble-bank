// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class stores the current state of the game.
	/// </summary>
	public class GameState :
		IEventListener<BubblesCollectedEvent>,
		IEventListener<CollectablesCollectedEvent>,
		IEventListener<LeavesCollectedEvent>
	{
		public int Score;
		public int clearBoardScore = 0;
		public int timeBonusScore = 0;
        public int totalScore
		{
			get
			{
				return Score + clearBoardScore + timeBonusScore;
			}
		}
		public readonly Dictionary<ColorBubbleType, int> ExplodedBubbles = new Dictionary<ColorBubbleType, int>();
		public readonly Dictionary<CollectableBubbleType, int> CollectedCollectables = new Dictionary<CollectableBubbleType, int>();
		public int CollectedLeaves;

		public GameState()
		{
			EventManager.RegisterListener<BubblesCollectedEvent>(this);
			EventManager.RegisterListener<CollectablesCollectedEvent>(this);
			EventManager.RegisterListener<LeavesCollectedEvent>(this);
			Reset();
		}

		public void Reset()
		{
			Score = 0;
			ExplodedBubbles.Clear();
			CollectedCollectables.Clear();
			CollectedLeaves = 0;

			foreach (var value in Enum.GetValues(typeof(ColorBubbleType)))
				ExplodedBubbles.Add((ColorBubbleType)value, 0);
			
			foreach (var value in Enum.GetValues(typeof(CollectableBubbleType)))
				CollectedCollectables.Add((CollectableBubbleType)value, 0);
		}

		public void HandleEvent(BubblesCollectedEvent evt)
		{
			ExplodedBubbles[evt.Type] += evt.Amount;
		}
		
		public void HandleEvent(CollectablesCollectedEvent evt)
		{
			CollectedCollectables[evt.Type] += evt.Amount;
		}

		public void HandleEvent(LeavesCollectedEvent evt)
		{
			CollectedLeaves += evt.Amount;
		}
	}
}
