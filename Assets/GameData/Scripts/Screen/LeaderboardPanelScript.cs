using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;

public class LeaderboardPanelScript : MonoBehaviour
{
    public GameObject leaderboardPrefab;
    public Transform container;
    public GameObject infoObj;
    public Sprite winnerBgSprite;
    public Sprite normalBgSprite;
    public Sprite winnerProfileBgSprite;
    public Sprite normalProfileBgSprite;
    public Sprite coinRewardSprite, cashRewardSprite;
    public Sprite searchingOpponentAvatarSprite;
    public Sprite[] badgeRankSprite;
    public Color winnerNameTextColor, normalNameTextColor;
    public TextMeshProUGUI titleText;
    public GameObject collectButtonObj;
    public GameObject closeButtonObj;
    public void CollectButtonClick()
    {
        API_SendData.ResultsSendData sendData = new API_SendData.ResultsSendData();
        sendData.user_prize_id = currentTournamentData.tournamentGame.id;
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
                    API_ReceiveData.ResultCollectReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.ResultCollectReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {
                            Debug.LogError("Collected successfully for game id :" + currentTournamentData.tournamentGame.id);
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
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
    API_ReceiveData.TournamentLeaderboardReceiveData currentTournamentData;
    public void ShowLeeaderboard(string jsonString,API_ReceiveData.Result result)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        try
        {
            API_ReceiveData.TournamentLeaderboardReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.TournamentLeaderboardReceiveData>(jsonString, settings);
            currentTournamentData = data;
            foreach (Transform item in container)
            {
                Destroy(item.gameObject);
            }
            if (data != null)
            {
                if (data.http_code == 200)
                {
                    if (data.existingPlayers.Count>0)
                    {
                        infoObj.SetActive(data.existingPlayers[0].rank == 0);
                        titleText.text = data.existingPlayers[0].rank == 0 ? "RESULTS PENDING" : "LEADERBOARD";
                        closeButtonObj.SetActive(data.existingPlayers[0].rank == 0);
                        collectButtonObj.SetActive(data.existingPlayers[0].rank != 0);
                    }
                    else
                    {
                        titleText.text = "RESULTS PENDING";
                        closeButtonObj.SetActive(true);
                        collectButtonObj.SetActive(false);
                    }
                    for (int i = 0; i < result.total_players; i++)
                    {
                        GameObject go = Instantiate(leaderboardPrefab, container);
                        LeaderboardTournamentItemScript item = go.GetComponent<LeaderboardTournamentItemScript>();
                        if (i < data.existingPlayers.Count)
                        {
                            bool isRewardAvailable = (data.existingPlayers[i].rank < 4 && data.existingPlayers[i].rank != 0);
                            item.rewardImage.gameObject.SetActive(isRewardAvailable);
                            if (i < data.prizePool.Count && data.existingPlayers[i].rank != 0)
                            {
                                item.badgeImage.sprite = badgeRankSprite[i];
                                item.bgImage.sprite = data.existingPlayers[i].rank == 1 ? winnerBgSprite : normalBgSprite;
                                item.avatarBgImage.sprite = data.existingPlayers[i].rank == 1 ? winnerProfileBgSprite : normalProfileBgSprite;
                                item.rankText.text = "" + (i + 1);
                            }
                            else
                            {
                                item.badgeImage.sprite = badgeRankSprite[3];
                                item.bgImage.sprite = normalBgSprite;
                                item.avatarBgImage.sprite = normalProfileBgSprite;
                                item.rankText.text = "";
                            }
                            item.badgeImage.SetNativeSize();
                           
                            //item.avatarImage.sprite = DataHandler.inst.userSprites[data.existingPlayers[i].];
                            
                            item.nameText.text = data.existingPlayers[i].username;
                            item.idText.text = i == 0 ? "ID: " + data.existingPlayers[i].id.ToString() : "";
                            if (data.existingPlayers[i].rank != 0)
                            {
                                if (i < data.prizePool.Count)
                                {
                                    if (data.prizePool[i].currency_type == 2)// coin
                                    {
                                        item.rewardImage.sprite = coinRewardSprite;
                                        item.rewardText.text = "" + data.prizePool[i].amount;
                                        item.rewardImage.SetNativeSize();
                                    }
                                    else
                                    {
                                        item.rewardImage.sprite = cashRewardSprite;
                                        item.rewardText.text = "$" + ((float)data.prizePool[i].amount / 100);
                                        item.rewardImage.SetNativeSize();
                                    }
                                }
                            }

                            item.scoreText.transform.parent.gameObject.SetActive(data.existingPlayers[i].total_score > 0);
                            if (data.existingPlayers[i].total_score == 0)
                            {
                                item.nowPlayingText.text = "Now playing....";
                            }
                            else
                            {
                                item.scoreText.text = data.existingPlayers[i].total_score.ToString();
                                item.nowPlayingText.text = "";
                            }
                            item.searchingForOpponentText.text = "";
                        }
                        else
                        {
                            if (infoObj.activeSelf == false)
                                infoObj.SetActive(true);
                            item.badgeImage.gameObject.SetActive(false);
                            item.bgImage.sprite = i == 0 ? winnerBgSprite : normalBgSprite;
                            item.scoreText.transform.parent.gameObject.SetActive(false);
                            item.rewardText.transform.parent.gameObject.SetActive(false);
                            item.avatarImage.sprite = searchingOpponentAvatarSprite;
                            item.avatarImage.SetNativeSize();
                            item.avatarBgImage.sprite = normalProfileBgSprite;
                            item.searchingForOpponentText.text = "Searching for opponent";
                            item.nowPlayingText.text = "";
                            item.nameText.text = "";
                            item.idText.text = "";
                        }
                        item.nameText.color = i==0 ?winnerNameTextColor : normalNameTextColor;
                    }
                    this.gameObject.SetActive(true);
                }
                else
                {
                    NotificationHandler.Instance.ShowNotification(data.error_message);
                }
            }
            else
            {
                Debug.LogError("Error while parsing the data leaderboard TournamentLeaderboardReceiveData " + jsonString);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while showing leaderboard : " + e);
        }
    }

    public void CloseClick()
    {
        this.gameObject.SetActive(false);   
        SoundHandler.Instance.PlayButtonClip();
    }

    public void InfoClick()
    {
        this.gameObject.SetActive(false);
        HomeUIHandler.inst.infoPanel.ShowPopUp("Result Pending", "This tournament is still in progress as other players are currently playing their turns. your position is not final, and final results will appear in the results tab once all players finished playing.");// infoScreen.SetActive(true);
    } 
}
