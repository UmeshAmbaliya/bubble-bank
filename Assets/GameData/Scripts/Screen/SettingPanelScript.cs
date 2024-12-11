using BubbleShooterKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelScript : MonoBehaviour
{
    public Image soundImage;
    public Image musicImage;
    public Image vibrationImage;
    public Sprite onSPrite;
    public Sprite offSPrite;
    public string privacyPolicyAndroid;
    public string privacyPolicyIos;
    public string termsOfUseUrl;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (DataHandler.inst.Sound == 1)
            soundImage.sprite = onSPrite;
        else
            soundImage.sprite = offSPrite;
     
        if (DataHandler.inst.Music == 1)
            musicImage.sprite = onSPrite;
        else
            musicImage.sprite = offSPrite;

        if (DataHandler.inst.Vibration == 1)
            vibrationImage.sprite = onSPrite;
        else
            vibrationImage.sprite = offSPrite;
    }

    public void SoundClick()
    {
        if (DataHandler.inst.Sound == 1)
        {
            DataHandler.inst.Sound = 0; soundImage.sprite = offSPrite;
        }
        else
        {
            DataHandler.inst.Sound = 1; soundImage.sprite = onSPrite;
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void MusicClick()
    {
        if (DataHandler.inst.Music == 1)
        {
            DataHandler.inst.Music = 0; musicImage.sprite = offSPrite;
        }
        else
        {
            DataHandler.inst.Music = 1; musicImage.sprite = onSPrite;
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void VibrationClick()
    {
        if (DataHandler.inst.Vibration == 1)
        {
            DataHandler.inst.Vibration = 0; vibrationImage.sprite = offSPrite;
        }
        else
        {
            DataHandler.inst.Vibration = 1; vibrationImage.sprite = onSPrite;
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    public void PrivacyPolicyClick()
    {
#if UNITY_ANDROID
        Application.OpenURL(privacyPolicyAndroid);
#else
        Application.OpenUrl(privacyPolicyIos);
#endif
        SoundHandler.Instance.PlayButtonClip();
    }

    public void TermOfUseClick()
    {
        Application.OpenURL(termsOfUseUrl);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void CloseClick()
    {
        GameScreen gs = FindObjectOfType<GameScreen>();
        if (gs != null)
            gs.UnlockInput();
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }

}
