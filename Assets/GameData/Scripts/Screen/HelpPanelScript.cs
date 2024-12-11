using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class HelpPanelScript : MonoBehaviour
{
    public GameObject mainPanelObj;
    public GameObject helpOptionObj;
    public GameObject commonHelpObj;
    public GameObject supportObject;

    public string backString;
    [Header("Support references")]
    public TMP_InputField supportEmailInput;
    public TMP_InputField supportDescriptionInput;
    public string supportSubjectString = "Support";
    public TextMeshProUGUI titleText;


    public void OpenPanel(GameObject openObj,string Title)
    {
        titleText.text = Title;
        mainPanelObj.SetActive(true);
        helpOptionObj.SetActive(false);
        commonHelpObj.SetActive(false);
        supportObject.SetActive(false);
        openObj.SetActive(true);
    } 

    public void BackClick()
    { 
        if (backString == "HelpOption")
        {
            if (helpOptionObj.gameObject.activeSelf)
            {
                mainPanelObj.SetActive(false);
                helpOptionObj.SetActive(false);
            }
            else
            {
                helpOptionObj.SetActive(true);
            }
            commonHelpObj.SetActive(false);
            supportObject.SetActive(false);
        }
        else if (backString == "Login")
        {
            HomeUIHandler.inst.existingUserLoginScript.gameObject.SetActive(true);
            helpOptionObj.SetActive(false);
            commonHelpObj.SetActive(false);
            supportObject.SetActive(false);
            mainPanelObj.SetActive(false);
        }
        else if (backString == "Register")
        {
            HomeUIHandler.inst.registrationScript.gameObject.SetActive(true);
            helpOptionObj.SetActive(false);
            commonHelpObj.SetActive(false);
            supportObject.SetActive(false);
            mainPanelObj.SetActive(false);
        }
        else
        {
            helpOptionObj.SetActive(false);
            commonHelpObj.SetActive(false);
            supportObject.SetActive(false);
            mainPanelObj.SetActive(false);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void SupportSubmitClick()
    {
        SoundHandler.Instance.PlayButtonClip();
        string email = supportEmailInput.text.Replace(" ","");
        string description = supportDescriptionInput.text;
        if (!validateEmail(email))
        {
            NotificationHandler.Instance.ShowNotification("Enter valid email please!");
            return;
        }
        if (string.IsNullOrEmpty(description))
        {
            NotificationHandler.Instance.ShowNotification("Enter description please!");
            return;
        }
        API_SendData.SupportSendData dataSend = new API_SendData.SupportSendData();
        dataSend.email = email;
        dataSend.description = description;
        dataSend.subject = supportSubjectString;

        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.supportAPI, JsonUtility.ToJson(dataSend), true, (success, receiveData) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.supportAPI + "  response : " + receiveData);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.SupportReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.SupportReceiveData>(receiveData, settings);
                if (data != null)
                {
                    if (data.http_code == 200)
                    {
                        NotificationHandler.Instance.ShowNotification("Submitted your request successfully!");
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(data.error_message);
                    }
                }
                else
                {
                    Debug.LogError("Error while parsing support receive data");
                }
            }
        });
    }

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

    public void FeedbackAPICall()
    {
        string description = "description";
        API_SendData.FeedbackSendData dataSend = new API_SendData.FeedbackSendData();
        dataSend.description = description;

        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.feedbackAPI, JsonUtility.ToJson(dataSend), true, (success, receiveData) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.feedbackAPI + "  response : " + receiveData);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.FeedbackReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.FeedbackReceiveData>(receiveData, settings);
                if (data != null)
                {
                    if (data.http_code == 200)
                    {
                        NotificationHandler.Instance.ShowNotification("Submitted your request successfully!");
                    }
                    else
                    {
                        NotificationHandler.Instance.ShowNotification(data.error_message);
                    }
                }
                else
                {
                    Debug.LogError("Error while parsing support receive data");
                }
            }
        });
    }
}
