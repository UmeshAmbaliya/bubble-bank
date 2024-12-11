// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The base type of bubbles. Note how the bubble prefabs have their
	/// associated sprite be a child of the root game object, while their
	/// colliders are on the root game object. This is done in order to
	/// prevent physics artifacts when animating the bubbles (i.e., only
	/// the sprites move but not the colliders).
	/// </summary>
	public abstract class Bubble : MonoBehaviour
	{
		public int Row;
		public int Column;
		[HideInInspector]
		public GameLogic GameLogic;
		
		public float Speed = 14.0f;
		
		public bool CollidingWithAnotherBubble;
		
		private Vector3 shootDir;
		private bool shooting;
		public bool isInView;
		 

        private CircleCollider2D circleCollider;
		private SpriteRenderer spriteRenderer;
		private Sprite originalSprite;
		private Camera mainCamera;

		public bool IsBeingDestroyed;
		
		protected virtual void Awake()
		{
			circleCollider = GetComponent<CircleCollider2D>();
			spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
			originalSprite = spriteRenderer.sprite;
			mainCamera = Camera.main;
        }

		protected virtual void OnEnable()
		{
			isInView = false;
			shooting = false;
			CollidingWithAnotherBubble = false;
			circleCollider.enabled = true;
			IsBeingDestroyed = false;
		}
		
		protected virtual void OnDisable()
		{
			spriteRenderer.sprite = originalSprite;
		}

		public virtual bool CanBeDestroyed()
		{
			return true;
		}

		public void Explode()
		{
			circleCollider.enabled = false;
			if (isCollidedWithBottomLine)
			{
				isCollidedWithBottomLine = false;
                DataHandler.inst.isBubbleCollidedWithBottomLine = false;
			}
        }

		public void Shoot(Vector2 dir)
		{
			shooting = true;
			shootDir = dir.normalized;
		}
		
		public void ForceStop()
		{
			shooting = false;
		}

		private void Stop(Bubble touchedBubble)
		{
			if (shooting)
			{
				shooting = false;
				GameLogic.HandleMatches(this, touchedBubble);
			}
		}

		public void ReverseDirection()
		{
			shootDir.x *= -1;
		}
		
		protected void FixedUpdate()
		{
			if (shooting)
			{
				transform.position += shootDir * Speed * Time.deltaTime;
				var leftEdge = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
				var rightEdge = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
				if (transform.position.x - spriteRenderer.bounds.size.x / 2 <= leftEdge.x ||
				    transform.position.x + spriteRenderer.bounds.size.x / 2 >= rightEdge.x)
				{
					ReverseDirection();
				}
			}
		}

		protected void OnTriggerEnter2D(Collider2D other)
		{
			var otherBubble = other.GetComponent<Bubble>();
			if (otherBubble != null)
			{
				CollidingWithAnotherBubble = true;
				otherBubble.Stop(this);
			}
		}

		public void SetColliderEnabled(bool colliderEnabled)
		{
			circleCollider.enabled = colliderEnabled;
		}
		bool isCollidedWithBottomLine = false;
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == 13 && isInView && !GameLogic.GameLost)
            {
                isCollidedWithBottomLine = true;
                Debug.LogError("Other : " + other.gameObject.name + "   " + this.gameObject.name);
				//GameLogic.GameLostByBubbleTouchBottomLine();// = true;
				DataHandler.inst.isBubbleCollidedWithBottomLine = true;
            }
        }

        public virtual void ShowExplosionFx(FxPool fxPool)
		{

		}

		public void SetInView(bool isSet)
		{
			isInView = isSet;
			if (transform.childCount>0)
			{
				transform.GetChild(0).gameObject.SetActive(isSet);
			}
        }
	}
}
