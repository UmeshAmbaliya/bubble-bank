// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Assertions;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class contains the logic associated to the popup for buying coins.
    /// </summary>
    public class BuyCoinsPopup : Popup
    {
        public GameConfiguration GameConfig;

        public CoinsSystem CoinsSystem;
        
        [SerializeField]
        private GameObject purchasableItems = null;

        [SerializeField]
        private GameObject purchasableItemPrefab = null;

        private PurchasableItem currentPurchasableItem;
        private Popup loadingPopup;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(purchasableItems);
            Assert.IsNotNull(purchasableItemPrefab);
        }

        protected override void Start()
        {
            base.Start();
            CoinsSystem.Subscribe(OnCoinsChanged);

            foreach (var item in GameConfig.IapItems)
            {
                var row = Instantiate(purchasableItemPrefab);
                row.transform.SetParent(purchasableItems.transform, false);
                row.GetComponent<PurchasableItem>().Fill(item);
                row.GetComponent<PurchasableItem>().BuyCoinsPopup = this;
            }
        }

        private void OnDestroy()
        {
            CoinsSystem.Unsubscribe(OnCoinsChanged);
        }

        public void OnBuyButtonPressed(int numCoins)
        {
            CoinsSystem.BuyCoins(numCoins);
        }

        public void OnCloseButtonPressed()
        {
            Close();
        }

        private void OnCoinsChanged(int numCoins)
        {
            if (currentPurchasableItem != null)
                currentPurchasableItem.PlayCoinParticles();
            GetComponent<PlaySound>().Play("CoinsPopButton");
        }

        public void OpenLoadingPopup()
        {
            #if UNITY_IOS
            ParentScreen.OpenPopup<LoadingPopup>("Popups/LoadingPopup",
                popup => { loadingPopup = popup; });
            #endif
        }

        public void CloseLoadingPopup()
        {
            #if UNITY_IOS
            if (loadingPopup != null)
            {
                loadingPopup.Close();
            }
            #endif
        }

        public void SetCurrentPurchasableItem(PurchasableItem item)
        {
            currentPurchasableItem = item;
        }
    }
}
