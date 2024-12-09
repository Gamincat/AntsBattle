using System;
using System.Collections.Generic;
using System.Threading;
using AdjustSdk;
//using com.adjust.sdk;
//using AdjustSdk;
//using AdjustSdk.Adjust;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public static class AdjustSDKProvider
{
    public static Adjust AdjustObject { get; private set; }

    private static string AppToken => AdjustParams.Instance.AppToken;
    public static bool IsInitialized { get; private set; }

    public static async UniTask Init(CancellationToken ct)
    {
        if (IsInitialized) return;

        var environment =
#if UNITY_EDITOR || DEBUG || DEVELOPMENT_BUILD
            AdjustEnvironment.Sandbox;
#else
        AdjustEnvironment.Production;
#endif
        var config = new AdjustConfig(AppToken, environment);
        config.LogLevel =
#if UNITY_EDITOR || DEBUG || DEVELOPMENT_BUILD
            AdjustLogLevel.Verbose
#else
            AdjustLogLevel.Warn
#endif
        ;


        config.ExternalDeviceId = SystemInfo.deviceUniqueIdentifier;
        config.IsSendingInBackgroundEnabled = true;
        //config.setLogDelegate(Debug.Log);


        // GDPR配慮の場合はここで更に処理が必要
        // https://help.adjust.com/ja/article/privacy-features-unity-sdk#provide-consent-data-to-google-digital-markets-act-compliance
        var adjustThirdPartySharing = new AdjustThirdPartySharing(null);
        adjustThirdPartySharing.AddGranularOption("google_dma", "eea", GDPR.IsGDPRTarget ? "1" : "0");
        if (GDPR.IsGDPRTarget)
        {
            //GDPR圏は同意ステータスが取れないので送信しない
            adjustThirdPartySharing.AddGranularOption("google_dma", "ad_personalization", "0");
            adjustThirdPartySharing.AddGranularOption("google_dma", "ad_user_data", "0");
        }

        Adjust.TrackThirdPartySharing(adjustThirdPartySharing);


        AdjustObject = new GameObject("Adjust").AddComponent<Adjust>();
        Adjust.InitSdk(config);


        await UniTask.DelayFrame(1, PlayerLoopTiming.Update, ct);
#if UNITY_EDITOR
        Debug.Log($"Adjust. Environment = {environment}");
#endif
        IsInitialized = true;
    }


    public static void LogEvent(string eventToken)
    {
        if (!IsInitialized)
        {
            return;
        }

        var adjustEvent = new AdjustEvent(eventToken);
        Adjust.TrackEvent(adjustEvent);
    }


    public static void RevenueEvent(UnityEngine.Purchasing.Product product)
    {
        if (!IsInitialized)
        {
            return;
        }

        // Productから価格と通貨を取得
        var price = (double)product.metadata.localizedPrice;
        var currency = product.metadata.isoCurrencyCode;

        // Adjustイベントを作成

        var token =
#if UNITY_ANDROID
                ""
#else
                ""
#endif
            ;

        var adjustEvent = new AdjustEvent(token);

        // 取得した価格と通貨をsetRevenueに渡す
        adjustEvent.SetRevenue(price, currency);
        //TransactionID
        adjustEvent.TransactionId = product.transactionID;

        adjustEvent.AddCallbackParameter("productID", product.definition.id);
        adjustEvent.AddCallbackParameter("transactionID", product.transactionID);


        // イベントをトラッキング
        Adjust.TrackEvent(adjustEvent);
    }

    public static void MiscEvent(string token)
    {
        if (!IsInitialized)
        {
            return;
        }

        // Adjustイベントを作成
        var adjustEvent = new AdjustEvent(token);
        // イベントをトラッキング
        Adjust.TrackEvent(adjustEvent);
    }

    public static void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
    {
        return; //既にAdjustで広告収益の情報が取れているのと(MTG時に口頭で確認)、Jigsortにこのような実装がなかったのでひとまず無効化
        /*
        var adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);

        adjustAdRevenue.setRevenue(adInfo.Revenue, "USD");
        adjustAdRevenue.setAdRevenueNetwork(adInfo.NetworkName);
        adjustAdRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
        adjustAdRevenue.setAdRevenuePlacement(adInfo.Placement);

        Adjust.trackAdRevenue(adjustAdRevenue);
        */
    }

    public static void TransactionEvent(string eventToken, string transactionID)
    {
        if (!IsInitialized)
        {
            return;
        }

        var adjustEvent = new AdjustEvent(eventToken);
        adjustEvent.TransactionId = transactionID;
        Adjust.TrackEvent(adjustEvent);
    }


    private static void TryPushCampaignEvent(string key, string[] array)
    {
        var count = PlayerPrefs.GetInt(key, 0);
        count++;
        PlayerPrefs.SetInt(key, count);

        if (count < 10 || count % 10 != 0) return;
        var index = (count / 10) - 1;

        if (index >= array.Length) return;
        MiscEvent(array[index]);
    }

    public static void InterstitialEvent(MaxSdkBase.AdInfo adInfo)
    {
        const string key = "InterstitialEventKey";
        var array = AdjustTokens.TokenInterstitials;
        TryPushCampaignEvent(key, array);
    }


    public static void RewardEvent(MaxSdkBase.AdInfo adInfo)
    {
        const string key = "RewardEventKey";
        var array = AdjustTokens.TokenRewards;
        TryPushCampaignEvent(key, array);
    }

    // public static void FossilEvent(Fossil myFossil)
    // {
    //     const string key = "FossilEventKey";
    //     var array = AdjustTokens.TokenFossils;
    //     TryPushCampaignEvent(key, array);
    // }
}