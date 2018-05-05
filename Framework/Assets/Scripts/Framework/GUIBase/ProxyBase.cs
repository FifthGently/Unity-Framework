namespace Frameworks
{
    using System;
    using System.Collections.Generic;

    public class ProxyBase
    {
        private Dictionary<EventEnum, EventListenerDelegate> eunmEventCallbackMap;
        private Dictionary<string, EventListenerDelegate> flashEventCallbackMap;

        public virtual bool Initialize()
        {
            InitEvent();
            return true;
        }

        public virtual void InitEvent() { }

        public virtual void Update() { }

        public virtual void Destroy()
        {
            RemoveAllEventListeners();
            eunmEventCallbackMap = null;
            flashEventCallbackMap = null;
        }

        public void AddEventListener(EventEnum eventType, EventListenerDelegate callback)
        {
            if (callback == null) return;

            if (eunmEventCallbackMap == null)
                eunmEventCallbackMap = new Dictionary<EventEnum, EventListenerDelegate>();

            if (eunmEventCallbackMap.ContainsKey(eventType))
                eunmEventCallbackMap[eventType] += callback;
            else
                eunmEventCallbackMap.Add(eventType, callback);

            EventProxy.AddEventListener(eventType, callback);
        }

        public void AddEventListener(string eventType, EventListenerDelegate callback)
        {
            if (callback == null) return;

            if (flashEventCallbackMap == null)
                flashEventCallbackMap = new Dictionary<string, EventListenerDelegate>();

            if (flashEventCallbackMap.ContainsKey(eventType))
                flashEventCallbackMap[eventType] += callback;
            else
                flashEventCallbackMap.Add(eventType, callback);

            EventProxy.AddEventListener(eventType, callback);
        }

        public void RemoveEventListener(EventEnum eventType, EventListenerDelegate callback)
        {
            if (callback == null) return;

            if (eunmEventCallbackMap == null) return;

            if (eunmEventCallbackMap.ContainsKey(eventType))
                eunmEventCallbackMap[eventType] -= callback;

            EventProxy.RemoveEventListener(eventType, callback);
        }

        public void RemoveEventListener(string eventType, EventListenerDelegate callback)
        {
            if (callback == null) return;

            if (flashEventCallbackMap == null) return;

            if (flashEventCallbackMap.ContainsKey(eventType))
                flashEventCallbackMap[eventType] -= callback;

            EventProxy.RemoveEventListener(eventType, callback);
        }

        public void RemoveAllEventListeners()
        {
            if (eunmEventCallbackMap != null)
            {
                foreach (EventEnum eventType in eunmEventCallbackMap.Keys)
                {
                    EventListenerDelegate de = eunmEventCallbackMap[eventType];
                    if (de == null) continue;
                    Delegate[] lis = de.GetInvocationList();
                    for (int i = 0; i < lis.Length; i++)
                    {
                        EventProxy.RemoveEventListener(eventType, lis[i] as EventListenerDelegate);
                    }
                }
                eunmEventCallbackMap.Clear();
            }

            if (flashEventCallbackMap != null)
            {
                foreach (string eventType in flashEventCallbackMap.Keys)
                {
                    EventListenerDelegate de = flashEventCallbackMap[eventType];
                    if (de == null) continue;
                    Delegate[] lis = de.GetInvocationList();
                    for (int i = 0; i < lis.Length; i++)
                    {
                        EventProxy.RemoveEventListener(eventType, lis[i] as EventListenerDelegate);
                    }
                }
                flashEventCallbackMap.Clear();
            }
        }

        public void BroadcastEvent(BaseEvent baseEvent)
        {
            EventProxy.BroadcastEvent(baseEvent);
        }

        public void BroadcastEvent(EventEnum eventType, object date)
        {
            EventProxy.BroadcastEvent(eventType, date);
        }

        public void BroadcastEvent(string eventType, object date)
        {
            EventProxy.BroadcastEvent(eventType, date);
        }

        public void BroadcastEvent(EventEnum eventType, BaseEvent baseEvent = null)
        {
            EventProxy.BroadcastEvent(eventType, baseEvent);
        }

        public void BroadcastEvent(string eventType, BaseEvent baseEvent = null)
        {
            EventProxy.BroadcastEvent(eventType, baseEvent);
        }
    }
}
