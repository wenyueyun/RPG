using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Game.Core.Event
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/26 15:10:27 
	* 描述： EventDispatcher 
	*************************/
    public class EventObject
    {
        private Dictionary<string, BaseEvent> eventDic = new Dictionary<string, BaseEvent>();
        public EventObject()
        {

        }
        public void AddEvent(string eventName, Action<string, object> handle)
        {
            BaseEvent evt;
            if (eventDic.TryGetValue(eventName, out evt))
            {
                evt.action += handle;
            }
            else
            {
                evt = new BaseEvent();
                evt.eventName = eventName;
                evt.action += handle;
                eventDic.Add(eventName, evt);
            }
        }

        public void RemoveEvent(string eventName, Action<string, object> handle)
        {
            BaseEvent evt;
            if (eventDic.TryGetValue(eventName, out evt))
            {
                evt.action -= handle;
            }
        }

        public void Fire(string eventName, object obj = null)
        {
            BaseEvent evt;
            if (eventDic.TryGetValue(eventName, out evt) && evt.action != null)
            {
                evt.action(eventName, obj);
            }
        }
    }

    public class BaseEvent
    {
        public string sender;
        public string eventName;
        public Action<string, object> action;
    }

}
