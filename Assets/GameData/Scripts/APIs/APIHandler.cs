using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections.Generic;

public class APIHandler : MonoBehaviour
{
    public static APIHandler Instance;

    public string baseUrl;
    public string testUrl;
    public bool isTestMode;

    [Header("APis List")]
    public string usernameSignUpAPI;
    public string loginAPI;
    public string userInfoAPI;
    public string avatarUpdateAPI;
    public string accountSignUpAPI;
    public string promocodeSubmitAPI;
    public string offerDetailAPI;

    [Header("Game apis")]
    public string firstTutorialGameApi;
    public string calculateScoreApi;
    public string gameCheckAPI;
    public string endGameAPI;
    public string resumeGameAPI;

    [Header("Daily reward API list")]
    public string getDailyRewardInfoAPI;
    public string dailyRewardClaimAPi;

    [Header("Tournaments")]
    public string getMatchesAPI;
    public string getMatchDetailAPI;
    public string matchPlayApi;
    public string tournamantLeaderboardAPI;
    public string resultCollectAllAPI;
    public string resultCollectAPI;
    public string resultsAPI;
    [Header("Help and support")]
    public string supportAPI;
    public string feedbackAPI;

    [Header("Transaction")]
    public string transactionHistoryAPI;
    public string offerClaimDetailAPI;
    public string withdrawalAPI;
    public string depositeAPI;

    [Header("UI Loader")]
    [SerializeField] private GameObject loaderObj;
    CanvasGroup cgLoader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (isTestMode)
                baseUrl = testUrl;
        }
        else
        {
            if (Instance != this)
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
//#if UNITY_EDITOR
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.L))
//        {
//            if (loaderObj.activeSelf)
//            {
//                HideLoader();
//            }
//            else
//            {
//                ShowLoader();
//            }
//        }
//    }
//#endif
    private void Start()
    {
        cgLoader = loaderObj.GetComponent<CanvasGroup>();
    }

    #region Post Api Method
    //public void RequestPostAPI(string url, WWWForm form, bool isTokenRequire, Action<bool, string> callback)
    //{
    //    StartCoroutine(ApiRequestPost(url, form, isTokenRequire, callback));
    //}

    public void RequestPostAPI(string url, string form, bool isTokenRequire, Action<bool, string> callback, bool isLoaderShow = true)
    {
        StartCoroutine(ApiRequestPost(url, form, isTokenRequire, callback, isLoaderShow));
    }

    //IEnumerator ApiRequestPost(string uri, WWWForm form, bool isTokenRequire, Action<bool, string> callback)
    //{
    //    loaderObj.SetActive(true);
    //    UnityWebRequest uwr = UnityWebRequest.Post(uri, form);
    //    uwr.SetRequestHeader("Content-Type", "application/json");
    //    if (isTokenRequire)
    //    {
    //        uwr.SetRequestHeader("Authorization", "Bearer "/* + DataHandler.Instance.LoginToken*/);
    //    }
    //    yield return uwr.SendWebRequest();
    //    loaderObj.SetActive(false);
    //    if (uwr.isNetworkError || uwr.error != null)
    //    {
    //        callback.Invoke(false, "");
    //    }
    //    else
    //    {
    //        callback.Invoke(true, uwr.downloadHandler.text.ToString());
    //    }
    //}

    IEnumerator ApiRequestPost(string uri, string json, bool isTokenRequire, Action<bool, string> callback, bool isLoaderShow)
    {
        if (isLoaderShow)
        {
            ShowLoader(); 
        }
        UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, json);
        request.SetRequestHeader("Content-Type", "application/json");
        if (isTokenRequire)
        {
            request.SetRequestHeader("Authorization", "Bearer " + DataHandler.inst.LoginToken);
        }
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (loaderObj.activeSelf)
            HideLoader();
        if (request.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(false, request.error);
        }
        else
        {
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }

    #endregion End Post Method

    #region Put Api Method
    public void RequestPutAPI(string uri, byte[] data, bool isTokenRequire, Action<bool, string> callback)
    { 
        StartCoroutine(ApiRequestPut(uri, data, isTokenRequire, callback));
    }
    public IEnumerator ApiRequestPut(string uri, byte[] data, bool isTokenRequire, Action<bool, string> callback)
    { 
        UnityWebRequest uwr = UnityWebRequest.Put(new Uri(uri), data);
        uwr.SetRequestHeader("Content-Type", "application/json");
        if (isTokenRequire)
        {
            uwr.SetRequestHeader("Authorization", "Bearer "  + DataHandler.inst.LoginToken );
        }
        yield return uwr.SendWebRequest();


        if (uwr.isNetworkError || uwr.error != null)
        {
            callback.Invoke(false, "");
        }
        else
        {
            callback.Invoke(true, uwr.downloadHandler.text.ToString());
        }
    }
    #endregion End Put Method

    #region Get API Method
    public void RequestGetAPI(string url, bool isTokenRequire, bool isLoaderShow, Action<bool, string> callback)
    {
        StartCoroutine(ApiRequestGet(url, isTokenRequire, isLoaderShow, callback));
    }

    IEnumerator ApiRequestGet(string uri, bool isTokenRequire, bool isLoaderShow, Action<bool, string> callback)
    {
        if (isLoaderShow)
            ShowLoader(); 

        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        if (isTokenRequire)
        {
            uwr.SetRequestHeader("Authorization", "Bearer "  + DataHandler.inst.LoginToken );
        }
        yield return uwr.SendWebRequest();

        if (isLoaderShow)
            HideLoader();

        if (uwr.isNetworkError || uwr.error != null)
        {
            callback.Invoke(false, "");
        }
        else if (uwr.isDone)
        {
            callback.Invoke(true, uwr.downloadHandler.text.ToString());
        }
    }
    #endregion End Get Method

    #region Loader Methods

    public void ShowLoader()
    {
        loaderObj.SetActive(true);
        cgLoader.alpha = 0;
        cgLoader.DOKill();
        cgLoader.DOFade(1, 0.1f);
    }

    public void HideLoader()
    {
        cgLoader.DOKill();
        cgLoader.DOFade(0, 0.1f).OnComplete(() => 
        {
            loaderObj.SetActive(false);
        });
    }

    #endregion
}


