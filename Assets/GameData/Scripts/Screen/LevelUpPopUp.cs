using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopUp : MonoBehaviour
{
    public Text levelText;
    public GameObject coinRewardObj;
    public GameObject cashRewardObj;
    public GameObject xpRewardObj;

    public Text coinText;
    public Text cashText;
    public Text xpText;

    int coinReward = 0;
    int cashReward = 0;
    int xpReward = 0;

    public void SetData(List<API_ReceiveData.DailyRewardItem> allItems)
    {
        coinRewardObj.SetActive(false);
        cashRewardObj.SetActive(false);
        xpRewardObj.SetActive(false);
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i].reward_type == 1)
            {
                coinRewardObj.SetActive(true);
                coinText.text = "+" + allItems[i].reward_amount;
                coinReward = allItems[i].reward_amount;
            }
            else if (allItems[i].reward_type == 2)
            {
                cashRewardObj.SetActive(true);
                cashText.text = "$" + (allItems[i].reward_amount / 100);
                cashReward = allItems[i].reward_amount;
            }
            else
            {
                xpRewardObj.SetActive(true);
                xpText.text = "" + allItems[i].reward_amount;
                xpReward = allItems[i].reward_amount;
            }
        }
    }

    public void ClaimClick()
    {
        if(cashRewardObj.activeSelf)
        {
            DataHandler.inst.Cash += cashReward;
            HomeUIHandler.inst.UpdateCoinCashText();
        }
        if (coinRewardObj.activeSelf)
        {
            DataHandler.inst.Coin += coinReward;
            CoinVFXHandler.Instance.StartMoveCoin();
        } 
        if (xpRewardObj.activeSelf)
        {
            HomeUIHandler.inst.AddXP(xpReward);
        }
        //DailyRewardHandler.Instance.CheckForDailyReward();
        HomeUIHandler.inst.SilentGetUserData();
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
        DailyRewardHandler.Instance.Start24HourTimer();
    }
}
