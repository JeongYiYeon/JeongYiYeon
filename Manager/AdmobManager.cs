using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using GoogleMobileAds.Common;

public class AdmobManager : MonoSingleton<AdmobManager>
{
    private AppOpenAd appOpenAd;
    private RewardedInterstitialAd rewardAd;

#if ADMOB_TEST
    private const string adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_ANDROID
    private const string adUnitId = "ca-app-pub-4325556403049642/9196035379";
#elif UNITY_IOS
    private const string adUnitId = "ca-app-pub-4325556403049642/3379542739";
#else
    private const string adUnitId = "unused";
#endif

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadRewardAD(adUnitId);
        });
    }

    private void LoadRewardAD(string adUnitId)
    {
        if (rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }

        var adRequest = new AdRequest();

        RewardedInterstitialAd.Load(adUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad with error : " + error);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : " + ad.GetResponseInfo());

                InitRewardAd(ad);
            });
    }

    private void InitRewardAd(RewardedInterstitialAd ad)
    {
        rewardAd = ad;

        ad.OnAdFullScreenContentOpened += () =>
        {
            SoundManager.Instance.PauseBGM(true);
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardAD(adUnitId);
            SoundManager.Instance.PauseBGM(false);
        };
        ad.OnAdImpressionRecorded += () =>
        {
        };
        ad.OnAdClicked += () =>
        {
            SoundManager.Instance.PauseBGM(true);
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadRewardAD(adUnitId);
            SoundManager.Instance.PauseBGM(false);
        };
        ad.OnAdPaid += (AdValue adValue) =>
        {
            //LoadRewardAD(adUnitId);
        };
    }

    private bool IsAdAvailable
    {
        get
        {
            return (rewardAd != null && rewardAd.CanShowAd());
        }
    }

    public void OnClickShowRewardAD(Action successCb = null)
    {
        if(IsAdAvailable == true)
        {
            rewardAd.Show((Reward reward) =>
            {
                if(successCb != null)
                {
                    successCb.Invoke();
                }
            });
        }
    }

}
