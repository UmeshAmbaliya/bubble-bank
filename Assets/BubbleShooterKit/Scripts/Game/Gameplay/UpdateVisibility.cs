// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// Helper class that manages the visibility of the bubbles in a level.
	/// </summary>
	public class UpdateVisibility : MonoBehaviour
	{
		public bool Visible { get; private set; }

		private void OnBecameVisible()
		{
			Visible = true;
		}
		
		private void OnBecameInvisible()
		{
			Visible = false;
		}
	}
}
