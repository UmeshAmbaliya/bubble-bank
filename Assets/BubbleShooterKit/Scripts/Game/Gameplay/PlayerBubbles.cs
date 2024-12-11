// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class is responsible for managing the primary and secondary bubbles
	/// in the game.
	/// </summary>
	public class PlayerBubbles : MonoBehaviour
	{
		public GameConfiguration GameConfig;
		public GameScreen GameScreen;
		public GameLogic GameLogic;
		public BubblePool BubblePool;
		public BubbleFactory BubbleFactory;
		public Shooter Shooter;
		public GameObject EnergyOrb;
		public Image EnergyOrbFill;
		public ParticleSystem EnergyOrbParticles;
		public GameObject SwapBubblesIcon;
		public Fox Fox;
		
		[SerializeField]
		private RectTransform primaryBubblePivot = null;
		[SerializeField]
		private RectTransform secondaryBubblePivot = null;
		
		[SerializeField]
		private TextMeshProUGUI numBubblesText = null;
		[SerializeField]
		private TextMeshProUGUI numBubblesTextBorder = null;

		private LevelInfo currentLevelInfo;	
		
		private GameObject primaryBubble;
		private GameObject secondaryBubble;
		private GameObject currentBubble;
		
		private bool swappingBubbles;
		private bool spawningNextBubbles;
		
		public int NumBubblesLeft { get; private set; }

		public bool IsSpecialBubbleActive;
		private float energyOrbFillLevel;
		
		public Bubble LastShotBubble { get; private set; }
		public Vector3 LastShotDir { get; private set; }

		public bool HasBubblesLeftToShoot => primaryBubble != null;

		private bool isPlayingEndGameSequence;
		public bool isFailBubble;

        private void Awake()
		{
			Assert.IsNotNull(primaryBubblePivot);	
			Assert.IsNotNull(secondaryBubblePivot);	
		}

		public void Initialize(LevelInfo levelInfo)
		{
			currentLevelInfo = levelInfo;
			
			primaryBubble = null;
			secondaryBubble = null;
			currentBubble = null;
			swappingBubbles = false;
			spawningNextBubbles = false;
			IsSpecialBubbleActive = false;
			energyOrbFillLevel = 0.0f;
			EnergyOrbFill.fillAmount = 0.0f;
			EnergyOrbParticles.Stop();
			
			NumBubblesLeft = levelInfo.NumBubbles;
			numBubblesText.text = NumBubblesLeft.ToString();
			numBubblesTextBorder.text = numBubblesText.text;
			
			StartCoroutine(SpawnInitialBubbles());
		}

		public void OnGameEnded()
		{
			EnergyOrbParticles.Stop();
		}
		
		public void OnGameContinued()
		{
			if (energyOrbFillLevel >= 1.0f)
				EnergyOrbParticles.Play();
			
			NumBubblesLeft = GameConfig.NumExtraBubbles;
			numBubblesText.text = NumBubblesLeft.ToString();
			numBubblesTextBorder.text = numBubblesText.text;
			swappingBubbles = false;
			spawningNextBubbles = false;
			StartCoroutine(SpawnInitialBubbles());
		}

		public void SwapBubbles()
		{
			if (swappingBubbles || spawningNextBubbles)
				return;

			if (secondaryBubble == null)
				return;

			if (IsSpecialBubbleActive)
				return;

			if (isPlayingEndGameSequence)
				return;

			var primaryBubblePos = primaryBubble.transform.position;
			var secondaryBubblePos = secondaryBubble.transform.position;
			
			var waypointsSecondary = new []
			{
				secondaryBubblePos,
				secondaryBubblePos + new Vector3(-0.2f, -0.5f, 0),
				primaryBubblePos
			};
			secondaryBubble.transform.DOPath(waypointsSecondary, 0.2f, PathType.CatmullRom);
			
			var waypointsPrimary = new []
			{
				primaryBubblePos,
				primaryBubblePos + new Vector3(0.2f, 0.5f, 0),
				secondaryBubblePos
			};
			var seq = DOTween.Sequence();
			seq.AppendCallback(() => { swappingBubbles = true; });
			seq.Append(primaryBubble.transform.DOPath(waypointsPrimary, 0.2f, PathType.CatmullRom));
			seq.AppendCallback(() =>
			{
				var tempBubble = primaryBubble;
				primaryBubble = secondaryBubble;
				secondaryBubble = tempBubble;
				currentBubble = primaryBubble;
				swappingBubbles = false;
				CheckPrimaryBallColorValid();

            });

		}
		
		private IEnumerator SpawnInitialBubbles()
		{
			// Need to wait 1 frame for the UI layout to work.
			yield return new WaitForEndOfFrame();
			Debug.LogError("SpawnInitialBubbles");

            Shooter.transform.position = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);	
			
			CreatePrimaryBubble();
			CreateSecondaryBubble();
			
			currentBubble = primaryBubble;
		}
		
		private void CreatePrimaryBubble()
		{
			primaryBubble = BubbleFactory.CreateRandomColorBubble();
			primaryBubble.transform.position = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);
            primaryBubble.layer = 11;
        }

		private void CreateSecondaryBubble()
		{
			secondaryBubble = BubbleFactory.CreateRandomColorBubble();
			secondaryBubble.transform.position = CanvasUtils.CanvasToWorldPoint(secondaryBubblePivot);
            secondaryBubble.layer = 11;
        }


		private IEnumerator SpawnNextBubbles(float delay)
		{
			if (spawningNextBubbles)
				yield return null;

			spawningNextBubbles = true;
			
			yield return new WaitForSeconds(delay);

			var primaryWorldPos = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);
			var primaryBubblePos = new Vector3(primaryWorldPos.x, primaryWorldPos.y, 0);
			var secondaryWorldPos = CanvasUtils.CanvasToWorldPoint(secondaryBubblePivot);
			var secondaryBubblePos = new Vector3(secondaryWorldPos.x, secondaryWorldPos.y, 0);
			
			var waypointsSecondary = new []
			{
				secondaryBubblePos,
				secondaryBubblePos + new Vector3(-0.15f, -0.3f, 0),
				primaryBubblePos
			};
			var seq = DOTween.Sequence();
			seq.Append(secondaryBubble.transform.DOPath(waypointsSecondary, 0.2f, PathType.CatmullRom));
			primaryBubble = secondaryBubble;
			currentBubble = primaryBubble;
			seq.AppendCallback(() =>
			{
				currentBubble.transform.position = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);
				if (NumBubblesLeft > 1)
					CreateNextSecondaryBubble();
			});
		}

		private void CreateNextSecondaryBubble()
		{
			var secondaryWorldPos = CanvasUtils.CanvasToWorldPoint(secondaryBubblePivot);
			var secondaryBubblePos = new Vector3(secondaryWorldPos.x, secondaryWorldPos.y, 0);
			secondaryBubble = BubbleFactory.CreateRandomColorBubble();
			secondaryBubble.transform.position = secondaryBubblePos + new Vector3(1.0f, 0, 0);
			secondaryBubble.layer = 11;
            var waypointsSecondary = new []
			{ 
				secondaryBubblePos + new Vector3(1.0f, 0, 0),
				secondaryBubblePos + new Vector3(0.15f, 0.3f, 0),
				secondaryBubblePos
			};
			var seq = DOTween.Sequence();
			seq.Append(secondaryBubble.transform.DOPath(waypointsSecondary, 0.2f, PathType.CatmullRom));
			seq.AppendCallback(() => 
			{
				spawningNextBubbles = false;
				if (isFailBubble)
				{
					isFailBubble = false;
					CheckPrimaryBallColorValid(); 
				}
			});
		}

		public void CheckPrimaryBallColorValid()
		{
			if (primaryBubble != null && isFailBubble == false && BubbleFactory.currentBubbleTypes.Count > 0)
			{
				ColorBubble cb = primaryBubble.GetComponent<ColorBubble>();
				if (cb!=null)
				{
					if (!BubbleFactory.currentBubbleTypes.Contains(cb.Type))
					{
						Debug.LogError("primary "+cb.Type + " type not found in list");
						Destroy(primaryBubble);
						if (primaryBubble == null)
						{
							Debug.Log("Primary bubble destroyed successfully");
						}
                        primaryBubble = BubbleFactory.CreateRandomColorBubble();
                        primaryBubble.transform.position = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);
                        currentBubble = primaryBubble;
                        primaryBubble.layer = 11;
					}
				}
				else
				{
					Debug.Log("ColorBubble null");
                    //Debug.Break();
                }
			}

            if (secondaryBubble != null && isFailBubble == false && BubbleFactory.currentBubbleTypes.Count > 0)
            {
                ColorBubble cb = secondaryBubble.GetComponent<ColorBubble>();
                if (cb != null)
                {
                    if (!BubbleFactory.currentBubbleTypes.Contains(cb.Type))
                    {
                        Debug.LogError("secondary " + cb.Type + " type not found in list");
                        Destroy(secondaryBubble);
                        if (secondaryBubble == null)
                        {
                            Debug.Log("Primary bubble destroyed successfully");
                        }
                        var secondaryWorldPos = CanvasUtils.CanvasToWorldPoint(secondaryBubblePivot);
                        var secondaryBubblePos = new Vector3(secondaryWorldPos.x, secondaryWorldPos.y, 0);
                        secondaryBubble = BubbleFactory.CreateRandomColorBubble();
                        secondaryBubble.transform.position = secondaryBubblePos;
                        secondaryBubble.layer = 11;
                    }
                }
                else
                {
                    Debug.Log("ColorBubble null");
                    //Debug.Break();
                }
            }
        }
		
		public void ShootBubble(Vector2 shootDir, Vector2 hitPoint)
		{
			if (currentBubble == null)
				return;

			if (swappingBubbles)
				return;

			if (GameScreen.IsInputLocked)
				return;
			
			SoundPlayer.PlaySoundFx("Shoot");
			
			GameScreen.LockInput();

			Fox.PlayShootingAnimation();
			
			LastShotDir = shootDir;
			var newPos = currentBubble.transform.position;
			newPos.y = hitPoint.y;
			currentBubble.transform.position = newPos;
			currentBubble.GetComponent<Bubble>().Shoot(shootDir);
			LastShotBubble = currentBubble.GetComponent<Bubble>();
			currentBubble.layer = 9;
			currentBubble = null;

			if (!IsSpecialBubbleActive)
			{
				//NumBubblesLeft -= 1;
				if (NumBubblesLeft < 1)
				{
					primaryBubble = null;
					numBubblesText.text = "0";
					numBubblesTextBorder.text = "0";
				}
				else
				{
					numBubblesText.text = NumBubblesLeft.ToString();
					numBubblesTextBorder.text = numBubblesText.text;
					StartCoroutine(SpawnNextBubbles(GameplayConstants.NewBubbleSpawningDelay));
				}
			}
		}
		
		public void CreateEnergyBubble()
		{
			if (energyOrbFillLevel < 1.0f)
				return;

			if (IsSpecialBubbleActive)
				return;

			if (isPlayingEndGameSequence)
				return;
			
			EnergyOrbParticles.Stop();

			IsSpecialBubbleActive = true;
			
			energyOrbFillLevel = 0.0f;
			EnergyOrbFill.DOFillAmount(energyOrbFillLevel, 0.1f);

			primaryBubble.GetComponent<Bubble>().SetColliderEnabled(false);

			var energyBubble = BubblePool.EnergyBubblePool.GetObject();
			energyBubble.GetComponent<SpecialBubble>().GameLogic = GameLogic;
			energyBubble.transform.position = CanvasUtils.CanvasToWorldPoint(EnergyOrbFill.rectTransform);
			var primaryBubbleWorldPos = CanvasUtils.CanvasToWorldPoint(primaryBubblePivot);
			var primaryBubblePos = new Vector3(primaryBubbleWorldPos.x, primaryBubbleWorldPos.y, 0);
			var waypoints = new []
			{
				energyBubble.transform.position,
				energyBubble.transform.position + new Vector3(-1.5f, 0.5f, 0),
				primaryBubblePos
			};
			energyBubble.transform.DOPath(waypoints, 0.2f, PathType.CatmullRom);
			
			currentBubble = energyBubble;
		}
		
		public void FillEnergyOrb()
		{
			energyOrbFillLevel += 1.0f / 5;
			Mathf.Clamp(energyOrbFillLevel, 0.0f, 1.0f);
			EnergyOrbFill.DOFillAmount(energyOrbFillLevel, 0.1f);
			if (energyOrbFillLevel >= 1.0f && !EnergyOrbParticles.isPlaying)
				EnergyOrbParticles.Play();
		}

		public void StopEnergyOrbParticles()
		{
			if (EnergyOrbParticles.isPlaying)
				EnergyOrbParticles.Stop();
		}

		public void CreatePurchasableBoosterBubble(PurchasableBoosterBubbleType bubbleType)
		{
			IsSpecialBubbleActive = true;	
			primaryBubble.GetComponent<Bubble>().SetColliderEnabled(false);
			
			switch (bubbleType)
			{
				case PurchasableBoosterBubbleType.RainbowBubble:
				case PurchasableBoosterBubbleType.HorizontalBomb:
				case PurchasableBoosterBubbleType.CircleBomb:
					var bubble = BubblePool.PurchasableBoosterBubblePools[(int)bubbleType].GetObject();
					bubble.GetComponent<Bubble>().GameLogic = GameLogic;
					bubble.transform.position = primaryBubble.transform.position;
					currentBubble = bubble;
					break;
			}
		}
		
		public void ProcessInput(GameObject go)
		{
			if (go == secondaryBubble)
			{
				SwapBubbles();
			}
			else if (go == SwapBubblesIcon)
			{
				SwapBubblesIcon.GetComponent<Animator>().SetTrigger("Pressed");
				SwapBubbles();
			}
			else if (go == EnergyOrb)
			{
				CreateEnergyBubble();
			}
		}

		public void OnSpecialBubbleShot()
		{
			IsSpecialBubbleActive = false;
			currentBubble = primaryBubble;
			primaryBubble.GetComponent<Bubble>().SetColliderEnabled(true);
			SoundPlayer.PlaySoundFx("Energy");
		}

		public void PlayEndOfGameSequence()
		{
			StartCoroutine(PlayEndGameSequence());
		}
		
		private IEnumerator PlayEndGameSequence()
		{
			isPlayingEndGameSequence = true;	
			
			yield return new WaitForSeconds(GameplayConstants.EndOfGameBubbleSequenceStartingDelay);

			BubbleFactory.ResetAvailableShootingBubbles(currentLevelInfo);
			
			while (NumBubblesLeft >= 1)
			{
				var seq = DOTween.Sequence();
				seq.AppendCallback(() =>
				{
					GameLogic.DestroyBubble(primaryBubble.GetComponent<ColorBubble>());
					
					//NumBubblesLeft -= 1;
					if (NumBubblesLeft < 1)
					{
						primaryBubble = null;
						numBubblesText.text = "0";
						numBubblesTextBorder.text = "0";
						GameScreen.StartCoroutine(GameScreen.OpenWinPopupAsync());
					}
					else
					{
						numBubblesText.text = NumBubblesLeft.ToString();
						numBubblesTextBorder.text = numBubblesText.text;
						StartCoroutine(SpawnNextBubbles(GameplayConstants.NewBubbleSpawningDelay));
					}
				});
						
				yield return new WaitForSeconds(GameplayConstants.EndOfGameBubbleSequenceSpeed);
			}
			
			yield return new WaitForEndOfFrame();
			
			GameScreen.CloseLevelCompletedAnimation();
			isPlayingEndGameSequence = false;	
		}

		public bool IsPlayingEndGameSequence()
		{
			return isPlayingEndGameSequence;
		}
	}
}