namespace API_SendData
{
    public class UsernameSignUpSendData
    {
        public string username;
        public int avatar;
    }

    public class AccountSignUpSendData
    {
        public string first_name;
        public string last_name;
        public string phone_number;
        public string birthday;
        public string country;
        public string zip_code;
        public string email;
        public string uuid;
    }

    public class AvatarUpdateSendData
    {
        public int avatar;
    }
    [System.Serializable]
    public class CalculateScoreSendData
    {
        public int user_game_id;
        public int time_left;
        public int bubbles_same_popped_count;
        public int bubbles_diff_popped_count;
        public int life_count;
        public int bubbles_left;
        public int bubbles_hidden_left;
        public int tournament_id;
        public int color_ball;
        public List<List<int>> bubbles_in_view;
        public List<List<int>> bubbles_hidden;
    }

    [System.Serializable]
    public class EndGameSendData
    {
        public int user_game_id; 
    }

    public class DailyRewardClaimSendData
    {
        public int day;
    }
    #region TOURNAMENT  

    public class TournamentPlaySendData
    {
        public int tournament_id;
    }
    
    public class TournamentLeaderboardSendData
    {
        public int user_game_id;
    }

    public class ResultsSendData
    {
        public int user_prize_id;
    }
    public class TournamentResultSendData
    {
        public int user_game_id;
    }
    #endregion
    public class PromoCodeSubmitSendData
    {
        public string promo_code;
    } 

    public class OfferDetailSendData
    {
        public string promo_code;
    }

  
    #region Help and support
    public class SupportSendData
    {
        public string email;
        public string subject;  
        public string description;
    }
    public class FeedbackSendData
    {
        public string description;
    }
    #endregion

    #region PAYMENT
    public class PaymentOfferClaimSendData
    {
        public int offer_id;
    }

    public class PaymentWithdrawalSendData
    {
        public int withdrawal_amount;
        public string paypal_email;
    }

    public class PaymentDepositeSendData
    {
        public int deposit_amount;
        public string paypal;
        public long credit_card_number;
        public string exp_date;
        public int cvc;
        public int zip_code;
        public int remember_card;
        public int offer_id;
    }

    #endregion
}

namespace API_ReceiveData
{
    [System.Serializable]
    public class LogInReceiveData
    {
        public int http_code;
        public string error_message;
    }
    [System.Serializable]
    public class UsernameSignUpReceiveData
    {
        public int http_code;
        public string uuid;
        public string error_message;
    }
    [System.Serializable]
    public class UserInfoReceiveData
    {
        public int http_code;
        public UserData user_data;
        public string error_message;
    }
    // account balance : cash + bonus cash
    // withdrawable amount : cash
    // user can only withdrawal cash with integer number.
    [System.Serializable]
    public class UserData
    {
        public int level;
        public int current_xp;
        public int coins;
        public int cash;
        public int bonus_cash;
        public int xp_required;
        public string promo_code;
        public int avatar_id;
        public int user_id;
        public string username;
    }

    [System.Serializable]
    public class RegistrationReceivedData
    {
        public int http_code;
        public string error_message;
    }

    public class PromoCodeSubmitReceiveData
    {
        public int http_code;
        public int reward_type;
        public int reward_amount;
        public string error_message;
    }


