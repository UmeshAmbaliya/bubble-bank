using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SplashHandler : MonoBehaviour
{
    public static SplashHandler instance;
    private void Awake()
    {
        instance = this;
    }
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI fillPercentageText;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private GameObject screen;
    float fillValue;
    float fillSpeed = 0.5f;
    public bool isLoadSuccess = false;
    // Start is called before the first frame update
    void Start()
    {
        if (DataHandler.inst.isFirstTimeLoad)
        {
            screen.SetActive(true);
            StartCoroutine(StartLoadingCoroutine());
        }
        else
        {
            isLoadSuccess = true;
            screen.SetActive(false);
        }
    }

    IEnumerator StartLoadingCoroutine()
    {
        cg.DOKill();
        cg.alpha = 1;
        fillValue = 0;
        fillImage.fillAmount = fillValue;
        fillPercentageText.text = fillValue + "%";
        screen.SetActive(true);
        yield return new WaitForEndOfFrame();
        float r = Random.Range(0.5f, 0.8f);
        bool isReachTargetFill = false;
        bool isTargetCompleted = false;
        while (fillImage.fillAmount <= 0.999f)
        {
            if (fillValue < 1)
                fillValue += Random.Range(0.001f, 0.01f);
            else
                fillValue = 1;

            if (isReachTargetFill == false && isTargetCompleted == false)
            {
                if (fillValue > r)
                    isReachTargetFill = true;
            }
            while (isReachTargetFill && isTargetCompleted == false)
            {
                yield return new WaitForSeconds(0.1f);
                if (isLoadSuccess)
                {
                    isTargetCompleted = true;
                    isReachTargetFill = false;
                    break;
                }
            }
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, fillValue, fillSpeed * Time.deltaTime);
            fillPercentageText.text = ((fillImage.fillAmount*100).ToString("0.0")) + "%";
            yield return new WaitForEndOfFrame();
        }

        cg.DOFade(0, 0.5f).OnComplete(() => 
        {
            DataHandler.inst.isFirstTimeLoad = false;
            DataHandler.inst.AppOpenCount += 1;
            if (DataHandler.inst.AppOpenCount %3 == 0)
            {
                if (HomeUIHandler.inst!=null)
                {
                    HomeUIHandler.inst.CallGetOfferApi();
                }
            }
            screen.SetActive(false);
        });
    } 
}
