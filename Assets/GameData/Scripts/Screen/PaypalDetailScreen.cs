using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaypalDetailScreen : MonoBehaviour
{
    public TMP_InputField paypalEmailInput;
    public TransactionType type;
    public void CloseClick()
    {
        HomeUIHandler.inst.currentDepositCloseCount += 1;
        if (HomeUIHandler.inst.currentDepositCloseCount % 3 == 0)
        {
            HomeUIHandler.inst.depositNowScreen.SetActive(true);
        }
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ConfirmClick()
    {
        string paypal = paypalEmailInput.text;
        paypal = paypal.Replace(" ", "");
        if (HomeUIHandler.inst.validateEmail(paypal))
        {
            if (type == TransactionType.Deposit)
            {
                API_SendData.PaymentDepositeSendData sendData = new API_SendData.PaymentDepositeSendData();
                sendData.paypal = paypal;
                sendData.deposit_amount = ShopPanelHandler.Instance.currentBuyAmount * 100;
                APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.depositeAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
                {
                    Debug.LogError("API : " + APIHandler.Instance.depositeAPI + "  response : " + jsonString);
                    if (success)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        try
                        {
                            API_ReceiveData.DepositReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.DepositReceiveData>(jsonString, settings);
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
                            Debug.LogError("Error while parsing the Deposit receive data : " + jsonString);
                            throw;
                        }
                    }
                });
            }
            else if(type == TransactionType.Withdrawl)
            {

            }
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Invalid email!");
        }

        SoundHandler.Instance.PlayButtonClip();
    }
}
