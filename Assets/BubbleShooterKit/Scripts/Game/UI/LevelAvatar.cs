// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class manages the player avatar that is displayed on the level scene.
    /// </summary>
    public class LevelAvatar : MonoBehaviour
    {
        private bool floating;
        private float runningTime;

        private void Update()
        {
            if (!floating)
                return;

            var deltaHeight = Mathf.Sin(runningTime + Time.deltaTime);
            var newPos = transform.position;
            newPos.y += deltaHeight * 0.002f;
            transform.position = newPos;
            runningTime += Time.deltaTime * 2;
        }

        public void StartFloatingAnimation()
        {
            floating = true;
        }
    }
}
