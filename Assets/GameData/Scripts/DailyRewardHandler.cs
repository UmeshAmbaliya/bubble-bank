using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardHandler : MonoBehaviour
{
    [System.Serializable]
    public class DailyRewardUIData
    {
        public TextMeshProUGUI texts;
        public GameObject collectedObj;
        public Image[] grayImage;
    }


    public static DailyRewardHandler Instance;
    public TextMeshProUGUI timerText;
    public int maxHours = 24;
    public int currentDay = -1;
    public string claimDate;
    public GameObject claimButton;
    public GameObject dailyRewardScreen;
    public DailyRewardUIData[] uiData;
    TimeSpan remainingTime;
    public Material grayMat;
    //string ClaimDate
    //{
    //    get
    //    {
    //        return PlayerPrefs.GetString("DailyRewardClaimDate1", "");
    //    }
    //    set
    //    {
    //        PlayerPrefs.SetString("DailyRewardClaimDate1", value);
    //    }
    //}
    API_ReceiveData.DailyRewardReceiveData currentDailyReward;
    private void Awake()
    { 
        Instance = this;
    }

    public void GetRewardedDataFromAPI()
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.getDailyRewardInfoAPI, "{}", true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.getDailyRewardInfoAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.DailyRewardReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.DailyRewardReceiveData>(data, settings);
                currentDailyReward = _data;
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        if (_data.dailyRewards == null)
                        {
                            claimButton.SetActive(false);
                            timerText.transform.parent.gameObject.SetActive(true);
                            remainingTime = new TimeSpan(_data.hours_left, _data.minutes_left, 0);
                            StartTimer(true);
                        }
                        else
                        {
                            timerText.transform.parent.gameObject.SetActive(false);
                            for (int i = 0; i < uiData.Length; i++)
                            {
                                if (_data.dailyRewards[i].reward_type == 1)
                                {
                                    uiData[i].texts.text = "$" + ((float)_data.dailyRewards[i].reward_amount/(float)100);
                                }
                                else if (_data.dailyRewards[i].reward_type == 2)
                                {
                                    uiData[i].texts.text = "" +_data.dailyRewards[i].reward_amount;
                                }
                                else
                                {
                                    uiData[i].texts.text = "%" + _data.dailyRewards[i].reward_amount;
                                }
                            }
                            SetDailyRewardData();
                            claimButton.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.LogError(APIHandler.Instance.getDailyRewardInfoAPI + " error : " + _data.error_message);
                        //NotificationHandler.Instance.ShowNotification(_data.error_message);
                    }
                   
                }
            }
        },false);
    } 

    public void SetDailyRewardData()
    {
        int lastDayCollect = 0;
        currentDay = currentDailyReward.dailyAvailable;
        for (int i = 0; i < uiData.Length; i++)
        {
            for (int j = 0; j < uiData[i].grayImage.Length; j++)
            {
                uiData[i].grayImage[j].material = null;
            }
        }
        for (int i = 0; i < uiData.Length; i++)
        {
            int x = i + 1;
            if (uiData[i].grayImage.Length>0)
                uiData[i].grayImage[0].rectTransform.DOKill();
            
            if (x<lastDayCollect)
            {
                uiData[i].collectedObj.SetActive(true);
            }
            else if (x==currentDay)
            {
                uiData[i].grayImage[0].rectTransform.localScale = Vector3.one * 0.98f;
                uiData[i].grayImage[0].rectTransform.DOScale(Vector3.one * 1.03f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                uiData[i].collectedObj.SetActive(false);
            }
            else
            {
                for (int j = 0; j < uiData[i].grayImage.Length; j++)
                {
                    uiData[i].grayImage[j].material = grayMat;
                }
                uiData[i].collectedObj.SetActive(false);
            }
        }   
        claimButton.SetActive(true);
    }
    void StartTimer(bool needToStartTimer=true)
    {
        if (timerCoRef != null)
        {
            StopCoroutine(timerCoRef);
        }
        if(needToStartTimer)
            timerCoRef = StartCoroutine(TimerCoroutine());
    }

    Coroutine timerCoRef;
    IEnumerator TimerCoroutine()
    {
        if (remainingTime.TotalSeconds>=1)
        {
            timerText.transform.parent.gameObject.SetActive(true);
        }
        while (true)
        {
            remainingTime = remainingTime.Add(new TimeSpan(0, 0, -1));
            if (remainingTime.TotalSeconds <= 0)
            {
                timerText.text = "COLLECT";
                Debug.LogError("Break here");
                timerText.transform.parent.gameObject.SetActive(false);
                GetRewardedDataFromAPI();
                break;
            }
            else
            {
                timerText.text = string.Format("{0:00}:{1:00}:{2:00}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
            }
            yield return new WaitForSeconds(1);
        }
        Debug.LogError("Break here 2");
        timerCoRef = null;
    } 
    public void ClaimClick(int day)
    {
        Debug.LogError("Day : " + day + "  currentDay : " + currentDay);
        SoundHandler.Instance.PlayButtonClip();
        if (currentDay == -1)
        {
            return;
        }
        if (day == currentDay - 1)
        {
            API_SendData.DailyRewardClaimSendData sendData = new API_SendData.DailyRewardClaimSendData();
            sendData.day = currentDay;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.dailyRewardClaimAPi, JsonUtility.ToJson(sendData), true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.dailyRewardClaimAPi + "  response : " + data);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.DailyRewardClaimReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.DailyRewardClaimReceiveData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            claimButton.SetActive(false);

                            if (uiData[day].grayImage.Length > 0)
                                uiData[day].grayImage[0].rectTransform.DOKill();

                            uiData[day].collectedObj.SetActive(true);
                           
                            currentDay += 1;
                            HomeUIHandler.inst.SilentGetUserData();
                            if (currentDay > 7)
                            {
                                currentDay = 1;
                            }
                            DateTime dtNow = DateTime.UtcNow;
                            if (_data.leveled_up)
                            {
                                ShowLevelUpPanel(currentDay - 1);
                            }
                            else
                            {
                                RewardClaimSuccess(currentDay - 1);
                                Start24HourTimer();
                            }
                            dailyRewardScreen.SetActive(false);
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                    }
                }
            });
        }
    }

    public void Start24HourTimer()
    {
        remainingTime = new TimeSpan(24, 0, 0);
        StartTimer(true);
    }

    public void ShowLevelUpPanel(int day)
    {
        List<API_ReceiveData.DailyRewardItem> items = new List<API_ReceiveData.DailyRewardItem>();
        if (day == 6)
        {
            for (int i = day; i < currentDailyReward.dailyRewards.Count; i++)
            {
                items.Add(currentDailyReward.dailyRewards[i]);
            }
        }
        else
        {
            items.Add(currentDailyReward.dailyRewards[day]); 
        }
        HomeUIHandler.inst.levelUpPanel.SetData(items);
        HomeUIHandler.inst.levelUpPanel.levelText.text = (DataHandler.inst.Level + 1).ToString();
        HomeUIHandler.inst.levelUpPanel.gameObject.SetActive(true);
    }

    void RewardClaimSuccess(int dayOfCollection)
    {
        if (currentDailyReward != null)
        {
            if (dayOfCollection == 6)
            {
                for (int i = dayOfCollection; i < currentDailyReward.dailyRewards.Count; i++)
                {
                    if (currentDailyReward.dailyRewards[i].reward_type == 1)
                    {
                        DataHandler.inst.Cash += currentDailyReward.dailyRewards[i].reward_amount;
                    }
                    else if (currentDailyReward.dailyRewards[i].reward_type == 2)
                    {
                        DataHandler.inst.Coin += currentDailyReward.dailyRewards[i].reward_amount;
                        CoinVFXHandler.Instance.StartMoveCoin();
                    }
                    else
                    {
                        HomeUIHandler.inst.AddXP(currentDailyReward.dailyRewards[i].reward_amount);
                    }
                }
            }
            else
            {
                if (currentDailyReward.dailyRewards[dayOfCollection].reward_type == 1)
                {
                    DataHandler.inst.Cash += currentDailyReward.dailyRewards[dayOfCollection].reward_amount;
                }
                else if (currentDailyReward.dailyRewards[dayOfCollection].reward_type == 2)
                {
                    DataHandler.inst.Coin += currentDailyReward.dailyRewards[dayOfCollection].reward_amount;
                    CoinVFXHandler.Instance.StartMoveCoin();
                }
                else
                {
                    HomeUIHandler.inst.AddXP(currentDailyReward.dailyRewards[dayOfCollection].reward_amount);
                }
            }
            HomeUIHandler.inst.UpdateCoinCashText();
            HomeUIHandler.inst.SilentGetUserData();
        }
    }

    public void CloseClick()
    {
        dailyRewardScreen.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OpenDailyPopUp()
    {
        dailyRewardScreen.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }
}
