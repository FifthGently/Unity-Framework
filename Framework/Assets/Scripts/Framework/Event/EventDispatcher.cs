namespace Frameworks
{
    using System.Collections.Generic;
    using System.Linq;

    public class EventDispatcher
    {
        private Dictionary<EventEnum, EventListenerDelegate> eunmEventCallbackMap;
        private Dictionary<string, EventListenerDelegate> flashEventCallbackMap;

        public bool AddEventListener(EventEnum type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (eunmEventCallbackMap == null)
                eunmEventCallbackMap = new Dictionary<EventEnum, EventListenerDelegate>();

            if (eunmEventCallbackMap.ContainsKey(type))
                eunmEventCallbackMap[type] += callback;
            else
                eunmEventCallbackMap.Add(type, callback);

            return true;
        }

        public bool AddEventListener(string type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (flashEventCallbackMap == null)
                flashEventCallbackMap = new Dictionary<string, EventListenerDelegate>();

            if (flashEventCallbackMap.ContainsKey(type))
                flashEventCallbackMap[type] += callback;
            else
                flashEventCallbackMap.Add(type, callback);
            
            return true;
        }

        public bool RemoveEventListener(EventEnum type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (eunmEventCallbackMap == null) return false;

            if (eunmEventCallbackMap.ContainsKey(type))
            {
                eunmEventCallbackMap[type] -= callback;
                return true;
            }
            else return false;
        }

        public bool RemoveEventListener(EventEnum type)
        {
            if (eunmEventCallbackMap == null) return false;

            if(eunmEventCallbackMap.ContainsKey(type))
            {
                eunmEventCallbackMap.Remove(type);
                return true;
            }
            else return false;
        }

        public bool RemoveEventListener(string type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (flashEventCallbackMap == null) return false;

            if (flashEventCallbackMap.ContainsKey(type))
            {
                flashEventCallbackMap[type] -= callback;
                return true;
            }
            else return false;
        }

        public bool RemoveEventListener(string type)
        {
            if (flashEventCallbackMap == null) return false;

            if (flashEventCallbackMap.ContainsKey(type))
            {
                flashEventCallbackMap.Remove(type);
                return true;
            }
            else return false;
        }

        public void RemoveAll()
        {
            if (eunmEventCallbackMap != null)
                eunmEventCallbackMap.Clear();

            if (flashEventCallbackMap != null)
                flashEventCallbackMap.Clear();
        }

        public bool BroadcastEvent(EventEnum type, BaseEvent baseEvent)
        {
            if (eunmEventCallbackMap == null) return false;

            if (eunmEventCallbackMap.ContainsKey(type))
            {
                if (eunmEventCallbackMap[type] != null)
                {
                    eunmEventCallbackMap[type](baseEvent);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool BroadcastEvent(string type, BaseEvent baseEvent)
        {
            if (flashEventCallbackMap == null) return false;

            if (flashEventCallbackMap.ContainsKey(type))
            {
                if (flashEventCallbackMap[type] != null)
                {
                    flashEventCallbackMap[type](baseEvent);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// 暂时只适用于 广播用string注册的事件
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        public bool BroadcastEvent(BaseEvent baseEvent)
        {
            if (flashEventCallbackMap == null) return false;

            if (baseEvent == null) return false;

            string type = baseEvent.Type;
            if (flashEventCallbackMap.ContainsKey(type))
            {
                if (flashEventCallbackMap[type] != null)
                {
                    flashEventCallbackMap[type](baseEvent);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool HasEventListener(EventEnum type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (eunmEventCallbackMap == null) return false;

            List<System.Delegate> list;
            if (eunmEventCallbackMap.ContainsKey(type))
            {
                list = eunmEventCallbackMap[type].GetInvocationList().ToList();
                return list.Contains<System.Delegate>(callback);
            }
            else return false;
        }

        public bool HasEventListener(string type, EventListenerDelegate callback)
        {
            if (callback == null) return false;

            if (flashEventCallbackMap == null) return false;

            List<System.Delegate> list;
            if (flashEventCallbackMap.ContainsKey(type))
            {
                list = flashEventCallbackMap[type].GetInvocationList().ToList();
                return list.Contains<System.Delegate>(callback);
            }
            else return false;
        }

        public void Destory()
        {
            RemoveAll();
            eunmEventCallbackMap = null;
            flashEventCallbackMap = null;
        }
    }
}
