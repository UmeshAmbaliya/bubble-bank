// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class manages the high-level logic of the home screen.
	/// </summary>
	public class HomeScreen : BaseScreen
	{
		[SerializeField]
		private GameObject bgMusicPrefab = null;
		
		[SerializeField]
		private GameObject purchaseManagerPrefab = null;
		
		protected override void Start()
		{
			base.Start();
			
			var bgMusic = FindObjectOfType<BackgroundMusic>();
			if (bgMusic == null)
				Instantiate(bgMusicPrefab);
			
#if BUBBLE_SHOOTER_ENABLE_IAP
			var purchaseManager = FindObjectOfType<PurchaseManager>();
			if (purchaseManager == null)
				Instantiate(purchaseManagerPrefab);
#endif
		}
		
        public void OnSettingsButtonPressed()
        {
            OpenPopup<SettingsPopup>("Popups/SettingsPopup");
        }
	}
}
