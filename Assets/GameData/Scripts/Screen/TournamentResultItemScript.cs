using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentResultItemScript : MonoBehaviour
{
    API_ReceiveData.Result data;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI totalPlayerText;
    [SerializeField] private TextMeshProUGUI lastRefreshTimeText;
    [SerializeField] private TextMeshProUGUI priseText;
    [SerializeField] private GameObject priseCoinObj;
    [SerializeField] private GameObject priseCashObj;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankImage;
    [SerializeField] private GameObject notRankObj;
    [SerializeField] private GameObject collectButton;
    [SerializeField] private GameObject winObj;
    [SerializeField] private GameObject notWinObj;

    public void SetData(API_ReceiveData.Result data)
    {
        //Debug.LogError(JsonUtility.ToJson(data));
        this.data = data;
        titleText.text = data.title;
        if (data.total_players != 0)
        {
            totalPlayerText.text = data.total_players+" Players";
        }
        lastRefreshTimeText.text = "0 mins ago";

        if (data.rank != 0)
        {
            winObj.SetActive(true);
            notWinObj.SetActive(false);
            if (data.currency_type == 2)// coin
            {
                priseText.text = data.amount.ToString();
                priseCoinObj.SetActive(true);
            }
            else// cash or bonus cash
            {
                priseText.text = "$"+((float)data.amount/100);
                priseCashObj.SetActive(true);
            }
           
            if (data.rank <= 3)
            {
                rankImage.gameObject.SetActive(true);
                notRankObj.SetActive(false);
                rankImage.sprite = TournamentScreenHandler.instance.resultRankSprites[data.rank - 1];
            }
            else
            {
                rankImage.gameObject.SetActive(false);
                notRankObj.SetActive(true);
                rankText.text = "" + data.rank;
                notWinObj.GetComponent<TextMeshProUGUI>().text = "Better luck\r\nnext time";
            }
            collectButton.SetActive(data.rank <= 3 && data.reward_collected == 1);
        }
        else
        {
            rankImage.gameObject.SetActive(false);
            notRankObj.SetActive(true);
            notWinObj.GetComponent<TextMeshProUGUI>().text = "Pending..."; 
            collectButton.SetActive(false);
            notWinObj.SetActive(true);
            winObj.SetActive(false);
            rankText.text = "";
        } 
    }

    public void OnClickCollect()
    {
        if (data.rank != 0 && data.rank <= 3)
        {
            API_SendData.ResultsSendData sendData = new API_SendData.ResultsSendData();
            sendData.user_prize_id = data.id;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.resultCollectAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.resultCollectAPI + "  response : " + jsonString);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    try
                    {
                        API_ReceiveData.ResultCollectReceiveData dataParsed = JsonConvert.DeserializeObject<API_ReceiveData.ResultCollectReceiveData>(jsonString, settings);
                        if (dataParsed != null)
                        {
                            if (dataParsed.http_code == 200)
                            {
                                if (data.currency_type == 1)// cash
                                {
                                    HomeUIHandler.inst.congratulationsResultScreenScript.ShowPanel(data.amount,0);
                                }
                                else if (data.currency_type == 2)//coins
                                {
                                    HomeUIHandler.inst.congratulationsResultScreenScript.ShowPanel(0, data.amount);
                                }
                                else if (data.currency_type == 3)// bonus cash
                                {
                                    HomeUIHandler.inst.congratulationsResultScreenScript.ShowPanel(data.bonus_amount, 0,true);
                                }
                                else
                                {
                                    NotificationHandler.Instance.ShowNotification("Currency type is : " + data.currency_type);
                                }
                                Debug.LogError("Collected successfully for game id :" + data.id); 
                            }
                            else
                            {
                                NotificationHandler.Instance.ShowNotification(dataParsed.error_message);
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
        }
        else
        {
            if (data.rank == 0)
            {
                // show result pending screen here
            }
            NotificationHandler.Instance.ShowNotification("Tournament id is null!");
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OnClickLeaderboard()
    {
        if (data.id != 0 )
        {
            API_SendData.TournamentLeaderboardSendData sendData = new API_SendData.TournamentLeaderboardSendData();
            sendData.user_game_id = data.id;
            Debug.LogError("Send data : " + JsonUtility.ToJson(sendData));
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.tournamantLeaderboardAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.tournamantLeaderboardAPI + "  response : " + jsonString);
                if (success)
                {
                    TournamentScreenHandler.instance.leaderboardPanelScript.ShowLeeaderboard(jsonString,data);
                }
                else
                {
                    NotificationHandler.Instance.ShowNotification("Check your internet connection!");
                }
            });
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Tournament id is null!");
        }
        SoundHandler.Instance.PlayButtonClip();
    }
}
