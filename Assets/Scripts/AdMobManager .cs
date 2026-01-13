using GoogleMobileAds.Api;
using UnityEngine;


public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }

    private bool IsAdsIntegrated;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            IsAdsIntegrated = true;
        });
    }

    #region BannerAd

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    [SerializeField] private string _bannerAdUnitId = "ca-app-pub-2049344133749409/2553746147";
#elif UNITY_IPHONE
             private string _bannerAdUnitId = "";
#else
             private string _bannerAdUnitId = "";
#endif

    private BannerView _bannerView;

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyAd();
        }

        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
    }

    public void LoadBannerAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    #endregion

    #region InterstitialAd

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    [SerializeField] private string _interstitialAdUnitId = "";
#elif UNITY_IPHONE
             private string _interstitialAdUnitId = "";
#else
             private string _interstitialAdUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(_interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded.");
            _interstitialAd = ad;
        });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    #endregion

    #region RewardAd

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    [SerializeField] private string _rewardAdUnitId = "";
#elif UNITY_IPHONE
             private string _rewardAdUnitId = "";
#else
             private string _rewardAdUnitId = "unused";
#endif

    private RewardedAd _rewardedAd;

    public void LoadRewardedAd()
    {

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");
        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded.");
            _rewardedAd = ad;
        });
    }

    public void ShowRewardedAd()
    {
        //  if (!IsAdsIntegrated) { UIManager.Instance.GetReward(); return; }
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            bool isRewardEarned = false;

            _rewardedAd.Show((Reward reward) =>
            {
                isRewardEarned = true;
                // UIManager.Instance.GetReward();
                Debug.Log($"Rewarded ad rewarded the user. Type: {reward.Type}, Amount: {reward.Amount}");
            });

            // Handle when ad is closed
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                if (!isRewardEarned)
                {
                    //  UIManager.Instance.SkipReward();
                    Debug.Log("User skipped ad → No reward given.");
                }
            };
        }
        else
        {
            // UIManager.Instance.SkipReward();
            Debug.Log("Rewarded ad is not ready.");
        }
    }
    #endregion
}
