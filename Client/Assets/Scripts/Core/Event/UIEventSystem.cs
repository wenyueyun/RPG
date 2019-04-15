using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Core.Event
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/26 16:05:49 
	* 描述： UIEventSystem   UI事件
	*************************/
    public class UIEventSystem : EventTrigger
    {
        //添加事件组件
        public static UIEventSystem Get(GameObject obj)
        {
            UIEventSystem evtSystem = obj.GetComponent<UIEventSystem>();
            if (evtSystem == null)
            {
                evtSystem = obj.AddComponent<UIEventSystem>();
            }
            return evtSystem;
        }

        //添加事件
        public void AddEventListener(EventTriggerType evtType, UnityAction<BaseEventData> handle)
        {
            Entry entry = new Entry();
            entry.eventID = evtType;
            entry.callback.AddListener(handle);

            if (triggers == null)
            {
                triggers = new List<Entry>();
            }

            triggers.Add(entry);
        }

        //删除事件
        public static void ClearEvents(GameObject obj)
        {
            UIEventSystem evtSystem = obj.GetComponent<UIEventSystem>();
            if (evtSystem != null)
            {
                evtSystem.triggers.Clear();
            }
        }
    }
}
