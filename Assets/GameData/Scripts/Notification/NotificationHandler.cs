using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{

    public RectTransform notificationContainer;
    public TextMeshProUGUI text;

    public static NotificationHandler Instance;
    bool isShowing = false;
    public float timeOfNotification;
    float dummyTime = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (Instance != this)
            { 
                DestroyImmediate(this.gameObject);
            }
        }
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    ShowNotification("Test Notification");
        //}
        if (isShowing)
        {
            if (notificationContainer.anchoredPosition.y > 0)
            {
                notificationContainer.anchoredPosition = Vector2.Lerp(notificationContainer.anchoredPosition, new Vector2(0, -1), 8 * Time.deltaTime);
            }
            dummyTime += Time.deltaTime;
            if (dummyTime>=timeOfNotification)
            {
                isShowing = false;
            }
        }
        else
        {
            if (notificationContainer.gameObject.activeSelf)
            {
                notificationContainer.anchoredPosition = Vector2.Lerp(notificationContainer.anchoredPosition, new Vector2(0, 300), 8 * Time.deltaTime);
                if (notificationContainer.anchoredPosition.y>150)
                {
                    notificationContainer.gameObject.SetActive(false);
                }
            }
        }
    }
    public void ShowNotification(string str)
    {
        notificationContainer.gameObject.SetActive(true);
        dummyTime = 0;
        text.text = str;
        isShowing = true;
    }

    public void CloseNotificationOnPointerDown()
    {
        if (isShowing)
        {
            dummyTime = timeOfNotification;
        }
    }
}
