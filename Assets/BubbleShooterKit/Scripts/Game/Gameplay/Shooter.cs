// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class manages the in-game shooting ray.
	/// </summary>
	public class Shooter : MonoBehaviour
	{
		public GameObject DotPrefab;
		public GameScreen GameScreen;
		public PlayerBubbles PlayerBubbles;
		public RectTransform PrimaryBubblePivot;
        public RectTransform arrowTransform;


		public float MaxAngle = 30f;

		private Camera mainCamera;

		private bool isMouseDown;

		private const int DefaultMaxDots = 100;
		private const int SuperAimMaxDots = 17;
		private const float DotGap = 0.15f;
		private const float startGap = 1.5f;

		private bool dotsHidden;

		private bool hitDetected;
		private Vector2 hitPoint;
		private Vector2 shootDir;
		
		public readonly List<SpriteRenderer> dots = new List<SpriteRenderer>();

		public bool isInputEnabled;
		private bool isUserPressing;

		private float tileHeight;
		public Color dotColor;
		private void Start()
		{
			mainCamera = Camera.main;
			CreateDots(DefaultMaxDots);
		}

		public void Initialize(float height)
		{
			tileHeight = height;
		}

		private void CreateDots(int numDots)
		{
			const float startAlpha = 0f;
			const float alphaIncrease = 0.2f;
			for (var i = 0; i < numDots; i++)
			{
				var dot = Instantiate(DotPrefab);
				dot.name = "Dot" + i;
				dot.transform.parent = transform;
				dot.transform.localPosition = Vector3.zero;

				var spriteRenderer = dot.GetComponent<SpriteRenderer>();
				dots.Add(spriteRenderer);

				var color = spriteRenderer.color;
				color.a = startAlpha + Mathf.Clamp(i * alphaIncrease, 0, 1);
				spriteRenderer.color = color;
			}
		}

		public void ApplySuperAim()
		{
			foreach (var dot in dots)
				Destroy(dot);
			dots.Clear();
			
			CreateDots(SuperAimMaxDots);
		}

		private void OrientDots()
		{
			var leftEdge = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
			var rightEdge = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
			
			var normalDir = shootDir.normalized;
            float angle = Mathf.Atan2(normalDir.y, normalDir.x) * Mathf.Rad2Deg;
            arrowTransform.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            var hasReversed = false;
			var reversed = 0;
			var reversedLeft = false;
			SpriteRenderer lastDot = null;
			float minX = 0;
			int bubbleFoundIndex = -1;
            //Debug.LogError("HandleTouchMove "+ angle + " " +normalDir);
            for (var i = 0; i < dots.Count; i++)
			{
				var dot = dots[i];
				if (bubbleFoundIndex > 0)
				{ 
                    dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
					continue;
                }
                var newPos = new Vector3(normalDir.x, normalDir.y) * i * DotGap;
				dot.transform.localPosition = newPos;
                RaycastHit2D rayCast = Physics2D.Raycast(dot.transform.position, Vector2.zero);
                if (rayCast.collider != null && rayCast.collider.gameObject.layer == 9)
                    bubbleFoundIndex = i;

				if (i <= 5)
					dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
				else
					dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 1);

                if (dot.transform.localPosition.x <= leftEdge.x)
				{ 
                    hasReversed = true;
					reversed = i;
					reversedLeft = true;
					if(i+1 <= dots.Count -1)
						minX = dots[i+1].transform.localPosition.x;
                    break;
				}
				
				if (dot.transform.localPosition.x >= rightEdge.x)
				{
                    if (i + 1 <= dots.Count - 1)
                        minX = dots[i + 1].transform.localPosition.x;
                    hasReversed = true;
					reversed = i;
					break;
				}
			}

			if (hasReversed && bubbleFoundIndex == -1)
			{
				for (var i = reversed; i < dots.Count; i++)
				{
					var dot = dots[i];
					if (bubbleFoundIndex > 0)
					{
                        dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
                        if (i == dots.Count - 1)
                            for (int k = 0; k < dots.Count; k++)
                            {
                                dot = dots[k];
                                if (lastDot != null)
                                {
                                    if (lastDot.transform.localPosition.y > dot.transform.localPosition.y)
                                        lastDot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);

                                    if (reversedLeft)
                                    {
                                        if (dot.transform.localPosition.x < minX)
                                            dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
                                    }
                                    else
                                    {
                                        if (dot.transform.localPosition.x > minX)
                                            dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
                                    } 
                                }
                                lastDot = dot;
                            }
                        continue;
                    }

					var newPos = new Vector3(-normalDir.x, normalDir.y) * i * DotGap;
					newPos.x += reversedLeft ? -rightEdge.x * 2 : rightEdge.x * 2;
					newPos.y -= tileHeight / 2.0f;
					dot.transform.localPosition = newPos;

                    RaycastHit2D rayCast = Physics2D.Raycast(dot.transform.position, Vector2.zero);
                    if (rayCast.collider != null && rayCast.collider.gameObject.layer == 9)
							bubbleFoundIndex = i;

					dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 1);
                    //var spriteRenderer = dot.GetComponent<SpriteRenderer>();
                    //var color = spriteRenderer.color;
                    //color.a = startAlpha + Mathf.Clamp(idx * alphaIncrease, 0, 1);
                    //spriteRenderer.color = color;

                    if (i == dots.Count - 1)
                        for (int k = 0; k < dots.Count; k++)
                        {
                            dot = dots[k];
                            if (lastDot != null)
                            {
                                if (lastDot.transform.localPosition.y > dot.transform.localPosition.y)
                                    lastDot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);

                                if (reversedLeft)
                                {
                                    if (dot.transform.localPosition.x < minX)
                                        dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
                                }
                                else
                                {
                                    if (dot.transform.localPosition.x > minX)
                                        dot.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0);
                                }
                            }
                            lastDot = dot;
                        }
                }
			}
		}

		private void ShowDots()
		{
			dotsHidden = false;
			foreach (var dot in dots)
			{
				var spriteRenderer = dot.GetComponent<SpriteRenderer>();
				spriteRenderer.DOKill();
				spriteRenderer.DOFade(1.0f, 0);
			}
		}
		
		private void HideDots()
		{
			dotsHidden = true;
			foreach (var dot in dots)
			{
				var spriteRenderer = dot.GetComponent<SpriteRenderer>();
				spriteRenderer.DOKill();
				spriteRenderer.DOFade(0.0f, 1.0f);
			}
		}

		private void HandleTouchDown()
		{
			//Debug.LogError("HandleTouchDown");
			isUserPressing = true;
		}

		private void HandleTouchUp()
		{
			isUserPressing = false;	
			
			if (dotsHidden)
				return;
			
			if (hitDetected)
			{
				hitDetected = false;
				PlayerBubbles.ShootBubble(shootDir, hitPoint);
			}

			HideDots();
		}
		
		private void HandleTouchMove(Vector2 touch)
		{
			if (!isUserPressing)
				return;
		
			var point = mainCamera.ScreenToWorldPoint(touch);
			var direction = point - transform.position;
			direction.Normalize();
			
			var angle = Vector2.Angle(new Vector2(1, 0), direction);
			var shouldHideDots = angle <= MaxAngle || angle >= 180 - MaxAngle;
			if (shouldHideDots)
			{
				HideDots();					
				return;
			}

			if (dotsHidden)
				ShowDots();
			
			var hit = Physics2D.Raycast(transform.position, direction);
			if (hit.collider != null)
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
				    hit.collider.gameObject.layer == LayerMask.NameToLayer("Bubble") ||
				    hit.collider.gameObject.layer == LayerMask.NameToLayer("FireBubble") ||
					hit.collider.gameObject.layer == LayerMask.NameToLayer("BottomEdge"))
				{
					hitDetected = true;
					hitPoint = hit.point;
					shootDir = direction;
					//Debug.LogError("Shoot dir : " + shootDir.normalized);
				}
			}
			OrientDots();
		}

		private void Update()
		{
			if (!isInputEnabled)
				return;
			//Debug.LogError("Shoot"+1);
			if (!GameScreen.CanPlayerShoot())
			{
				if(!dotsHidden)
					HideDots();
                return;
			}
            //Debug.LogError("Shoot" + 2);
            var touches = Input.touches;
			if (touches.Length > 0)
			{
				var touch = touches[0];

				if (touch.phase == TouchPhase.Began)
				{
					var pivot = CanvasUtils.CanvasToWorldPoint(PrimaryBubblePivot);
					var mousePos = mainCamera.ScreenToWorldPoint(touch.position);
					if (mousePos.y >= pivot.y)
					{
						isMouseDown = true;
						HandleTouchDown();
					}
				}
				else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
				{
					isMouseDown = false;
					HandleTouchUp();
				}
				else if (isMouseDown)
				{
					var pivot = CanvasUtils.CanvasToWorldPoint(PrimaryBubblePivot);
					var mousePos = mainCamera.ScreenToWorldPoint(touch.position);
					if (mousePos.y >= pivot.y)
					{
						HandleTouchMove(touch.position);
					}
					else
					{
						isMouseDown = false;
						HideDots();
					}
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				//Debug.LogError("Mouse down");
				var pivot = CanvasUtils.CanvasToWorldPoint(PrimaryBubblePivot);
				var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				//Debug.Log("Mouse pos " + mousePos.y + "  Pivot : " + pivot.y);
				if (mousePos.y >= pivot.y)
				{
					isMouseDown = true;
					HandleTouchDown();
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
                //Debug.LogError("Mouse Up");
                isMouseDown = false;
				HandleTouchUp();
			}
			else if (isMouseDown)
			{
				var pivot = CanvasUtils.CanvasToWorldPoint(PrimaryBubblePivot);
				var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				if (mousePos.y >= pivot.y)
				{
					HandleTouchMove(Input.mousePosition);
				}
				else
				{
					isMouseDown = false;
					HideDots();
				}
			}
		}

		public void SetInputEnabled(bool inputEnabled)
		{
			isInputEnabled = inputEnabled;
		}
	}
}
