using UnityEngine;
using System.Collections.Generic;
using Core.MVC;

namespace TGameAsset
{
    public enum EM_EntityPoolType
    {
        GameObject, //同时存在多份的情况使用，比如子弹、头顶预制等
        Object,     //普通缓存池，一般用这个
        View,       //特地给view开个类实体池
    }

    public class EntityPool
    {
        // 实体缓存池也要慎用，因为里面保留了对资源的引用，相关的asset是一直在内存中的
        static EntityPool m_Instance;
        public static EntityPool GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new EntityPool();
            return m_Instance;
        }

        Transform m_Root;
        // 一般同时存在多份的情况使用，池子里保存一份，使用时只是拷贝一份给上层用
        Dictionary<string, GameObject> m_GameObjsDic = new Dictionary<string, GameObject>();
        // 普通缓存池，把自己从池子里删了给上层用
        Dictionary<string, List<GameObject>> m_ObjectsDic = new Dictionary<string, List<GameObject>>();
        // 因为view比较大，这里为了重复使用而特别起个池子
        Dictionary<string, List<View>> m_ViewsDic = new Dictionary<string, List<View>>();
        public void F_Init()
        {
            m_Root = new GameObject("EntityPoolRoot").transform;
        }

        public void CheckIn(EM_EntityPoolType poolType, string path, Object obj)
        {
            if (poolType == EM_EntityPoolType.GameObject)
            {
                GameObject gObj = obj as GameObject;
                gObj.SetActive(false);
                if (m_GameObjsDic.ContainsKey(path))
                {
                    GameObject.Destroy(gObj);
                }
                else
                {
                    m_GameObjsDic[path] = gObj;
                    gObj.transform.parent = m_Root;
                }
            }
            else if (poolType == EM_EntityPoolType.Object)
            {
                GameObject gObj = obj as GameObject;
                gObj.SetActive(false);
                gObj.transform.parent = m_Root;
                List<GameObject> list = null;
                if (!m_ObjectsDic.TryGetValue(path, out list))
                {
                    list = new List<GameObject>();
                    m_ObjectsDic[path] = list;
                }
                list.Add(gObj);
            }
            else
            {
                View view = obj as View;
                view.F_Reset();
                List<View> list = null;
                if (!m_ViewsDic.TryGetValue(path, out list))
                {
                    list = new List<View>();
                    m_ViewsDic[path] = list;
                }
                list.Add(view);
            }
        }

        public Object CheckOut(EM_EntityPoolType poolType, string path)
        {
            Object ret = null;
            GameObject gObj = null;
            if (poolType == EM_EntityPoolType.GameObject)
            {
                if (m_GameObjsDic.TryGetValue(path, out gObj))
                {
                    gObj = GameObject.Instantiate(gObj);
                    gObj.SetActive(true);
                    ret = gObj;
                }
            }
            else if (poolType == EM_EntityPoolType.Object)
            {
                List<GameObject> list = null;
                if (m_ObjectsDic.TryGetValue(path, out list))
                {
                    if (list.Count > 0)
                    {
                        gObj = list[list.Count - 1];
                        gObj.SetActive(true);
                        ret = gObj;
                        list.RemoveAt(list.Count - 1);
                    }
                }
            }
            else
            {
                List<View> list = null;
                if (m_ViewsDic.TryGetValue(path, out list))
                {
                    if (list.Count > 0)
                    {
                        var view = list[list.Count - 1];
                        view.F_Reset();
                        ret = view;
                        list.RemoveAt(list.Count - 1);
                    }
                }
            }
            return ret;
        }

        public void F_Remove(EM_EntityPoolType poolType, string path)
        {
            if (poolType == EM_EntityPoolType.GameObject)
            {
                if (m_GameObjsDic.ContainsKey(path))
                {
                    GameObject.Destroy(m_GameObjsDic[path]);
                    m_GameObjsDic.Remove(path);
                }
            }
            else if (poolType == EM_EntityPoolType.Object)
            {
                List<GameObject> list = null;
                if (m_ObjectsDic.TryGetValue(path, out list))
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        GameObject.Destroy(list[i]);
                    }
                    list.Clear();
                    m_ObjectsDic.Remove(path);
                }
            }
            else
            {
                List<View> list = null;
                if (m_ViewsDic.TryGetValue(path, out list))
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        GameObject.Destroy(list[i]);
                    }
                    list.Clear();
                    m_ObjectsDic.Remove(path);
                }
            }
        }

        public void F_Clear()
        {
            foreach(var ele in m_GameObjsDic)
            {
                if (ele.Value != null)
                    GameObject.Destroy(ele.Value);
            }
            m_GameObjsDic.Clear();

            foreach (var ele in m_ObjectsDic)
            {
                if (ele.Value != null)
                {
                    for(int i = 0; i < ele.Value.Count; ++i)
                        GameObject.Destroy(ele.Value[i]);
                }
            }
            m_ObjectsDic.Clear();

            foreach (var ele in m_ViewsDic)
            {
                if (ele.Value != null)
                {
                    for (int i = 0; i < ele.Value.Count; ++i)
                        GameObject.Destroy(ele.Value[i]);
                }
            }
            m_ViewsDic.Clear();
        }
    }

}
