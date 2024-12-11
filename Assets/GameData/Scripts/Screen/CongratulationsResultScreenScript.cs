using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CongratulationsResultScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject cashObj;
    [SerializeField] private GameObject coinObj;
    [SerializeField] private Text coinText;
    [SerializeField] private TextMeshProUGUI cashText;
    int coinAmount;
    public void ShowPanel(int cash, int coins, bool isBonusCash = false)
    {
        if (cash != 0)
        {
            if (isBonusCash)
            {
                DataHandler.inst.BonusCash += cash;
            }
            else
            {
                DataHandler.inst.Cash += cash;
            }
            coinAmount = 0;
            cashObj.SetActive(true);
            coinObj.SetActive(false);
            cashText.text = "$" + ((float)cash / 100);
          
        }
        else
        {
            DataHandler.inst.Coin += coins;
            coinAmount = coins;
            cashObj.SetActive(false);
            coinObj.SetActive(true);
            coinText.text = "" + coins;
        }
        this.gameObject.SetActive(true);
    }

    public void CollectClick()
    {
        if (coinAmount != 0)
        {
            CoinVFXHandler.Instance.StartMoveCoin();
        }
        else
        {
            HomeUIHandler.inst.UpdateCoinCashText();
        }
        HomeUIHandler.inst.SilentGetUserData();
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip(); 
    }
}
