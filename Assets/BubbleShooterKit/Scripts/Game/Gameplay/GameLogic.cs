// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class handles the core gameplay logic of the game.
    /// </summary>
    public class GameLogic : MonoBehaviour
    {
        public API_SendData.CalculateScoreSendData scoreSendData = new API_SendData.CalculateScoreSendData();
        [SerializeField]
        private GameConfiguration gameConfig = null;

        [SerializeField]
        private GameScreen gameScreen = null;

        [SerializeField]
        private GameScroll gameScroll = null;

        [SerializeField]
        private PlayerBubbles playerBubbles = null;

        [SerializeField]
        private BubbleFactory bubbleFactory = null;

        [SerializeField]
        private GameUi gameUi = null;

        [SerializeField]
        private FxPool fxPool = null;
        [SerializeField]
        private ObjectPool scoreTextPool = null;

        public GameState GameState { get; } = new GameState();

        public bool GameStarted { get; private set; }
        public bool GameWon { get; private set; }
        public bool GameLost { get; set; }

        private readonly List<Bubble> newBubbles = new List<Bubble>();

        public bool IsChainingBoosters { get; private set; }
        public bool IsChainingVoids { get; private set; }

        private bool shouldChainVoids;
        private int voidCounter;

        private readonly List<Bubble> currentExplodingBubbles = new List<Bubble>();

        private Level level;
        private LevelInfo levelInfo;
        private float tileWidth;
        private float tileHeight;
        private float totalWidth;
        private float totalHeight;
        private List<List<Vector2>> tilePositions;
        private List<GameObject> leaves;

        private Bubble lastShotBubble;
        private Bubble lastTouchedBubble;

        private bool didBubbleCollideWithTop;
        public int currentGameId = 0;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            //get current game id : 
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.gameCheckAPI, "{}", true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.gameCheckAPI + "  response : " + data);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.CheckGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.CheckGameReceiveData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            currentGameId = _data.user_game_id;
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification("game_check : " + _data.error_message);
                        }
                    }
                }
            }, false);
        }

        public void SetGameInfo(Level lvl, LevelInfo lvlInfo, float tileW, float tileH, float totalW, float totalH, List<List<Vector2>> positions, List<GameObject> levelLeaves)
        {
            level = lvl;
            levelInfo = lvlInfo;
            tileWidth = tileW;
            tileHeight = tileH;
            totalWidth = totalW;
            totalHeight = totalH;
            tilePositions = positions;
            leaves = levelLeaves;
        }

        public void Reset()
        {
            GameState.Reset();
            GameStarted = false;
            GameWon = false;
            GameLost = false;
        }



        public void HandleMatches(Bubble shotColorBubble, Bubble touchedBubble)
        {
            lastShotBubble = shotColorBubble;
            lastTouchedBubble = touchedBubble;
            shotColorBubble.SetInView(true);
            HandleMatches(shotColorBubble, touchedBubble.Row, touchedBubble.Column);
        }

        public void HandleMatches(Bubble shotColorBubble, int touchedRow, int touchedColumn)
        {
            var layoutInfo = new ScreenLayoutInfo
            {
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                TotalWidth = totalWidth,
                TotalHeight = totalHeight
            };

            var emptyNeighboursInfo = LevelUtils.GetEmptyNeighbours(level, touchedRow, touchedColumn, layoutInfo);

            var minDistance = float.MaxValue;
            var minIndex = -1;
            for (var i = 0; i < emptyNeighboursInfo.Count; i++)
            {
                var pos = emptyNeighboursInfo[i].Position;
                var distance = Vector2.Distance(shotColorBubble.transform.position, pos);
                if (distance < minDistance && emptyNeighboursInfo[i].Row >= 0)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }

            if (minIndex == -1)
                return;

            var tileInfo = emptyNeighboursInfo[minIndex];
            var seq = DOTween.Sequence();
            seq.Append(shotColorBubble.transform.DOMove(tileInfo.Position, 0.05f));
            seq.AppendCallback(() => RunPostShootingLogic(shotColorBubble));
            if (tileInfo.Row >= level.Rows)
            {
                level.AddBottomRow();
            }

            level.Tiles[tileInfo.Row][tileInfo.Column] = shotColorBubble;
            shotColorBubble.Row = tileInfo.Row;
            shotColorBubble.Column = tileInfo.Column;

            const float strength = GameplayConstants.BubbleHitStrength;
            gameScreen.MoveNeighbours(shotColorBubble, shotColorBubble.Row - 2, strength * 0.5f);
            gameScreen.MoveNeighbours(shotColorBubble, shotColorBubble.Row - 1, strength);
            gameScreen.MoveNeighbours(shotColorBubble, shotColorBubble.Row + 1, strength);
            gameScreen.MoveNeighbours(shotColorBubble, shotColorBubble.Row + 2, strength * 0.5f);
            gameScreen.ShakeBubble(shotColorBubble, playerBubbles.LastShotDir, strength).AppendCallback(() =>
            {
                //Debug.Log("UnlockInput 4");
                gameScreen.UnlockInput();
                //gameScroll.PerformScroll();
                //Debug.LogError("ShakeBubble");
            });
        }

        private void RunPostShootingLogic(Bubble shotColorBubble)
        {
            SoundPlayer.PlaySoundFx("Bubble");
            //Debug.Log("Set here 1");

            if (!ResolveTouchedBoosterBubbles(shotColorBubble))
            {
                ResolveTouchedClouds(shotColorBubble);

                ColorBubble shootBubble = shotColorBubble.GetComponent<ColorBubble>();

                if (shootBubble != null)
                {
                    scoreSendData.color_ball = (int)shootBubble.Type;

                    var matches = LevelUtils.GetMatches(level, shootBubble);
                    //Debug.LogError(matches.Count);

                    if (matches.Count >= GameplayConstants.NumBubblesNeededForMatch)
                    {
                        playerBubbles.isFailBubble = false;
                        scoreSendData.bubbles_same_popped_count += matches.Count;
                        Debug.Log("Called destroy tiles from here 3  " + (matches.OfType<Bubble>().ToList().Count));
                        DestroyTiles(matches.OfType<Bubble>().ToList());
                    }
                    else
                    {
                        //scoreSendData.bubbles_same_popped_count = 0;
                        //scoreSendData.bubbles_diff_popped_count = 0;
                        gameUi.LifeDeduct();
                        CheckGoals();
                        playerBubbles.isFailBubble = true;
                       // no bubble popped.. life deduct
                    }
                }
            }

            if (shotColorBubble.GetComponent<SpecialBubble>() != null)
            {
                var bubblesToExplode = LevelUtils.GetNeighboursInRadius(level, shotColorBubble, 2);
                Debug.Log("Called destroy tiles from here 4 : " + bubblesToExplode.Count);
                DestroyTiles(bubblesToExplode, false, false);

                var fx = fxPool.EnergyBubblePool.GetObject();
                if (didBubbleCollideWithTop)
                    fx.transform.position = shotColorBubble.transform.position;
                else
                    fx.transform.position = lastTouchedBubble.transform.position;
                playerBubbles.OnSpecialBubbleShot();
            }
            else if (shotColorBubble.GetComponent<PurchasableBoosterBubble>() != null)
            {
                Debug.Log("RunPostShootingLogic 2");
                var purchasableBooster = shotColorBubble.GetComponent<PurchasableBoosterBubble>();

                if (didBubbleCollideWithTop)
                {
                    foreach (var tile in level.Tiles[0])
                        if (tile != null)
                            lastTouchedBubble = tile;
                }

                var bubblesToExplode = purchasableBooster.Resolve(level, shotColorBubble, lastTouchedBubble);
                Debug.Log("Called destroy tiles from here 5 : " + bubblesToExplode.Count);
                DestroyTiles(bubblesToExplode, false, false);
                DestroyBubble(shotColorBubble);

                GameObject fx;
                if (shotColorBubble.GetComponent<HorizontalBombBoosterBubble>() != null)
                    fx = fxPool.GetBoosterBubbleParticlePool(BoosterBubbleType.HorizontalBomb).GetObject();
                else
                    fx = fxPool.EnergyBubblePool.GetObject();

                if (didBubbleCollideWithTop)
                    fx.transform.position = shotColorBubble.transform.position;
                else
                    fx.transform.position = lastTouchedBubble.transform.position;

                playerBubbles.OnSpecialBubbleShot();

                didBubbleCollideWithTop = false;
            }
            //StartCoroutine(GetRemainingBubblesCo());
        }
        //IEnumerator GetRemainingBubblesCo()
        //{
        //	yield return new WaitForSeconds(2);

        //          GetHiddenBubbles();
        //      }
        private bool ResolveTouchedBoosterBubbles(Bubble shotBubble)
        {
            var resolvedBooster = false;
            var neighbours = LevelUtils.GetNeighbours(level, shotBubble);
            foreach (var bubble in neighbours)
            {
                var boosterBubble = bubble.GetComponent<BoosterBubble>();
                if (boosterBubble != null)
                {
                    var bubblesToExplode = boosterBubble.Resolve(level, shotBubble);
                    Debug.Log("Called destroy tiles from here 6 : " + bubblesToExplode.Count);
                    DestroyTiles(bubblesToExplode, false, false);
                    resolvedBooster = true;
                }
            }

            if (resolvedBooster)
                DestroyBubble(shotBubble);

            return resolvedBooster;
        }

        private void ResolveTouchedClouds(Bubble shotBubble)
        {
            var neighbours = LevelUtils.GetNeighbours(level, shotBubble);
            foreach (var bubble in neighbours.OfType<ColorBubble>())
            {
                if (bubble.CoverType == CoverType.Cloud)
                {
                    RemoveCover(bubble.gameObject);
                }
            }
        }

        private void DestroyTiles(List<Bubble> tiles, bool fall = false, bool transformVoids = true)
        {
            //Debug.Log("DestroyTiles : " + tiles.Count);
            playerBubbles.FillEnergyOrb();
            currentExplodingBubbles.Clear();
            currentExplodingBubbles.AddRange(tiles);

            var bubblesToExplode = new List<Bubble>();
            foreach (var bubble in tiles)
            {
                if (fall)
                    DestroyBubbleFalling(bubble);
                else
                    bubblesToExplode.Add(bubble);
            }

            var simulatedBubblesToExplode = SimulatedRingExplodeBubbles(bubblesToExplode);
            RingExplodeBubbles(simulatedBubblesToExplode, transformVoids);
            //UpdateAvailableColors();

            if (fall)
            {
                CheckGoals();
                //gameScroll.PerformScroll();
            }
        }

        private void UpdateAvailableColors()
        {
            bubbleFactory.PostLevelInitialize(level);
        }

        private void RingExplodeBubbles(List<Bubble> bubbles, bool transformVoids = true)
        {
            var i = 0;
            foreach (var bubble in bubbles)
            {
                var seq = DOTween.Sequence();

                seq.AppendInterval(0.1f * i);
                if (bubble.CanBeDestroyed())
                {
                    var colorBubble = bubble.GetComponent<ColorBubble>();
                    if (colorBubble != null && colorBubble.CoverType != CoverType.Ice)
                    {
                        seq.AppendCallback(() =>
                        {
                            bubble.Explode();
                            var animator = bubble.transform.GetChild(0).GetComponent<Animator>();
                            if (animator != null && animator.gameObject.activeInHierarchy)
                                animator.SetTrigger("Explode");
                        });
                    }

                    seq.AppendInterval(0.03f);
                    seq.AppendCallback(() => { DestroyBubble(bubble, transformVoids); });
                }

                if (i == bubbles.Count - 1)
                {
                    seq.AppendCallback(RemoveFloatingBubbles);
                    seq.AppendCallback(() => {/* gameScroll.PerformScroll(); */Debug.Log("UnlockInput 3"); gameScreen.UnlockInput(); });
                    seq.AppendCallback(() => IsChainingBoosters = false);
                    seq.AppendCallback(() =>
                    {
                        if (shouldChainVoids)
                        {
                            shouldChainVoids = false;
                            IsChainingVoids = true;
                            gameScreen.LockInput();
                            ++voidCounter;
                            StartCoroutine(ChainVoids());
                        } 
                    });
                }
                ++i;
            } 
        }
 

        private List<Bubble> SimulatedRingExplodeBubbles(List<Bubble> bubbles)
        {
            var explodedBubbles = new List<Bubble>();
            var i = 0;
            while (bubbles.Count > 0)
            {
                var ring = LevelUtils.GetRing(level, playerBubbles.LastShotBubble, i);
                foreach (var bubble in ring)
                {
                    if (bubbles.Contains(bubble))
                    {
                        bubbles.Remove(bubble);
                        explodedBubbles.Add(bubble);
                    }
                }
                ++i;

                if (i >= 20)
                {
                    Debug.Log("This should never happen. Aborting loop.");
                    Debug.Log(bubbles.Count);
                    foreach (var bubble in bubbles)
                        Debug.Log(bubble);
                    break;
                }
            }

            return explodedBubbles;
        }

        private IEnumerator ChainVoids()
        {
            yield return new WaitForSeconds(GameplayConstants.VoidChainSpeed);

            var processedBubbles = new List<ColorBubble>();
            foreach (var bubble in newBubbles)
            {
                var matches = LevelUtils.GetMatches(level, bubble.GetComponent<ColorBubble>());
                var newMatches = new List<ColorBubble>(matches.Count);
                foreach (var match in matches)
                    if (!processedBubbles.Contains(match) && match.isInView)
                        newMatches.Add(match);

                processedBubbles.AddRange(matches);

                if (newMatches.Count >= GameplayConstants.NumBubblesNeededForMatch)
                {
                    Debug.Log("Called destroy tiles from here 2 : " + newMatches.OfType<Bubble>().ToList());
                    DestroyTiles(newMatches.OfType<Bubble>().ToList());
                }
            }

            newBubbles.Clear();

            yield return new WaitForSeconds(GameplayConstants.VoidChainFinishDelay);
            --voidCounter;
            if (voidCounter == 0)
            {
                IsChainingVoids = false;
                Debug.Log("UnlockInput 5");
                gameScreen.UnlockInput();
            }
        }

        public void DestroyBubble(Bubble bubble, bool transformVoids = true)
        {
            if (bubble != null)
            {
                if (bubble.IsBeingDestroyed)
                    return;

                bubble.IsBeingDestroyed = true;

                var colorBubble = bubble.GetComponent<ColorBubble>();

                if (bubble.Row == 0 &&
                    leaves.Count > 0 &&
                    leaves[bubble.Column] != null)
                {
                    if (colorBubble == null ||
                        colorBubble.CoverType == CoverType.None ||
                        colorBubble.CoverType == CoverType.Cloud)
                    {
                        DestroyLeaf(bubble.Column);
                        EventManager.RaiseEvent(new LeavesCollectedEvent(1));
                    }
                }

                if (colorBubble != null)
                {
                    if (colorBubble.CoverType == CoverType.Ice)
                    {
                        RemoveCover(bubble.gameObject);
                        bubble.IsBeingDestroyed = false;
                        return;
                    }

                    CheckForBlockers(bubble);

                    if (colorBubble.CoverType == CoverType.Cloud)
                        RemoveCover(colorBubble.gameObject);

                    if (transformVoids)
                    {
                        var transformedBubbles = TransformVoidBubbles(bubble);
                        foreach (var tbubble in transformedBubbles)
                            if (!newBubbles.Contains(tbubble))
                                newBubbles.Add(tbubble);

                        foreach (var tbubble in transformedBubbles)
                        {
                            var matches = LevelUtils.GetMatches(level, tbubble.GetComponent<ColorBubble>());
                            if (matches.Count >= GameplayConstants.NumBubblesNeededForMatch)
                            {
                                shouldChainVoids = true;
                                break;
                            }
                        }
                    }

                    EventManager.RaiseEvent(new BubblesCollectedEvent(colorBubble.Type, 1));

                    SoundPlayer.PlaySoundFx("Explode");
                }

                if (bubble.CanBeDestroyed())
                    bubble.ShowExplosionFx(fxPool);


                BoosterBubble boosterBubble = bubble.GetComponent<BoosterBubble>();
                if (boosterBubble != null && boosterBubble.isInView)
                {
                    var bubblesToExplode = boosterBubble.Resolve(level, lastShotBubble);
                    bubblesToExplode.RemoveAll(x => currentExplodingBubbles.Contains(x));
                    Debug.Log("Called destroy tiles from here 7 : " + bubblesToExplode.Count);
                    DestroyTiles(bubblesToExplode);
                }

                if (bubble.GetComponent<BombBubble>() != null)
                    SoundPlayer.PlaySoundFx("Bomb");
                else if (bubble.GetComponent<HorizontalBombBubble>() != null)
                    SoundPlayer.PlaySoundFx("BombHorizontal");
                else if (bubble.GetComponent<ColorBombBubble>() != null)
                    SoundPlayer.PlaySoundFx("ColorBomb");

                bubble.GetComponent<PooledObject>().Pool.ReturnObject(bubble.gameObject);
                level.Tiles[bubble.Row][bubble.Column] = null;

                OnBubbleExploded(bubble);
            }
        }

        private void DestroyBubbleFalling(Bubble bubble)
        {
            if (bubble.GetComponent<BlockerBubble>() != null &&
                bubble.GetComponent<BlockerBubble>().Type == BlockerBubbleType.StickyBubble)
            {
                DestroyBubble(bubble);
                SoundPlayer.PlaySoundFx("Sticky");
            }
            else
            {
                level.Tiles[bubble.Row][bubble.Column] = null;
                var falling = bubble.GetComponent<Falling>();
                if (falling != null)
                    falling.Fall();

                OnBubbleExploded(bubble);
                var colorBubble = bubble.GetComponent<ColorBubble>();
                if (colorBubble != null)
                    EventManager.RaiseEvent(new BubblesCollectedEvent(colorBubble.Type, 1));
            }
        }

        private void OnBubbleExploded(Bubble bubble)
        {
            var collectableBubble = bubble.GetComponent<CollectableBubble>();
            if (collectableBubble != null)
                EventManager.RaiseEvent(new CollectablesCollectedEvent(collectableBubble.Type, 1));

            GameState.Score += gameConfig.DefaultBubbleScore;
            gameUi.UpdateScore(GameState.Score);

            //var scoreText = scoreTextPool.GetObject();
            //scoreText.transform.position = bubble.transform.position;
            //scoreText.GetComponent<ScoreText>().Initialize(gameConfig.DefaultBubbleScore);
        }

        private void CheckForBlockers(Bubble bubble)
        {
            DestroyStones(bubble);
        }

        private void DestroyStones(Bubble bubble)
        {
            var neighbours = new List<Bubble>();
            var stonesToDestroy = new List<BlockerBubble>();
            var tileNeighbours = LevelUtils.GetNeighbours(level, bubble);
            foreach (var n in tileNeighbours)
            {
                if (!currentExplodingBubbles.Contains(n) && !neighbours.Contains(n))
                    neighbours.Add(n);
            }

            foreach (var n in neighbours)
            {
                var blocker = n.GetComponent<BlockerBubble>();
                if (blocker != null && blocker.Type == BlockerBubbleType.Stone)
                    stonesToDestroy.Add(blocker);
            }

            foreach (var stone in stonesToDestroy)
            {
                DestroyBubble(stone);
                SoundPlayer.PlaySoundFx("Stone");
            }
        }

        private List<Bubble> TransformVoidBubbles(Bubble bubble)
        {
            var retBubbles = new List<Bubble>();

            var adjacentVoidBubbles = new List<BlockerBubble>();
            var tileNeighbours = LevelUtils.GetNeighbours(level, bubble).OfType<BlockerBubble>();
            foreach (var n in tileNeighbours)
            {
                if (!adjacentVoidBubbles.Contains(n) &&
                    n.GetComponent<BlockerBubble>().Type == BlockerBubbleType.VoidBubble &&
                    !currentExplodingBubbles.Contains(n))
                    adjacentVoidBubbles.Add(n);
            }

            foreach (var voidBubble in adjacentVoidBubbles)
            {
                var newBubble = bubbleFactory.CreateColorBubble(bubble.GetComponent<ColorBubble>().Type);
                newBubble.GetComponent<Bubble>().Row = voidBubble.Row;
                newBubble.GetComponent<Bubble>().Column = voidBubble.Column;
                newBubble.GetComponent<Bubble>().GameLogic = this;
                newBubble.transform.position = voidBubble.transform.position;
                retBubbles.Add(newBubble.GetComponent<Bubble>());
                level.Tiles[voidBubble.Row][voidBubble.Column] = newBubble.GetComponent<Bubble>();
            }

            foreach (var voidBubble in adjacentVoidBubbles)
            {
                voidBubble.GetComponent<PooledObject>().Pool.ReturnObject(voidBubble.gameObject);
                voidBubble.ShowExplosionFx(fxPool);
                SoundPlayer.PlaySoundFx("Void");
            }

            return retBubbles;
        }

        private void DestroyLeaf(int column)
        {
            if (leaves[column] != null)
            {
                leaves[column].GetComponent<Animator>().SetTrigger("Release");
                leaves[column].GetComponent<Leaf>().Destroy();
                leaves[column] = null;
                SoundPlayer.PlaySoundFx("Leaf");
            }
        }

        private void RemoveFloatingBubbles()
        {
            StartCoroutine(RemoveFloatingBubblesCoroutine());
        }

        private IEnumerator RemoveFloatingBubblesCoroutine()
        {
            var floatingIslands = LevelUtils.FindFloatingIslands(level);
            var tilesToRemove = new List<Bubble>();
            foreach (var island in floatingIslands)
            {
                var isSticky = island.Count >= 2 && island.Find(x =>
                                   x.GetComponent<BlockerBubble>() != null &&
                                   x.GetComponent<BlockerBubble>().Type == BlockerBubbleType.StickyBubble);
                if (!isSticky)
                {
                    foreach (var tile in island)
                    {
                        if (tile.isInView)
                            tilesToRemove.Add(tile);
                    }
                }
            }

            foreach (var bubble in tilesToRemove)
            {
                var blocker = bubble.GetComponent<BlockerBubble>();
                if (blocker != null && blocker.Type != BlockerBubbleType.IronBubble && blocker.isInView)
                {
                    blocker.ShowExplosionFx(fxPool);
                }
                else
                {
                    if (bubble.GetComponent<ColorBubble>() != null)
                    {
                        var animator = bubble.GetComponentInChildren<Animator>();
                        if (animator != null && animator.gameObject.activeInHierarchy)
                            animator.SetTrigger("Falling");
                    }
                }
            }

            scoreSendData.bubbles_diff_popped_count += tilesToRemove.Count;
            if (tilesToRemove.Count > 0)
            {
                yield return new WaitForSeconds(GameplayConstants.FloatingIslandsRemovalDelay);
                Debug.Log("Destroy tile called 1 : " + tilesToRemove.Count);
                DestroyTiles(tilesToRemove, true);
            }
            else
            {
                yield return null;
                CheckGoals();
            } 
        }

        public void HandleTopRowMatches(Bubble bubble)
        {
            bubble.ForceStop();
            didBubbleCollideWithTop = true;

            var column = 0;
            var minDist = Mathf.Infinity;
            for (var i = 0; i < level.Tiles[0].Count; i++)
            {
                var tilePos = tilePositions[0][i];
                var newPos = tilePos;
                var newDist = Vector2.Distance(bubble.transform.position, newPos);
                if (newDist <= minDist)
                {
                    minDist = newDist;
                    column = i;
                }
            }

            HandleMatches(bubble, 0, column);
        }

        private void RemoveCover(GameObject bubble)
        {
            var colorBubble = bubble.GetComponent<ColorBubble>();
            var coverType = colorBubble.CoverType;
            var pos = colorBubble.transform.position;
            colorBubble.CoverType = CoverType.None;

            if (coverType == CoverType.Ice)
                SoundPlayer.PlaySoundFx("Ice");
            else if (coverType == CoverType.Cloud)
                SoundPlayer.PlaySoundFx("Cloud");

            var cover = bubble.transform.GetChild(1).gameObject;
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                var animator = cover.GetComponent<Animator>();
                if (animator != null && cover.activeInHierarchy)
                    cover.GetComponent<Animator>().SetTrigger("Explode");
            });
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                var fx = fxPool.GetCoverParticlePool(coverType).GetObject();
                fx.transform.position = pos;
                cover.GetComponent<PooledObject>().Pool.ReturnObject(cover);
            });
        }

        private void CheckGoals()
        {
            if (GameWon || GameLost)
                return;
           
            var allGoalsCompleted = true;
            foreach (var goal in levelInfo.Goals)
            {
                if (!goal.IsComplete(GameState) || true)
                {
                    allGoalsCompleted = false;
                    break;
                }
            }

            if (allGoalsCompleted && !GameWon)
            {
                GameWon = true;
                EndGame();

                var nextLevel = PlayerPrefs.GetInt("next_level");
                if (nextLevel == 0)
                    nextLevel = 1;
                if (levelInfo.Number == nextLevel)
                {
                    PlayerPrefs.SetInt("next_level", levelInfo.Number + 1);
                    PlayerPrefs.SetInt("unlocked_next_level", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("unlocked_next_level", 0);
                }

                if (playerBubbles.NumBubblesLeft > 1)
                {
                    gameScreen.OpenLevelCompletedAnimation();
                    playerBubbles.PlayEndOfGameSequence();
                }
                else
                {
                    gameScreen.StartCoroutine(gameScreen.OpenWinPopupAsync());
                }
            }
            gameScreen.SetVisibleLines();
            CalculateScoreAPICall();
            //if (!allGoalsCompleted && playerBubbles.NumBubblesLeft <= 1 && !playerBubbles.HasBubblesLeftToShoot)
            //{
            //    GameLost = true;
            //    EndGame();
            //    gameScreen.StartCoroutine(gameScreen.OpenOutOfBubblesPopupAsync());
            //}


        }

        public void StartGame()
        {
            GameStarted = true;
        }

        public void EndGame()
        {
            GameStarted = false;
            playerBubbles.OnGameEnded();
        }

        public void RestartGame()
        {
            gameScreen.OnGameRestarted();
            StartCoroutine(gameScreen.InitializeLevel());
            StartGame();
        }

        public void ContinueGame()
        {
            GameStarted = true;
            GameWon = false;
            GameLost = false;
            gameScreen.OnGameContinued();
            playerBubbles.OnGameContinued();
        }

        public void GameLostByBubbleTouchBottomLine()
        {
            Debug.LogError("Game Over here");
            GameLost = true;
            EndGame();
            CalculateScoreAPICall();
            //GameEndAPICall();
            //gameScreen.OpenTimeUpPopUp();
        }

        public void GameLostByTimerEnd()
        {
            GameLost = true;
            Debug.LogError("Timer over here");
            CalculateScoreAPICall();
            EndGame();
            //GameEndAPICall();
            //gameScreen.OpenTimeUpPopUp();
        }

        public void GameWinByClearBoard()
        {
            GameState.clearBoardScore = 1800;
            GameWon = true;
            EndGame();
            Debug.LogError("Win here.. clear board..Play particles here..");
        }

        Coroutine win_loseCoRef;
        public GameObject[] winPs;
        IEnumerator Win_LoseCoroutine()
        {
            float timeTakenInWinSequence = 0;
            if (GameWon)
            {
                for (int i = 0; i < winPs.Length; i++)
                {
                    float f = 0;
                    if (i==0)
                    {
                        f = 0.2f;
                    }
                    else
                    {
                        f = UnityEngine.Random.Range(0.5f, 1.1f);
                    }
                    timeTakenInWinSequence += f;
                    Vector3 targetPos = new Vector3(UnityEngine.Random.Range(-2.0f, 2.0f), UnityEngine.Random.Range(-1.0f, 2.0f), 0);
                    winPs[i].transform.localPosition = targetPos;
                    winPs[i].SetActive(true);
                    StartCoroutine(DisableWinPS(winPs[i]));
                    yield return new WaitForSeconds(f);
                    if (timeTakenInWinSequence > 2.5f)
                    {
                        break;
                    }
                }
                float timeRemain = 3- timeTakenInWinSequence;
                if(timeRemain > 0)
                {
                    yield return new WaitForSeconds(timeRemain);
                }
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
            Debug.LogError("End Game Call 5");
            GameEndAPICall();
            if (GameWon)
            {
                gameScreen.OpenWinPopup();
            }
            else if (GameLost)
            {
                gameScreen.OpenTimeUpPopUp();
            }
        }

        IEnumerator DisableWinPS(GameObject g)
        {
            yield return new WaitForSeconds(4);
            g.SetActive(true);  
        }

        public void CalculateScoreAPICall()
        {
            List<List<Bubble>> tiles = level.GetTiles();
            List<List<Bubble>> sceneTiles = new List<List<Bubble>>();
            List<List<Bubble>> notInView = new List<List<Bubble>>();
            for (int i = 0; i < tiles.Count; i++)
            {
                List<Bubble> tile = tiles[i];
                for (int j = 0; j < tile.Count; j++)
                {
                    if (tile[j] != null)
                    {
                        if (tile[j].isInView)
                        {
                            sceneTiles.Add(tile);
                        }
                        else
                        {
                            notInView.Add(tile);
                        }
                    }
                    else
                    {
                        sceneTiles.Add(tile);
                    }
                    break;
                }
            }
            int hiddenBubblesLeftCount = 0;
            int bubbleLeftCount = 0;
            List<ColorBubbleType> sceneColorBubbleAvailableTypes = new List<ColorBubbleType>();
            List<List<int>> currentView = new List<List<int>>();
            List<List<int>> nextView = new List<List<int>>();
            for (int i = 0; i < sceneTiles.Count; i++)
            {
                List<Bubble> tile = sceneTiles[i];
                List<int> view = new List<int>();
                string s = "";
                for (int j = 0; j < tile.Count; j++)
                {
                    if (tile[j] != null)
                    {
                        bool isAdd = false;
                        if (tile[j] is ColorBubble)
                        {
                            ColorBubble cb = (ColorBubble)tile[j];
                            if (!sceneColorBubbleAvailableTypes.Contains(cb.Type))
                            {
                                sceneColorBubbleAvailableTypes.Add(cb.Type);
                            }
                            s += ""+ ((int)cb.Type) + ", ";
                            view.Add(((int)cb.Type));
                            isAdd = true;
                        }
                        if (!isAdd)
                        {
                            view.Add(-1);
                        }
                        bubbleLeftCount++;
                    }
                    else
                    {
                        s += "-1,";
                        view.Add(-1);
                    }
                }
                currentView.Add(view);
                //Debug.LogError("Tile" + i + " :" + s);
            }

            bubbleFactory.ResetSceneViewShootingBubble(sceneColorBubbleAvailableTypes);
            if (!GameWon && !GameLost)
            {
                playerBubbles.CheckPrimaryBallColorValid();
            }
            
            for (int i = 0; i < notInView.Count; i++)
            {
                List<Bubble> tile = notInView[i];
                List<int> view = new List<int>();
                int tileBubble = 0;
                for (int j = 0; j < tile.Count; j++)
                {
                    if (tile[j] != null)
                    {
                        tileBubble++;
                        if (i < notInView.Count)
                        {
                            hiddenBubblesLeftCount++;
                        }
                        if (tile[j] is ColorBubble)
                        {
                            ColorBubble cb = (ColorBubble)tile[j];
                            if (cb!=null)
                            {
                                view.Add(((int)cb.Type));
                            }
                            else
                            {
                                view.Add(-1);
                            }
                        }
                        else
                        {
                            view.Add(-1);
                        }
                    }
                    else
                    {
                        view.Add(-1);
                    }
                }
                nextView.Add(view);
            }
            scoreSendData.bubbles_in_view = currentView;
            scoreSendData.bubbles_hidden = nextView;
            scoreSendData.time_left = (int)gameUi.currentGameTime;
            scoreSendData.life_count = gameUi.currentLifeCount;
            scoreSendData.bubbles_left = bubbleLeftCount;
            scoreSendData.bubbles_hidden_left = hiddenBubblesLeftCount;
            scoreSendData.user_game_id = currentGameId;// DataHandler.inst.apiLevelData.user_game_id;
            scoreSendData.tournament_id = DataHandler.inst.apiLevelData.tournament_game_id;
            //Debug.LogError("Send bubbles_same_popped_count : " + scoreSendData.bubbles_same_popped_count + "  bubbles_diff_popped_count : " + scoreSendData.bubbles_diff_popped_count);
            Debug.Log("Send Data calculate_score : " + JsonConvert.SerializeObject(scoreSendData));
            if (DataHandler.inst.TutorialLevelInt == 1)
            {
                int yourscore = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
                int timeBonus = ((int)scoreSendData.time_left) * 126;
                GameState.Score = yourscore;
                Debug.LogError("Wom :" + GameWon + "  Lose : " + GameLost);
                if (GameWon)
                    GameState.timeBonusScore = timeBonus;
                else
                    GameState.timeBonusScore = 0;

                if (GameWon || GameLost)
                {
                    if (win_loseCoRef != null)
                        StopCoroutine(win_loseCoRef);
                    win_loseCoRef = StartCoroutine(Win_LoseCoroutine());
                }
                gameUi.UpdateGameScore();
                return;
            }

            int yourscore2 = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
            int timeBonus2 = ((int)scoreSendData.time_left) * 126;
            GameState.Score = yourscore2;
            gameUi.UpdateGameScore();


            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.calculateScoreApi, JsonUtility.ToJson(scoreSendData), true, (success, dataString) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.calculateScoreApi + "  response : " + dataString);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.CalculateScoreReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.CalculateScoreReceiveData>(dataString, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            if (_data.score_breakdown == null)
                            {
                                int yourscore = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
                                int timeBonus = ((int)scoreSendData.time_left) * 126;
                                GameState.Score = yourscore;
                                Debug.LogError("Wom :" + GameWon + "  Lose : " + GameLost);
                                if (GameWon)
                                    GameState.timeBonusScore = timeBonus;
                                else
                                    GameState.timeBonusScore = 0;

                                if (GameWon || GameLost)
                                {
                                    if (win_loseCoRef != null)
                                        StopCoroutine(win_loseCoRef);
                                    win_loseCoRef = StartCoroutine(Win_LoseCoroutine());
                                }
                                Debug.LogError("Score :" + GameState.Score + " timeBonus :" + GameState.timeBonusScore + " total :" + GameState.totalScore + " clear board score :" + GameState.clearBoardScore);
                            }
                            else
                            {
                                GameState.Score = _data.score_breakdown.your_score;
                                GameState.clearBoardScore = _data.score_breakdown.clear_board;
                                Debug.LogError("Wom :" + GameWon + "  Lose : " + GameLost);
                                if (GameWon)
                                    GameState.timeBonusScore = _data.score_breakdown.time_bonus;
                                else
                                    GameState.timeBonusScore = 0;

                                int yourscore = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
                                int timeBonus = ((int)scoreSendData.time_left) * 126;
                                Debug.LogError("local score :" + yourscore);

                                if (GameWon || GameLost)
                                {
                                    if (win_loseCoRef != null)
                                        StopCoroutine(win_loseCoRef);
                                    win_loseCoRef = StartCoroutine(Win_LoseCoroutine());
                                }
                                Debug.LogError("Score :" + GameState.Score + " timeBonus :" + GameState.timeBonusScore + " total :" + GameState.totalScore + " clear board score :" + GameState.clearBoardScore);
                            }
                        }
                        else
                        {
                            int yourscore = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
                            int timeBonus = ((int)scoreSendData.time_left) * 126;
                            GameState.Score = yourscore;
                            Debug.LogError("Wom :" + GameWon + "  Lose : " + GameLost);
                            if (GameWon)
                                GameState.timeBonusScore = timeBonus;
                            else
                                GameState.timeBonusScore = 0;

                            if (GameWon || GameLost)
                            {
                                if (win_loseCoRef != null)
                                    StopCoroutine(win_loseCoRef);
                                win_loseCoRef = StartCoroutine(Win_LoseCoroutine());
                            }
                            Debug.LogError("Score :" + GameState.Score + " timeBonus :" + GameState.timeBonusScore + " total :" + GameState.totalScore + " clear board score :" + GameState.clearBoardScore);
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                            Debug.LogError(_data.error_message);
                        }
                        gameUi.UpdateGameScore();
                    }
                }
                else
                {
                    int yourscore = scoreSendData.bubbles_same_popped_count * 30 + scoreSendData.bubbles_diff_popped_count * 150;
                    int timeBonus = ((int)scoreSendData.time_left) * 126;
                    GameState.Score = yourscore;
                    if (GameWon || GameLost)
                    {
                        if (win_loseCoRef != null)
                            StopCoroutine(win_loseCoRef);
                        win_loseCoRef = StartCoroutine(Win_LoseCoroutine());
                    }
                    gameUi.UpdateGameScore();
                }
            },false);
        }

        public void GameEndAPICall(bool isLoaderRequired = false)
        {
            API_SendData.EndGameSendData sendData = new API_SendData.EndGameSendData();
            sendData.user_game_id = currentGameId;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.endGameAPI, JsonUtility.ToJson(sendData), true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.endGameAPI + "  response : " + data);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.EndGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.EndGameReceiveData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            Debug.LogError("Game end successfull");
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                    }
                }
            },isLoaderRequired);
        }
    }
}
