// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This component makes bubbles fall to gravity when they are located in
	/// a floating group of bubbles.
	/// </summary>
	public class Falling : MonoBehaviour
	{
		private SpriteRenderer spriteRenderer;
		private CircleCollider2D circleCollider;
		private Rigidbody2D body;
		private bool falling;

		private void Awake()
		{
			spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
			circleCollider = GetComponent<CircleCollider2D>();
			body = GetComponent<Rigidbody2D>();
		}
		
		public void Fall()
		{
			falling = true;
			circleCollider.enabled = false;
			var sign = Random.Range(0, 2);
			var x = Random.Range(GameplayConstants.FallingMinXForce, GameplayConstants.FallingMaxXForce);
			var y = Random.Range(GameplayConstants.FallingMinYForce, GameplayConstants.FallingMaxYForce);
			body.velocity = new Vector2(sign == 0 ? x : -x, y);
			FadeOutSprite();
			if (gameObject.activeInHierarchy)
				StartCoroutine(AutoDestroy());
		}

		private void OnEnable()
		{
			falling = false;
			if (spriteRenderer != null)
			{
				var newColor = spriteRenderer.color;
				newColor.a = 1.0f;
				spriteRenderer.color = newColor;
			}
			circleCollider.enabled = true;
		}
		
		private void FixedUpdate()
		{
			if (falling)
				body.velocity = body.velocity + (Physics2D.gravity * GameplayConstants.FallingGravityMultiplier * Time.fixedDeltaTime);
		}

		private void FadeOutSprite()
		{
			var seq = DOTween.Sequence();
			seq.AppendInterval(GameplayConstants.FallingFadeOutDelay);
			seq.Append(spriteRenderer.DOFade(0.0f, GameplayConstants.FallingFadeOutDuration));
			seq.Play();
		}

		private IEnumerator AutoDestroy()
		{
			var colorBubble = GetComponent<ColorBubble>();
			if (colorBubble != null && colorBubble.CoverType != CoverType.None)
			{
				colorBubble.CoverType = CoverType.None;
				var cover = transform.GetChild(1).gameObject;
				cover.GetComponent<PooledObject>().Pool.ReturnObject(cover);
			}
			yield return new WaitForSeconds(2.0f);
			GetComponent<PooledObject>().Pool.ReturnObject(gameObject);
		}
	}
}
