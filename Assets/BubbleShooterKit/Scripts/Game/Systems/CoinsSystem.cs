// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class handles the coins system in the game. It is used to buy and spend coins and other classes
    /// can subscribe to it in order to receive a notification when the number of coins changes.
    /// </summary>
	[CreateAssetMenu(fileName = "CoinsSystem", menuName = "Bubble Shooter Kit/Systems/Coins system", order = 1)]
	public class CoinsSystem : ScriptableObject
    {
        public GameConfiguration GameConfig;
        
		private Action<int> onCoinsUpdated;
	    
        public void BuyCoins(int amount)
        {
            if (!PlayerPrefs.HasKey("num_coins"))
                PlayerPrefs.SetInt("num_coins", GameConfig.InitialCoins);
            
            var numCoins = PlayerPrefs.GetInt("num_coins");
            numCoins += amount;
            PlayerPrefs.SetInt("num_coins", numCoins);
            onCoinsUpdated?.Invoke(numCoins);
        }

        public void SpendCoins(int amount)
        {
            if (!PlayerPrefs.HasKey("num_coins"))
                PlayerPrefs.SetInt("num_coins", GameConfig.InitialCoins);
            
            var numCoins = PlayerPrefs.GetInt("num_coins");
            numCoins -= amount;
            if (numCoins < 0)
                numCoins = 0;
            PlayerPrefs.SetInt("num_coins", numCoins);
            onCoinsUpdated?.Invoke(numCoins);
        }

        public void Subscribe(Action<int> callback)
        {
            onCoinsUpdated += callback;
        }

        public void Unsubscribe(Action<int> callback)
        {
            if (onCoinsUpdated != null)
                onCoinsUpdated -= callback;
        }
	}
}
