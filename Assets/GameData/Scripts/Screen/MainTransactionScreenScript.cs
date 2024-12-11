using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainTransactionScreenScript : MonoBehaviour
{
    [Header("Screen references")]
    [SerializeField] PaypalDetailScreen paypalScreen;
    [SerializeField]
    private Image historyBg;
    [SerializeField] 
    private Image withdrawalBg;
    [SerializeField]
    private Sprite selectedSprite;
    [SerializeField] 
    private Sprite deSelectSprite;

    [SerializeField]
    private GameObject withdrawalObj;
    [SerializeField]
    private GameObject transactionObj;
    public Color headerSelectColor, headerDeSelectColor;
    [Header("Wihdrawal References")]
    [SerializeField]
    private TextMeshProUGUI wihdrawalAccountBalanceText;
    [SerializeField]
    private TextMeshProUGUI wihdrawalWithdrawableText;
    [SerializeField]
    private TMP_InputField wihdrawalAmountOfWithdrawalText;

    [Header("History References")]
    public GameObject historyPrefab;
    public Transform historyContainer;
    public GameObject noHistoryObj;
    public void HistoryClick()
    {
        noHistoryObj.SetActive(false);
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.transactionHistoryAPI, "{}", true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.transactionHistoryAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.PaymentWithdrawalReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.PaymentWithdrawalReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {
                            noHistoryObj.SetActive(true);
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(data.error_message);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error while parsing the data leaderboard TournamentLeaderboardReceiveData " + jsonString);
                    throw;
                }
            }
        });
        historyBg.sprite = selectedSprite;
        withdrawalBg.sprite = deSelectSprite;
        historyBg.transform.localScale = Vector3.one * -1;
        historyBg.transform.GetChild(0).localScale = Vector3.one * -1;
        withdrawalBg.transform.localScale = Vector3.one * -1;
        withdrawalBg.transform.GetChild(0).localScale = Vector3.one * -1;
        withdrawalBg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = headerSelectColor;
        historyBg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = headerDeSelectColor;

        withdrawalObj.SetActive(false);
        transactionObj.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }
    
    public void WithdrawalClick()
    {
        int totalBalance = DataHandler.inst.Cash + DataHandler.inst.BonusCash;

        wihdrawalAccountBalanceText.text = "$" + ((float)totalBalance / 100).ToString();
        wihdrawalWithdrawableText.text = "$" + ((float)DataHandler.inst.Cash / 100).ToString();
        wihdrawalAmountOfWithdrawalText.text = "$" + ((float)DataHandler.inst.Cash / 100).ToString();

        historyBg.sprite = deSelectSprite;
        withdrawalBg.sprite = selectedSprite;
        historyBg.transform.localScale = Vector3.one;
        historyBg.transform.GetChild(0).localScale = Vector3.one;
        withdrawalBg.transform.localScale = Vector3.one;
        withdrawalBg.transform.GetChild(0).localScale = Vector3.one;
        withdrawalBg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = headerDeSelectColor;
        historyBg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = headerSelectColor;
        transactionObj.SetActive(false);
        withdrawalObj.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }
     
    public void WithdrawAmountEndCheck(string str)
    {
        Debug.LogError(str);
        str = str.Replace("$", "");
        int withdrawableCash = (int)((float)DataHandler.inst.Cash / 100);
        int i = int.Parse(str);
        if (i >= withdrawableCash)
        {
            NotificationHandler.Instance.ShowNotification("You can withdraw max $" + withdrawableCash + " cash!");
            wihdrawalAmountOfWithdrawalText.text = "$" + withdrawableCash;
        }
        else
        {
            wihdrawalAmountOfWithdrawalText.text = "$" + i;
        }
    }

    public void WithdrawalSubmitClick()
    {
        paypalScreen.type = TransactionType.Withdrawl;
        paypalScreen.gameObject.SetActive(true);
        API_SendData.PaymentWithdrawalSendData sendData = new API_SendData.PaymentWithdrawalSendData();
        sendData.paypal_email = "";
        sendData.withdrawal_amount = 0;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.withdrawalAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.withdrawalAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.PaymentWithdrawalReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.PaymentWithdrawalReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {

                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(data.error_message);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error while parsing the data leaderboard TournamentLeaderboardReceiveData " + jsonString);
                    throw;
                }
            }
        });
        SoundHandler.Instance.PlayButtonClip();
    }

    public void BackClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void WithdrawlNeedHelpClick()
    {
        HomeUIHandler.inst.helpPanelScript.OpenPanel(HomeUIHandler.inst.helpPanelScript.supportObject, "Support");
        SoundHandler.Instance.PlayButtonClip();
    }
}
