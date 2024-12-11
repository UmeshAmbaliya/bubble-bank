// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to circle bomb bubbles.
	/// </summary>
	public class BombBoosterBubble : PurchasableBoosterBubble
	{
		public override List<Bubble> Resolve(Level level, Bubble shotBubble, Bubble touchedBubble)
		{
			var bubblesToExplode = new List<Bubble>();
			bubblesToExplode.AddRange(LevelUtils.GetNeighbours(level, this));
			bubblesToExplode.Add(this);
			return bubblesToExplode;
		}
	}
}
