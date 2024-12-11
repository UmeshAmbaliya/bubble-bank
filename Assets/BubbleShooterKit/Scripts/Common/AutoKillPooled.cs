// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class automatically returns the associated pooled object to its
    /// origin pool after a certain amount of time has passed. We use it
    /// for every pooled object in the game.
    /// </summary>
    public class AutoKillPooled : MonoBehaviour
    {
        public float Delay = 2.0f;

        private PooledObject pooledObject;
        private float accTime;

        private void OnEnable()
        {
            accTime = 0.0f;
        }

        private void Start()
        {
            pooledObject = GetComponent<PooledObject>();
        }

        private void Update()
        {
            accTime += Time.deltaTime;
            if (accTime >= Delay)
                pooledObject.Pool.ReturnObject(gameObject);
        }
    }
}
