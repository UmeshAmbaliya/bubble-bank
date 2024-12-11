// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class manages the high-level logic of the level screen.
    /// </summary>
    public class LevelScreen : BaseScreen
    {
        [SerializeField]
        private ScrollRect scrollRect = null;

        [SerializeField]
        private GameObject scrollView = null;

        [SerializeField]
        private GameObject avatarPrefab = null;

        [SerializeField]
        private GameObject rewardedAdButton = null;

        private void Awake()
        {
            Assert.IsNotNull(scrollRect);
            Assert.IsNotNull(scrollView);
            Assert.IsNotNull(avatarPrefab);
            Assert.IsNotNull(rewardedAdButton);
        }

        protected override void Start()
        {
            base.Start();
            
            scrollRect.vertical = false;

            #if UNITY_ADS
            rewardedAdButton.SetActive(true);
            #else
            rewardedAdButton.SetActive(false);
            #endif

            var avatar = Instantiate(avatarPrefab);
            avatar.transform.SetParent(scrollView.transform, false);

            var nextLevel = PlayerPrefs.GetInt("next_level");
            if (nextLevel == 0)
                nextLevel = 1;

            LevelButton currentButton = null;
            foreach (var button in scrollView.GetComponentsInChildren<LevelButton>())
            {
                if (button.NumLevel != nextLevel)
                    continue;
                currentButton = button;
                break;
            }

            if (currentButton == null)
                return;

            var newPos = scrollView.GetComponent<RectTransform>().anchoredPosition;
            newPos.y =
                scrollRect.transform.InverseTransformPoint(scrollView.GetComponent<RectTransform>().position).y -
                scrollRect.transform.InverseTransformPoint(currentButton.transform.position).y;
            newPos.y += Canvas.GetComponent<RectTransform>().rect.height / 2.0f;
            if (newPos.y < scrollView.GetComponent<RectTransform>().anchoredPosition.y)
                scrollView.GetComponent<RectTransform>().anchoredPosition = newPos;

            var targetPos = currentButton.transform.position + new Vector3(0, 1.0f, 0);

            LevelButton prevButton = null;
            if (PlayerPrefs.GetInt("unlocked_next_level") == 1)
            {
                foreach (var button in scrollView.GetComponentsInChildren<LevelButton>())
                {
                    if (button.NumLevel != PlayerPrefs.GetInt("last_selected_level"))
                        continue;
                    prevButton = button;
                    break;
                }
            }

            if (prevButton != null)
            {
                avatar.transform.position = prevButton.transform.position + new Vector3(0, 1.0f, 0);
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(0.5f);
                sequence.Append(avatar.transform.DOMove(targetPos, 0.8f));
                sequence.AppendCallback(() => avatar.GetComponent<LevelAvatar>().StartFloatingAnimation());
                sequence.AppendCallback(() => scrollRect.vertical = true);
            }
            else
            {
                avatar.transform.position = targetPos;
                avatar.GetComponent<LevelAvatar>().StartFloatingAnimation();
                scrollRect.vertical = true;
            }
        }
    }
}
