// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BubbleShooterKit
{
    /// <summary>
    /// Every button in the game plays an animation when pressed.
    /// This class, modeled after Unity's own Button, enables that behavior.
    /// </summary>
    public class AnimatedButton : UIBehaviour, IPointerClickHandler
    {
        [Serializable]
        private class ButtonClickedEvent : UnityEvent
        {
        }

        public bool Interactable = true;

        [SerializeField]
        private ButtonClickedEvent onClick = new ButtonClickedEvent();

        private Animator animator;

        private bool blockInput;

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable || eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!blockInput)
            {
                blockInput = true;
                Press();
                // Block the input for a short while to prevent spamming.
                StartCoroutine(BlockInputTemporarily());
            }
        }

        private void Press()
        {
            if (!IsActive())
                return;

            animator.SetTrigger("Pressed");
            StartCoroutine(InvokeOnClickAction());
        }

        private IEnumerator InvokeOnClickAction()
        {
            yield return new WaitForSeconds(0.1f);
            onClick.Invoke();
        }

        private IEnumerator BlockInputTemporarily()
        {
            yield return new WaitForSeconds(0.5f);
            blockInput = false;
        }
    }
}
