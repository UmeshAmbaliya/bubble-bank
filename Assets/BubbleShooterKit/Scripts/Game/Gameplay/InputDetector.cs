// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class is responsible for handling the player input during a game.
	/// </summary>
	public class InputDetector : MonoBehaviour
	{
		[SerializeField]
		private GameScreen gameScreen = null;
		[SerializeField]
		private PlayerBubbles playerBubbles = null;
		[SerializeField]
		private Shooter shooter = null;
		[SerializeField]
		private GameObject swapBubblesIcon = null;
		[SerializeField]
		private GameObject energyOrb = null;
		
		private Camera mainCamera;

		private void Awake()
		{
			Assert.IsNotNull(gameScreen);
			Assert.IsNotNull(playerBubbles);
			Assert.IsNotNull(shooter);
			Assert.IsNotNull(swapBubblesIcon);
			Assert.IsNotNull(energyOrb);
		}
		
		private void Start()
		{
			mainCamera = Camera.main;
		}
		
		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (gameScreen.CurrentPopups.Count > 0)
					return;
				
				var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				
				var pointer = new PointerEventData(EventSystem.current);
				pointer.position = mainCamera.WorldToScreenPoint(mousePos);
				var raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointer, raycastResults);
				if (raycastResults.Count > 0 &&
				    (raycastResults[0].gameObject == swapBubblesIcon || raycastResults[0].gameObject == energyOrb))
				{
					shooter.SetInputEnabled(false);
					playerBubbles.ProcessInput(raycastResults[0].gameObject);
				}
				else
				{
					var hit = Physics2D.Raycast(mousePos, Vector3.forward);
					if (hit.collider != null)
					{
						if(hit.collider.gameObject.layer == 13)
						{
                            shooter.SetInputEnabled(true);
						}
						else
						{
							shooter.SetInputEnabled(false);
							playerBubbles.ProcessInput(hit.collider.gameObject);
						}
					}
					else
					{
						shooter.SetInputEnabled(true);
					}
				}
			}
		}
	}
}
