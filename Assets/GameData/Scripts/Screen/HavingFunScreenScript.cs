using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HavingFunScreenScript : MonoBehaviour
{
    public GameObject havingFunQuestion;
    public GameObject rateObj;
    public GameObject helpUsImproveScreen;

    public Image[] stars;
    public Sprite starEmptySp;
    public Sprite starFilledSp;

    public TMP_InputField helpUsImproveContentInput;
    private void OnEnable()
    {
        havingFunQuestion.SetActive(true);
        rateObj.SetActive(false);
        helpUsImproveScreen.SetActive(false);
    }

    public void HavingFunYesClick()
    {
        havingFunQuestion.SetActive(false);
        rateObj.SetActive(true);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = starFilledSp;
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HavingFunNoClick()
    {
        havingFunQuestion.SetActive(false);
        helpUsImproveContentInput.text = "";
        helpUsImproveScreen.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HavingFunCloseClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void RateUsRateClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
        DataHandler.inst.RateUsGiven = 1;
        Application.OpenURL("https://www.google.com/");
    }

    public void RateUsLaterClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HelpUsImproveCloseClick()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HelpUsImproveSubmitClick()
    {
        string message = helpUsImproveContentInput.text;
        if (message.Length > 0)
        {
            API_SendData.FeedbackSendData sendData = new API_SendData.FeedbackSendData();
            sendData.description = message;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.feedbackAPI, JsonUtility.ToJson(sendData), true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.feedbackAPI + "  response : " + data);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.FeedbackReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.FeedbackReceiveData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            NotificationHandler.Instance.ShowNotification("Feedback submitted successfully");
                            this.gameObject.SetActive(false);
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                        this.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("Error while parsing jsonstring to json for feedback API");
                    }
                }
                else
                {
                    NotificationHandler.Instance.ShowNotification("Check your internet connection");
                }
            }, true);
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Please enter your message first!");
        }
      
        SoundHandler.Instance.PlayButtonClip();
    } 

    public void RateStarClick(int currentStarCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i>currentStarCount)
            {
                stars[i].sprite = starEmptySp;
            }
            else
            {
                stars[i].sprite = starFilledSp;
            }
        }
        SoundHandler.Instance.PlayButtonClip();
    }
}
