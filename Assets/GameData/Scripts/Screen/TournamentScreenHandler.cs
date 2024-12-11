using Newtonsoft.Json;
using UnityEngine;

public class TournamentScreenHandler : MonoBehaviour
{
    public static TournamentScreenHandler instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Tournament result references")]
    public GameObject tournamentResultPrefab;
    public Transform tournamentResultContainer;
    public Transform currentTournamentResultContainer;
    public GameObject noTournamentsFoundObj;
    public Sprite[] resultRankSprites;
    [Header("Leaderboard references")]
    public LeaderboardPanelScript leaderboardPanelScript;

    public void CallGetResultsAPI()
    {
        API_SendData.ResultsSendData sendData = new API_SendData.ResultsSendData();
        sendData.user_prize_id = 1;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.resultsAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.resultsAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.ResultsReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.ResultsReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {
                            noTournamentsFoundObj.SetActive(false);
                            foreach (Transform item in tournamentResultContainer)
                            {
                                if (item.name.Contains("TournamentItem"))
                                {
                                    Destroy(item.gameObject);
                                }
                            }
                            foreach (Transform item in currentTournamentResultContainer)
                            {
                                if (item.name.Contains("TournamentItem"))
                                {
                                    Destroy(item.gameObject);
                                }
                            }
                            bool isCurrentTournamentFound = data.results.FindAll(x=>x.status == "Current").Count > 0;
                            bool isCompletedTournamentFound = data.results.FindAll(x=>x.status == "Complete").Count > 0;
                            currentTournamentResultContainer.gameObject.SetActive(isCurrentTournamentFound);
                            tournamentResultContainer.gameObject.SetActive(isCompletedTournamentFound);
                            for (int i = 0; i < data.results.Count; i++)
                            {
                                if (data.results[i].status == "Current")
                                {
                                    if (!string.IsNullOrEmpty(data.results[i].title) || data.results[i].amount > 0)
                                    {
                                        GameObject go = Instantiate(tournamentResultPrefab, currentTournamentResultContainer);
                                        TournamentResultItemScript script = go.GetComponent<TournamentResultItemScript>();
                                        script.SetData(data.results[i]);
                                    }
                                }
                                else
                                {
                                    
                                    if (!string.IsNullOrEmpty(data.results[i].title) || data.results[i].amount > 0)
                                    {
                                        GameObject go = Instantiate(tournamentResultPrefab, tournamentResultContainer);
                                        TournamentResultItemScript script = go.GetComponent<TournamentResultItemScript>();
                                        script.SetData(data.results[i]);
                                    }
                                }
                            }
                            if (currentTournamentResultContainer.childCount <= 2)
                            {
                                noTournamentsFoundObj.SetActive(true);
                            }
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
    }


    public void CollectAllClick()
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.resultCollectAllAPI, "{}", true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.resultCollectAllAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.TournamentLeaderboardReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.TournamentLeaderboardReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {
                            HomeUIHandler.inst.SilentGetUserData();
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
}
