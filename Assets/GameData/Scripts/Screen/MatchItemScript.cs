using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using API_ReceiveData;
using API_SendData;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MatchItemScript : MonoBehaviour
{
    public GameObject[] allCoinEntryObjects;
    public GameObject[] allCashEntryObjects;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI[] prisePoolText;
    public TextMeshProUGUI[] entryFeeText;
    public TextMeshProUGUI playerCountText;
    public GameObject lockObject;
    public TextMeshProUGUI lockMatchText;
    public GameObject bonusWinBanner;
    public UnlockedTournament data;
    public Color entryValidColorCoin, entryValidColorCash, entryInvalidColor;
    public void SetData(UnlockedTournament data, bool locked = false)
    {
        this.data = data;
        titleText.text = data.title;
        if (data.entry_currency_type == 1)//cash
        {
            allCashEntryObjects[1].SetActive(true);
            entryFeeText[0].text = "$" + ((float)data.entry_fee / 100);
            int totalCash = (DataHandler.inst.Cash + DataHandler.inst.BonusCash);
            if (totalCash >= data.entry_fee)
            {
                entryFeeText[0].color = entryValidColorCash;
            }
            else
            {
                entryFeeText[0].color = entryInvalidColor;
            }

        }
        else if (data.entry_currency_type == 2)// coins
        {
            allCoinEntryObjects[1].SetActive(true);
            entryFeeText[1].text = "" + data.entry_fee;
            if (DataHandler.inst.Coin >= data.entry_fee)
            {
                entryFeeText[1].color = entryValidColorCoin;
            }
            else
            {
                entryFeeText[1].color = entryInvalidColor;
            }
        }
        else
        {
            allCashEntryObjects[1].SetActive(true);
            entryFeeText[0].text = "$" + ((float)data.entry_fee / 100);
            int totalCash = (DataHandler.inst.Cash + DataHandler.inst.BonusCash);
            if (totalCash >= data.entry_fee)
            {
                entryFeeText[0].color = entryValidColorCash;
            }
            else
            {
                entryFeeText[0].color = entryInvalidColor;
            }
        }
        playerCountText.text = data.total_players+" PLAYERS";
        if (locked)
        {
            lockMatchText.text = "Unlock at\r\nlevel " + data.level_req;
            lockObject.SetActive(true);
        }
        if (data.bonus_cash > 0)
        {
            bonusWinBanner.SetActive(true);
            bonusWinBanner.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "WIN UP TO $" + ((float)data.bonus_cash/100) + " BONUS CASH!";
            this.GetComponent<LayoutElement>().minHeight = 314;
        }

        if (data.prize_currency_type == 1)//cash
        {
            allCashEntryObjects[0].SetActive(true);
            prisePoolText[0].text = "$" + ((float)data.prize_total / 100);
        }
        else if (data.prize_currency_type == 2)//coin
        {
            allCoinEntryObjects[0].SetActive(true);
            prisePoolText[1].text = "" + data.prize_total;
        }
        else
        {
            allCashEntryObjects[0].SetActive(true);
            prisePoolText[0].text = "$" + ((float)data.prize_total / 100);
        }
    }

    public void OnClickPlay()
    {
        HomeUIHandler.inst.tournamentDetailScreenScript.SetData(data);
        SoundHandler.Instance.PlayButtonClip();
        //return;
        //TournamentPlaySendData sendData = new TournamentPlaySendData();
        //sendData.tournament_id = data.id;
        //APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.matchPlayApi, JsonUtility.ToJson(sendData), true, (success, dataString) =>
        //{
        //    Debug.LogError("API : " + APIHandler.Instance.matchPlayApi + "  response : " + dataString);
        //    if (success)
        //    {
        //        var settings = new JsonSerializerSettings
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //            MissingMemberHandling = MissingMemberHandling.Ignore
        //        };
        //        TournamentPlayReceiveData _data = JsonConvert.DeserializeObject<TournamentPlayReceiveData>(dataString, settings);
        //        if (_data != null)
        //        {
        //            if (_data.http_code == 200)
        //            {
        //                Debug.LogError("Load game scene here");
        //                DataHandler.inst.CurrentGameID = _data.tournament_game_id;
        //                DataHandler.inst.apiLevelData = _data;
        //                SceneManager.LoadScene("Game");
        //            }
        //            else
        //            {
        //                NotificationHandler.Instance.ShowNotification(_data.error_message);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        NotificationHandler.Instance.ShowNotification("Check your internet connection");
        //    }
        //});
    }

    public void InfoButtonClick()
    {
        Debug.LogError("Info button clicked");
        HomeUIHandler.inst.infoPanel.ShowPopUp("BONUS CASH", "Bonus cash can be used to\r\nparticipate in tournaments, but\r\ncannot be withdrawn.\r\nwinnings earned by using bonus\r\ncash can be withdrawn.");
        SoundHandler.Instance.PlayButtonClip();
    }
}
