// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to color bomb bubbles.
	/// </summary>
	public class ColorBombBubble : BoosterBubble
	{
		public override List<Bubble> Resolve(Level level, Bubble shotBubble)
		{
	        var bubblesToExplode = new List<Bubble>();

			if (shotBubble.GetComponent<ColorBubble>() != null)
			{
				foreach (var row in level.Tiles)
				{
					foreach (var bubble in row)
					{
						if (bubble != null &&
						    bubble.GetComponent<ColorBubble>() != null &&
						    bubble.GetComponent<ColorBubble>().Visible &&
						    bubble.GetComponent<ColorBubble>().Type == shotBubble.GetComponent<ColorBubble>().Type)
						{
							bubblesToExplode.Add(bubble);
						}
					}
				}
			}

			bubblesToExplode.Add(this);
			
			return bubblesToExplode;
		}
	}
}
