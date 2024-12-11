// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// Utility class that provides some helper canvas methods.
	/// </summary>
	public static class CanvasUtils
	{
		public static Vector2 CanvasToWorldPoint(RectTransform rt)
		{
			var worldCorners = new Vector3[4];
			rt.GetWorldCorners(worldCorners);
			var width = worldCorners[3].x - worldCorners[0].x;
			var height = worldCorners[2].y - worldCorners[3].y;
			return new Vector2(worldCorners[0].x + width/2, worldCorners[0].y + height/2);
		}

	}
}
