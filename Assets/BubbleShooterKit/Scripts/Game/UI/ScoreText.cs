// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// The class associated to the score text prefab that is displayed under a bubble
    /// when it explodes.
    /// </summary>
    public class ScoreText : MonoBehaviour
    {
        [SerializeField]
        private float textDuration = 1.0f;

        [SerializeField]
        private float fadeDuration = 0.2f;

        [SerializeField]
        private TextMeshPro scoreText = null;
        
        [SerializeField]
        private TextMeshPro scoreTextBorder = null;
     
        private float accTime;

        public void Initialize(int score)
        {
            scoreText.text = score.ToString();
            scoreTextBorder.text = scoreText.text;
        }

        private void OnEnable()
        {
            scoreText.alpha = 1.0f;
            scoreTextBorder.alpha = 1.0f;
        }

        private void Update()
        {
            accTime += Time.deltaTime;
            if (accTime >= textDuration)
            {
                accTime = 0.0f;
                var seq = DOTween.Sequence();
                scoreText.DOFade(0.0f, fadeDuration);
                seq.Append(scoreTextBorder.DOFade(0.0f, fadeDuration));
                seq.AppendCallback(() => GetComponent<PooledObject>().Pool.ReturnObject(gameObject));
            }
        }
    }
}
