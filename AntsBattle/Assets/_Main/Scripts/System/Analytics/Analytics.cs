using System.Collections.Generic;
using GameAnalyticsSDK;

namespace GaminCat.Analytics
{
    public static class Analytics
    {
        private static readonly Dictionary<string, object> _customEventPayload = new Dictionary<string, object>();

        public static class Key
        {
            public const string play_session = "play_session";
            public const string game_started = "game_started";
            public const string level_attempt = "level_attempt";
            public const string level_started = "level_started";
            public const string level_restart = "level_restarted";
            public const string level_skip = "level_skipped";
            public const string level_fail = "level_failed";
            public const string level_complete = "level_complete";
            public const string all_levels_complete = "completed_all_levels";
            public const string round_started = "round_started";
            public const string round_complete = "round_complete";
            public const string tutorial_attempt = "tutorial_attempt";
            public const string tutorial_complete = "tutorial_complete";
            public const string high_score = "high_score";
            public const string item_upgrade = "item_upgrade";
            public const string character_upgrade = "character_upgrade";
            public const string upgrade_purchase = "upgrade_purchase";
            public const string building_upgrade = "building_upgrade";
            public const string hint_usage = "hint_usage";
            public const string spent_credits = "spent_credits";
            public const string purchase = "purchase";
            public const string content_unlocked = "content_unlocked";

            public static class Param
            {
                public const string session_count = "session_count";
                public const string level = "level";
                public const string rank = "rank";
                public const string upgrade_level = "upgrade_level";
                public const string tutorial = "tutorial";
                public const string item = "item";
                public const string upgrade = "upgrade";
                public const string character = "character";
                public const string building = "building";
                public const string score = "score";
                public const string success = "success";
                public const string virtual_currency_name = "virtual_currency_name";
                public const string num_attempts = "num_attempts";
                public const string test_group = "test_group";
                public const string product_identifier = "product_identifier";
                public const string currency = "currency";
                public const string price = "price";
                public const string quantity = "quantity";
                public const string order_id = "order_id";
                public const string content = "content";
                public const string content_id = "content_id";
                public const string result = "result";
                public const string placement = "placement";
                public const string cost = "cost";

                public static class PostFix
                {
                    public const string transaction_amount = "transaction_amount";
                    public const string remaining = "remaining";
                    public const string total_earned = "total_earned";
                    public const string total_spent = "total_spent";
                }
            }
        }

        // public static void LevelStarted(int level)
        // {
        //     LogEvent(Key.level_started, new Dictionary<string, object>()
        //     {
        //         { "level", level }
        //     });
        // }
        //
        // public static void LevelComplete(int level, bool isSkip)
        // {
        //     LogEventWithSkipFlag(Key.level_complete, new Dictionary<string, object>()
        //     {
        //         { "level", level }
        //     }, isSkip);
        // }
        //
        // public static void LevelFail(int level)
        // {
        //     LogEvent(Key.level_fail, new Dictionary<string, object>()
        //     {
        //         { "level", level }
        //     });
        // }

        private static void LogEvent(string eventName, Dictionary<string, object> eventParams)
        {
            GameEvent gameEvent = new GameEvent
            {
                eventName = eventName,
                eventParams = eventParams
            };

            GameAnalytics.LogEvent(gameEvent);
        }


        private static void LogEventWithSkipFlag(string eventName, Dictionary<string, object> eventParams, bool isSkip)
        {
            GameEvent gameEvent = new GameEvent
            {
                eventName = eventName,
                eventParams = eventParams
            };

            GameAnalytics.LogEventWithScore(gameEvent, isSkip ? 0 : 1); //0　スキップ 1クリア
        }

        //広告関連
        public static void AdInterDisplay(MaxSdkBase.AdInfo adInfo)
        {
            var networkName = string.IsNullOrEmpty(adInfo.NetworkName) ? "unknown" : adInfo.NetworkName;
            var placement = string.IsNullOrEmpty(adInfo.Placement) ? "unknown" : adInfo.Placement;
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, networkName, placement);
        }

        public static void AdRewardDisplay(MaxSdkBase.AdInfo adInfo)
        {
            var networkName = string.IsNullOrEmpty(adInfo.NetworkName) ? "unknown" : adInfo.NetworkName;
            var placement = string.IsNullOrEmpty(adInfo.Placement) ? "unknown" : adInfo.Placement;
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, networkName, placement);
        }

     
        
//         public static void ChallengeEvent(string logData, int level)
//         {
//             //デザインイベントを送信するテスト、次のバージョンでこれが正しく動作しているようなら、Failedをその他として使用する運用をやめる
//             GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Challenge:{logData}", level);
//
// #if UNITY_EDITOR
//             Log.Debug($"Challenge:{logData}" + ":" + level);
// #endif
//         }
    }
}