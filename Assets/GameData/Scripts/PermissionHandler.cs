using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PermissionHandler : MonoBehaviour
{
    public static PermissionHandler instance;
    public PermissionCallbacks LocationPermitCallback { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance!= this)
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
    
    Action<bool> currentLocationCallback;
    public void CheckForLocationPermit(Action<bool> callback)
    {
#if UNITY_EDITOR
        callback.Invoke(true);
#elif UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.LogError("The user authorized use of the microphone.");
            callback.Invoke(true);
        }
        else
        {
            currentLocationCallback = callback;
            // Request location permission
            LocationPermitCallback = new PermissionCallbacks();
            LocationPermitCallback.PermissionGranted += LocationPermitGranted;
            LocationPermitCallback.PermissionDenied += LocationPermitDenied;
            LocationPermitCallback.PermissionDeniedAndDontAskAgain += LocationPermitDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.FineLocation, LocationPermitCallback);
        }
#endif
    }

    private void LocationPermitDeniedAndDontAskAgain(string obj)
    {
        Debug.LogError("LocationPermitDeniedAndDontAskAgain:" + obj);
    }

    private void LocationPermitDenied(string obj)
    {
        Debug.LogError("LocationPermitDenied:" + obj);
    }

    private void LocationPermitGranted(string obj)
    {
        Debug.LogError("LocationPermitGranted:" + obj);
        if (currentLocationCallback != null)
        {
            currentLocationCallback.Invoke(true);
        }
    }

    /*public void AskLocationPermission()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.LogError("The user authorized use of the microphone.");
        }
        else
        {
            // Request location permission
            LocationPermitCallback = new PermissionCallbacks();
            LocationPermitCallback.PermissionGranted += LocationPermitGranted;
            LocationPermitCallback.PermissionDenied += LocationPermitDenied;
            LocationPermitCallback.PermissionDeniedAndDontAskAgain += LocationPermitDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.FineLocation, LocationPermitCallback);
        }
    }*/
}
