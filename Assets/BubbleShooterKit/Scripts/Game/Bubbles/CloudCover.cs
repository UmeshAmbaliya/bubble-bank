// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to cloud covers.
	/// </summary>
	public class CloudCover : BubbleCover
	{
		private FxPool destructionFxPool;

		private SpriteRenderer spriteRenderer;
		private Sprite originalSprite;
		
		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			originalSprite = spriteRenderer.sprite;
		}

		private void OnDisable()
		{
			spriteRenderer.sprite = originalSprite;
			spriteRenderer.sortingOrder = 2;
		}
	}
}
