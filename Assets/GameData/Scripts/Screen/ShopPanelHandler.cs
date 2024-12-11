using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TransactionType
{
    None,
    Deposit,
    Withdrawl
}
public class ShopPanelHandler : MonoBehaviour
{
    public static ShopPanelHandler Instance;
    [SerializeField] private int[] amountArr;
    [SerializeField] private int[] bonusCashArr;
    [SerializeField] private DepositScreenScript depositScreen;
    [SerializeField] private CardDetailScreenScript cardDetailScreen;
    [SerializeField] private MainTransactionScreenScript transactionScreen;
    [SerializeField] private GameObject contactSupportScreen;
    [HideInInspector] public int currentBuyAmount;
    private void Awake()
    {
        Instance = this;
    }

    public void ShopContactSupportClick()
    {
        contactSupportScreen.SetActive(true);
        Debug.LogError("Open contact support panel here");
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ContactSupportGetClick()
    {
        contactSupportScreen.SetActive(false);
        HomeUIHandler.inst.helpPanelScript.OpenPanel(HomeUIHandler.inst.helpPanelScript.supportObject, "Contact Support");
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ContactSupportCloseClick()
    {
        contactSupportScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void DepositClick(int indexAmount)
    {
        SoundHandler.Instance.PlayButtonClip();
        if (DataHandler.inst.ISRegisteredUser != 0)
        {
            currentBuyAmount = amountArr[indexAmount];
            depositScreen.currentDepositCash = currentBuyAmount * 100;
            depositScreen.currentDepositBonusCash = bonusCashArr[indexAmount] * 100;
            depositScreen.gameObject.SetActive(true);
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Please register to access shop!");
        }
    }

    #region HomeClicks
    public void HomeBalanceClick()
    {
        SoundHandler.Instance.PlayButtonClip();
        if (DataHandler.inst.ISRegisteredUser != 0)
        {
            transactionScreen.WithdrawalClick();
            transactionScreen.gameObject.SetActive(true);
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Please register to access balance!");
        }
    }

    public void HomeTransactionClick()
    {
        SoundHandler.Instance.PlayButtonClip();
        if (DataHandler.inst.ISRegisteredUser != 0)
        {
            transactionScreen.HistoryClick();
            transactionScreen.gameObject.SetActive(true);
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Please register to access transactions!");
        }
    }
    #endregion
}
