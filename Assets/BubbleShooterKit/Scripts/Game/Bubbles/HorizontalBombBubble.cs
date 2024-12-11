// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to horizontal bomb bubbles.
	/// </summary>
    public class HorizontalBombBubble : BoosterBubble
    {
        public override List<Bubble> Resolve(Level level, Bubble shotBubble)
        {
	        var bubblesToExplode = new List<Bubble>();
	        
	        for (var i = 0; i < level.Tiles[Row].Count; i++)
	        {
		        var bubble = level.Tiles[Row][i];
		        if (bubble != null)
			        bubblesToExplode.Add(bubble);
	        }

	        return bubblesToExplode;
        }
    }
}
