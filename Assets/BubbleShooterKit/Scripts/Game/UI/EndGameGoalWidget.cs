// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class EndGameGoalWidget : MonoBehaviour
	{
		public Image Image;
		public Image TickImage;
		public Image CrossImage;

		public void Initialize(bool completed, LevelGoalWidget widget)
		{
			Image.sprite = widget.Image.sprite;
			TickImage.enabled = completed;
			CrossImage.enabled = !completed;
		}
	}
}
