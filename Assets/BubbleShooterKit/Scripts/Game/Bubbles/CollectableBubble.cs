// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to collectable bubbles.
	/// </summary>
	public class CollectableBubble : Bubble
	{
		public CollectableBubbleType Type;

		public override void ShowExplosionFx(FxPool fxPool)
		{
			var fx = fxPool.GetCollectableBubbleParticlePool(Type).GetObject();
			fx.transform.position = transform.position;
		}
	}
}
