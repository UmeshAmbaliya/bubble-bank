using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections;
using Unity.VisualScripting;

public class HomeUIHandler : MonoBehaviour
{
    public static HomeUIHandler inst;
    
    private void Awake()
    {
        if (inst != null)
            Destroy(inst);
        inst = this;
    }
    [Header("All profile references")]
    public Image avatarSelectionProfileImage;
    public Image[] userProfileImages;
    public TextMeshProUGUI[] userNameTexts;
    public Text[] userNameUITexts;
    public TextMeshProUGUI[] allUserIdText;
    public Text[] allUserIdUIText;

    [Header("All screen references")]
    public AvatarSelectionScript avatarSelectionScript;
    public ExistingUserLoginScript existingUserLoginScript;
    public RegistrationScript registrationScript;
    public OfferDetailPopUp offerDetailScreen;
    public HelpPanelScript helpPanelScript;
    public LevelUpPopUp levelUpPanel;
    public LeaderboardPanelScript leaderboardPanelScript;
    public CongratulationsResultScreenScript congratulationsResultScreenScript;
    public HowToPlayScript howToPlayScript;
    public TournamentDetailScreenScript tournamentDetailScreenScript;
    public CustomFooter footerScript;
    public HavingFunScreenScript havingFunScreenScript;
    public InfoScreenScript infoPanel;
    public GameObject settingScreen;
    public GameObject tutorialPlayScreen;
    public GameObject depositNowScreen;
    public GameObject tournamentRulesScreen;
    public GameObject joinPreviousGameScreen;
 
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject addCashScreen;
    [SerializeField] private GameObject addCoinScreen;


    [Header("Home References")]
    [SerializeField] private TextMeshProUGUI homeBonusCashText;
    [SerializeField] private TextMeshProUGUI homeAvailableWithdrawalCashText;
    [SerializeField] private TextMeshProUGUI homeBalanceText;
    public GameObject[] homeRegisterButtons;
    public GameObject matchesPrefab;
    public Transform matchesContainer;

    [Header("HeaderReferences")]
    [SerializeField] private TextMeshProUGUI headerLevelText;
    [SerializeField] private TextMeshProUGUI headerCoinText;
    [SerializeField] private TextMeshProUGUI headerCashText;
    [SerializeField] private Image headerXpFillImage;
    [SerializeField] private TextMeshProUGUI headerXpPercentageText;

    [Header("Message Popup References")]
    public GameObject messagePopUp;
    public TextMeshProUGUI messgaeTitleText;
    public TextMeshProUGUI messageContentText;

    [Header("Tutorial Screen References")]
    public GameObject tutorialLevelScreen;
    public GameObject tutorialLevel1Obj,tutorialLevel2Obj, tutorialLevel3Obj;
    GameObject tutorialMatchObj;

