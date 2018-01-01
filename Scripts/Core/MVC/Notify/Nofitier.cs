using UnityEngine;
using System.Collections.Generic;
using System;
namespace Core.MVC
{
    public class Nofitier
    {
        public delegate void StandardDelegate(params object[] arg1);
        private Dictionary<string, StandardDelegate> m_evenMap = new Dictionary<string, StandardDelegate>();

        public void AddEventHandler(string eventName, StandardDelegate pFun)
        {
            if (!m_evenMap.ContainsKey(eventName))
            {
                m_evenMap[eventName] = pFun;
            }
            else
            {
                m_evenMap[eventName] += pFun;
            }
        }

        public void RemoveEventHandler(string eventName, StandardDelegate pFun)
        {
            if (m_evenMap.ContainsKey(eventName))
            {
                if (m_evenMap[eventName] != null)
                {
                    m_evenMap[eventName] -= pFun;
                }
            }
        }

        public void RaiseEvent(string eventName, params object[] e)
        {
            StandardDelegate fun = null;
            if (m_evenMap.TryGetValue(eventName, out fun))
            {
                if (fun != null)
                {
                    fun(e);
                }
            }
        }

        public bool HasEvent(string evntName)
        {
            return m_evenMap.ContainsKey(evntName);
        }

        public void RemoveAllEventHandler(string eventName)
        {
            if (m_evenMap.ContainsKey(eventName))
            {
                if (m_evenMap[eventName] != null)
                {
                    Delegate[] delegArr = m_evenMap[eventName].GetInvocationList();
                    for (int i = 0; i < delegArr.Length; ++i)
                    {
                        StandardDelegate deleg = (StandardDelegate)delegArr[i];
                        RemoveEventHandler(eventName, deleg);
                    }
                }
            }
        }

        public void ClearEventHandle()
        {
            if (m_evenMap.Count <= 0) return;
            var Keys = new List<string>(m_evenMap.Keys);
            for (int i = 0; i < Keys.Count; ++i)
            {
                RemoveAllEventHandler(Keys[i]);
            }
            m_evenMap.Clear();
        }
    }
}

