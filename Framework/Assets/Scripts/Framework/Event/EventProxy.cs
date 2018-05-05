namespace Frameworks
{
    public class EventProxy
    {
        private static EventDispatcher Dispatcher { get { return Singleton<EventDispatcher>.Instance; } }

        public static void Destroy()
        {
            Dispatcher.Destory();
        }

        public static void AddEventListener(EventEnum eventType, EventListenerDelegate callback)
        {
            Dispatcher.AddEventListener(eventType, callback);
        }

        public static void AddEventListener(string eventType, EventListenerDelegate callback)
        {
            Dispatcher.AddEventListener(eventType, callback);
        }

        public static void RemoveEventListener(EventEnum eventType, EventListenerDelegate callback)
        {
            Dispatcher.RemoveEventListener(eventType, callback);
        }

        public static void RemoveEventListener(string eventType, EventListenerDelegate callback)
        {
            Dispatcher.RemoveEventListener(eventType, callback);
        }

        public static void RemoveAllEventListeners()
        {
            Dispatcher.RemoveAll();
        }

        public static void BroadcastEvent(BaseEvent baseEvent)
        {
            Dispatcher.BroadcastEvent(baseEvent);
        }

        public static void BroadcastEvent(EventEnum eventType, object date)
        {
            BaseEvent baseEvent = new BaseEvent(eventType.ToString(), date);
            Dispatcher.BroadcastEvent(eventType, baseEvent);
        }

        public static void BroadcastEvent(string eventType, object date)
        {
            BaseEvent baseEvent = new BaseEvent(eventType, date);
            Dispatcher.BroadcastEvent(baseEvent);
        }

        public static void BroadcastEvent(EventEnum eventType, BaseEvent baseEvent = null)
        {
            Dispatcher.BroadcastEvent(eventType, baseEvent);
        }

        public static void BroadcastEvent(string eventType, BaseEvent baseEvent = null)
        {
            Dispatcher.BroadcastEvent(eventType, baseEvent);
        }

        public static void AddEventListener(string p, object OnBtnDown)
        {
            throw new System.NotImplementedException();
        }
    }
}
