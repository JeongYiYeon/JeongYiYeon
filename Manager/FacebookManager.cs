using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : Singleton<FacebookManager>
{
    private string accessToken = "";

    public string Token => accessToken;

    public void InitFB()
    {
        if(!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public void SignWithFacebook()
    {
        var perms = new List<string>() { "public_profile", "email" };

        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var token = AccessToken.CurrentAccessToken;
            accessToken = token.TokenString;          
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void ReSignwithFacebook()
    {
        FB.Mobile.RefreshCurrentAccessToken(RefreshCallback);
    }

    private void RefreshCallback(IAccessTokenRefreshResult result)
    {
        if (FB.IsLoggedIn)
        {
            var token = result.AccessToken;

            Debug.Log("UID: " + token.UserId);
            Debug.Log("Access Token: " + token.TokenString);
        }
        else
        {
            Debug.Log("Error: " + result.Error);

            SignWithFacebook();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();

            ReSignwithFacebook();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
