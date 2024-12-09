using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GaminCat.Analytics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class AdsManager : ScriptableObject
{
    //CustomField
    public enum RewardType
    {
        LevelUpPower,
        LevelUpRange,
        LevelUpCapacity,
        Trolley,
        ToHome,
        BonusSpeed,
        BonusStock,
        BonusSell,
        BuriedGold,
        AddCoin,
        AddGear,
        MuseumMassEntry,
        MuseumSpeedUp,
        MuseumOfflineProfit
    }

    //--------------------------------
    public static bool IsDontAds { get; set; } = false;

    //MyScriptableObjectが保存してある場所のパス
    private const string ScriptablePath = "AdsManager";

    //MyScriptableObjectの実体
    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Resources.Load<AdsManager>(ScriptablePath);
            return _instance;
        }
    }

    private static void CheckLoadingDictionary(Dictionary<string, bool> list, string id)
    {
        if (!list.ContainsKey(id))
        {
            list.Add(id, false);
        }
    }

    private static bool GetLoadingDictionary(Dictionary<string, bool> list, string id)
    {
        CheckLoadingDictionary(list, id);
        return list[id];
    }


    private static void SetLoadingDictionary(Dictionary<string, bool> list, string id, bool flag)
    {
        CheckLoadingDictionary(list, id);
        list[id] = flag;
    }

    private static void CheckAttemptDictionary(Dictionary<string, int> list, string id)
    {
        if (!list.ContainsKey(id))
        {
            list.Add(id, 0);
        }
    }

    private static int GetAttemptDictionary(Dictionary<string, int> list, string id)
    {
        CheckAttemptDictionary(list, id);
        return list[id];
    }


    private static void SetAttemptDictionary(Dictionary<string, int> list, string id, int attempt)
    {
        CheckAttemptDictionary(list, id);
        list[id] = attempt;
    }

    private static void AddAttemptDictionary(Dictionary<string, int> list, string id, int sum)
    {
        CheckAttemptDictionary(list, id);
        list[id] += sum;
    }

    [SerializeField] private string sdkKey;

    [SerializeField] private List<string> interstitialAdUnitIDs;
    [SerializeField] private List<string> interstitialAdUnitIDsIOS;

    [SerializeField] private List<string> rewardedAdUnitIDs;
    [SerializeField] private List<string> rewardedAdUnitIDsIOS;

    [SerializeField] private string bannerAdUnitID;
    [SerializeField] private string bannerAdUnitIDIOS;


#if UNITY_IOS
    public List<string> InterstitialAdUnitIDs => interstitialAdUnitIDsIOS;
    public List<string> RewardedAdUnitIDs => rewardedAdUnitIDsIOS;
    public string BannerAdUnitID => bannerAdUnitIDIOS;
#else
    public List<string> InterstitialAdUnitIDs => interstitialAdUnitIDs;
    public List<string> RewardedAdUnitIDs => rewardedAdUnitIDs;
    public string BannerAdUnitID => bannerAdUnitID;
