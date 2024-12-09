using System.Collections.Generic;
using GameAnalyticsSDK;

namespace GaminCat.Analytics
{
    public struct GameEvent
    {
        public string eventName;
        public Dictionary<string, object> eventParams;

        public GameEvent(string eventName)
        {
            this.eventName = eventName;
            this.eventParams = new Dictionary<string, object>();
        }

        public GameEvent(string eventName, Dictionary<string, object> eventParams)
        {
            this.eventName = eventName;
            this.eventParams = eventParams;
        }
    }
}
