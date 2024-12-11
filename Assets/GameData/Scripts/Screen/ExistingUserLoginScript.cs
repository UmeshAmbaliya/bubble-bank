using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExistingUserLoginScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField zipPostalCodeInput;
    [SerializeField] private TMP_InputField mobileNumberInput;
    [SerializeField] private TMP_InputField otpInput;
    [SerializeField] private TMP_Dropdown countryDropdown;
    [HideInInspector] public string backScreen;
    [SerializeField] private RegistrationScript registrationScript;
    [SerializeField] private Button sendButton;
    public bool isMobileVerified = false;

    public Image confirmImage;
    public Material GrayMaterial;

    void OnEnable()
    {
        FirebaseHandler.instance.isLogin = true;
        zipPostalCodeInput.text = string.Empty;
        mobileNumberInput.text = string.Empty;
        otpInput.text = string.Empty;
        UpdateActivity(false);
    }
      
    public void SendCodeClick()
    {
        if (isMobileVerified == false)
        {
            string zip =zipPostalCodeInput.text;
            if (string.IsNullOrEmpty(zip))
            {
                NotificationHandler.Instance.ShowNotification("Enter zipcode first!");
                SoundHandler.Instance.PlayButtonClip();
                return;
            }
            string mobileNumber = mobileNumberInput.text;
            string countryCode = countryDropdown.captionText.text;
            countryCode = "USA";
            string countryCodeNumber = registrationScript.CountryCodeNumberFind(countryCode);
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                if (DataHandler.inst.isIndianApp)
                {
                    countryCodeNumber = "91";
                }
                mobileNumber = "[+][" + countryCodeNumber + "][" + mobileNumber + "]";
        
                FirebaseHandler.instance.SendOTP(mobileNumber); 
            }
            else
            {
                NotificationHandler.Instance.ShowNotification("Enter valid mobile number!");
            }
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Code is already sent!");
        }
        SoundHandler.Instance.PlayButtonClip();
    }
    
    public void ConfirmClick()
    {
        if (isMobileVerified)
        {
            string otp = otpInput.text;
            if (string.IsNullOrEmpty(otp) || otp.Length < 6)
            {
                NotificationHandler.Instance.ShowNotification("Otp digits must be 6!");
            }
            else
            {
                Debug.LogError("call confirm otp API call here");
                FirebaseHandler.instance.VerifyCode(otp);
            }
        }
        else
        {
            NotificationHandler.Instance.ShowNotification("Please confirm your mobile number first!");
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void UpdateActivity(bool ismobileverify)
    {
        isMobileVerified = ismobileverify;
        if (isMobileVerified)
        {
            mobileNumberInput.interactable = false;
            zipPostalCodeInput.interactable = false;
            countryDropdown.interactable = false;
            otpInput.interactable = true;
            sendButton.interactable = false;
            confirmImage.material = null;
        }
        else
        {
            mobileNumberInput.interactable = true;
            zipPostalCodeInput.interactable = true;
            countryDropdown.interactable = true;
            sendButton.interactable = true;
            otpInput.interactable = false;
            confirmImage.material = GrayMaterial;
        }
    }

    public void CloseClick()
    {
        switch (backScreen)
        {
            case "choose_avatar":
                HomeUIHandler.inst.avatarSelectionScript.EnablePanel(true);
                break;
            case "registration":
                HomeUIHandler.inst.registrationScript.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void APILoginCall()
    {
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.loginAPI, JsonUtility.ToJson("{}"), true, (success, data) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.loginAPI + "  response : " + data);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                API_ReceiveData.LogInReceiveData _data = JsonConvert.DeserializeObject<API_ReceiveData.LogInReceiveData>(data, settings);
                if (_data != null)
                {
                    if (_data.http_code == 200)
                    {
                        DataHandler.inst.ISRegisteredUser = 1;
                        SplashHandler.instance.isLoadSuccess = false;
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
    }

    public void HelpClick()
    {
        HomeUIHandler.inst.helpPanelScript.backString = "Login";
        HomeUIHandler.inst.helpPanelScript.OpenPanel(HomeUIHandler.inst.helpPanelScript.supportObject, "Support");
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
}