    // EXTRA VARIABLES
    int previousUserGameId = 0;
    public int currentDepositCloseCount = 0;

    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    public bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }
    private void Start()
    {
        currentDepositCloseCount = 0;
        foreach (Transform item in matchesContainer)
        {
            Destroy(item.gameObject);
        }
        //DataHandler.inst.isFirstTimeLoad = false;
        if (DataHandler.inst.isFirstTimeLoad)
        {
            if (!string.IsNullOrEmpty(DataHandler.inst.LoginToken))
            {
                Debug.LogError("Token : " + DataHandler.inst.LoginToken);
                APIGetUserInfo(false);
                //if (DataHandler.inst.CurrentGameID!= -1)
                //{
                //    CheckMatchAPICall(DataHandler.inst.CurrentGameID);
                //    //check for game running or not.
                //}
            }
            else
            {
                SplashHandler.instance.isLoadSuccess = true;
                avatarSelectionScript.EnablePanel(true);
            }
        }
        else
        {
            //DataHandler.inst.CurrentGameID = -1;
            SilentGetUserData(true,true);
            //DataHandler.inst.isTutorialLevelComplete = true;
            if (DataHandler.inst.isTutorialLevelComplete)
            {
                footerScript.snap.UseSwipeGestures = false;
                tutorialLevelScreen.SetActive(true);
                tutorialLevel1Obj.SetActive(true); 
                tutorialLevel2Obj.SetActive(false);
                tutorialLevel3Obj.SetActive(false);
                //SplashHandler.instance.isLoadSuccess = true;
            }
            else if (DataHandler.inst.isComeAfterWin)
            {
                Debug.LogError("User cam back to home after win");
                DataHandler.inst.isComeAfterWin = false;
                ShowRateUsPanel();
            }
            StartCoroutine(CheckGameRunning(false,true));
        } 
    }

    void ShowRateUsPanel()
    {
        DataHandler.inst.MatchCompleteCount += 1;
        if (DataHandler.inst.MatchCompleteCount % 3 == 0 && DataHandler.inst.RateUsGiven == 0)
        {
            havingFunScreenScript.gameObject.SetActive(true);
        }
    }

    #region Join Previous game clicks
    IEnumerator CheckGameRunning(bool isNeedToShowMatchStartPopup, bool isNeedToEndGame)
    {
        yield return new WaitForEndOfFrame();
        int currentGameId = 0;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.gameCheckAPI, "{}", true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.gameCheckAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.CheckGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.CheckGameReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        currentGameId = _data.user_game_id;
                        previousUserGameId = currentGameId;
                        if (_data.complete != 0)//&& !isNeedToShowMatchStartPopup)// match is already completed
                        {
                            if (!isNeedToShowMatchStartPopup)
                            {
                                isNeedToEndGame = true;
                            }
                            else
                            {
                                //joinPreviousGameScreen.SetActive(true);
                                Debug.LogError("Show running game popup here");
                            }
                        } 
                        isNeedToEndGame = true;
                    }
                    else
                    {
                        Debug.LogError("game_check start: " + _data.error_message);
                        //NotificationHandler.Instance.ShowNotification("game_check start: " + _data.error_message);
                    }
                }
                if (isNeedToEndGame)
                {
                    API_SendData.EndGameSendData sendData = new API_SendData.EndGameSendData();
                    sendData.user_game_id = currentGameId;
                    APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.endGameAPI, JsonUtility.ToJson(sendData), true, (success, data) =>
                    {
                        Debug.LogError("API : " + APIHandler.Instance.endGameAPI + "  response : " + data);
                        if (success)
                        {
                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            API_ReceiveData.EndGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.EndGameReceiveData>(data, settings);
                            if (_data != null)
                            {
                                if (_data.http_code == 200)
                                {
                                    Debug.LogError("Game end successfull");
                                }
                                else
                                {
                                    Debug.LogError("End game start :" + _data.error_message);
                                    //NotificationHandler.Instance.ShowNotification("End game start :"+_data.error_message);
                                }
                            }
                        }
                    }, false);
                }
            }
        }, false);
        
    }
    public void PreviousGameContinueClick()
    {
        API_SendData.EndGameSendData sendData = new API_SendData.EndGameSendData();
        sendData.user_game_id = previousUserGameId;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.endGameAPI, JsonUtility.ToJson(sendData), true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.endGameAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.EndGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.EndGameReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        Debug.LogError("Game end successfull");
                    }
                    else
                    {
                        Debug.LogError("End game error :" + _data.error_message);
                        NotificationHandler.Instance.ShowNotification("End game error :" + _data.error_message);
                    }
                    joinPreviousGameScreen.SetActive(false);
                }
            }
        }, false);
    }

    public void PreviousGameQuitClick()
    {
        API_SendData.EndGameSendData sendData = new API_SendData.EndGameSendData();
        sendData.user_game_id = previousUserGameId;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.endGameAPI, JsonUtility.ToJson(sendData), true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.endGameAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.EndGameReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.EndGameReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        Debug.LogError("Game end successfull");
                    }
                    else
                    {
                        Debug.LogError("End game error :" + _data.error_message);
                        NotificationHandler.Instance.ShowNotification("End game error :" + _data.error_message);
                    }
                    joinPreviousGameScreen.SetActive(false);
                }
            }
        }, false);
    }

    #endregion

    #region Home Clicks
    public void HomeRegisterClick()
    {
        PermissionHandler.instance.CheckForLocationPermit((status) => 
        {
            registrationScript.gameObject.SetActive(true);
        });
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HomeSignInClick()
    {
        existingUserLoginScript.backScreen = "home";
        existingUserLoginScript.gameObject.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HomeEditProfileClick()
    {
        avatarSelectionScript.backScreen = "home";
        avatarSelectionScript.EnablePanel(false);
        SoundHandler.Instance.PlayButtonClip();
    }
     
    public void HomeShareInviteClick()
    {
        inviteScreen.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HomeSharePromocodeClick()
    {
        promocodeScreen.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    #endregion

    #region Tutorial Clicks
    public void TutorialLevel1NextClick()
    {
        tutorialLevel1Obj.SetActive(false);
        tutorialLevel2Obj.SetActive(true);
        tutorialLevel3Obj.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void TutorialLevel2DoneClick()
    {
        tutorialLevel3Obj.SetActive(true);
        tutorialLevel1Obj.SetActive(false);
        tutorialLevel2Obj.SetActive(false);
        MatchItemScript refMatchItem = matchesContainer.GetChild(0).GetComponent<MatchItemScript>();
        GameObject go = Instantiate(refMatchItem.gameObject, tutorialLevel3Obj.transform.parent);
        tutorialMatchObj = go;
        go.GetComponent<MatchItemScript>().SetData(refMatchItem.data, refMatchItem.lockObject.activeSelf);
        go.GetComponent<RectTransform>().sizeDelta = refMatchItem.GetComponent<RectTransform>().sizeDelta;
        go.transform.position = refMatchItem.transform.position;

        tutorialLevel3Obj.transform.position = go.transform.position;
        tutorialLevel3Obj.transform.SetAsLastSibling();
        SoundHandler.Instance.PlayButtonClip();
    } 

    public void TutorialLevelPlayClick()
    {
        footerScript.snap.UseSwipeGestures = true;
        DataHandler.inst.isTutorialLevelComplete = false;
        tutorialLevelScreen.SetActive(false);
        tutorialLevel1Obj.SetActive(false);
        tutorialLevel2Obj.SetActive(false);
        tutorialLevel3Obj.SetActive(false);
        if (tutorialMatchObj!=null)
        {
            tutorialMatchObj.GetComponent<MatchItemScript>().OnClickPlay();
        }
        else
        {
            Debug.LogError("Match tutorial object is null");
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    #endregion

    #region First Tutorial Play
    public void FirstTutorialPlayClick()
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.firstTutorialGameApi, JsonUtility.ToJson("{}"), true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.firstTutorialGameApi + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                data = data.Replace("null", "-1");
                API_ReceiveData.TournamentPlayReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.TournamentPlayReceiveData>(data, settings);
                if (_data.http_code == 200)
                {
                    Debug.LogError("Load game scene here");
                    //DataHandler.inst.CurrentGameID = _data.user_game_id;
                    DataHandler.inst.apiLevelData = _data;
                    DataHandler.inst.isBubbleCollidedWithBottomLine = false;
                    SceneManager.LoadScene("Game");
                }
                else
                {
                    DataHandler.inst.apiLevelData = null;
                    NotificationHandler.Instance.ShowNotification(_data.error_message);
                }
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("No internet connected!");
            }
        });
        SoundHandler.Instance.PlayButtonClip();
    }

    public void FirstTutorialSettingClick()
    {
        settingScreen.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }
    #endregion

    #region PromoCode
    [Header("Promo And Invite reference")]
    public GameObject promocodeScreen;
    public GameObject inviteScreen;
    public TMP_InputField promocode_Input;
    public TextMeshProUGUI needMoreCashCodeText;
    public void PromocodeSubmitClick()
    {
        string promoText = promocode_Input.text.ToString().Replace(" ", "");
        if (promoText.Length == 0)
        {
            NotificationHandler.Instance.ShowNotification("Enter your promocode!");
        }
        else
        {
            API_SendData.PromoCodeSubmitSendData sendData = new API_SendData.PromoCodeSubmitSendData();
            sendData.promo_code = promoText;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.promocodeSubmitAPI, JsonUtility.ToJson(sendData), true, (success, receiveString) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.promocodeSubmitAPI + "  response : " + receiveString);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.PromoCodeSubmitReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.PromoCodeSubmitReceiveData>(receiveString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {
                            if (data.reward_type == 1)
                            {
                                DataHandler.inst.Cash += data.reward_amount;
                            }
                            else if (data.reward_type == 2)
                            {
                                DataHandler.inst.Coin += data.reward_amount;
                                CoinVFXHandler.Instance.StartMoveCoin();
                            }
                            else
                            {
                                AddXP(data.reward_amount);
                            }
                            NotificationHandler.Instance.ShowNotification("Promocode applied successfully!");
                            SilentGetUserData();
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(data.error_message);
                        }
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification("Error while parsing promocode receive data");
                    }
                }
            });
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void PromoCodeCloseClick()
    {
        promocodeScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void NeedMoreCashInviteClick()
    {
        //NotificationHandler.Instance.ShowNotification("Invite click here");
        new NativeShare()
        .SetSubject("Subject goes here").SetText("Code "+needMoreCashCodeText.text+"!").SetUrl("https://testurl.com/")
        .Share();
        SoundHandler.Instance.PlayButtonClip();
    }

    public void NeedMoreCashCopyCode()
    {
        CopyToClipboard(needMoreCashCodeText.text);
        SoundHandler.Instance.PlayButtonClip();
    }

    void CopyToClipboard(string str)
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
        NotificationHandler.Instance.ShowNotification("Code copied to the clipboard!");
    }
    public void NeedMoreCashCloseClick()
    {
        inviteScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
    #endregion

    #region Deposite Now
    public void DepositeNowCloseClick()
    {
        depositNowScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void DepositeNowClick()
    {
        footerScript.GoToPanel(1);
        SoundHandler.Instance.PlayButtonClip();
    }
    #endregion

    #region ExtraEvents 
    public void ShowMessagePopUp(string titleText,string contentText)
    {
        messgaeTitleText.text = titleText;
        messageContentText.text = contentText;
        messagePopUp.SetActive(true);
    }

    public void MessageCloseClick()
    {
        messagePopUp.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void TournamentRulesCloseClick()
    {
        tournamentRulesScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void UpdateCoinCashText()
    {
        headerCoinText.text = "" + DataHandler.inst.Coin;
        int totalBalance = DataHandler.inst.Cash + DataHandler.inst.BonusCash;

        homeBalanceText.text = "$" + ((float)totalBalance / 100).ToString();
        headerCashText.text = "$" + ((float)totalBalance / 100);

        homeAvailableWithdrawalCashText.text = "$" + ((float)DataHandler.inst.Cash / 100).ToString();
        homeBonusCashText.text = "$" + ((float)DataHandler.inst.BonusCash / 100).ToString();
    }
    
    public void AddXP(int amount)
    {
        DataHandler.inst.CurrentXp += amount;
        bool isLevelUp = false;
        if (DataHandler.inst.CurrentXp >= DataHandler.inst.TargetXp)
        {
            isLevelUp = true;
            NotificationHandler.Instance.ShowNotification("Level UP!");
        }
        
        float xpFill = (float)DataHandler.inst.CurrentXp / (float)DataHandler.inst.TargetXp;
        
        if (xpFill >= 1)
            xpFill = 1;
        
        headerXpFillImage.fillAmount = xpFill;
        headerXpPercentageText.text = (xpFill * 100).ToString("0.00") + "%";

        if (isLevelUp)
        {
            CallGetOfferApi();
        }
    }

    #endregion

    #region Header Clicks

    public void HeaderAddCoinClick()
    {
        if (!tutorialLevelScreen.activeSelf)
        {
            addCoinScreen.SetActive(true);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HeaderAddCashClick()
    {
        if (!tutorialLevelScreen.activeSelf)
        {
            Debug.LogError(1212);
            addCashScreen.SetActive(true);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void AddCashCloseClick()
    {
        addCashScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void AddCoinCloseClick()
    {
        addCoinScreen.SetActive(false); 
        SoundHandler.Instance.PlayButtonClip();
    }

    public void AddCashGetClick()
    {
        footerScript.GoToPanel(1);
        addCashScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void AddCoinGetClick()
    {
        footerScript.GoToPanel(1);
        addCoinScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }  

    #endregion

    #region Option Clicks
    public void OptionOpenClick()
    {
        if (!tutorialLevelScreen.activeSelf)
        {
            optionsScreen.SetActive(true);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionSettingsClick()
    {
        settingScreen.SetActive(true);
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionHelpClick()
    {
        helpPanelScript.OpenPanel(helpPanelScript.helpOptionObj, "Help");
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionRateUsClick()
    {
        Application.OpenURL("https://www.google.com/");
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionAccountClick()
    {
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionCustomizeClick()
    {
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionHowToPlayClick()
    {
        howToPlayScript.gameObject.SetActive(true);
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionCloseClick()
    {
        optionsScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OptionLogOutClick()
    {
        SoundHandler.Instance.PlayButtonClip();
        PlayerPrefs.DeleteAll();
        DataHandler.inst.isFirstTimeLoad = true;
        DataHandler.inst.UserData = null;
        DataHandler.inst.isComeAfterWin = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region ALL API CALLS
    public void DepositBackClick()
    {
        DataHandler.inst.DepositBackIntCount += 1;
        if ((DataHandler.inst.DepositBackIntCount -1) % 5 == 0)
        {
            CallGetOfferApi();
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void CallGetOfferApi()
    {
        if (DataHandler.inst.UserData == null)
            return; 
         
        string promocode = DataHandler.inst.UserData.user_data.promo_code;
        API_SendData.OfferDetailSendData sendData = new API_SendData.OfferDetailSendData();
        sendData.promo_code = promocode;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.offerDetailAPI, JsonUtility.ToJson(sendData), true, (success, data_received) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.offerDetailAPI + "  response : " + data_received);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.OfferDetailReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.OfferDetailReceiveData>(data_received, settings);
                if (data != null)
                {
                    if (data.http_code == 200)
                    {
                        offerDetailScreen.SetData(data);
                        //offerDetailScreen.gameObject.SetActive(true);
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
    }

    public void SilentGetUserData(bool isNeedToGetMatches = false,bool isNeedGetDailyReward = false)
    {
        if (DataHandler.inst.UserData != null)
        {
            homeBonusCashText.text = "" + DataHandler.inst.UserData.user_data.bonus_cash;
            headerLevelText.text = "" + DataHandler.inst.UserData.user_data.level;
            avatarSelectionScript.currentSelectedAvatarProfile = DataHandler.inst.UserData.user_data.avatar_id;
            for (int i = 0; i < userProfileImages.Length; i++)
            {
                userProfileImages[i].sprite = DataHandler.inst.userSprites[DataHandler.inst.UserData.user_data.avatar_id];
            }
            for (int i = 0; i < allUserIdText.Length; i++)
            {
                allUserIdText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
            }
            for (int i = 0; i < allUserIdUIText.Length; i++)
            {
                allUserIdUIText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
            }
            for (int i = 0; i < userNameTexts.Length; i++)
            {
                userNameTexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
            }
            for (int i = 0; i < userNameUITexts.Length; i++)
            {
                userNameUITexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
            }
            needMoreCashCodeText.text = DataHandler.inst.UserData.user_data.promo_code;
        }
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.userInfoAPI, "{}", true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.userInfoAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.UserInfoReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.UserInfoReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        DataHandler.inst.UserData = _data;
                        DataHandler.inst.Coin = DataHandler.inst.UserData.user_data.coins;
                        DataHandler.inst.Cash = DataHandler.inst.UserData.user_data.cash;
                        DataHandler.inst.BonusCash = DataHandler.inst.UserData.user_data.bonus_cash;

                        DataHandler.inst.CurrentXp = _data.user_data.current_xp;
                        DataHandler.inst.TargetXp = DataHandler.inst.UserData.user_data.xp_required;
                        DataHandler.inst.Level = DataHandler.inst.UserData.user_data.level;

                        AddXP(0);
                        UpdateCoinCashText();

                        needMoreCashCodeText.text = DataHandler.inst.UserData.user_data.promo_code;

                        for (int i = 0; i < allUserIdText.Length; i++)
                        {
                            allUserIdText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
                        }
                        for (int i = 0; i < allUserIdUIText.Length; i++)
                        {
                            allUserIdUIText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
                        }

                        //int totalBalance = DataHandler.inst.Cash + DataHandler.inst.BonusCash;
                        //homeBalanceText.text = "$" + ((float)totalBalance / 100).ToString();
                        //homeAvailableWithdrawalCashText.text = "$" + ((float)DataHandler.inst.Cash / 100).ToString();
                        //homeBonusCashText.text = "$" + ((float)DataHandler.inst.BonusCash / 100).ToString();

                        headerLevelText.text = "" + DataHandler.inst.UserData.user_data.level;
                        avatarSelectionScript.currentSelectedAvatarProfile = DataHandler.inst.UserData.user_data.avatar_id;


                        for (int i = 0; i < userProfileImages.Length; i++)
                        {
                            userProfileImages[i].sprite = DataHandler.inst.userSprites[DataHandler.inst.UserData.user_data.avatar_id];
                        }
                        for (int i = 0; i < userNameTexts.Length; i++)
                        {
                            userNameTexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
                        }
                        for (int i = 0; i < userNameUITexts.Length; i++)
                        {
                            userNameUITexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
                        }

                        if (isNeedToGetMatches)
                            GetMatchesFromAPI(true);
                      
                        if(isNeedGetDailyReward)
                            DailyRewardHandler.Instance.GetRewardedDataFromAPI();
                    }
                }
            }
        });
    }

    public void APIGetUserInfo(bool isLoaderRequired)
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.userInfoAPI, "{}", true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.userInfoAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.UserInfoReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.UserInfoReceiveData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            DataHandler.inst.UserData = _data;
                            DataHandler.inst.Coin = DataHandler.inst.UserData.user_data.coins;
                            DataHandler.inst.Cash = DataHandler.inst.UserData.user_data.cash;
                            DataHandler.inst.BonusCash = DataHandler.inst.UserData.user_data.bonus_cash;

                            DataHandler.inst.CurrentXp = DataHandler.inst.UserData.user_data.current_xp;
                            DataHandler.inst.TargetXp = DataHandler.inst.UserData.user_data.xp_required;
                            DataHandler.inst.Level = DataHandler.inst.UserData.user_data.level;
                            
                            AddXP(0);
                            UpdateCoinCashText();

                            for (int i = 0; i < allUserIdText.Length; i++)
                            {
                                allUserIdText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
                            }
                            for (int i = 0; i < allUserIdUIText.Length; i++)
                            {
                                allUserIdUIText[i].text = "ID:" + DataHandler.inst.UserData.user_data.user_id;
                            }

                            headerLevelText.text = "" + DataHandler.inst.UserData.user_data.level;
                            avatarSelectionScript.currentSelectedAvatarProfile = DataHandler.inst.UserData.user_data.avatar_id;

                            for (int i = 0; i < userProfileImages.Length; i++)
                            {
                                userProfileImages[i].sprite = DataHandler.inst.userSprites[DataHandler.inst.UserData.user_data.avatar_id];
                            }
                            for (int i = 0; i < userNameTexts.Length; i++)
                            {
                                userNameTexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
                            }
                            for (int i = 0; i < userNameUITexts.Length; i++)
                            {
                                userNameUITexts[i].text = "" + DataHandler.inst.UserData.user_data.username;
                            }
                            avatarSelectionScript.gameObject.SetActive(false);
                            
                            DailyRewardHandler.Instance.GetRewardedDataFromAPI();
                            existingUserLoginScript.gameObject.SetActive(false);
                            GetMatchesFromAPI();
                            if (DataHandler.inst.ISRegisteredUser == 1)
                            {
                                DataHandler.inst.TutorialLevelInt = 0;
                                for (int i = 0; i < homeRegisterButtons.Length; i++)
                                {
                                    homeRegisterButtons[i].SetActive(false);
                                }
                            }
                            tutorialPlayScreen.SetActive(DataHandler.inst.TutorialLevelInt == 1);
                            if (SplashHandler.instance.isLoadSuccess == false)
                            {
                                StartCoroutine(CheckGameRunning(true, false));
                            }
                            SplashHandler.instance.isLoadSuccess = true;
                        }
                        else
                        {
                            SplashHandler.instance.isLoadSuccess = true;
                            avatarSelectionScript.EnablePanel(true);
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                    }
                    else
                    {
                        SplashHandler.instance.isLoadSuccess = true;
                        PlayerPrefs.DeleteAll();
                        avatarSelectionScript.EnablePanel(true);
                    }
                }
                catch (System.Exception e)
                {
                    PlayerPrefs.DeleteAll();
                    SplashHandler.instance.isLoadSuccess = true;
                    avatarSelectionScript.EnablePanel(true);
                    Debug.LogError("Error while get user info : "+e);
                    throw;
                }
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Check your internet connection");
            }
        }, isLoaderRequired);
    }

    public void GetMatchesFromAPI(bool isLoaderShow = false)
    {
        APIHandler.Instance.RequestGetAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.getMatchesAPI,true, isLoaderShow, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.getMatchesAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.TournamentListReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.TournamentListReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        foreach (Transform item in matchesContainer)
                        {
                            Destroy(item.gameObject);
                        }
                        for (int i = 0; i < _data.unlocked_tournaments.Count; i++)
                        {
                            GameObject go = Instantiate(matchesPrefab, matchesContainer);
                            go.GetComponent<MatchItemScript>().SetData(_data.unlocked_tournaments[i]);
                        }
                        for (int i = 0; i < _data.locked_tournaments.Count; i++)
                        {
                            GameObject go = Instantiate(matchesPrefab, matchesContainer);
                            go.GetComponent<MatchItemScript>().SetData(_data.locked_tournaments[i],true);
                        }
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(_data.error_message);
                    }
                }
                else
                {
                    NotificationHandler.Instance.ShowNotification("Error while parsing data tounaments");
                }
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Check your internet connection");
            }
        });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResumeGameAPICall();
        }
    }

    public void ResumeGameAPICall()
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.resumeGameAPI, "{}", true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.resumeGameAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                //API_ReceiveData.CheckGameReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.CheckGameReceiveData>(jsonString, settings);
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Check your internet connection");
            }
        }, false);
    }

    //public void CheckMatchAPICall(int currentGameID) 
    //{
    //    API_SendData.CheckGameSendData sendData = new API_SendData.CheckGameSendData();
    //    sendData.user_game_id = currentGameID;
    //    APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.gameCheckAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
    //    {
    //        Debug.LogError("API : " + APIHandler.Instance.gameCheckAPI + "  response : " + jsonString);
    //        if (success)
    //        {
    //            var settings = new JsonSerializerSettings
    //            {
    //                NullValueHandling = NullValueHandling.Ignore,
    //                MissingMemberHandling = MissingMemberHandling.Ignore
    //            };
    //            API_ReceiveData.CheckGameReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.CheckGameReceiveData>(jsonString, settings);
    //            if (data != null)
    //            {
    //                if (data.http_code == 200 && data.complete == 0)
    //                {
    //                    Debug.LogError("Game is running here");

    //                }
    //                else
    //                {
    //                    DataHandler.inst.CurrentGameID = -1;
    //                    if(!string.IsNullOrEmpty(data.error_message))
    //                        NotificationHandler.Instance.ShowNotification(data.error_message);
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogError("Error While parsing json in CheckGame API CALL");
    //            }
    //        }
    //        else
    //        {
    //            NotificationHandler.Instance.ShowNotification("Check your internet connection");
    //        }
    //    }, false);
    //}

    #endregion
}