#endif

    [SerializeField] private float interval = 15f;
    private Tween _doErrorTimer;

    private Dictionary<string, bool> BeginLoadAds { get; set; } = new Dictionary<string, bool>();
    private Dictionary<string, int> RetryAttemptAds { get; set; } = new Dictionary<string, int>();
    private float LastShowAdTime { get; set; }


    public bool AudioEnable
    {
        get => MaxSdk.IsMuted();
        set => MaxSdk.SetMuted(value);
    }

    private GameObject LinkObject { get; set; }
    private Action OnGetReward { get; set; }
    private Action OnFailedReward { get; set; }
    public Action OnLoadStartReward { get; set; }
    public Action OnLoadEndReward { get; set; }

    public Action OnAnyFailedLoad { get; set; }
    public Action OnAnyGetReward { get; set; }
    public Action OnCloseInterstitial { get; set; }

    private bool IsGetReward { get; set; }
    private bool IsBeginShowAnyInterstitial { get; set; }

    public bool IsInitialize { get; set; }


    public bool IsAllAdsReady => IsInitialize && InterstitialAdUnitIDs.All(MaxSdk.IsInterstitialReady) &&
                                 RewardedAdUnitIDs.All(MaxSdk.IsRewardedAdReady);

    private bool IsAdReady(string id)
    {
        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            return IsInitialize && MaxSdk.IsInterstitialReady(id);
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            return IsInitialize && MaxSdk.IsRewardedAdReady(id);
        }
        else if (id == BannerAdUnitID)
        {
            return true;
        }

        return false;
    }

    public async UniTask InitAds(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;

        //リワード　インステ　バナー
        //OnAdLoaded
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnLoadedEvent;
        //OnAdLoadFailed
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnLoadFailedEvent;

        //OnAdDisplayFailed
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnFailedToDisplayEvent;


        //OnAdDisplayed
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnDisplayedEvent;

        //OnAdClicked
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnClickedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnClickedEvent;

        //OnAdHidden
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnHiddenEvent;


        //OnAdReceived
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        //その他（バナー用）
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        //収益
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        var adUnitIds = new List<string>();
        if (InterstitialAdUnitIDs.Count > 0) adUnitIds.AddRange(InterstitialAdUnitIDs);
        if (RewardedAdUnitIDs.Count > 0) adUnitIds.AddRange(RewardedAdUnitIDs);
        if (BannerAdUnitID.Length > 0) adUnitIds.Add(BannerAdUnitID);
        var adUnitIdDistinct = adUnitIds.Distinct();


        MaxSdk.SetSdkKey(sdkKey);
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(false);
        if (adUnitIdDistinct.Any())
        {
            MaxSdk.InitializeSdk(adUnitIdDistinct.ToArray());
        }
        else
        {
            MaxSdk.InitializeSdk();
        }


        token.ThrowIfCancellationRequested();
        await UniTask.WaitUntil(() => IsInitialize, PlayerLoopTiming.Update, token);
    }

    private void OnMaxInitialized(MaxSdkBase.SdkConfiguration configuration)
    {
        if (!MaxSdk.IsInitialized()) return;
        IsInitialize = true;
#if DEVELOPMENT_BUILD
            //デバッガーを表示
            MaxSdk.ShowMediationDebugger();
#endif
    }

    public void LoadAds()
    {
        var adUnitIds = new List<string>();
        adUnitIds.AddRange(InterstitialAdUnitIDs);
        adUnitIds.AddRange(RewardedAdUnitIDs);
        adUnitIds.Add(BannerAdUnitID);
        var adUnitIdDistinct = adUnitIds.Distinct();
        //広告読み込み
        foreach (var id in adUnitIdDistinct)
        {
            LoadAd(id);
        }
    }


    private void UpdateLastShowAdTime()
    {
        LastShowAdTime = Time.realtimeSinceStartup;
    }


    //広告読み込み
    private void LoadAd(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        if (GetLoadingDictionary(BeginLoadAds, id)) return;
        SetLoadingDictionary(BeginLoadAds, id, true);

        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            MaxSdk.LoadInterstitial(id);
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            if (LinkObject) OnLoadStartReward?.Invoke();
            MaxSdk.LoadRewardedAd(id);
        }
        else if (id == BannerAdUnitID)
        {
            if (CheckIAPNoAd()) return;
            MaxSdk.CreateBanner(id, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(id, "adaptive_banner", "true");
            MaxSdk.SetBannerBackgroundColor(id, Color.white);
            MaxSdk.StartBannerAutoRefresh(id);
        }
    }

    private bool CheckIAPNoAd()
    {
//        return IAPServer.GetActiveFlagLink(IAPDataSet.ProductName.NoADs);
        return false;
    }

    //読み込み完了
    private void OnLoadedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        SetLoadingDictionary(BeginLoadAds, id, false);
        SetAttemptDictionary(RetryAttemptAds, id, 0);


        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            if (LinkObject) OnLoadEndReward?.Invoke();
        }
        else if (id == BannerAdUnitID)
        {
            MaxSdk.ShowBanner(BannerAdUnitID);
        }
    }

    //読み込み失敗
    private void OnLoadFailedEvent(string id, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("ロード失敗:" + errorInfo.AdLoadFailureInfo);
        SetLoadingDictionary(BeginLoadAds, id, false);
        AddAttemptDictionary(RetryAttemptAds, id, 1);
        var retryDelay = Mathf.Pow(2, Mathf.Min(6, GetAttemptDictionary(RetryAttemptAds, id)));
        DOVirtual.DelayedCall(retryDelay, () => LoadAd(id));

        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            OnAnyFailedLoad?.Invoke();
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
        }
        else if (id == BannerAdUnitID)
        {
        }
    }


    //インステ広告表示
    public void ShowInterstitialAd(int idNumber)
    {
        if (IsDontAds) return;

        var beginTime = Time.realtimeSinceStartup;
        if (beginTime - LastShowAdTime < interval) return;

        //既にいずれかのインステが表示されている
        if (IsBeginShowAnyInterstitial) return;
        var id = InterstitialAdUnitIDs[idNumber % InterstitialAdUnitIDs.Count];
        MaxSdk.ShowInterstitial(id);
        IsBeginShowAnyInterstitial = true;
    }

    // リワード広告表示
    public void ShowRewardedAd(int idNumber, Action startAction, Action endAction, Action getRewardEvent,
        Action failedRewardEvent, Button button,
        GameObject loadingObject,
        TokenObject tokenObject, RewardType rewardType, int level)
    {
        if (IsDontAds)
        {
            getRewardEvent?.Invoke();
            AnalyticsManager.RewardEvent(rewardType, level);
            return; //デバッグ環境ではリワード非表示
        }


        tokenObject.ResetToken();
        LinkObject = tokenObject.gameObject;
        IsGetReward = false;


        OnFailedReward = failedRewardEvent;
        OnLoadStartReward = () =>
        {
            button.interactable = false;
            loadingObject.SetActive(true);
            startAction?.Invoke();
        };
        OnLoadEndReward = () =>
        {
            button.interactable = true;
            loadingObject.SetActive(false);
            endAction?.Invoke();
        };

        OnGetReward = getRewardEvent;
        OnGetReward += () => { AnalyticsManager.RewardEvent(rewardType, level); };


        OnLoadStartReward?.Invoke();


        var id = RewardedAdUnitIDs[idNumber % RewardedAdUnitIDs.Count];
        if (!IsInitialize || !IsAdReady(id))
        {
            if (LinkObject) OnFailedReward?.Invoke();
            OnLoadEndReward?.Invoke();
            return;
        }

        var ct = tokenObject.CancelToken;
        _doErrorTimer?.Kill();
        _doErrorTimer = DOVirtual.DelayedCall(5f, tokenObject.Cancel); //リワードを表示しようとしてから５秒間表示できなければ失敗
        _doErrorTimer.SetLink(tokenObject.gameObject);

        LoadReward(RewardedAdUnitIDs[idNumber % RewardedAdUnitIDs.Count], ct).SuppressCancellationThrow().ContinueWith(
            isCancel =>
            {
                if (!isCancel) return;
                if (LinkObject) OnLoadEndReward?.Invoke();
                if (LinkObject) OnFailedReward?.Invoke();

                OnGetReward = null;
                OnFailedReward = null;
                OnLoadStartReward = null;
                OnLoadEndReward = null;
                OnAnyFailedLoad?.Invoke();
            }
        ).Forget();
    }

    private async UniTask LoadReward(string id, CancellationToken ct)
    {
        while (!IsAdReady(id))
        {
            LoadAd(id);
            ct.ThrowIfCancellationRequested();
            await UniTask.WaitUntil(() => !GetLoadingDictionary(BeginLoadAds, id), cancellationToken: ct);
        }

        MaxSdk.ShowRewardedAd(id);
    }

    //表示成功
    private void OnDisplayedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            //SoundManager.Main.MuteForAds();
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            //SoundManager.Main.MuteForAds();
            OnLoadEndReward?.Invoke();
        }
        else if (id == BannerAdUnitID)
        {
        }
    }


    //表示失敗
    private void OnFailedToDisplayEvent(string id, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ShowFail" + id +
                  "\n[Message]" + errorInfo.Message +
                  "\n[Code]" + errorInfo.Code +
                  "\n[Waterfall]" + errorInfo.WaterfallInfo.Name +
                  "\n[AdLoadFailureInfo]" + errorInfo.AdLoadFailureInfo +
                  "\n[MediatedNetworkErrorMessage]" + errorInfo.MediatedNetworkErrorMessage
        );

        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            IsBeginShowAnyInterstitial = false;
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            if (LinkObject) OnFailedReward?.Invoke();
        }
        else if (id == BannerAdUnitID)
        {
        }

        LoadAd(id);
    }


    //広告クリック
    private void OnClickedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
    }


    //広告を閉じた
    private void OnHiddenEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
        //DOVirtual.DelayedCall(0.5f, () => SoundManager.Main.UpdateMute());

        LoadAd(id);

        if (InterstitialAdUnitIDs.Any(b => b == id))
        {
            IsBeginShowAnyInterstitial = false;
            UpdateLastShowAdTime();
            OnCloseInterstitial?.Invoke();

            //インステ表示終了
            Analytics.AdInterDisplay(adInfo);
            AdjustSDKProvider.InterstitialEvent(adInfo);
        }
        else if (RewardedAdUnitIDs.Any(b => b == id))
        {
            if (IsGetReward)
            {
                return; //もしリワードを獲得できているなら失敗ではない
            }

            if (LinkObject) OnFailedReward?.Invoke();
        }
        else if (id == BannerAdUnitID)
        {
        }
    }


    // リワード広告報酬獲得
    private void OnRewardedAdReceivedRewardEvent(string msg, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        UpdateLastShowAdTime();

        IsGetReward = true;
        AdjustSDKProvider.RewardEvent(adInfo);
        Analytics.AdRewardDisplay(adInfo);
        if (LinkObject)
        {
            OnGetReward?.Invoke();
            OnAnyGetReward?.Invoke();
        }
    }


    //バナーの高さを取得
    public int GetAdaptiveBannerHeight()
    {
//        if (!MaxSdk.IsInitialized()) return 0f;   //初期化が完了するより前にこちらの値にアクセスする事ができた

        if (CheckIAPNoAd()) return 0;
        return (int)(MaxSdkUtils.GetAdaptiveBannerHeight() * 2.25f);


        /*戻り値がピクセルではなく、大体３倍の数が帰ってくると海外のスレッドにあったため、まずはそのような対応(実機で、３倍だと少し大きすぎたので調整)
        https://github.com/AppLovin/AppLovin-MAX-Unity-Plugin/issues/130
        常に正確な数値を取るにはアダプティブバナーをOffにするように記載があったけれど、アダプティブバナーをつけないとリジェクトされる場合があるそうなので、こちらを優先
        */
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(BannerAdUnitID);
        MaxSdk.StopBannerAutoRefresh(BannerAdUnitID);
        MaxSdk.DestroyBanner(BannerAdUnitID);
    }

    private void OnBannerAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdExpandedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdCollapsedEvent(string id, MaxSdkBase.AdInfo adInfo)
    {
    }


    //広告による収益が発生した
    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        var revenue = adInfo.Revenue;
        // Miscellaneous data
        var countryCode =
            MaxSdk.GetSdkConfiguration()
                .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        var networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        var placement = adInfo.Placement; // The placement this ad's postbacks are tied to


        //AdjustSDKProvider.TrackAdRevenue(adInfo);
        SendFirebaseEvent(adInfo);
    }

    //Firebase送信用
    public void SendFirebaseEvent(MaxSdkBase.AdInfo adInfo)
    {
        // try
        // {
        //     // 以下はMAXの場合、revenueが-1のときはエラーなので、バリデーションルールをそこに合わせる
        //     // https://dash.applovin.com/documentation/mediation/unity/getting-started/advanced-settings#impression-level-user-revenue-api:~:text=The%20value%20of%20Revenue%20may%20be%20%2D1%20in%20the%20case%20of%20an%20error.
        //     if (adInfo.Revenue < 0) return;
        //
        //     // https://firebase.google.com/docs/analytics/measure-ad-revenue?hl=ja
        //     var revenue = adInfo.Revenue;
        //     var impressionParameters = new[]
        //     {
        //         new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
        //         new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
        //         new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
        //         new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
        //         new Firebase.Analytics.Parameter("value", revenue),
        //         new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        //     };
        //     Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        // }
        // catch (Exception e)
        // {
        //     Debug.LogWarning(e);
        // }
    }
}