using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseHandler : MonoBehaviour
{
    #region Instance
    public static FirebaseHandler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance!=this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    FirebaseAuth auth;
    FirebaseUser user;
    PhoneAuthProvider phoneAuthProvider;
    private string verificationId;
    public bool isLogin;
    void Start()
    {
        InitFirebase();
    }

    #region INITIALIZE
    void InitFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        CheckTokenValidity();
    }

    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    private void CheckTokenValidity()
    {
        Debug.LogError("Max time :" + GetCurrentUnixTimestampSeconds());
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.TokenAsync(false).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    string idToken = task.Result;

                    // Parse the JWT token to extract the expiration time
                    string[] tokenParts = idToken.Split('.');
                    string payload = tokenParts[1];
                    string decodedPayload = DecodeBase64Url(payload);
                    var tokenJson = Google.MiniJSON.Json.Deserialize(decodedPayload) as Dictionary<string, object>;
                    long expirationTimeSeconds = Convert.ToInt64(tokenJson["exp"]);
                    Debug.LogError("expirationTimeSeconds : " + expirationTimeSeconds);
                    // Check if the token has expired
                    if (expirationTimeSeconds < GetCurrentUnixTimestampSeconds())
                    {
                        Debug.LogError("CheckTokenValidity token has expired.");
                        // Perform actions when the token has expired, such as refreshing the token
                    }
                    else
                    {
                        Debug.LogError("CheckTokenValidity token is still valid.");
                    }
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("CheckTokenValidity Error retrieving token: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("CheckTokenValidity User is not logged in.");
        }
    }

    private long GetCurrentUnixTimestampSeconds()
    {
        long l = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return l;
    }

    private string DecodeBase64Url(string base64Url)
    {
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        string padded = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        byte[] base64Bytes = Convert.FromBase64String(padded);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }
    #endregion

    #region Otp Send And Verify
    public void SendOTP(string phoneNumber)
    {
        APIHandler.Instance.ShowLoader();
        Debug.LogError("Send OTP called : " + phoneNumber);
        phoneAuthProvider = PhoneAuthProvider.GetInstance(auth);
        phoneAuthProvider.VerifyPhoneNumber(
          new PhoneAuthOptions
          {
              PhoneNumber = phoneNumber,
              TimeoutInMilliseconds = 60000,
              ForceResendingToken = null
          },
          verificationCompleted: (credential) =>
          {
              Debug.LogError("Firebase verificationCompleted cred : " + credential.ToString());
              // Auto-sms-retrieval or instant validation has succeeded (Android only).
              // There is no need to input the verification code.
              // `credential` can be used instead of calling GetCredential().
              //SignInWithCredential(credential);
          },
          verificationFailed: (error) =>
          {
              if (isLogin)
              {
                  HomeUIHandler.inst.existingUserLoginScript.UpdateActivity(false);
              }
              else
              {
                  HomeUIHandler.inst.registrationScript.UpdateActivity(false);
              }
              APIHandler.Instance.HideLoader();
              NotificationHandler.Instance.ShowNotification("Invalid mobile number : "+ error);
              Debug.LogError("Firebase verificationFailed error : " + error.ToString());
              // The verification code was not sent.
              // `error` contains a human readable explanation of the problem.
          },
        codeSent: (id, token) =>
        {
            APIHandler.Instance.HideLoader();
            NotificationHandler.Instance.ShowNotification("Code sent successfully");
            Debug.LogError("Firebase codeSent id : " + id.ToString() + "  token : " + token.ToString());
            verificationId = id;
            if (isLogin)
            {
                HomeUIHandler.inst.existingUserLoginScript.UpdateActivity(true);  
            }
            else
            {
                HomeUIHandler.inst.registrationScript.UpdateActivity(true);
            }
            //DataHandler.inst.LoginToken = token.ToString();
            // Verification code was successfully sent via SMS.
            // `id` contains the verification id that will need to passed in with
            // the code from the user when calling GetCredential().
            // `token` can be used if the user requests the code be sent again, to
            // tie the two requests together.
        }, codeAutoRetrievalTimeOut: (id) =>
        {
            if (isLogin)
            {
                HomeUIHandler.inst.existingUserLoginScript.UpdateActivity(false);  
            }
            else
            {
                HomeUIHandler.inst.registrationScript.UpdateActivity(false);
            }
            APIHandler.Instance.HideLoader();
            Debug.LogError("Firebase codeAutoRetrievalTimeOut id : " + id.ToString());
            // Auto-retrieval timed out
            Debug.LogError("Phone verification timeout");
        });
    }

    public void VerifyCode(string verificationCode)
    {
        APIHandler.Instance.ShowLoader();
        Debug.LogError("Verify code called : " + verificationCode);
        // Verify the verification code entered by the user
        PhoneAuthCredential credential = phoneAuthProvider.GetCredential(verificationId, verificationCode);
        SignInWithCredential(credential);
    }

    private void SignInWithCredential(PhoneAuthCredential credential)
    {
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Phone authentication failed: " + task.Exception);
                NotificationHandler.Instance.ShowNotification("Authentication failed : " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Phone authentication successful!");
                FirebaseUser user = task.Result;
                GetAuthToken(task.Result); 
                Debug.Log("User ID: " + user.UserId);
            }
        });
    }
    private void GetAuthToken(FirebaseUser user)
    { 
        var _ = user.TokenAsync(true).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("TokenAsync was canceled.");
                NotificationHandler.Instance.ShowNotification("Task cancelled");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("TokenAsync encountered an error: " + task.Exception);
                NotificationHandler.Instance.ShowNotification("Task Error : "+task.Exception);
                return;
            }

            string idToken = task.Result;
            Debug.LogError("Firebase Token 1 : " + idToken + " isLogin : "+isLogin);

            DataHandler.inst.LoginToken = idToken;
            if (isLogin)
            {
                HomeUIHandler.inst.existingUserLoginScript.APILoginCall();
            }
            else
            { 
                HomeUIHandler.inst.registrationScript.RegisrationCall();
            }
            // Send token to your backend via HTTPS
            // ...
        });

        //FirebaseUser user1 = FirebaseAuth.DefaultInstance.CurrentUser;
        //var _2 = user1.TokenAsync(true).ContinueWith(task =>
        //{
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("TokenAsync was canceled.");
        //        return;
        //    }

        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("TokenAsync encountered an error: " + task.Exception);
        //        return;
        //    }

        //    string idToken = task.Result;
        //    Debug.LogError("Firebase Token 2 : " + idToken);



        //    //DataHandler.inst.LoginToken = idToken;
        //    //if (isLogin)
        //    //{
        //    //    HomeUIHandler.inst.existingUserLoginScript.isMobileVerified = true;
        //    //    HomeUIHandler.inst.existingUserLoginScript.APILoginCall();
        //    //}
        //    //else
        //    //{
        //    //    HomeUIHandler.inst.registrationScript.isMobileVerified = true;
        //    //    HomeUIHandler.inst.registrationScript.RegisrationCall();
        //    //}
        //    // Send token to your backend via HTTPS
        //    // ...
        //});
        // Token retrieved, now you can use it
    }

    #endregion

}
