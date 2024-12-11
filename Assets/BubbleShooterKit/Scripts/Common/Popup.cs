// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BubbleShooterKit
{
    /// <summary>
    /// The base class all the popups in the game derive from.
    /// </summary>
	public class Popup : MonoBehaviour
	{
		[HideInInspector]
        public BaseScreen ParentScreen;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        private Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            if(OnOpen!=null)
            OnOpen.Invoke();
        }

        public void Close()
        {
            OnClose.Invoke();
            Debug.LogError("Close 1");
            if (ParentScreen != null)
            {
                Debug.LogError("Close 2");
                ParentScreen.ClosePopup();
            }
            
            if (animator != null)
            {
                animator.Play("Close");
                StartCoroutine(DestroyPopup());
            }
            else
            {
                Debug.LogError("Close 4");
                Destroy(gameObject);
            }
        }

	    private IEnumerator DestroyPopup()
        {
            yield return new WaitForSeconds(0.5f);
            Debug.LogError("Close 3");
            Destroy(gameObject);
        }
	}
}
