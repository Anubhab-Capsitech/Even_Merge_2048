using System;
using UnityEngine;
using Firebase.Analytics;

public class AdAnalytics : MonoBehaviour
{
    public static AdAnalytics Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this when a banner ad is clicked
    /// </summary>
    public void BannerAdClicked()
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            Debug.LogWarning("[AdAnalytics] BannerAdClicked called but Firebase not ready.");
            return;
        }

        try
        {
            FirebaseAnalytics.LogEvent("banner_ad_click");
            Debug.Log("[AdAnalytics] Banner ad click logged.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[AdAnalytics] Failed to log banner_ad_click: " + ex);
        }
    }

    /// <summary>
    /// Call this when an interstitial ad is shown
    /// </summary>
    public void InterstitialAdShown()
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            Debug.LogWarning("[AdAnalytics] InterstitialAdShown called but Firebase not ready.");
            return;
        }

        try
        {
            FirebaseAnalytics.LogEvent("interstitial_ad_shown");
            Debug.Log("[AdAnalytics] Interstitial ad shown logged.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[AdAnalytics] Failed to log interstitial_ad_shown: " + ex);
        }
    }

    /// <summary>
    /// Call this when a rewarded ad is completed
    /// </summary>
    public void RewardedAdCompleted(string rewardType)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            Debug.LogWarning("[AdAnalytics] RewardedAdCompleted called but Firebase not ready.");
            return;
        }

        try
        {
            FirebaseAnalytics.LogEvent(
                "rewarded_ad_completed",
                new Parameter("reward_type", rewardType)
            );
            Debug.Log("[AdAnalytics] Rewarded ad completed logged: " + rewardType);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[AdAnalytics] Failed to log rewarded_ad_completed: " + ex);
        }
    }
}
