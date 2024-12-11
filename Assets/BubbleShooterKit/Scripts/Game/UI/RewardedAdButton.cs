// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace BubbleShooterKit
{
	/// <summary>
	/// The rewarded advertisement button that is present in the level screen.
	/// </summary>
	public class RewardedAdButton : MonoBehaviour
	{
		[SerializeField]
		private GameConfiguration gameConfig;
		
		[SerializeField]
		private CoinsSystem coinsSystem;
		
		[SerializeField]
		private LevelScreen levelScreen;
		
		public void ShowRewardedAd()
		{
			#if UNITY_ADS
			if (Advertisement.IsReady("rewardedVideo"))
			{
				var options = new ShowOptions { resultCallback = HandleShowResult };
				Advertisement.Show("rewardedVideo", options);
			}
			#endif
		}

		#if UNITY_ADS
		private void HandleShowResult(ShowResult result)
		{
			switch (result)
			{
				case ShowResult.Finished:
					var rewardCoins = gameConfig.RewardedAdCoins;
            		coinsSystem.BuyCoins(rewardCoins);
					levelScreen.OpenPopup<AlertPopup>("Popups/AlertPopup", popup =>
					{
						popup.SetText($"You earned {rewardCoins} coins!");
					});
					break;
			}
		}
		#endif
	}
}
