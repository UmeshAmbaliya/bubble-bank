using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomFooter : MonoBehaviour
{
    public SimpleScrollSnap snap;
    public void onTransitionEffect(GameObject go,float f)
    {
        if (go.name == "" + snap.startingPanel)
        {
            ManageFooterImage(f);
        }
    }

    public void OnPanelCenteredCallback(Int32 index1, int index2)
    {
        btns[index1].GetChild(0).transform.DOKill();
        btns[index2].GetChild(0).transform.DOKill();

        btns[index1].GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.up * 45;
        bottomText[index1].DOKill();
        bottomText[index1].DOFade(1, 0.15f);

        //btns[index1].GetChild(0).transform.DOScale(Vector3.one * 1.1f, 0.2f);

        btns[index2].GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        btns[index2].GetChild(0).transform.DOScale(Vector3.one, 0.2f);

        bottomText[index2].DOKill();
        bottomText[index2].DOFade(0, 0.15f);

        HomeUIHandler.inst.SilentGetUserData();
    }

    float screenSize;
    public RectTransform[] btns;
    public CanvasGroup[] bottomText;
    public CanvasScaler canvasScaler;
    public float marginBtns;
    public float multiplier = 0;

    void Start()
    {
        btns[0].GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.up * 45;
        bottomText[0].alpha =1.0f;
        rtFooterSelection.sizeDelta = btns[0].sizeDelta;
        screenSize = canvasScaler.referenceResolution.x; 
        marginBtns = btns[1].anchoredPosition.x - btns[0].anchoredPosition.x;
        multiplier = marginBtns/ screenSize;
        rtFooterSelection.anchoredPosition = new Vector2(marginBtns * snap.startingPanel, 0);
        Vector3 pos = btns[0].position;
        pos.x = rtFooterSelection.position.x;
        rtFooterSelection.position = pos;
    }

    // Update is called once per frame
    public void GoToPanel(int index)
    {
        snap.GoToPanel(index);
        SoundHandler.Instance.PlayButtonClip();
        if (index == 0)
        {
            HomeUIHandler.inst.GetMatchesFromAPI(true);
        }
        else if (index == 2)
        {
            TournamentScreenHandler.instance.CallGetResultsAPI();
        }
    }
    public RectTransform rtFooterSelection;

    public void ManageFooterImage(float posX)
    {
        rtFooterSelection.anchoredPosition = new Vector3(marginBtns*snap.startingPanel - posX * multiplier, rtFooterSelection.anchoredPosition.y,0);
    }
}
