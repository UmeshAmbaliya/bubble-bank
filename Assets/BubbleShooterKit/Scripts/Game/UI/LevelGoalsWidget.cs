// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BubbleShooterKit
{
	public class LevelGoalsWidget : MonoBehaviour
	{
		public GameObject CollectBubblesGoalPrefab;
		public GameObject CollectCollectablesGoalPrefab;
		public GameObject CollectLeavesGoalPrefab;

		public List<Sprite> BubbleSprites;
		public List<Sprite> CollectableSprites;
		public Sprite LeafSprite;

		private List<GameObject> goalWidgets;
		
		private void Awake()
		{
			Assert.IsNotNull(CollectBubblesGoalPrefab);
			Assert.IsNotNull(CollectCollectablesGoalPrefab);
			Assert.IsNotNull(CollectLeavesGoalPrefab);
			Assert.IsNotNull(LeafSprite);
		}
		
		public void Initialize(List<LevelGoal> goals, List<ObjectPool> randomizedBubblePrefabs)
		{
			return;
			goalWidgets = new List<GameObject>(goals.Count);
			
			foreach (var goal in goals)
			{
				var bubbleGoal = goal as CollectBubblesGoal;
				if (bubbleGoal != null)
				{
					var obj = Instantiate(CollectBubblesGoalPrefab);
					var goalItem = obj.GetComponent<CollectBubblesGoalWidget>();
					goalItem.Image.sprite = BubbleSprites[(int)bubbleGoal.Type];
					goalItem.Text.text = bubbleGoal.Amount.ToString();
					goalItem.Initialize(bubbleGoal.Type, bubbleGoal.Amount);
					goalItem.transform.SetParent(transform, false);
					goalWidgets.Add(obj);
				}

				var randomBubbleGoal = goal as CollectRandomBubblesGoal;
				if (randomBubbleGoal != null)
				{
					var obj = Instantiate(CollectBubblesGoalPrefab);
					var goalItem = obj.GetComponent<CollectBubblesGoalWidget>();
					var prefab = randomizedBubblePrefabs[(int)randomBubbleGoal.Type];
					var temp = prefab.GetObject();
					goalItem.Image.sprite = temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
					goalItem.Text.text = randomBubbleGoal.Amount.ToString();
					goalItem.Initialize(temp.GetComponent<ColorBubble>().Type, randomBubbleGoal.Amount);
					randomBubbleGoal.ConcreteType = temp.GetComponent<ColorBubble>().Type;
					goalItem.transform.SetParent(transform, false);
					temp.GetComponent<PooledObject>().Pool.ReturnObject(temp);
					goalWidgets.Add(obj);
				}
				
				var collectableBubbleGoal = goal as CollectCollectablesGoal;
				if (collectableBubbleGoal != null)
				{
					var obj = Instantiate(CollectCollectablesGoalPrefab);
					var goalItem = obj.GetComponent<CollectCollectablesGoalWidget>();
					goalItem.Image.sprite = CollectableSprites[(int)collectableBubbleGoal.Type];
					goalItem.Text.text = collectableBubbleGoal.Amount.ToString();
					goalItem.Initialize(collectableBubbleGoal.Type, collectableBubbleGoal.Amount);
					goalItem.transform.SetParent(transform, false);
					goalWidgets.Add(obj);
				}

				var leafGoal = goal as CollectLeavesGoal;
				if (leafGoal != null)
				{
					var obj = Instantiate(CollectLeavesGoalPrefab);
					var goalItem = obj.GetComponent<CollectLeavesGoalWidget>();
					goalItem.Image.sprite = LeafSprite;
					goalItem.Text.text = leafGoal.Amount.ToString();
					goalItem.Initialize(leafGoal.Amount);
					goalItem.transform.SetParent(transform, false);
					goalWidgets.Add(obj);
				}
			}
		}

		public void Reset()
		{
			foreach (var widget in goalWidgets)
				Destroy(widget);
			
			goalWidgets.Clear();
		}
	}
}
