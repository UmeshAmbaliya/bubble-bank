using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailScreenScript : MonoBehaviour
{
    [SerializeField] TMP_InputField cardNumberInput;
    [SerializeField] TMP_InputField expDateInput;
    [SerializeField] TMP_InputField cvvInput;
    [SerializeField] TMP_InputField zipCodeInput;
    [SerializeField] Image cardImage;
    [SerializeField] Sprite[] cardSprites;
    [SerializeField] Toggle saveCardToggle;
    private void OnEnable()
    {
        cardNumberInput.text = "";
        expDateInput.text = "";
        cvvInput.text = "";
        zipCodeInput.text = "";
        isCardSave = false;
        saveCardToggle.isOn = isCardSave;
    }
    public void CloseClick()
    {
        HomeUIHandler.inst.currentDepositCloseCount += 1;
        if (HomeUIHandler.inst.currentDepositCloseCount % 3 == 0)
        {
            HomeUIHandler.inst.depositNowScreen.SetActive(true);
        }
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void CompleteDepositClick()
    {
        string cardNumber = cardNumberInput.text;
        string cardType = CreditCardValidator.IsValidUSACreditCard(cardNumber);
        SoundHandler.Instance.PlayButtonClip();
        if (string.IsNullOrEmpty(cardType))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your card number!");
            return;
        }
        string expDate = expDateInput.text;
        if (!CreditCardValidator.IsValidExpiryDate(expDate))
        {
            NotificationHandler.Instance.ShowNotification("Please enter expiry date of your card number!");
            return;
        }
        string cvv = cvvInput.text;
        if (cvv.Length < 3)
        {
            NotificationHandler.Instance.ShowNotification("Please enter valid cvv!");
            return;
        }
        string zipCode = zipCodeInput.text;
        if (string.IsNullOrEmpty(zipCode))
        {
            NotificationHandler.Instance.ShowNotification("Please enter ZipCode!");
            return;
        }
        if (isCardSave)
        {
            Debug.LogError("Save card here");
        }
        else
        {
            Debug.LogError("User didn't saved this card");
        }
        API_SendData.PaymentDepositeSendData sendData = new API_SendData.PaymentDepositeSendData();
        sendData.credit_card_number = long.Parse(cardNumber);
        sendData.exp_date = expDate;
        sendData.cvc = int.Parse(cvv);
        sendData.zip_code = int.Parse(zipCode);
        sendData.deposit_amount = ShopPanelHandler.Instance.currentBuyAmount * 100;
        sendData.paypal = "";
        sendData.remember_card = isCardSave ? 1 :0;
        sendData.offer_id = 1;
        APIHandler.Instance.RequestPostAPI(APIHandler.Instance.baseUrl + APIHandler.Instance.depositeAPI, JsonUtility.ToJson(sendData), true, (success, jsonString) =>
        {
            Debug.LogError("API : " + APIHandler.Instance.depositeAPI + "  response : " + jsonString);
            if (success)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    API_ReceiveData.DepositReceiveData data = JsonConvert.DeserializeObject<API_ReceiveData.DepositReceiveData>(jsonString, settings);
                    if (data != null)
                    {
                        if (data.http_code == 200)
                        {

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

    public void OnEndSelectCardNumber(string endString)
    {
        string cardNumber = endString.Replace(" ", "");
        string cardType = CreditCardValidator.IsValidUSACreditCard(cardNumber);
        if (string.IsNullOrEmpty(cardType))
        {
            NotificationHandler.Instance.ShowNotification("Please enter your card number!");
            return;
        }
        else
        {
            cardImage.sprite = cardSprites[0];
            switch (cardType)
            {
                case "VISA":
                    cardImage.sprite = cardSprites[0];
                    break;
                case "MASTERCARD":
                    cardImage.sprite = cardSprites[1];
                    break;
                case "AMERICANEXPRESS":
                    cardImage.sprite = cardSprites[2];
                    break;
                default:
                    return;
            }
        }
    }
    bool isCardSave = false;
    public void OnSaveCardToggleValueChange(bool isTrue)
    {
        isCardSave = isTrue;
        SoundHandler.Instance.PlayButtonClip();
    }
}
