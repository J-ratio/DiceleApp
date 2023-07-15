using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;

public class GoogleAdMobController : MonoBehaviour
{
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private AppOpenAd appOpenAd;
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private DataManager DataManager;
    private float deltaTime;
    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;

    [SerializeField]
    private RectTransform BannerBG;


    public void Start()
    {
        DataManager = GetComponent<DataManager>();

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
    }


// These ad units are configured to always serve test ads.
#if UNITY_ANDROID
private string _adUnitId = "ca-app-pub-4135738881392273/8735483251";
#elif UNITY_IPHONE
private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
private string _adUnitId = "unused";
#endif


    private RewardedAd rewardedAd;

  /// <summary>
  /// Loads the rewarded ad.
  /// </summary>
  public void ShowRewardedAd(int i)
  {
      // Clean up the old ad before loading a new one.
      if (rewardedAd != null)
      {
            rewardedAd.Destroy();
            rewardedAd = null;
      }

      Debug.Log("Loading the rewarded ad.");

      // create our request used to load the ad.
      var adRequest = CreateAdRequest();
      adRequest.Keywords.Add("unity-admob-sample");

      // send the request to load the ad.
      RewardedAd.Load(_adUnitId, adRequest,
          (RewardedAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("Rewarded ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Rewarded ad loaded with response : "
                        + ad.GetResponseInfo());

              rewardedAd = ad;
          });

        showRewardedAd(i);
  }


    

    public void showRewardedAd(int i)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                switch(i) 
                {
                    case 1:
                        DataManager.Collect(true);
                        break;
                    case 2:
                        DataManager.RetryLvl();
                        break;
                    case 3:
                        DataManager.StartCustomLevel();
                        break;
                    case 4:
                        DataManager.AwardGold();
                        break;
                }
            });
        }
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    #region BANNER ADS

    public void RequestBannerAd()
    {
        // These ad units are configured to always serve test ads.
        #if UNITY_EDITOR
                string adUnitId = "unused";
        #elif UNITY_ANDROID
                string adUnitId = "ca-app-pub-4135738881392273/3303177261";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize adSize = new AdSize((int)GetComponent<RectTransform>().sizeDelta.x,(int) BannerBG.sizeDelta.y);

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);


        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    #endregion

}