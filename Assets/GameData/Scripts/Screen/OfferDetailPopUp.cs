using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OfferDetailPopUp : MonoBehaviour
{
    public TextMeshProUGUI offerDetailTitleText;
    public TextMeshProUGUI offerDetailCashText;
    public TextMeshProUGUI offerDetailBonusCashText;
    public TextMeshProUGUI offerDetailCoinText;
    public TextMeshProUGUI offerDetailRemainTimerText;
    public TextMeshProUGUI offerDetailBuyTextText;
    Coroutine timerCoRef;

    public bool isExpired = false;
    API_ReceiveData.OfferDetailReceiveData data;
    public void SetData(API_ReceiveData.OfferDetailReceiveData data)
    {
        this.data = data;
        DateTime dt = data.offer.created_at;
        DateTime dtNow = DateTime.Now.ToUniversalTime();
        TimeSpan timeRemain = new TimeSpan(24,0,0) - (dtNow - dt);
        if (timeRemain.TotalSeconds<=0)
        {
            isExpired = true;
            Debug.LogError("Offer is expired : "+data.offer.id);
        }
        else
        {
            offerDetailTitleText.text = "" + data.offer.title;
            offerDetailCashText.text = "$" + data.offer.bonus_cash.ToString();
            offerDetailBonusCashText.text = "$" + data.offer.bonus_cash.ToString();
            offerDetailCoinText.text = data.offer.coins.ToString();
            offerDetailCoinText.text = data.offer.coins.ToString();
            offerDetailBuyTextText.text = "BUY FOR $"+(data.offer.deposit_amount/100).ToString();
            if (timerCoRef!=null)
            {
                StopCoroutine(timerCoRef);
            }
            timerCoRef = StartCoroutine(StartTimer(timeRemain));
            this.gameObject.SetActive(true);
        }

    }
    IEnumerator StartTimer(TimeSpan remainTime)
    {
        while (remainTime.TotalSeconds>0)
        {
            yield return new WaitForSeconds(1);
            remainTime.Add(new TimeSpan(0, 0, -1));
            string timer = string.Format("{0:00}:{1:00}:{2:00}", remainTime.Hours, remainTime.Minutes, remainTime.Seconds);
            offerDetailRemainTimerText.text ="Time remain : " + timer;
        }
    }

    public void BuyClick()
    {
        API_SendData.PaymentOfferClaimSendData sendData = new API_SendData.PaymentOfferClaimSendData();
        sendData.offer_id = data.offer.id;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.offerClaimDetailAPI, JsonUtility.ToJson(sendData), true, (success, data_received) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.offerClaimDetailAPI + "  response : " + data_received);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.PaymentOfferClaimDetailReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.PaymentOfferClaimDetailReceiveData>(data_received, settings);
                if (data != null)
                {
                    if (data.http_code == 200)
                    {
                        //offerDetailScreen.gameObject.SetActive(true);
                        HomeUIHandler.inst.SilentGetUserData();
                        //HomeUIHandler.inst.depositNowScreen.SetActive(true);
                        this.gameObject.SetActive(false);
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(data.error_message);
                    }
                }
                else
                {
                    NotificationHandler.Instance.ShowNotification("Error while parsing data");
                }
            }
        });
        SoundHandler.Instance.PlayButtonClip();
    }

    public void CloseClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
}
