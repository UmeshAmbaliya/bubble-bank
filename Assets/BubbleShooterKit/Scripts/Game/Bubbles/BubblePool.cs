// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class stores all the bubble pools used during the game.
	/// </summary>
	public class BubblePool : MonoBehaviour
	{
		public List<ObjectPool> ColorBubblePools;
		public List<ObjectPool> BoosterBubblePools;
		public List<ObjectPool> BlockerBubblePools;
		public List<ObjectPool> CollectableBubblePools;
		public List<ObjectPool> CoverPools;
		public List<ObjectPool> PurchasableBoosterBubblePools;
		public ObjectPool EnergyBubblePool;
		public ObjectPool LeafPool;

		public ObjectPool GetColorBubblePool(ColorBubbleType type)
		{
			return ColorBubblePools[(int)type];
		}
		
		public ObjectPool GetBoosterBubblePool(BoosterBubbleType type)
		{
			return BoosterBubblePools[(int)type];
		}
		
		public ObjectPool GetBlockerBubblePool(BlockerBubbleType type)
		{
			return BlockerBubblePools[(int)type];
		}
		
		public ObjectPool GetCollectableBubblePool(CollectableBubbleType type)
		{
			return CollectableBubblePools[(int)type];
		}
		
		public ObjectPool GetCoverPool(CoverType type)
		{
			return CoverPools[(int)type];
		}
		
		public ObjectPool GetPurchasableBoosterBubblePool(BoosterBubbleType type)
		{
			return PurchasableBoosterBubblePools[(int)type];
		}
	}
}
