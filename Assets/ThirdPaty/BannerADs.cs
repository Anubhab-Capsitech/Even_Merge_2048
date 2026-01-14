using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class BannerADs : MonoBehaviour
{
    public string IOS_ID;
    public string Android_ID;
    
    // Test ad units - use these for testing
    #if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/6300978111"; // Test banner
    #elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/2934735716"; // Test banner
    #else
        private string _adUnitId = "unused";
    #endif
    
    // Production ad units - uncomment these when ready for production
    // #if UNITY_ANDROID
    //     private string _adUnitId = "ca-app-pub-2049344133749409/2553746147";
    // #elif UNITY_IPHONE
    //     private string _adUnitId = "ca-app-pub-8530302013109448/6163232092";
    // #else
    //     private string _adUnitId = "unused";
    // #endif

    BannerView _bannerView;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("BannerAd initialized");
            LoadAd();
        });
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyAd();
        }

        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        
        // IMPORTANT: Register event listeners
        ListenToAdEvents();
    }

    public void LoadAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        
        var adRequest = new AdRequest();

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
            
            // Retry after 5 seconds
            Invoke("LoadAd", 5f);
        };
        
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void OnDestroy()
    {
        DestroyAd();
    }
}