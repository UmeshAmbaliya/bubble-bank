// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class manages the high-level logic of the game screen.
    /// </summary>
    public class GameScreen : BaseScreen
    {
        public int LevelNum = 1;

        public GameConfiguration GameConfig;
        public GameLogic GameLogic;
        public GameScroll GameScroll;
        public Shooter Shooter;

        public CheckForFreeLives FreeLivesChecker;

        public BubbleFactory BubbleFactory;
        public BubblePool BubblePool;
        public FxPool FxPool;
        public ObjectPool ScoreTextPool;
        public GameObject TopLinePrefab;

        public GameUi GameUi;
        public LevelGoalsWidget LevelGoalsWidget;

        public PlayerBubbles PlayerBubbles;

        public Image BackgroundImage;

        public GameObject TopCanvas;

        public GameObject LevelCompletedAnimationPrefab;
        private GameObject levelCompletedAnimation;

        [SerializeField]
        private InGameBoostersWidget inGameBoostersWidget = null;

        [SerializeField]
        private Fox fox = null;

        [HideInInspector]
        public bool IsInputLocked;

        private Vector3 bubblePos;

        private float tileWidth;
        private float tileHeight;

        private float totalWidth;
        private float totalHeight;

        private Level level;
        private LevelInfo levelInfo;

        private List<List<Vector2>> tilePositions = new List<List<Vector2>>();

        private GameObject topLine;
        private readonly List<GameObject> leaves = new List<GameObject>();
        int visibleRowCount = 0;
        public Transform topPointTransform;

        public GameObject settingPanel;
        private void Awake()
        {
            Assert.IsNotNull(TopLinePrefab);
        }

        protected override void Start()
        {
            base.Start();

            var tempBubble = BubblePool.GetColorBubblePool(ColorBubbleType.Black).GetObject();
            tileWidth = tempBubble.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
            tileHeight = tempBubble.GetComponentInChildren<SpriteRenderer>().bounds.size.y;
            tempBubble.GetComponent<PooledObject>().Pool.ReturnObject(tempBubble);

            InitializeObjectPools();
            StartCoroutine(InitializeLevel());

            //BackgroundImage.sprite = levelInfo.BackgroundSprite;

            inGameBoostersWidget.Initialize(GameConfig, levelInfo);

            //OpenPopup<LevelGoalsPopup>("Popups/LevelGoalsPopup", popup =>
            //{
            //    popup.SetGoals(levelInfo);
            //});
            GameLogic.StartGame();
        }

        public void OnGameRestarted()
        {
            foreach (var cover in FindObjectsOfType<IceCover>())
                cover.GetComponent<PooledObject>().Pool.ReturnObject(cover.gameObject);

            foreach (var cover in FindObjectsOfType<CloudCover>())
                cover.GetComponent<PooledObject>().Pool.ReturnObject(cover.gameObject);

            ResetObjectPools();

            Destroy(topLine);

            GameLogic.Reset();
            GameScroll.Reset();

            leaves.Clear();
            tilePositions.Clear();

            LevelGoalsWidget.Reset();

            BubbleFactory.Reset();
        }

        private void InitializeObjectPools()
        {
            foreach (var pool in BubblePool.GetComponentsInChildren<ObjectPool>())
                pool.Initialize();

            foreach (var pool in FxPool.GetComponentsInChildren<ObjectPool>())
                pool.Initialize();

            ScoreTextPool.Initialize();
        }

        private void ResetObjectPools()
        {
            foreach (var pool in BubblePool.GetComponentsInChildren<ObjectPool>())
                pool.Reset();

            foreach (var pool in FxPool.GetComponentsInChildren<ObjectPool>())
                pool.Reset();

            ScoreTextPool.Reset();
        }

        public IEnumerator InitializeLevel()
        {
            var lastSelectedLevel = PlayerPrefs.GetInt("last_selected_level");
            if (lastSelectedLevel == 0)
                lastSelectedLevel = LevelNum;
            Debug.Log("1 load level : " + lastSelectedLevel);
            LoadLevel(lastSelectedLevel);
            BubbleFactory.PreLevelInitialize(levelInfo);
            StartCoroutine( CreateLevel());
            yield return new WaitForSeconds(0.03f);
            BubbleFactory.PostLevelInitialize(level);
            Shooter.Initialize(tileHeight);

            GameUi.ScoreWidget.Fill(levelInfo.Star1Score, levelInfo.Star2Score, levelInfo.Star3Score);

            LevelGoalsWidget.Initialize(levelInfo.Goals, BubbleFactory.RandomizedColorBubblePrefabs);

            PlayerBubbles.Initialize(levelInfo);

            GameLogic.SetGameInfo(level, levelInfo, tileWidth, tileHeight, totalWidth, totalHeight, tilePositions, leaves);
            GameScroll.SetGameInfo(level, tileHeight, tilePositions, topLine, leaves);
        }

        private void LoadLevel(int levelNum)
        {
            levelInfo = FileUtils.LoadLevel(levelNum);
            level = new Level(levelInfo.Rows, levelInfo.Columns);
        }
        int totalHiddenRows = 0;
        IEnumerator CreateLevel()
        {
            const float tileWidthMultiplier = GameplayConstants.TileWidthMultiplier;
            const float tileHeightMultiplier = GameplayConstants.TileHeightMultiplier;

            tilePositions = new List<List<Vector2>>();
            var evenWidth = level.Columns;
            var oddWidth = level.Columns - 1;
            for (var i = 0; i < level.Rows; i++)
            {
                if (i % 2 == 0)
                {
                    var row = new List<Vector2>(evenWidth);
                    row.AddRange(Enumerable.Repeat(new Vector2(), evenWidth));
                    tilePositions.Add(row);
                }
                else
                {
                    var row = new List<Vector2>(oddWidth);
                    row.AddRange(Enumerable.Repeat(new Vector2(), oddWidth));
                    tilePositions.Add(row);
                }
            }
            List<float> allYPoses = new List<float>();
            int startIndex = 4 - level.Rows;
            for (int i = 0; i < level.Rows; i++)
            {
                float yPos = topPointTransform.transform.position.y + (0.54f * startIndex) - 0.17f;
                startIndex -= 1;
                allYPoses.Add(yPos);
            }
            for (var j = 0; j < level.Rows; j++)
            {
                var selectedRow = level.Tiles[j];
                for (var i = 0; i < selectedRow.Count; i++)
                {
                    float rowOffset;
                    if (j % 2 == 0)
                        rowOffset = 0;
                    else
                        rowOffset = tileWidth * 0.5f;

                    tilePositions[j][i] = new Vector2(
                        (i * tileWidth * tileWidthMultiplier) + rowOffset,
                        -j * tileHeight * tileHeightMultiplier);
                        //allYPoses[j]);
                }
            }

            totalWidth = level.Columns * tileWidth * tileWidthMultiplier;
            totalHeight = level.Rows * tileHeight * tileHeightMultiplier - 0.01f;

            Camera.main.orthographicSize = (totalWidth * 1.02f) * (Screen.height / (float)Screen.width) * 0.5f;
            var bottomPivot = new Vector2(0, Camera.main.pixelHeight * GameplayConstants.BottomPivotHeight);
            var bottomPivotPos = Camera.main.ScreenToWorldPoint(bottomPivot);
            foreach (var row in tilePositions)
            {
                for (var i = 0; i < row.Count; i++)
                {
                    var tile = row[i];
                    var newPos = tile;
                    newPos.x -= totalWidth / 2f;
                    newPos.x += (tileWidth * tileWidthMultiplier) / 2f;
                    newPos.y += bottomPivotPos.y + totalHeight;// + LevelUtils.currentLevelOffsetY;
                    row[i] = newPos;
                }
            }
            float currentLevelOffsetY = 0;
            yield return new WaitForSeconds(0.03f);
            //topPointTransform.transform.localPosition = new Vector3(0,-88.5f,0);
            //topPointTransform.parent = null;
            Debug.Log("TopPOint " + topPointTransform.transform.position);
            if (level.Rows>7)
            {
                currentLevelOffsetY = topPointTransform.transform.position.y - tilePositions[level.Rows - 7][0].y - 0.27f;
                Debug.Log(currentLevelOffsetY + "  "+ topPointTransform.transform.position.y +"   "+ tilePositions[level.Rows - 7][0].y);
                //tilePositions[level.Rows - 7][0] = new Vector2(0, -5);  
            }
            else
            {
                currentLevelOffsetY = tilePositions[level.Rows][0].y - topPointTransform.transform.position.y + 0.27f;
            }
            LevelUtils.currentLevelOffsetY = currentLevelOffsetY;

            for (var j = 0; j < level.Rows; j++)
            {
                var selectedRow = level.Tiles[j];
                //Debug.LogError("selectedRow count : " + selectedRow.Count +  "  Tiles : " + levelInfo.Tiles[j].Tiles.Count);
                for (var i = 0; i < selectedRow.Count; i++)
                {
                    var tileInfo = levelInfo.Tiles[j].Tiles[i];
                    var tile = BubbleFactory.CreateBubble(tileInfo);
                    if (tile != null)
                    {
                        tile.transform.position = tilePositions[j][i] +new Vector2(0, currentLevelOffsetY);
                        level.Tiles[j][i] = tile.GetComponent<Bubble>();
                        tile.GetComponent<Bubble>().Row = j;
                        tile.GetComponent<Bubble>().Column = i;
                    }
                }
            }
            currentVisibleRows = 6;
            totalHiddenRows = level.Rows - currentVisibleRows +1;
            //Debug.LogError("Current Line set here");
            SetVisibleLines();
            DrawTopLine();
            DrawTopLeaves();
        }
         
        private void DrawTopLine()
        {
            topLine = Instantiate(TopLinePrefab);
            var topRowHeight = GetTopRowHeight();
            var newPos = topLine.transform.position;
            newPos.y = topRowHeight + (tileHeight * 0.6f);
            topLine.transform.position = newPos;
        }

        private void DrawTopLeaves()
        {
            if (levelInfo.Goals.Find(x => x is CollectLeavesGoal) != null)
            {
                var topRowHeight = GetTopRowHeight();
                for (var i = 0; i < level.Columns; i++)
                {
                    if (level.Tiles[0][i] != null)
                    {
                        var leaf = BubblePool.LeafPool.GetObject();
                        leaf.GetComponent<Leaf>().FxPool = FxPool;
                        leaf.transform.position = new Vector2(tilePositions[0][i].x, topRowHeight + tileHeight);
                        leaves.Add(leaf);
                    }
                    else
                    {
                        leaves.Add(null);
                    }
                }
            }
        }

        private float GetTopRowHeight()
        {
            var topRow = level.Tiles[0];
            var topRowHeight = 0f;
            foreach (var tile in topRow)
            {
                if (tile != null)
                {
                    topRowHeight = tile.transform.position.y;
                    break;
                }
            }

            return topRowHeight;
        }

        public void LockInput()
        {
            IsInputLocked = true;
        }

        public void UnlockInput()
        {
            if (!GameLogic.IsChainingBoosters && !GameLogic.IsChainingVoids)
                IsInputLocked = false;
        }

        public void MoveNeighbours(Bubble shotColorBubble, int row, float strength)
        {
            if (row < 0 || row >= level.Rows)
                return;

            foreach (var bubble in level.Tiles[row])
            {
                if (bubble != null)
                {
                    if (Math.Abs(bubble.Column - shotColorBubble.Column) <= 1)
                    {
                        var offsetDir = bubble.transform.position - shotColorBubble.transform.position;
                        offsetDir.Normalize();
                        ShakeBubble(bubble, offsetDir, strength);
                    }
                }
            }
        }

        public Sequence ShakeBubble(Bubble bubble, Vector3 offsetDir, float strength)
        {
            var seq = DOTween.Sequence();
            var child = bubble.transform.GetChild(0);
            seq.Append(child.transform.DOBlendableMoveBy(offsetDir * strength, 0.15f)
                .SetEase(Ease.Linear));
            seq.Append(child.transform.DOBlendableMoveBy(-offsetDir * strength, 0.2f).SetEase(Ease.Linear));
            seq.Play();

            var colorBubble = bubble.GetComponent<ColorBubble>();
            if (colorBubble != null && colorBubble.CoverType != CoverType.None)
            {
                seq = DOTween.Sequence();
                var cover = bubble.transform.GetChild(1);
                seq.Append(cover.transform.DOBlendableMoveBy(offsetDir * strength, 0.15f)
                    .SetEase(Ease.Linear));
                seq.Append(cover.transform.DOBlendableMoveBy(-offsetDir * strength, 0.2f).SetEase(Ease.Linear));
                seq.Play();
            }

            return seq;
        }

        public bool CanPlayerShoot()
        {
            return PlayerBubbles.NumBubblesLeft >= 1 &&
                   !IsInputLocked &&
                   GameLogic.GameStarted &&
                   CurrentPopups.Count == 0;
        }

        public IEnumerator OpenWinPopupAsync()
        {
            Debug.LogError("End Game Call 2");
            GameLogic.GameEndAPICall();
            fox.PlayHappyAnimation();
            yield return new WaitForSeconds(GameplayConstants.WinPopupDelay);
            //GameEnd APICALL here
            OpenWinPopup();
        }

        public IEnumerator OpenLosePopupAsync()
        {
            Debug.LogError("End Game Call 3");
            GameLogic.GameEndAPICall();
            fox.PlaySadAnimation();
            yield return new WaitForSeconds(GameplayConstants.LosePopupDelay);
            //GameEnd APICALL here
            OpenLosePopup();
        }

        public void OpenWinPopup()
        {
            OpenPopup<GameOverPanelScript>("Popups/WelldonePopup", popup =>
            {
                if (DataHandler.inst.TutorialLevelInt == 1)
                {
                    DataHandler.inst.TutorialLevelInt = 0;
                    DataHandler.inst.isTutorialLevelComplete = true;
                }
                //var gameState = GameLogic.GameState;
                //var levelStars = PlayerPrefs.GetInt("level_stars_" + levelInfo.Number);
                //if (gameState.Score >= levelInfo.Star3Score)
                //{
                //    popup.SetStars(3);
                //    PlayerPrefs.SetInt("level_stars_" + levelInfo.Number, 3);
                //}
                //else if (gameState.Score >= levelInfo.Star2Score)
                //{
                //    popup.SetStars(2);
                //    if (levelStars < 3)
                //    {
                //        PlayerPrefs.SetInt("level_stars_" + levelInfo.Number, 2);
                //    }
                //}
                //else if (gameState.Score >= levelInfo.Star3Score)
                //{
                //    popup.SetStars(1);
                //    if (levelStars < 2)
                //    {
                //        PlayerPrefs.SetInt("level_stars_" + levelInfo.Number, 1);
                //    }
                //}
                //else
                //{
                //    popup.SetStars(0);
                //}
                //var levelScore = PlayerPrefs.GetInt("level_score_" + levelInfo.Number);
                //if (levelScore < gameState.Score)
                //{
                //    PlayerPrefs.SetInt("level_score_" + levelInfo.Number, gameState.Score);
                //}
                popup.SetInfo(GameLogic);
                //popup.SetGoals(levelInfo.Goals, gameState, LevelGoalsWidget);
            });
        }

        public void OpenLosePopup()
        {
            FreeLivesChecker.RemoveLife();
            OpenPopup<LosePopup>("Popups/LosePopup", popup =>
            {
                if (DataHandler.inst.TutorialLevelInt == 1)
                {
                    DataHandler.inst.TutorialLevelInt = 0;
                    DataHandler.inst.isTutorialLevelComplete = true;
                }
                var gameState = GameLogic.GameState;
                popup.SetScore(gameState.Score);
                popup.SetGoals(levelInfo.Goals, gameState, LevelGoalsWidget);
            });
        }

        public IEnumerator OpenOutOfBubblesPopupAsync()
        {
            Debug.LogError("End Game Call 4");
            GameLogic.GameEndAPICall();
            yield return new WaitForSeconds(GameplayConstants.OutOfBubblesPopupDelay);
            //GameEnd APICALL here
            OpenOutOfBubblesPopup();
        }

        private void OpenOutOfBubblesPopup()
        {
            OpenPopup<OutOfBubblesPopup>("Popups/OutOfBubblesPopup", popup =>
            {
                popup.SetInfo(this);
                OpenTopCanvas();
            });
        }

        public void OpenTimeUpPopUp()
        {
            OpenPopup<GameOverPanelScript>("Popups/TimeUpsPopUp", popup =>
            {
                if (DataHandler.inst.TutorialLevelInt == 1)
                {
                    DataHandler.inst.TutorialLevelInt = 0;
                    DataHandler.inst.isTutorialLevelComplete = true;
                }
                popup.SetInfo(GameLogic,false);
            });
        }
          
        public void OpenCoinsPopup()
        {
            OpenPopup<BuyCoinsPopup>("Popups/BuyCoinsPopup");
        }

        public void OpenLevelCompletedAnimation()
        {
            SoundPlayer.PlaySoundFx("LevelComplete");
            levelCompletedAnimation = Instantiate(LevelCompletedAnimationPrefab);
            levelCompletedAnimation.transform.SetParent(Canvas.transform, false);
        }

        public void CloseLevelCompletedAnimation()
        {
            if (levelCompletedAnimation != null)
                Destroy(levelCompletedAnimation);
        }

        public void OpenTopCanvas()
        {
            TopCanvas.SetActive(true);
        }

        public void CloseTopCanvas()
        {
            TopCanvas.SetActive(false);
        }

        public void OnPauseButtonPressed()
        {
            if (!PlayerBubbles.IsPlayingEndGameSequence() && GameLogic.GameStarted)
            {
                //GameLogic.GameLost = true;
                //GameLogic.GameEndAPICall(true);
                //StartCoroutine(ChangeSceneToHome());

                LockInput();
                exitGamePopup.SetActive(true);
                //OpenPopup<PausePopup>("Popups/PausePopup");
            }
        }

        public GameObject exitGamePopup;
        public void OnPressExitPopupYesButton()
        {
            GameLogic.GameLost = true;
            Debug.LogError("End Game Call 1");
            GameLogic.GameEndAPICall(true);
            StartCoroutine(ChangeSceneToHome());
        }

        public void OnPressExitPopupNoButton()
        {
            exitGamePopup.SetActive(false);
            UnlockInput();
        }

        IEnumerator ChangeSceneToHome()
        {
            yield return new WaitUntil(() => APIHandler.Instance.transform.GetChild(0).GetChild(0).gameObject.activeSelf == false);
            DataHandler.inst.isComeAfterWin = true;
            SceneManager.LoadScene("Home");
        }

        public void OnSettingButtonPressed()
        {
            if (!PlayerBubbles.IsPlayingEndGameSequence() && GameLogic.GameStarted)
            {
                LockInput();
                //OpenPopup<SettingsPopup>("Popups/SettingsPopup");
                settingPanel.SetActive(true);
            }
        }

        public void PenalizePlayer()
        {
            FreeLivesChecker.RemoveLife();
        }

        public void OnGameContinued()
        {
            CloseTopCanvas();
            Debug.LogError("UnlockInput 1");
            UnlockInput();
        }

        public void ApplyBooster(PurchasableBoosterBubbleType boosterBubbleType)
        {
            switch (boosterBubbleType)
            {
                case PurchasableBoosterBubbleType.SuperAim:
                    Shooter.ApplySuperAim();
                    break;

                case PurchasableBoosterBubbleType.RainbowBubble:
                case PurchasableBoosterBubbleType.HorizontalBomb:
                case PurchasableBoosterBubbleType.CircleBomb:
                    PlayerBubbles.CreatePurchasableBoosterBubble(boosterBubbleType);
                    break;
            }
        }
        int currentVisibleRows;
        public void SetVisibleLines(int AddLine = 0)
        {
            totalHiddenRows -= AddLine;
            currentVisibleRows += AddLine;
            //Debug.Log("Current Line set here "+currentVisibleRows);
            int totalRemainingBubblesInView = 0;
            for (int i = totalHiddenRows - 2; i < level.Tiles.Count; i++)
            {
                for (int j = 0; j < level.Tiles[i].Count; j++)
                {
                    if (level.Tiles[i][j] != null)
                    {
                        if (level.Tiles[i][j] is ColorBubble && ((ColorBubble)level.Tiles[i][j]).Type == ColorBubbleType.None)
                        {
                            level.Tiles[i][j].gameObject.SetActive(false);
                            level.Tiles[i][j] = null;
                        }
                        else
                        {
                            totalRemainingBubblesInView++;
                            level.Tiles[i][j].SetInView(true);
                        }
                    }
                }
            }
            //Debug.Log("Total bubbles in view : " + totalRemainingBubblesInView);
            if (totalRemainingBubblesInView == 0)
            {
                GameLogic.GameWinByClearBoard();
            }
            if (DataHandler.inst.isBubbleCollidedWithBottomLine)
            {
                GameLogic.GameLostByBubbleTouchBottomLine();
                Debug.LogError("Game over called here");
            }
          
        }

        public bool CanScrollDown()
        {
            //Debug.LogError(currentVisibleRows + " " + level.Tiles.Count +"  "+level.Rows + totalHiddenRows);
            bool canScroll = totalHiddenRows > 2;
            return canScroll;
        }
    }
}
