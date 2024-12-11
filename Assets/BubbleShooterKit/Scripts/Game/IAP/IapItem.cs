// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace BubbleShooterKit
{
    /// <summary>
    /// The available coin icons.
    /// </summary>
    public enum CoinIcon
    {
        VerySmall,
        Small,
        MediumSmall,
        MediumLarge,
        Large,
        VeryLarge
    }

    /// <summary>
    /// An in-app purchasable item.
    /// </summary>
    [Serializable]
    public class IapItem
    {
        public string StoreId;
        public int NumCoins;
        public int Discount;
        public bool MostPopular;
        public bool BestValue;
        public CoinIcon CoinIcon;

#if UNITY_EDITOR
        /// <summary>
        /// Draws the IAP item.
        /// </summary>
        public void Draw()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Store id");
            StoreId = EditorGUILayout.TextField(StoreId, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Coins");
            NumCoins = EditorGUILayout.IntField(NumCoins, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Discount");
            Discount = EditorGUILayout.IntField(Discount, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Most popular");
            MostPopular = EditorGUILayout.Toggle(MostPopular);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Best value");
            BestValue = EditorGUILayout.Toggle(BestValue);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Coin icon");
            CoinIcon = (CoinIcon)EditorGUILayout.EnumPopup(CoinIcon, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
#endif
    }
}
