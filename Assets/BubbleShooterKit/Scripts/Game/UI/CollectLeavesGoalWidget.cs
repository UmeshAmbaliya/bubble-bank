// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	public class CollectLeavesGoalWidget : LevelGoalWidget, IEventListener<LeavesCollectedEvent>
	{
		private int numLeaves;
		
		private void Start()
		{
			EventManager.RegisterListener(this);
		}
		
		private void OnDestroy()
		{
			EventManager.UnregisterListener(this);
		}

		public void Initialize(int amount)
		{
			numLeaves = amount;
			TickImage.enabled = false;
		}
		
		public void HandleEvent(LeavesCollectedEvent evt)
		{
			numLeaves -= evt.Amount;
			if (numLeaves <= 0)
			{
				if (!TickImage.enabled)
					SoundPlayer.PlaySoundFx("ReachedGoal");
				TickImage.enabled = true;
				numLeaves = 0;
			}
			Text.text = numLeaves.ToString();
		}
	}
}
