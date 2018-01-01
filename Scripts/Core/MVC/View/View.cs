using UnityEngine;
using System.Collections.Generic;
using System;

namespace Core.MVC
{
    public abstract class View : MonoBehaviour
    {

        protected Model m_bindModel;
        Dictionary<string, Nofitier.StandardDelegate> m_AllFun = new Dictionary<string, Nofitier.StandardDelegate>();

        public bool V_IsLoaded = false;

        public string V_PrefabPath;

        public virtual void Init(Model model)
        {
            m_bindModel = model;
        }

        protected void  BindModel(Enum attribute, Nofitier.StandardDelegate fun)
        {
            if (null != m_bindModel)
            {
                string keyName = string.Format("{0}{1}", m_bindModel.GetModelName(), attribute);
                if (m_AllFun.ContainsKey(keyName))
                {
                    m_bindModel.RemoveEventHandler(keyName, m_AllFun[keyName]);
                    m_AllFun.Remove(keyName);
                }
                m_bindModel.AddEventHandler(keyName, fun);
                m_AllFun.Add(keyName, fun);
            }
            {
                Debug.LogError("没有绑定数据模型");
            }
        }

        protected void UnBindModel(Enum attribute, Nofitier.StandardDelegate fun)
        {
            if (null != m_bindModel)
            {
                string keyName = string.Format("{0}{1}", m_bindModel.GetModelName(), attribute);
                if (m_AllFun.ContainsKey(keyName))
                {
                    m_bindModel.RemoveEventHandler(keyName, fun);
                    m_AllFun.Remove(keyName);
                }
            }
            else
            {
                Debug.LogError("没有绑定数据模型");
            }
        }

        protected void UnAllBindModel(Enum attribute)
        {
            if (null == m_bindModel)
            {
                return;
            }m_bindModel.RemoveAllEventHandler(m_bindModel.GetModelName() + attribute);
        }

        public virtual void F_Reset()
        {
            ClearModelAndBind();
        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {
            ClearModelAndBind();
        }

        private void ClearModelAndBind()
        {
            if (m_bindModel != null)
            {
                if (m_AllFun.Count > 0)
                {
                    var iter = m_AllFun.GetEnumerator();
                    while(iter.MoveNext())
                    {
                        m_bindModel.RemoveEventHandler(iter.Current.Key, iter.Current.Value);
                    }
                    iter.Dispose();
                }
                m_bindModel = null;
            }
        }
    }
}

