using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinVFXHandler : MonoBehaviour
{
    public static CoinVFXHandler Instance;
    [SerializeField] private GameObject pileOfCoins;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    //[SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int coinsAmount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartMoveCoin();
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (coinsAmount == 0)
            coinsAmount = 10; // you need to change this value based on the number of coins in the inspector

        initialPos = new Vector2[coinsAmount];
        initialRotation = new Quaternion[coinsAmount];

        for (int i = 0; i < pileOfCoins.transform.childCount; i++)
        {
            initialPos[i] = pileOfCoins.transform.GetChild(i).position;
            initialRotation[i] = pileOfCoins.transform.GetChild(i).rotation;
        }
    }
    
    public void StartMoveCoin()
    {
        pileOfCoins.SetActive(true);
        var delay = 0f;

        Vector3 startPos = Vector3.zero;
        if (startPoint!=null)
            startPos = startPoint.transform.position;

        for (int i = 0; i < pileOfCoins.transform.childCount; i++)
        {
            pileOfCoins.transform.GetChild(i).position = startPos +new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f, 0.5f),0);
            pileOfCoins.transform.GetChild(i).DOScale(0.25f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
            pileOfCoins.transform.GetChild(i).transform.DOMove(endPoint.transform.position, 0.8f).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            pileOfCoins.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            pileOfCoins.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.3f).SetEase(Ease.OutBack);
            delay += 0.1f;
            endPoint.transform.DOScale(1.1f, 0.1f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
        }

        StartCoroutine(CountDollars());
    }

    IEnumerator CountDollars()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        HomeUIHandler.inst.UpdateCoinCashText();
    }
}
