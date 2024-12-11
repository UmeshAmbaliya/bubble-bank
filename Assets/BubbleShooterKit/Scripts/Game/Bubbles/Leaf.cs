// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The type associated to collectable leaves.
	/// </summary>
	public class Leaf : MonoBehaviour
	{
		public FxPool FxPool;

		private Animator animator;
		private GameObject mouth;
		private GameObject tears;
		
		private const float DestroyDelay = 2.0f;
		private bool destroy;
		private float accTime;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			mouth = transform.GetChild(1).gameObject;
			tears = transform.GetChild(2).gameObject;
		}
		
		protected virtual void OnEnable()
		{
			destroy = false;
			accTime = 0.0f;
		}
		
		public void PlayParticleFx()
		{
			var particles = FxPool.LeafParticlePool.GetObject();
			particles.transform.position = transform.position;
		}

		public void Destroy()
		{
			destroy = true;
		}
		
		private void Update()
		{
			if (destroy)
			{
				accTime += Time.deltaTime;
				if (accTime >= DestroyDelay)
				{
					animator.SetTrigger("Reset");
					mouth.transform.localScale = Vector3.one;
					tears.SetActive(true);
					GetComponent<PooledObject>().Pool.ReturnObject(gameObject);
				}
			}
		}
	}
}
