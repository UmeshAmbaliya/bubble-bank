using BubbleShooterKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelScript : Popup
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI clearBoardScore;
    public TextMeshProUGUI timeBonusText;
    public TextMeshProUGUI totalScoreText;
    [HideInInspector]
    public GameLogic logicScript;
    public void OkayClick()
    {
        Debug.LogError("Ok well done click here");
        SoundHandler.Instance.PlayButtonClip();
        DataHandler.inst.isComeAfterWin = true;
        SceneManager.LoadScene("Home");
    }

    public void CloseClick()
    {
        Debug.LogError("Close well done click here");
        SoundHandler.Instance.PlayButtonClip();
        DataHandler.inst.isComeAfterWin = true;
        SceneManager.LoadScene("Home");
    }

    public void SetInfo(GameLogic screen,bool isWin = true)
    {
        logicScript = screen;
        scoreText.text = logicScript.GameState.Score.ToString();
        if (isWin)
        {
            clearBoardScore.text = logicScript.GameState.clearBoardScore.ToString();
            timeBonusText.text = logicScript.GameState.timeBonusScore.ToString();
        }
        else
        {
            clearBoardScore.text = "-----";
            timeBonusText.text = "-----";
        }
        totalScoreText.text = logicScript.GameState.totalScore.ToString();   
    }
}
