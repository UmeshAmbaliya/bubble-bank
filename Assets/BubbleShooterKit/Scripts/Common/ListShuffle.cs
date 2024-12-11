// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;

namespace BubbleShooterKit
{
    /// <summary>
    /// A utility class that contains an extension method to shuffle lists.
    /// </summary>
	public static class ListShuffle
	{
		private static readonly Random Rng = new Random();

		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = Rng.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
