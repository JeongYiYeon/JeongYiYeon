using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Firebase.Auth;
using System;
using Cysharp.Threading.Tasks;
using Google;
using System.Threading.Tasks;
using System.Text;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
#endif

public class FirebaseManager : Singleton<FirebaseManager>
{
    private FirebaseAuth auth = null;
    private FirebaseUser user = null;

#if UNITY_IOS
    private AppleAuthManager appleAuthManager;
#endif

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

    public void InitFirebase(Action successCb = null)
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                user = auth.CurrentUser;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                Firebase.Messaging.FirebaseMessaging.SubscribeAsync("notice");

                if (successCb != null)
                {
                    successCb.Invoke();
                }
            }
            else
            {
            }
        });


    }

    public void SignInGuest()
    {
        if (auth == null)
        {
            Debug.LogError("파베 초기화 안됨");
            return;
        }

        if (user == null)
        {
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }
                user = task.Result.User;
                Debug.LogFormat("firebase guest signIn userid {0}", user.UserId);

                UserData.Instance.user.SetUDID(user.UserId);
            });
        }
        else
        {
            Debug.LogFormat("guest login {0}", user.UserId);
            UserData.Instance.user.SetUDID(user.UserId);
        }
    }

    public void SignInGoogle(Action successCb = null)
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (SignInStatus result) =>
        {
            if (result == SignInStatus.Success)
            {
                var localUser = (PlayGamesLocalUser)Social.localUser;
                var googleIdToken = localUser.GetIdToken();
                Debug.LogError($"GPGS 로그인 성공 : {PlayGamesPlatform.Instance.GetIdToken()}");

                Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

                auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        return;
                    }

                    user = task.Result.User;
                    UserData.Instance.user.SetUDID(user.UserId);

                    LoadingManager.Instance.ActiveOneLineAlram("구글 연동 완료");

                    if(successCb != null)
                    {
                        successCb.Invoke();
                    }
                });

            }
            else
            {
                Debug.LogError("GPGS 로그인 실패");
                LoadingManager.Instance.ActiveOneLineAlram("구글 연동 실패");
            }
        });

#elif UNITY_IOS
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            // Copy this value from the google-service.json file.
            // oauth_client with type == 3
            WebClientId = "375073508622-r2qmcetvmkgf4o4a26bt33kge6g3ou3h.apps.googleusercontent.com"
        };

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task => {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
            }
            else
            {

                Credential credential = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                    }
                    else if (task.IsFaulted)
                    {
                        signInCompleted.SetException(task.Exception);
                    }
                    else
                    {
                        user = task.Result.User;
                        UserData.Instance.user.SetUDID(user.UserId);

                        LoadingManager.Instance.ActiveOneLineAlram("구글 연동 완료");

                        if (successCb != null)
                        {
                            successCb.Invoke();
                        }
                    }
                });
            }
        });
#endif
    }

#if UNITY_IOS
    public void SignInApple()
    {
        var deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(deserializer);

        if (appleAuthManager != null)
        {
            appleAuthManager.Update();
        }

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        Credential fbCredential = null;

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    var userId = appleIdCredential.User;
                    var email = appleIdCredential.Email;
                    var fullName = appleIdCredential.FullName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                    fbCredential = OAuthProvider.GetCredential("apple.com", identityToken, authorizationCode, null);

                    auth.CurrentUser.LinkWithCredentialAsync(fbCredential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("SignInWithCredentialAsync was canceled.");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);

                            return;
                        }

                        user = task.Result.User;
                        UserData.Instance.user.SetUDID(user.UserId);
                    });
                }
            },
            error =>
            {
                Debug.Log("Apple Signin Error");
            });
    }
#endif

    public IEnumerator IESignInFaceBook()
    {
        FacebookManager.Instance.InitFB();

        yield return new WaitUntil(() => !string.IsNullOrEmpty(FacebookManager.Instance.Token));

        Credential credential = FacebookAuthProvider.GetCredential(FacebookManager.Instance.Token);

        auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                
                return;
            }

            user = task.Result.User;
            UserData.Instance.user.SetUDID(user.UserId);
        });
    }

    public void UnSign()
    {
        auth.CurrentUser.UnlinkAsync(auth.CurrentUser.ProviderId).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UnlinkAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UnlinkAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result.User;
        });
    }

#if UNITY_ANDROID
    public void InitGPGS(Action successCb = null)
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /* Don't force refresh */)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        if(successCb != null)
        {
            successCb.Invoke();
        }
    }
#endif
}

