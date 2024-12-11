// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The in-game animated fox.
	/// </summary>
	public class Fox : MonoBehaviour
	{
		private Animator bodyAnimator;
		[SerializeField]
		private Animator armsAnimator = null;

		private void Awake()
		{
			bodyAnimator = GetComponent<Animator>();
		}
		
		public void PlayHappyAnimation()
		{
			//bodyAnimator.SetTrigger("Happy");
		}

		public void PlaySadAnimation()
		{
			//bodyAnimator.SetTrigger("Sad");
		}

		public void PlayShootingAnimation()
		{
			//armsAnimator.SetTrigger("Shooting");
		}
	}
}
