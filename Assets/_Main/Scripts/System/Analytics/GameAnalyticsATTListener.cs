using System;
using GameAnalyticsSDK;

namespace GaminCat.Analytics
{
    public class GameAnalyticsATTListener : IGameAnalyticsATTListener
    {
        private Action _onInitialized;

        public void GameAnalyticsATTListenerNotDetermined()
        {
            _onInitialized?.Invoke();
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            _onInitialized?.Invoke();
        }

        public void GameAnalyticsATTListenerDenied()
        {
            _onInitialized?.Invoke();
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            _onInitialized?.Invoke();
        }

        public void WhenInitialized(Action action)
        {
            _onInitialized = action;
        }
    }
}
