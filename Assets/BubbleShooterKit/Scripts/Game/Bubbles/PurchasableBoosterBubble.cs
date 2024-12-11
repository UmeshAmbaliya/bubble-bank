// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The base type of purchasable booster bubbles.
	/// </summary>
	public abstract class PurchasableBoosterBubble : Bubble
	{
		public PurchasableBoosterBubbleType Type;

		public abstract List<Bubble> Resolve(Level level, Bubble shotBubble, Bubble touchedBubble);
	}
}

