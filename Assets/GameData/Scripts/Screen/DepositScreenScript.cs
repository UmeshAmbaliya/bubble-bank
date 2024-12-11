using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DepositScreenScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField depositAmountInput;
    [SerializeField] private TextMeshProUGUI bonusCashText;
    [SerializeField] private TextMeshProUGUI newBalanceText;
    [SerializeField] private GameObject bonusCashInfoObject;
    [SerializeField] private CardDetailScreenScript cardDetailScreenScript;
    [SerializeField] private PaypalDetailScreen paypalDetailScreen;
    bool isCheckMarkSelected;
    public int currentDepositCash;
    public int currentDepositBonusCash;
    private void OnEnable()
    {
        depositAmountInput.text = "$" + ((float)currentDepositCash / 100);
        bonusCashText.text = "$" +((float) currentDepositBonusCash/100);
        newBalanceText.text = "$"+((float)(currentDepositBonusCash + currentDepositCash + DataHandler.inst.Cash + DataHandler.inst.BonusCash)/100);
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.SetActive(false);
        }
    }
    public void CreditCardClick()
    {
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        cardDetailScreenScript.gameObject.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void PaypalClick()
    {
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        paypalDetailScreen.type = TransactionType.Deposit;
        paypalDetailScreen.gameObject.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OnChangeValueToggle(bool selected)
    {
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        isCheckMarkSelected = selected;
        SoundHandler.Instance.PlayButtonClip();
    }

    public void TermAndConditionClick()
    {
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        Application.OpenURL("https://www.google.com");
        SoundHandler.Instance.PlayButtonClip();
    }

    public void BackClick()
    {
        if (bonusCashInfoObject.activeSelf)
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        HomeUIHandler.inst.currentDepositCloseCount += 1;
        if (HomeUIHandler.inst.currentDepositCloseCount % 3 == 0)
        {
            HomeUIHandler.inst.depositNowScreen.SetActive(true);
        }
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void BonusCashInfoClick()
    {
        if (bonusCashInfoObject.activeSelf) 
        {
            bonusCashInfoObject.gameObject.SetActive(false);
        }
        else
        {
            bonusCashInfoObject.gameObject.SetActive(true);
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    private void Update()
    {
        if (bonusCashInfoObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject g = EventSystem.current.currentSelectedGameObject;
                if (g == null)
                {
                    bonusCashInfoObject.SetActive(false);
                    SoundHandler.Instance.PlayButtonClip();
                }
            }
        }
    }

    public void HelpTapHereClick()
    {
        Debug.LogError("Help tap here");
        HomeUIHandler.inst.helpPanelScript.OpenPanel(HomeUIHandler.inst.helpPanelScript.supportObject, "Support");
        SoundHandler.Instance.PlayButtonClip();
    }

    public void OnEndInputAmount(string str)
    {
        string s = str;
        s = s.Replace("$", "");
        s = s.Replace(" ", "");
        float amount = float.Parse(s);
        int finalAmount = (int)(100 * amount);
        s = ((float)finalAmount / 100).ToString();
        Debug.LogError(finalAmount);
        depositAmountInput.text = "$" + s;
    }
}
