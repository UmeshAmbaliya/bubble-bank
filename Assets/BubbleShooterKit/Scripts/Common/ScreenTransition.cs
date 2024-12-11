// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// Helper component to transition from one scene to another.
    /// </summary>
    public class ScreenTransition : MonoBehaviour
    {
        public string Scene = "<Insert scene name>";
        public float Duration = 1.0f;
        public Color Color = Color.black;

        public void PerformTransition()
        {
            Transition.LoadLevel(Scene, Duration, Color);
        }
    }
}

