using System;
using System.Collections.Generic;
namespace GaminCat.Analytics
{
    internal class EventQueue
    {
        private readonly Queue<GameEvent> _queue = new Queue<GameEvent>();

        public void Enqueue(GameEvent gameEvent)
        {
            var clonedParams = new Dictionary<string, object>(gameEvent.eventParams);
            _queue.Enqueue(new GameEvent(gameEvent.eventName, clonedParams));
        }

        public void ProcessEventQueue(Action<GameEvent> action)
        {
            while (_queue.Count > 0)
            {
                action(_queue.Dequeue());
            }
        }
    }
}
