// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class contains the logic associated to the popup that is shown before
    /// starting a game.
    /// </summary>
    public class StartGamePopup : Popup
    {
        public List<Sprite> ColorBubbleSprites;
        public List<Sprite> CollectableBubbleSprites;
        public Sprite LeafSprite;
        
        [SerializeField]
        private TextMeshProUGUI levelText = null;
        
        [SerializeField]
        private TextMeshProUGUI numBubblesText = null;

        [SerializeField]
        private Sprite enabledStarSprite = null;

        [SerializeField]
        private Image star1Image = null;

        [SerializeField]
        private Image star2Image = null;

        [SerializeField]
        private Image star3Image = null;

        [SerializeField]
        private GameObject goalPrefab = null;

        [SerializeField]
        private GameObject goalGroup = null;

        [SerializeField]
        private GameObject playButton = null;

        private int numLevel;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(levelText);
            Assert.IsNotNull(numBubblesText);
            Assert.IsNotNull(enabledStarSprite);
            Assert.IsNotNull(star1Image);
            Assert.IsNotNull(star2Image);
            Assert.IsNotNull(star3Image);
            Assert.IsNotNull(goalPrefab);
            Assert.IsNotNull(goalGroup);
            Assert.IsNotNull(playButton);
        }

        public void LoadLevelData(int levelNum)
        {
            numLevel = levelNum;
            Debug.LogError("0 load level : " + levelNum);
            var level = FileUtils.LoadLevel(numLevel);
            levelText.text = "Level " + numLevel;
            numBubblesText.text = level.NumBubbles.ToString();
            var stars = PlayerPrefs.GetInt("level_stars_" + numLevel);
            if (stars == 1)
            {
                star1Image.sprite = enabledStarSprite;
            }
            else if (stars == 2)
            {
                star1Image.sprite = enabledStarSprite;
                star2Image.sprite = enabledStarSprite;
            }
            else if (stars == 3)
            {
                star1Image.sprite = enabledStarSprite;
                star2Image.sprite = enabledStarSprite;
                star3Image.sprite = enabledStarSprite;
            }

            var randomColors = new List<ColorBubbleType>();
            randomColors.AddRange(level.AvailableColors);
            randomColors.Shuffle();

            PlayerPrefs.SetInt("num_available_colors", randomColors.Count);
            for (var i = 0; i < randomColors.Count; i++)
            {
                var color = randomColors[i];
                PlayerPrefs.SetInt($"available_colors_{i}", (int)color);
            }
            foreach (var goal in level.Goals)
            {
                var goalItem = Instantiate(goalPrefab);
                goalItem.transform.SetParent(goalGroup.transform, false);
                if (goal is CollectBubblesGoal)
                {
                    var concreteGoal = (CollectBubblesGoal)goal;
                    goalItem.GetComponent<GoalItem>().Initialize(ColorBubbleSprites[(int)concreteGoal.Type], concreteGoal.Amount);
                }
                else if (goal is CollectRandomBubblesGoal)
                {
                    var concreteGoal = (CollectRandomBubblesGoal)goal;
                    goalItem.GetComponent<GoalItem>().Initialize(ColorBubbleSprites[(int)randomColors[(int)concreteGoal.Type]], concreteGoal.Amount);
                }
                else if (goal is CollectCollectablesGoal)
                {
                    var concreteGoal = (CollectCollectablesGoal)goal;
                    goalItem.GetComponent<GoalItem>().Initialize(CollectableBubbleSprites[(int)concreteGoal.Type], concreteGoal.Amount);
                }
                else if (goal is CollectLeavesGoal)
                {
                    var concreteGoal = (CollectLeavesGoal)goal;
                    goalItem.GetComponent<GoalItem>().Initialize(LeafSprite, concreteGoal.Amount);
                }
            }
        }

        public void OnPlayButtonPressed()
        {
            PlayerPrefs.SetInt("last_selected_level", numLevel);
            GetComponent<ScreenTransition>().PerformTransition();
        }
    }
}
