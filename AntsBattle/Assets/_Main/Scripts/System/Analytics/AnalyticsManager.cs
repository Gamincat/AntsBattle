using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public enum ProgressType
    {
        UnlockStep,
        Skip,
        Reward
    }

    public enum SendType
    {
        BattleAntCount,//兵隊あり
        WorkAntCount,//増築あり
        FeederAntCount,//給餌あり
        UnlockStep,
        QueenLevel,
        WaveLevel,
        AllAntCount
    }


    public static void ProgressionEvent(GAProgressionStatus state, SendType sendType, int level)
    {
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
        Debug.Log("GA:state" + state + ":mode" + sendType + ":level" + level);
#endif
#if UNITY_EDITOR
        return;
#endif
        GameAnalytics.NewProgressionEvent(state, sendType.ToString(), level.ToString(), "");
    }
    public static void RewardEvent(AdsManager.RewardType rewardType, int level)
    {
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
        Debug.Log("GA:rewardType" + rewardType + ":" + level);
#endif
#if UNITY_EDITOR
        return;
#endif
        var logData = rewardType.ToString();
        GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"RewardEvent:{logData}", level);
    }

    public static void DesignEvent(string message)
    {
      
#if UNITY_EDITOR
        return;
#endif
        GA_Design.NewEvent(message , null , false );
    }

}