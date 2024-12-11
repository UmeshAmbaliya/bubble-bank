using API_ReceiveData;
using API_SendData;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TournamentDetailScreenScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Text[] winAmountRankText;
    [SerializeField] private Text fourthWinnerAmountText;
    [SerializeField] private Text fourthWinnerRankText;
    [SerializeField] private Text totalPlayersText;
    [SerializeField] private Text entryFeeText;
    [SerializeField] private Color entryValidColor, entryInvalidColor;
    [SerializeField] private Image[] entryIconImage;
    [SerializeField] private Sprite cashSprite;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private GameObject[] rankCoinIconList;
    [SerializeField] private Image playNowButton;
    [SerializeField] private Sprite[] playNowSP;
    [SerializeField] private GameObject tournamentInfoPopUp;
    // Start is called before the first frame update
    public string responseDummy;
    public bool isTestMatch;
    UnlockedTournament data;
    public void SetData(UnlockedTournament data)
    {
        TournamentPlaySendData sendData = new TournamentPlaySendData();
        sendData.tournament_id = data.id;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.getMatchDetailAPI, JsonUtility.ToJson(sendData), true, (success, dataString) =>
        {
            Debug.Log("API : " + APIHandler.Instance.getMatchDetailAPI + "  response : " + dataString); 
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                TournamentDetailReceiveData _data = JsonConvert.DeserializeObject<TournamentDetailReceiveData>(dataString, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    { 
                        this.data = data;
                        titleText.text = data.title;
                        totalPlayersText.text = data.total_players.ToString();
                        if (data.entry_currency_type == 2)
                        {
                            entryFeeText.text = "" + data.entry_fee;
                            if (DataHandler.inst.Coin < data.entry_fee)
                                entryFeeText.color = entryInvalidColor;
                            else
                                entryFeeText.color = entryValidColor;

                            for (int i = 0; i < entryIconImage.Length; i++)
                            {
                                entryIconImage[i].sprite = coinSprite;
                            }
                            playNowButton.sprite = playNowSP[1];
                        }
                        else
                        {
                            if ((DataHandler.inst.Cash + DataHandler.inst.BonusCash) < data.entry_fee)
                                entryFeeText.color = entryInvalidColor;
                            else
                                entryFeeText.color = entryValidColor;
                            entryFeeText.text = "$" + ((float)data.entry_fee / 100);

                            for (int i = 0; i < entryIconImage.Length; i++)
                            {
                                entryIconImage[i].sprite = cashSprite;
                            }
                            playNowButton.sprite = playNowSP[0];
                        }
                        for (int i = 0; i < _data.prize_pool.Count; i++)
                        {
                            if (i < 3)
                            {
                                if (_data.prize_pool[i].currency_type == 2)//coins
                                {
                                    winAmountRankText[i].text = "" + _data.prize_pool[i].amount;
                                }
                                else  //cash || bonus cash
                                {
                                    winAmountRankText[i].text = "$" + ((float)_data.prize_pool[i].amount / 100);
                                }
                            }
                        }

                        if (_data.prize_pool.Count >= 4)
                        {
                            //fourthWinnerAmountText.transform.parent.gameObject.SetActive(true);
                            fourthWinnerAmountText.gameObject.SetActive(true);
                            entryIconImage[1].gameObject.SetActive(true);
                            if (_data.prize_pool[3].currency_type == 2)// coins
                            {
                                fourthWinnerAmountText.text = "" + _data.prize_pool[3].amount;
                            }
                            else
                            {
                                fourthWinnerAmountText.text = "$" + ((float)_data.prize_pool[3].amount / 100);
                            }
                            fourthWinnerRankText.text = "Rank: #4";
                        }
                        else
                        {
                            fourthWinnerAmountText.transform.parent.gameObject.SetActive(false);
                            //fourthWinnerAmountText.gameObject.SetActive(false);
                            //fourthWinnerAmountText.text = "";
                            //entryIconImage[1].gameObject.SetActive(false);
                            //fourthWinnerRankText.text = "Rank:";
                        }
                       
                        if (_data.prize_pool[0].currency_type == 2)
                        {
                            for (int i = 0; i < rankCoinIconList.Length; i++)
                            {
                                rankCoinIconList[i].SetActive(true);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < rankCoinIconList.Length; i++)
                            {
                                rankCoinIconList[i].SetActive(false);
                            }
                        }
                        this.gameObject.SetActive(true);
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(_data.error_message);
                    }
                }
                else
                {
                    Debug.LogError("_Data is null");
                }
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Check your internet connection!");
            }
        });
        
    }

    public void CloseClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip(); 
    }

    public void PlayClick()
    {
        SoundHandler.Instance.PlayButtonClip();
        if (data.entry_currency_type == 2)//coin
        {
            if (DataHandler.inst.Coin < data.entry_fee)
            {
                NotificationHandler.Instance.ShowNotification("Not enough coins!");
                return;
            }
        }
        else         {
            if ((DataHandler.inst.Cash + DataHandler.inst.BonusCash) < data.entry_fee)
            {
                NotificationHandler.Instance.ShowNotification("Not enough cash!");
                return;
            }
        }
        
        TournamentPlaySendData sendData = new TournamentPlaySendData();
        sendData.tournament_id = data.id;
        Debug.Log("Send Data : " + JsonUtility.ToJson(sendData));
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.matchPlayApi, JsonUtility.ToJson(sendData), true, (success, dataString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.matchPlayApi + "  response : " + dataString);
            if(isTestMatch)
                dataString = responseDummy;
            //success = true;
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                dataString = dataString.Replace("null", "-1");
                TournamentPlayReceiveData _data = JsonConvert.DeserializeObject<TournamentPlayReceiveData>(dataString, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        Debug.Log("Load game scene here");
                        //DataHandler.inst.CurrentGameID = _data.tournament_game_id;
                        //if(_data.puzzle == null || _data.puzzle.Count == 0)
                        //    _data.puzzle = _data.generated_puzzle;
                        DataHandler.inst.apiLevelData = _data;
                        DataHandler.inst.isBubbleCollidedWithBottomLine = false;
                        SceneManager.LoadScene("Game");
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(_data.error_message);
                    }
                }
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Check your internet connection");
            }
        });
    }

    public void InfoButtonClick()
    {
        tournamentInfoPopUp.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void CloseTournamentInfoPopUp()
    {
        tournamentInfoPopUp.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
}
