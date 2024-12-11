using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.UIElements;

public class HowToPlayScript : MonoBehaviour
{
    [SerializeField] SimpleScrollSnap _SimpleScrollSnap;
    [SerializeField] TextMeshProUGUI _ButtonText;

    int temp = 0;
    private void OnEnable()
    {
        if (temp > 0)
            _SimpleScrollSnap.GoToPanel(0);
        else
            temp = 1;

        if (_SimpleScrollSnap.Toggles != null)
        {
            for (int i = 0; i < _SimpleScrollSnap.Toggles.Length; i++)
            {
                _SimpleScrollSnap.Toggles[i].SetIsOnWithoutNotify(false);
            }
            _SimpleScrollSnap.Toggles[0].SetIsOnWithoutNotify(true);
        }
    }

    //this method is used to next or continue button click.......
    public void OnClickNextOrContinue()
    {
        if (_SimpleScrollSnap != null)
        {
            //Debug.LogError("Current:" + _SimpleScrollSnap.SelectedPanel + " :Total:" + _SimpleScrollSnap.Panels.Length);
            if(_SimpleScrollSnap.SelectedPanel >= _SimpleScrollSnap.Panels.Length - 1)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                _SimpleScrollSnap.GoToNextPanel();
            }
        }
        SoundHandler.Instance.PlayButtonClip();
    }

    //this method is used to get the event of change page.......
    public void OnPanelCenteredCallback(Int32 index1, int index2)
    {
        //Debug.Log("Index1:" + index1 + ", " + index2);
        if(index1 == _SimpleScrollSnap.Panels.Length - 1)
        {
            _ButtonText.text = "Continue";
        }
        else
        {
            _ButtonText.text = "Next";
        }
        if (index1 == 0)
        {
            _SimpleScrollSnap.SnapSpeed = 15;
        }
    }

    private void OnDisable()
    {
        _SimpleScrollSnap.SnapSpeed = 300;
    }

    //this method is used to close button click.......
    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
}