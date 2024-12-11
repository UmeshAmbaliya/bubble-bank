// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	public class GameUi : MonoBehaviour
	{
        [SerializeField]
        private GameScroll gameScroll = null;
        [SerializeField] 
        private GameLogic gamelogic = null; 
        public ScoreWidget ScoreWidget; 
		public float currentGameTime;
		public int currentLifeCount;
        public Image[] lifeImages;
        public Sprite lifeEnableSprite;
        public Sprite lifeDisableSprite;

        private void Start()
        {
            currentGameTime = 180;
            currentLifeCount = 3;
            LifeUIUpdate();
            UpdateGameScore();
        }
        public TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI gameScoreText;
        public void UpdateScore(int score)
		{
			ScoreWidget.UpdateProgressBar(score);
		}

        private void Update()
        {
            if (gamelogic.GameWon == false && gamelogic.GameLost == false)
            { 
                currentGameTime -= Time.deltaTime;
                if (currentGameTime <= 0)
                {
                    currentGameTime = 0;
                    var ts = TimeSpan.FromSeconds(currentGameTime);
                    timerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                    gamelogic.GameLostByTimerEnd();
                }
                else
                {
                    var ts = TimeSpan.FromSeconds(currentGameTime);
                    timerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                }
            }
        }

        public void UpdateGameScore()
        {
            gameScoreText.text = gamelogic.GameState.Score.ToString();
        }

        public void LifeDeduct()
        {
            currentLifeCount -= 1;
            if (currentLifeCount < 0)
            {
                Debug.LogError("life is 0.. Refill life and get scroller one step down");
                FullLifeAndGetScrollOneStepDown();
            }
            else
            {
                LifeUIUpdate();
            }
        }

        public void LifeUIUpdate()
        {
            for (int i = 0; i < lifeImages.Length; i++)
            {
                if (i < currentLifeCount)
                {
                    lifeImages[i].sprite = lifeEnableSprite;
                }
                else
                {
                    lifeImages[i].sprite = lifeDisableSprite;
                }
            }
        }

        public void FullLifeAndGetScrollOneStepDown()
        {
            currentLifeCount = 3;
            LifeUIUpdate();
            gameScroll.ScrollLevel(-1);
        }
    }
}
