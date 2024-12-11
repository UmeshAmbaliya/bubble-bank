// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to color bubbles.
	/// </summary>
	[RequireComponent(typeof(CircleCollider2D))]
	public class ColorBubble : Bubble
	{
		public ColorBubbleType Type;
		public CoverType CoverType;
		
		public bool Visible => transform.GetChild(0).GetComponent<UpdateVisibility>().Visible;

		private float accTime;

		private Transform childTransform;
		private Vector3 originalScale;

		protected override void Awake()
		{
			base.Awake();
			childTransform = transform.GetChild(0);
			originalScale = childTransform.localScale;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			childTransform.localScale = originalScale;
			CoverType = CoverType.None;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			childTransform.localScale = originalScale;
			if (transform.childCount >= 2)
			{
				var cover = transform.GetChild(1);
				cover.GetComponent<PooledObject>().Pool.ReturnObject(cover.gameObject);
			}
		}

		private void Update()
		{
			accTime += Time.deltaTime;
			if (accTime >= GameplayConstants.BubbleBlinkRate)
			{
				accTime = 0.0f;
				var rnd = Random.Range(0, 2);
				if (rnd == 0)
					childTransform.GetComponent<Animator>().SetTrigger("Blink");
			}
		}

		public override void ShowExplosionFx(FxPool fxPool)
		{
			var fx = fxPool.GetColorBubbleParticlePool(Type).GetObject();
			fx.transform.position = transform.position;
		}
	}
}
