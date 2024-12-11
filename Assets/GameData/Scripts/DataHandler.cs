using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public static DataHandler inst;
    public bool isIndianApp = false;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (inst!=this)
            {
                //Debug.LogError(DataHandler.inst.gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }

    public string LoginToken
    {
        get
        {
            return PlayerPrefs.GetString("UserToken", "");
        }
        set
        {
            PlayerPrefs.SetString("UserToken", value);
        }
    }

    public int Sound
    {
        get
        {
            return PlayerPrefs.GetInt("Sound", 1);
        }
        set
        {
            PlayerPrefs.SetInt("Sound", value);
        }
    }
    public int Music
    {
        get
        {
            return PlayerPrefs.GetInt("Music", 1);
        }
        set
        {
            PlayerPrefs.SetInt("Music", value);
        }
    }

    public int Vibration
    {
        get
        {
            return PlayerPrefs.GetInt("Vibration", 1);
        }
        set
        {
            PlayerPrefs.SetInt("Vibration", value);
        }
    }

    public int TutorialLevelInt
    {
        get
        {
            return PlayerPrefs.GetInt("TutorialLevelInt", 1);
        }
        set
        {
            PlayerPrefs.SetInt("TutorialLevelInt", value);
        }
    }

    public int ISRegisteredUser
    {
        get
        {
            return PlayerPrefs.GetInt("ISRegisteredUser", 0);
        }
        set
        {
            PlayerPrefs.SetInt("ISRegisteredUser", value);
        }
    }

    public string UUID
    {
        get
        {
            return PlayerPrefs.GetString("UUID_User", "");
        }
        set
        {
            PlayerPrefs.SetString("UUID_User", value);
        }
    }

    public int Cash
    {
        get
        {
            return PlayerPrefs.GetInt("Cash", 0);
        }
        set
        {
            PlayerPrefs.SetInt("Cash", value);
        }
    }

    public int BonusCash
    {
        get
        {
            return PlayerPrefs.GetInt("BonusCash", 0);
        }
        set
        {
            PlayerPrefs.SetInt("BonusCash", value);
        }
    }


    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("Coin", 0);
        }
        set
        {
            PlayerPrefs.SetInt("Coin", value);
        }
    }

    public int CurrentXp
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentXp", 0);
        }
        set
        {
            PlayerPrefs.SetInt("CurrentXp", value);
        }
    }

    public int TargetXp
    {
        get
        {
            return PlayerPrefs.GetInt("TargetXp", 0);
        }
        set
        {
            PlayerPrefs.SetInt("TargetXp", value);
        }
    }

    public int DepositBackIntCount
    {
        get
        {
            return PlayerPrefs.GetInt("DepositBackIntCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("DepositBackIntCount", value);
        }
    }

    public int AppOpenCount
    {
        get
        {
            return PlayerPrefs.GetInt("AppOpenCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("AppOpenCount", value);
        }
    }

     
    public int Level
    {
        get
        {
            return PlayerPrefs.GetInt("UserLevel", 0);
        }
        set
        {
            PlayerPrefs.SetInt("UserLevel", value);
        }
    }
    public int MatchCompleteCount
    {
        get
        {
            return PlayerPrefs.GetInt("MatchCompleteCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("MatchCompleteCount", value);
        }
    }

    public int RateUsGiven
    {
        get
        {
            return PlayerPrefs.GetInt("RateUsGiven", 0);
        }
        set
        {
            PlayerPrefs.SetInt("RateUsGiven", value);
        }
    }
    public string PaypalEmail
    {
        get
        {
            return PlayerPrefs.GetString("PaypalEmail", "");
        }
        set
        {
            PlayerPrefs.SetString("PaypalEmail", value);
        }
    }
    public API_ReceiveData.UserInfoReceiveData UserData;
    public Sprite[] userSprites;

    [Header("Game Data")]
    public API_ReceiveData.TournamentPlayReceiveData apiLevelData;
    public bool isBubbleCollidedWithBottomLine = false;
    public bool isTutorialLevelComplete = false;
    public bool isComeAfterWin = false;
    bool isFirstLoad = true;
    public bool isFirstTimeLoad 
    {
        get
        {
            return isFirstLoad;
        }
        set
        {
            isFirstLoad = value;
        }
    }
}
