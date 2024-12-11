// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class manages the free-lives counter.
	/// </summary>
	public class CheckForFreeLives : MonoBehaviour
	{
		public GameConfiguration GameConfig;
		public LivesSystem LivesSystem;
		public CoinsSystem CoinsSystem;
		
        private Action<TimeSpan, int> onCountdownUpdated;
        private Action<int> onCountdownFinished;

		private bool isRunningCountdown;
		private float accTime;
        private TimeSpan timeSpan;

		private void Awake()
		{
		    if (!PlayerPrefs.HasKey("num_lives"))
			    PlayerPrefs.SetInt("num_lives", GameConfig.MaxLives);   
		
            var numLives = PlayerPrefs.GetInt("num_lives");
			var maxLives = GameConfig.MaxLives;
			var timeToNextLife = GameConfig.TimeToNextLife;
			if (numLives < maxLives && PlayerPrefs.HasKey("next_life_time"))
			{
                TimeSpan remainingTime;
                var prevNextLifeTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("next_life_time")));
				var now = DateTime.Now;
				if (prevNextLifeTime > now)
				{
					remainingTime = prevNextLifeTime - now;
                    if (numLives < maxLives)
	                    StartCountdown((int)remainingTime.TotalSeconds);
				}
				else
				{
					remainingTime = now - prevNextLifeTime;
					var livesToGive = ((int)remainingTime.TotalSeconds / timeToNextLife) + 1;
                    numLives = numLives + livesToGive;
                    if (numLives > maxLives)
                        numLives = maxLives;
                    PlayerPrefs.SetInt("num_lives", numLives);
                    if (numLives < maxLives)
                        StartCountdown(timeToNextLife - ((int)remainingTime.TotalSeconds % timeToNextLife));
				}
			}
		}
		
		private void Update()
		{
            if (!isRunningCountdown)
            {
                return;
            }

            accTime += Time.deltaTime;
            if (accTime >= 1.0f)
            {
                accTime = 0.0f;
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));
                SetTimeToNextLife((int)timeSpan.TotalSeconds);
                var numLives = PlayerPrefs.GetInt("num_lives");
	            onCountdownUpdated?.Invoke(timeSpan, numLives);
	            if ((int)timeSpan.TotalSeconds == 0)
                {
                    StopCountdown();
                    AddLife();
                }
            }
		}

		private void StartCountdown(int timeToNextLife)
        {
            SetTimeToNextLife(timeToNextLife);
            timeSpan = TimeSpan.FromSeconds(timeToNextLife);
            isRunningCountdown = true;

            if (onCountdownUpdated == null)
                return;
	        
            var numLives = PlayerPrefs.GetInt("num_lives");
            onCountdownUpdated(timeSpan, numLives);
        }

		private void StopCountdown()
        {
            isRunningCountdown = false;
            var numLives = PlayerPrefs.GetInt("num_lives");
	        onCountdownFinished?.Invoke(numLives);
        }

		private void SetTimeToNextLife(int seconds)
		{
            var nextLifeTime = DateTime.Now.Add(TimeSpan.FromSeconds(seconds));
            PlayerPrefs.SetString("next_life_time", nextLifeTime.ToBinary().ToString());
		}

        public void Subscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
        {
            onCountdownUpdated += updateCallback;
            onCountdownFinished += finishCallback;
            var maxLives = GameConfig.MaxLives;
            var numLives = PlayerPrefs.GetInt("num_lives");
            if (numLives < maxLives)
	            onCountdownUpdated?.Invoke(timeSpan, numLives);
            else
	            onCountdownFinished?.Invoke(numLives);
        }

        public void Unsubscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
        {
            if (onCountdownUpdated != null)
                onCountdownUpdated -= updateCallback;
	        
            if (onCountdownFinished != null)
                onCountdownFinished -= finishCallback;
        }
	    
        public void AddLife()
        {
            LivesSystem.AddLife(GameConfig);

            var numLives = PlayerPrefs.GetInt("num_lives");
            var maxLives = GameConfig.MaxLives;
            if (numLives < maxLives)
            {
                if (!isRunningCountdown)
                {
                    var timeToNextLife = GameConfig.TimeToNextLife;
                    StartCountdown(timeToNextLife);
                }
            }
            else
            {
                StopCountdown();
            }
        }

        public void RemoveLife()
        {
            return;
	        LivesSystem.RemoveLife(GameConfig);
	        
            var numLives = PlayerPrefs.GetInt("num_lives");
            var maxLives = GameConfig.MaxLives;
            if (numLives < maxLives && !isRunningCountdown)
            {
                var timeToNextLife = GameConfig.TimeToNextLife;
                StartCountdown(timeToNextLife);
            }
        }
		
        public void RefillLives()
        {
	        LivesSystem.Refill(GameConfig);
            var refillCost = GameConfig.ExtraBubblesCost;
            CoinsSystem.SpendCoins(refillCost);
            StopCountdown();
        }
	}
}