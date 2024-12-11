// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class TopBound : MonoBehaviour
	{
		private GameScreen gameScreen;

		private void Start()
		{
			gameScreen = GameObject.Find("GameScreen").GetComponent<GameScreen>();
		}
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			var bubble = other.GetComponent<Bubble>();
			if (bubble != null && !bubble.CollidingWithAnotherBubble && gameScreen != null)
			{
				gameScreen.GameLogic.HandleTopRowMatches(bubble);
			}
		}
	}
}