    #region DailyReward
    public class DailyRewardItem
    {
        public int id;
        public DateTime created_at;
        public string title;
        public int reward_type;
        public int reward_amount;
        public int day;
    }

    public class DailyRewardReceiveData
    {
        public int http_code;
        public string error_message;
        public List<DailyRewardItem> dailyRewards;
        public int dailyAvailable;
        public string message;
        public int hours_left;
        public int minutes_left;
    }

    public class DailyRewardClaimReceiveData
    {
        public int http_code;
        public bool leveled_up;
        public string error_message;
    }
    #endregion

    #region Game Data
    public class CheckGameReceiveData
    {
        public int http_code;
        public int complete;
        public int user_game_id;
        public string error_message;
    }

    public class EndGameReceiveData
    {
        public int http_code;
        public string error_message;
    }

    #endregion

    #region Calculate score
    public class CalculateScoreReceiveData
    {
        public int http_code;
        public ScoreBreakdown score_breakdown;
        public string error_message;
    }

    public class ScoreBreakdown
    {
        public int your_score;
        public int clear_board;
        public int time_bonus;
        public int total_score;
    }
    #endregion

    #region Tournaments
    public class TournamentListReceiveData
    {
        public int http_code;
        public List<UnlockedTournament> unlocked_tournaments;
        public List<UnlockedTournament> locked_tournaments;
        public string error_message;
    }

    public class UnlockedTournament
    {
        public int id;
        public DateTime created_at;
        public int level_req;
        public int entry_currency_type;
        public int total_players;
        public int bonus_cash;
        public int entry_fee;
        public string title;
        public int prize_currency_type;
        public int prize_total;
    }
    [System.Serializable]
    public class TournamentPlayReceiveData
    {
        public int http_code;
        public int tournament_game_id;
        public int user_game_id;
        public List<List<int>> puzzle = new List<List<int>>();
        public string error_message;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TournamentLeaderboardExistingPlayer
    {
        public int id;
        public DateTime created_at;
        public int cheated;
        public int user_id;
        public int completed;
        public int time_left;
        public int bubbles_left;
        public int bubbles_same_popped_count;
        public int bubbles_diff_popped_count;
        public int life_count;
        public int bubbles_hidden_left;
        public int clear_board;
        public int time_bonus;
        public int your_score;
        public int total_score;
        public DateTime updated_at;
        public int tournament_game_id;
        public int rank;
        public string username;
        public string status_or_score;
    }

    public class PrizePool
    {
        public int id;
        public DateTime created_at;
        public int tournament_id;
        public int currency_type;
        public int amount;
        public int bonus_amount;
        public int rank;
        public int bonus_currency_type;
    }

    public class TournamentLeaderboardReceiveData
    {
        public int http_code;
        public TournamentGame tournamentGame;
        public List<TournamentLeaderboardExistingPlayer> existingPlayers;
        public List<PrizePool> prizePool;
        public string error_message;
    }

    public class TournamentGame
    {
        public int id;
        public DateTime created_at;
        public int tournament_id;
        public int tournament_game_status;
        public int current_player_count;
        public DateTime date_completed;
        public int game_puzzle_id;
        public int level_req;
        public int total_players;
        public string title;
    }

    public class ResultCollectReceiveData
    {
        public int http_code;
        public string error_message;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Result
    {
        public int id;
        public int rank;
        public DateTime created_at;
        public int total_players;
        public string title;
        public int reward_collected;
        public int currency_type;
        public int amount;
        public int bonus_amount;
        public string status;
    }

    public class ResultsReceiveData
    {
        public int http_code;
        public List<Result> results;
        public string error_message;
    }

    public class TournamentDetailReceiveData
    {
        public int http_code;
        public List<PrizePool> prize_pool;
        public string error_message;
    }
    #endregion
     
    #region Offer Detail
    public class Offer
    {
        public int id;
        public DateTime created_at;
        public int bonus_cash;
        public int coins;
        public int deposit_amount;
        public string title;
    }

    public class OfferDetailReceiveData
    {
        public int http_code;
        public Offer offer;
        public string error_message;
    }

    #endregion

    #region Help and support
    public class SupportReceiveData
    {
        public int http_code;
        public string error_message;
    }

    public class FeedbackReceiveData
    {
        public int http_code;
        public string error_message;
    }
    #endregion

    #region Transaction
    public class DepositReceiveData
    {
        public int http_code;
        public string error_message;
    }

    public class PaymentWithdrawalReceiveData
    {
        public int http_code;
        public string error_message;
    } 
    public class PaymentOfferClaimDetailReceiveData
    {
        public int http_code;
        public Offer offerDetails;
        public string error_message;
    }

    #endregion
}