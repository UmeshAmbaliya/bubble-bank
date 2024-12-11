// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	public class CollectCollectablesGoalWidget : LevelGoalWidget, IEventListener<CollectablesCollectedEvent>
	{
		private CollectableBubbleType collectableType;
		private int numBubbles;
		
		private void Start()
		{
			EventManager.RegisterListener(this);
		}
		
		private void OnDestroy()
		{
			EventManager.UnregisterListener(this);
		}

		public void Initialize(CollectableBubbleType type, int amount)
		{
			collectableType = type;	
			numBubbles = amount;
			TickImage.enabled = false;
		}
		
		public void HandleEvent(CollectablesCollectedEvent evt)
		{
			if (evt.Type == collectableType)
			{
				numBubbles -= evt.Amount;
				if (numBubbles <= 0)
				{
					if (!TickImage.enabled)
						SoundPlayer.PlaySoundFx("ReachedGoal");
					TickImage.enabled = true;
					numBubbles = 0;
				}
				Text.text = numBubbles.ToString();
			}
		}
	}
}

