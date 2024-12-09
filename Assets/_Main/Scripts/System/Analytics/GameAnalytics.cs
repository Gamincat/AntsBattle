using System.Collections.Generic;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using UnityEngine;
using UnityEngine.Analytics;

namespace GaminCat.Analytics
{
    public static class GameAnalytics
    {
        private static readonly GameAnalyticsATTListener _attListener = new GameAnalyticsATTListener();
        private static readonly EventQueue _eventQueue = new EventQueue();

        private static readonly Dictionary<string, GAProgressionStatus> _progressEventConvertMap =
            new Dictionary<string, GAProgressionStatus>
            {
                { Analytics.Key.level_started, GAProgressionStatus.Start },
                { Analytics.Key.level_complete, GAProgressionStatus.Complete },
                { Analytics.Key.level_fail, GAProgressionStatus.Fail }
            };

        public static bool Initialized { get; private set; }

        public static void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            var gameAnalytics = new GameObject("GameAnalyticsObj");
            gameAnalytics.AddComponent<GameAnalyticsSDK.GameAnalytics>();
            gameAnalytics.AddComponent<GA_SpecialEvents>();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _attListener.WhenInitialized(InternalInitialize);
                GameAnalyticsSDK.GameAnalytics.RequestTrackingAuthorization(_attListener);
            }
            else
            {
                InternalInitialize();
            }
        }

        private static void InternalInitialize()
        {
            GameAnalyticsSDK.GameAnalytics.Initialize();
            Initialized = true;
            _eventQueue.ProcessEventQueue(LogEvent);
        }


        public static void LogEvent(GameEvent gameEvent)
        {
            if (!Initialized)
            {
                _eventQueue.Enqueue(gameEvent);
                return;
            }

            if (!_progressEventConvertMap.ContainsKey(gameEvent.eventName))
            {
                return;
            }

            var payload = gameEvent.eventParams;
            var hasLevelValue = payload.TryGetValue(Analytics.Key.Param.level, out var level);
            Debug.Assert(hasLevelValue);

            var status = _progressEventConvertMap[gameEvent.eventName];

            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(status, $"{level}");
        }

        public static void LogEventWithScore(GameEvent gameEvent, int score)
        {
            if (!Initialized)
            {
                _eventQueue.Enqueue(gameEvent);
                return;
            }

            if (!_progressEventConvertMap.ContainsKey(gameEvent.eventName))
            {
                return;
            }

            var payload = gameEvent.eventParams;
            var hasLevelValue = payload.TryGetValue(Analytics.Key.Param.level, out var level);
            Debug.Assert(hasLevelValue);

            var status = _progressEventConvertMap[gameEvent.eventName];
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(status, $"{level}", score);
        }
    }
}