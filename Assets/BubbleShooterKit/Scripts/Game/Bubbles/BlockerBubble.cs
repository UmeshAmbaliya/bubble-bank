// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to blocker bubbles.
	/// </summary>
	public class BlockerBubble : Bubble
	{
		public BlockerBubbleType Type;
		
		public override void ShowExplosionFx(FxPool fxPool)
		{
			var fx = fxPool.GetBlockerBubbleParticlePool(Type).GetObject();
			fx.transform.position = transform.position;
		}

		public override bool CanBeDestroyed()
		{
			return Type != BlockerBubbleType.IronBubble;
		}
	}
}
