// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to rainbow bomb bubbles.
	/// </summary>
	public class RainbowBoosterBubble : PurchasableBoosterBubble
	{
		public override List<Bubble> Resolve(Level level, Bubble shotBubble, Bubble touchedBubble)
		{
			var bubblesToExplode = new List<Bubble>();
			var touchedColorBubble = touchedBubble.GetComponent<ColorBubble>();
			if (touchedColorBubble != null && touchedColorBubble.isInView)
				bubblesToExplode.AddRange(LevelUtils.GetMatches(level, touchedColorBubble));
			bubblesToExplode.Add(this);
			return bubblesToExplode;
		}
	}
}
