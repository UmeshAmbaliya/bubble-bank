// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The base type of booster bubbles.
	/// </summary>
	public abstract class BoosterBubble : Bubble
	{
		public BoosterBubbleType Type;

		public abstract List<Bubble> Resolve(Level level, Bubble shotBubble);
		
		public override void ShowExplosionFx(FxPool fxPool)
		{
			var fx = fxPool.GetBoosterBubbleParticlePool(Type).GetObject();
			fx.transform.position = transform.position;
		}
	}
}
