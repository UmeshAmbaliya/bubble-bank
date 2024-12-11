// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// Utility component to set the framerate of the game to a
	/// target value.
	/// </summary>
	public class SetFps : MonoBehaviour
	{
		public int Fps = 60;
		
		private void Start()
		{
			Application.targetFrameRate = Fps;
		}
	}
}
