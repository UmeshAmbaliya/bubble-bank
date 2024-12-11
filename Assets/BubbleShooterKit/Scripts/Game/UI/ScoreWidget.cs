// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterKit
{
    public class ScoreWidget : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText = null;
        [SerializeField]
        private Image progressBarImage = null;
        [SerializeField]
        private ScoreStarWidget star1Image = null;
        [SerializeField]
        private ScoreStarWidget star2Image = null;
        [SerializeField]
        private ScoreStarWidget star3Image = null;
        [SerializeField]
        private ParticleSystem star1Particles = null; 
        [SerializeField]
        private ParticleSystem star2Particles = null; 
        [SerializeField]
        private ParticleSystem star3Particles = null; 
        
        private int star1;
        private int star2;
        private int star3;

        private bool star1Achieved;
        private bool star2Achieved;
        private bool star3Achieved;

        public void Fill(int score1, int score2, int score3)
        {
            progressBarImage.fillAmount = 0;

            star1 = score1;
            star2 = score2;
            star3 = score3;
            
            star1Image.Deactivate();
            star2Image.Deactivate();
            star3Image.Deactivate();

            UpdateProgressBar(0);

            star1Achieved = false;
            star2Achieved = false;
            star3Achieved = false;
        }

        public void UpdateProgressBar(int score)
        {
            scoreText.text = score.ToString();
            progressBarImage.fillAmount = GetProgressValue(score) / 100.0f;

            if (score >= star1 && !star1Achieved)
            {
                star1Achieved = true;
                star1Image.Activate();
                star1Image.GetComponent<Animator>().SetTrigger("Achieved");
                star1Particles.Play();
                SoundPlayer.PlaySoundFx("StarProgressBar");
            }
            if (score >= star2 && !star2Achieved)
            {
                star2Achieved = true;
                star2Image.Activate();
                star2Image.GetComponent<Animator>().SetTrigger("Achieved");
                star2Particles.Play();
                SoundPlayer.PlaySoundFx("StarProgressBar");
            }
            if (score >= star3 && !star3Achieved)
            {
                star3Achieved = true;
                star3Image.Activate();
                star3Image.GetComponent<Animator>().SetTrigger("Achieved");
                star3Particles.Play();
                SoundPlayer.PlaySoundFx("StarProgressBar");
            }

            var offset = 165.0f;
            star1Image.transform.localPosition = progressBarImage.transform.localPosition +
                                                 new Vector3(
                                                     progressBarImage.rectTransform.rect.width *
                                                     (GetProgressValue(star1) / 100.0f) - offset, 0, 0);
            star2Image.transform.localPosition = progressBarImage.transform.localPosition +
                                                 new Vector3(
                                                     progressBarImage.rectTransform.rect.width *
                                                     (GetProgressValue(star2) / 100.0f) - offset, 0, 0);
            star3Image.transform.localPosition = progressBarImage.transform.localPosition +
                                                 new Vector3(progressBarImage.rectTransform.rect.width - offset, 0, 0);
        }

        private int GetProgressValue(int value)
        {
            const int oldMin = 0;
            var oldMax = star3;
            const int newMin = 0;
            const int newMax = 100;
            var oldRange = oldMax - oldMin;
            const int newRange = newMax - newMin;
            var newValue = (((value - oldMin) * newRange) / oldRange) + newMin;
            return newValue;
        }
    }
}
