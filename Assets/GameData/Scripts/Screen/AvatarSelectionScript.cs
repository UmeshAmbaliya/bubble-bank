using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class AvatarSelectionScript : MonoBehaviour
{
    [SerializeField] private GameObject chooseAvatarPopUp;
    [SerializeField] private GameObject selectImageAvatarPopUp;
    [SerializeField] private TMP_InputField avatarSelectionUsernameInput;
    public AvatarProfileItem[] selectImageAllAvatars;
    [HideInInspector] public int currentSelectedAvatarProfile = 0;
    [SerializeField] private string[] dummyAvatarNames;
    [SerializeField] private GameObject backSelectImageObj;
    public string backScreen;
    int lastProfileIndex;
    // Start is called before the first frame update
    public void EnablePanel(bool isChooseAvatar)
    {
        if (isChooseAvatar)
        {
            backScreen = "";
            chooseAvatarPopUp.SetActive(true);
            selectImageAvatarPopUp.SetActive(false);
        }
        else
        {
            chooseAvatarPopUp.SetActive(false);
            selectImageAvatarPopUp.SetActive(true);
            for (int i = 0; i < HomeUIHandler.inst.avatarSelectionScript.selectImageAllAvatars.Length; i++)
            {
                HomeUIHandler.inst.avatarSelectionScript.selectImageAllAvatars[i].UpdateSelection();
            }
        }
        this.gameObject.SetActive(true);
        if (backScreen == "home")
        {
            backSelectImageObj.SetActive(true);
        }
        else 
        {
            backSelectImageObj.SetActive(false);
        }
        lastProfileIndex = currentSelectedAvatarProfile;
    } 

    public void ChooseAvatarNextClick()
    { 
        if (string.IsNullOrEmpty(avatarSelectionUsernameInput.text))
        {
            NotificationHandler.Instance.ShowNotification("Enter valid name!");
            SoundHandler.Instance.PlayButtonClip();
            return;
        }

        API_SendData.UsernameSignUpSendData dataSend = new API_SendData.UsernameSignUpSendData();
        dataSend.username = avatarSelectionUsernameInput.text;
        dataSend.avatar = currentSelectedAvatarProfile;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.usernameSignUpAPI, JsonUtility.ToJson(dataSend), false, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.usernameSignUpAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.UsernameSignUpReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.UsernameSignUpReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        DataHandler.inst.LoginToken = _data.uuid;
                        DataHandler.inst.UUID = _data.uuid;
                        HomeUIHandler.inst.APIGetUserInfo(true);
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
        }, true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ChooseAvatarEditProfileClick()
    {
        chooseAvatarPopUp.SetActive(false);
        selectImageAvatarPopUp.SetActive(true);
        for (int i = 0; i < selectImageAllAvatars.Length; i++)
        {
            selectImageAllAvatars[i].UpdateSelection();
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ChooseAvatarRefreshClick()
    {
        string randomString = dummyAvatarNames[Random.Range(0, dummyAvatarNames.Length)];
        randomString += Random.Range(0, 10) + "" + Random.Range(0, 10);
        avatarSelectionUsernameInput.text = randomString;
        SoundHandler.Instance.PlayButtonClip();
    }

    public void ChooseAvatarSignInClick()
    {
        Debug.Log("Choose avatar sign in click here");
        HomeUIHandler.inst.existingUserLoginScript.backScreen = "choose_avatar";
        HomeUIHandler.inst.existingUserLoginScript.gameObject.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }


    public void SelectImageOkayClick()
    {
        if (backScreen == "home")
        {
            API_SendData.AvatarUpdateSendData dataSend = new API_SendData.AvatarUpdateSendData();
            dataSend.avatar = currentSelectedAvatarProfile;
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.avatarUpdateAPI, JsonUtility.ToJson(dataSend), true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.avatarUpdateAPI + "  response : " + data);
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
                            for (int i = 0; i < HomeUIHandler.inst.userProfileImages.Length; i++)
                            {
                                HomeUIHandler.inst.userProfileImages[i].sprite = DataHandler.inst.userSprites[currentSelectedAvatarProfile];
                            }
                            selectImageAvatarPopUp.SetActive(false);
                            this.gameObject.SetActive(false);
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                    }
                }
            });
        }
        else
        {
            for (int i = 0; i < HomeUIHandler.inst.userProfileImages.Length; i++)
            {
                HomeUIHandler.inst.userProfileImages[i].sprite = DataHandler.inst.userSprites[currentSelectedAvatarProfile];
            }
            EnablePanel(true);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void SelectImageCloseClick()
    {
        //if (lastProfileIndex != currentSelectedAvatarProfile)
        //{
        //    SelectImageOkayClick();
        //}
        //else
        //{
            selectImageAvatarPopUp.SetActive(false);
            this.gameObject.SetActive(false);
        currentSelectedAvatarProfile = lastProfileIndex;
        for (int i = 0; i < HomeUIHandler.inst.userProfileImages.Length; i++)
        {
            HomeUIHandler.inst.userProfileImages[i].sprite = DataHandler.inst.userSprites[currentSelectedAvatarProfile];
        }

        //}
        SoundHandler.Instance.PlayButtonClip();
    }
}
