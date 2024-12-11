// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class manages the background music of the game.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusic : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (PlayerPrefs.HasKey("music_enabled"))
            {
                var musicEnabled = PlayerPrefs.GetInt("music_enabled");
                if (musicEnabled == 0)
                    GetComponent<AudioSource>().mute = true;
            }
        }
    }
}
