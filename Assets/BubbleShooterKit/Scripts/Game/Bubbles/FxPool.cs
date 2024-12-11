// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class stores all the bubble particle pools used during the game.
	/// </summary>
	public class FxPool : MonoBehaviour
	{
		public List<ObjectPool> ColorBubbleParticlePools;
		public List<ObjectPool> BlockerBubbleParticlePools;
		public List<ObjectPool> BoosterBubbleParticlePools;
		public List<ObjectPool> CollectableBubbleParticlePools;
		public List<ObjectPool> CoverParticlePools;
		public ObjectPool LeafParticlePool;
		public ObjectPool EnergyBubblePool;

		public ObjectPool GetColorBubbleParticlePool(ColorBubbleType type)
		{
			return ColorBubbleParticlePools[(int)type];
		}
		
		public ObjectPool GetBlockerBubbleParticlePool(BlockerBubbleType type)
		{
			return BlockerBubbleParticlePools[(int)type];
		}
		
		public ObjectPool GetBoosterBubbleParticlePool(BoosterBubbleType type)
		{
			return BoosterBubbleParticlePools[(int)type];
		}
		
		public ObjectPool GetCollectableBubbleParticlePool(CollectableBubbleType type)
		{
			return CollectableBubbleParticlePools[(int)type];
		}
		
		public ObjectPool GetCoverParticlePool(CoverType type)
		{
			return CoverParticlePools[(int)type];
		}
	}
}
