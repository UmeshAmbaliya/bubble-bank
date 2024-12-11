// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class ScoreStarWidget : MonoBehaviour
	{
		public Image Image;
        public Sprite OnSprite;
        public Sprite OffSprite;

        private void Awake()
        {
            Image.sprite = OffSprite;
        }

        public void Activate()
        {
            Image.sprite = OnSprite;
        }

		public void Deactivate()
		{
			Image.sprite = OffSprite;
		}
	}
}
