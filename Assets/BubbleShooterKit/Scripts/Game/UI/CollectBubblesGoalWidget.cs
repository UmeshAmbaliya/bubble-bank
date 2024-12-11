// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	public class CollectBubblesGoalWidget : LevelGoalWidget, IEventListener<BubblesCollectedEvent>
	{
		private ColorBubbleType colorBubbleType;
		private int numBubbles;
		
		private void Start()
		{
			EventManager.RegisterListener(this);
		}

		private void OnDestroy()
		{
			EventManager.UnregisterListener(this);
		}

		public void Initialize(ColorBubbleType type, int amount)
		{
			colorBubbleType = type;	
			numBubbles = amount;
			TickImage.enabled = false;
		}
		
		public void HandleEvent(BubblesCollectedEvent evt)
		{
			if (evt.Type == colorBubbleType)
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
