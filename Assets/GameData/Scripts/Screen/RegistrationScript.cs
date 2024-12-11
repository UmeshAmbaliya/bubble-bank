using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;

public class RegistrationScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField firstNameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_Dropdown mmDropdown, ddDropdown, yyyyDropdown, countryDropdown;
    [SerializeField] private TMP_InputField zipPostalCodeInputField;
    [SerializeField] private TMP_InputField mobileNumberInputField;
    [SerializeField] private TMP_InputField otpInputField;
    [SerializeField] private Button sendCodeButton;
    [SerializeField] private Button verifyButton;
    public bool isMobileVerified = false;
    public CountryCodeData countryCodeData = new CountryCodeData();
    public Material GrayMaterial;
    public Image confirmImage;

    private void OnEnable()
    {
        emailInputField.text = "";
        firstNameInputField.text = "";
        lastNameInputField.text = "";
        zipPostalCodeInputField.text = "";
        mobileNumberInputField.text = "";
        otpInputField.text = "";
        FirebaseHandler.instance.isLogin = false;
        OnEndEditEmailInputField();
        UpdateActivity(false);
    } 

    //this method is used to Close button click.......
    public void OnClickCloseButton()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    //this method is used to send code button click.......
    public void OnClickSendCodeButton() 
    {
        string email = emailInputField.text.Replace(" ","");
        string firstName = firstNameInputField.text;
        string lastName = lastNameInputField.text;
        //string birthdate = "";
        string postalCode = zipPostalCodeInputField.text;
        string countryCode = "USA";//countryDropdown.captionText.text;
        //countryCode = "USA";
        string mobileNumber = mobileNumberInputField.text;
        string countryCodeNumber = CountryCodeNumberFind(countryCode);
        SoundHandler.Instance.PlayButtonClip();
        if (string.IsNullOrEmpty(email) || !HomeUIHandler.inst.validateEmail(email))
        {
            NotificationHandler.Instance.ShowNotification("Please enter a valid email!");
            return;
        }
        if (string.IsNullOrEmpty(firstName))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your first name!");
            return;
        }
        if (string.IsNullOrEmpty(lastName))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your last name!");
            return;
        }
        if (!BirthdateValid())
        {
            NotificationHandler.Instance.ShowNotification("Please enter a valid birth date!");
            return;
        }
        //birthdate = BirthdateString();
        if (string.IsNullOrEmpty(postalCode))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your zip code!");
            return;
        }
        if (string.IsNullOrEmpty(mobileNumber))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your phone number!");
            return;
        }

        DateTime birthDate = new DateTime(int.Parse(yyyyDropdown.captionText.text), int.Parse(mmDropdown.captionText.text), int.Parse(ddDropdown.captionText.text));
        int Years = new DateTime(DateTime.Now.Subtract(birthDate).Ticks).Year - 1;

        if (Years < 18)
        {
            HomeUIHandler.inst.ShowMessagePopUp("SORRY...", "You must be 18 years or older \nin order to proceed.");
            return;
        }
        //string mobileNumber = mobileNumberInputField.text;
        //string countryCode = countryDropdown.captionText.text;
        //string countryCodeNumber = CountryCodeNumberFind(countryCode);
        //countryCodeNumber = "91";
        if (DataHandler.inst.isIndianApp)
        {
            countryCodeNumber = "91";
        }
        mobileNumber = "[+][" + countryCodeNumber + "][" + mobileNumber + "]";
        Debug.LogError("Send Code Api "+mobileNumber);
       
        FirebaseHandler.instance.SendOTP(mobileNumber);
         
    }

    //this method is used to confirm button click.......
    public void OnClickConfirmButton()
    {
        Debug.LogError("OnClickConfirmButton:" + isMobileVerified);
        //if (isMobileVerified)
        //{
        //    RegisrationCall();
        //}
        //else
        //{
        SoundHandler.Instance.PlayButtonClip();
        string oTPCode = otpInputField.text;
        if (string.IsNullOrEmpty(oTPCode) || oTPCode.Length < 6)
        {
            NotificationHandler.Instance.ShowNotification("Otp digits must be 6!");
            return;
        }
        FirebaseHandler.instance.VerifyCode(oTPCode);
        //}
    }
    public void UpdateActivity(bool ismobileverify)
    {
        isMobileVerified = ismobileverify;
        if (isMobileVerified)
        {
            emailInputField.interactable = false;
            firstNameInputField.interactable = false;
            lastNameInputField.interactable = false;
            mmDropdown.interactable = false;
            ddDropdown.interactable = false;
            yyyyDropdown.interactable = false;
            countryDropdown.interactable = false;
            zipPostalCodeInputField.interactable = false;
            mobileNumberInputField.interactable = false;
            confirmImage.material = null;
            verifyButton.interactable = true;
            sendCodeButton.interactable = false;
            otpInputField.interactable = true;
        }
        else
        {
            emailInputField.interactable = true;
            firstNameInputField.interactable = true;
            lastNameInputField.interactable = true;
            mmDropdown.interactable = true;
            ddDropdown.interactable = true;
            yyyyDropdown.interactable = true;
            countryDropdown.interactable = true;
            zipPostalCodeInputField.interactable = true;
            mobileNumberInputField.interactable = true;
            confirmImage.material = GrayMaterial;
            verifyButton.interactable = false;
            sendCodeButton.interactable = true;
            otpInputField.interactable = false;
        }
    }
    public void RegisrationCall()
    {
        string email = emailInputField.text;
        string firstName = firstNameInputField.text;
        string lastName = lastNameInputField.text;
        string birthdate = BirthdateString();
        string postalCode = zipPostalCodeInputField.text;
        string countryCode = countryDropdown.captionText.text;
        countryCode = "USA";
        string mobileNumber = mobileNumberInputField.text;
        string countryCodeNumber = CountryCodeNumberFind(countryCode);

        API_SendData.AccountSignUpSendData registrationData = new API_SendData.AccountSignUpSendData();
        registrationData.first_name = firstName;
        registrationData.last_name = lastName;
        if (DataHandler.inst.isIndianApp)
        {
            registrationData.phone_number = "[+][" + 91 + "][" + mobileNumber + "]";
        }
        else
        {
            //registrationData.phone_number = "[+][" + countryCodeNumber + "][" + mobileNumber + "]";
            registrationData.phone_number = "+" + countryCodeNumber + "" + mobileNumber + "";
        }

        registrationData.birthday = birthdate;
        registrationData.country = "USA";
        registrationData.zip_code = postalCode;
        registrationData.email = email;
        registrationData.uuid = DataHandler.inst.UUID;

        string jsonString = JsonConvert.SerializeObject(registrationData);
        //Debug.LogError("Email:" + email + "  :FirstName:" + firstName + "  :LastName:" + lastName + "  :Birthdate:" + birthdate + "  :countryCode:" + countryCode + "   :PostalCode:" + postalCode + "  :Mobile:" + mobileNumber);
        Debug.LogError("RegisrationData:" + jsonString);
        if (isMobileVerified)
        {
            APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.accountSignUpAPI, jsonString, true, (success, data) =>
            {
                Debug.LogError("API : " + APIHandler.Instance.accountSignUpAPI + "  response : " + data);
                if (success)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    API_ReceiveData.RegistrationReceivedData _data = JsonConvert.DeserializeObject<API_ReceiveData.RegistrationReceivedData>(data, settings);
                    if (_data != null)
                    {
                        if (_data.http_code == 200)
                        {
                            for (int i = 0; i < HomeUIHandler.inst.homeRegisterButtons.Length; i++)
                            {
                                HomeUIHandler.inst.homeRegisterButtons[i].SetActive(false);
                            }
                            DataHandler.inst.ISRegisteredUser = 1;
                            this.gameObject.SetActive(false);
                        }
                        else
                        {
                            NotificationHandler.Instance.ShowNotification(_data.error_message);
                        }
                    }
                    else
                    {

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
            NotificationHandler.Instance.ShowNotification("Please verify your mobile number!");
        }
    }

    //this method is used to sign in button.......
    public void OnClickSignInButton()
    {
        HomeUIHandler.inst.existingUserLoginScript.backScreen = "registration";
        HomeUIHandler.inst.existingUserLoginScript.gameObject.SetActive(true);
        gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    //this method is used to need help button.......
    public void OnClickNeedHelpButton()
    {
        HomeUIHandler.inst.helpPanelScript.backString = "Register";
        HomeUIHandler.inst.helpPanelScript.OpenPanel(HomeUIHandler.inst.helpPanelScript.supportObject,"Support");
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    #region Email Validation
    

    //this method is used to on end edit email inputfield and check email is valid or not.......
    public void OnEndEditEmailInputField()
    {
        if (emailInputField != null)
        {
            emailInputField.text = emailInputField.text.Replace(" ", "");
            if (HomeUIHandler.inst.validateEmail(emailInputField.text))
            {
                Debug.LogError("Email is valid");
                emailInputField.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Email is not valid");
                emailInputField.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Birthdate validation and Dropdown Event
    [SerializeField] private Color normalC, placeholderC;
    public void MMDropdown_IndexChanged(int index)
    {
        if(index == 0)
        {
            mmDropdown.captionText.color = placeholderC;
        }
        else
        {
            mmDropdown.captionText.color = normalC;
        }
    }

    public void DDDropdown_IndexChanged(int index)
    {
        if (index == 0)
        {
            ddDropdown.captionText.color = placeholderC;
        }
        else
        {
            ddDropdown.captionText.color = normalC;
        }
    }

    public void yyyyDropdown_IndexChanged(int index)
    {
        if (index == 0)
        {
            yyyyDropdown.captionText.color = placeholderC;
        }
        else
        {
            yyyyDropdown.captionText.color = normalC;
        }
    }

    bool BirthdateValid()
    {
        Debug.LogError("mm:" + mmDropdown.value + " :DD:" + ddDropdown.value + "    :yyyy:" + yyyyDropdown.value);
        if (mmDropdown.value == 0)
        {
            return false;
        }
        if(ddDropdown.value == 0)
        {
            return false;
        }
        if(yyyyDropdown.value == 0)
        {
            return false;
        }
        return true;
    }

    string BirthdateString()
    {
        string birthdate = yyyyDropdown.captionText.text + "-" + mmDropdown.captionText.text + "-" + ddDropdown.captionText.text;
        return birthdate;
    }
    #endregion

    #region CountryCode
    public string CountryCodeNumberFind(string countryCode)
    {
        CountryData codeData = countryCodeData.CountryCodes.Find((x) => x.countryCode == countryCode);
        if(codeData != null)
        {
            Debug.LogError("Country Code value:" + codeData.countryCode + " :Num:" + codeData.countryNum);
            return codeData.countryNum;
        }
        return "";
    }
    #endregion
}

[Serializable]
public class CountryCodeData
{
    public List<CountryData> CountryCodes = new List<CountryData>();
}

[Serializable]
public class CountryData
{
    public string countryCode;
    public string countryNum;
}