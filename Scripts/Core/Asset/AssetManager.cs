using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TGameAsset
{
    public class AssetManager : MonoBehaviour
    {
        public enum EM_ABType
        {
            Scene,
            BaseUI,
        }

        int V_BuildType = 1;
        public static bool V_LoadByFile = true;
        Dictionary<string, AssetConfigItem> m_AssetPool = new Dictionary<string, AssetConfigItem>();
        //特殊资源包，里面ab包需要保持着不unload（切场景才unload旧场景）
        Dictionary<EM_ABType, AssetConfigItem> m_AssetBundlePool = new Dictionary<EM_ABType, AssetConfigItem>();
        const int m_PoolSize = 15;
        //无用多长时间后可以清理
        int m_CanCleanTime = 12;
        //清理间隔
        int m_CleanInternalTime = 2;
        float m_LastCleanTime = 0f;
        //同一时刻最多加载资源数
        const int m_MaxLoadNum = 3;

        static AssetManager m_Instance = null;
        public static AssetManager GetInstance()
        {
            if (m_Instance == null) m_Instance = new AssetManager();
            return m_Instance;
        }
        #region 加载接口
        public void F_GetAsset(string assetPath, Action<UnityEngine.Object> callback, bool Async = true, bool IsInstantiate = true,
                    int Priority = 0, bool forceCallBack = false, bool needDepend = false)
        {
            if (string.IsNullOrEmpty(assetPath)) return;

            F_GetAsset(assetPath, callback, typeof(GameObject), Async, IsInstantiate, Priority, forceCallBack, needDepend);
        }

        public void F_GetAssetInPool(string assetPath, EM_EntityPoolType poolType, Action<UnityEngine.Object> callback, Type type, 
                    bool Async, bool IsInstantiate = true, int Priority = 0, bool forceCallBack = false, bool needDepend = false)
        {
            if (string.IsNullOrEmpty(assetPath)) return;
            if (callback == null) return;

            UnityEngine.Object obj = EntityPool.GetInstance().CheckOut(poolType, assetPath);
           if (obj != null)
            {
                callback(obj);
                return;
            }

            F_GetAsset(assetPath, callback, type, Async, IsInstantiate, Priority, forceCallBack, needDepend);
        }


        public void F_GetAsset(string assetPath, Action<UnityEngine.Object> callback, Type type, bool Async, bool IsInstantiate = true,
                    int Priority = 0, bool forceCallBack = false, bool needDepend = false)
        {
            if (string.IsNullOrEmpty(assetPath)) return;
            if (callback == null) return;//因为有些逻辑依赖callback，并且，哪有只加载不使用的道理（加载场景特殊）
            if (m_AssetPool.ContainsKey(assetPath))
            {
                m_AssetPool[assetPath].V_LastRequestTime = Time.realtimeSinceStartup;
                m_AssetPool[assetPath].V_IsInstantiate = IsInstantiate;
                m_AssetPool[assetPath].F_AddFinishFun(callback);
                if (m_AssetPool[assetPath].V_IsResFinish)//可能已经加载完了，也可能在加载队列中
                {
                    m_AssetPool[assetPath].F_SendFinishEvent();
                }
                return;
            }
            else
            {
                AssetConfigItem newItem = new AssetConfigItem();
                newItem.V_AssetPath = assetPath;
                newItem.V_UseAsync = Async;
                newItem.V_ResType = type;
                newItem.V_IsInstantiate = IsInstantiate;
                newItem.V_Priority = Priority;
                newItem.V_ForceCallBack = forceCallBack;
                newItem.V_LoadDependence = needDepend;
                newItem.V_LastRequestTime = Time.realtimeSinceStartup;
                newItem.F_AddFinishFun(callback);
                if (Async)
                {
                    m_AssetPool[assetPath] = newItem;
                }
                else
                {
                    newItem.F_StartLoadAsset();
                }
            }
        }

        // 把场景资源的ab包加载到内存中，让后续的SceneManager.LoadScene使用
        public void F_LoadScene(string assetPath, Action<bool> callBack)
        {
            if (callBack != null)
            {
                callBack(true);
            }
        }
        #endregion

        #region 加载管理
        void Update()
        {
            if (Time.frameCount % 5 == 0)
            {
                CheckToLoadRes();
            }
        }

        List<string> CanStartLoad = new List<string>();
        bool StartCheckCleanRes = false;
        void CheckToLoadRes()
        {
            //遍历m_AssetPool中没有加载完成且没有开始加载的资源，加到一个待加载列表中，根据优先级排序加载
            CanStartLoad.Clear();
            StartCheckCleanRes = false;
            List<AssetConfigItem> canLoadList = new List<AssetConfigItem>();
            var iter = m_AssetPool.GetEnumerator();
            while(iter.MoveNext())
            {
                var tmp = m_AssetPool[iter.Current.Key];
                if (!tmp.V_IsResFinish && !tmp.V_HasStart)
                {
                    canLoadList.Add(tmp);
                }
            }
            iter.Dispose();
            canLoadList.Sort((a, b) => { return b.V_Priority.CompareTo(a.V_Priority); });

            for (int i = 0; i < canLoadList.Count && i < m_MaxLoadNum; ++i)
            {
                if (!canLoadList[i].V_IsResFinish)
                {
                    canLoadList[i].F_StartLoadAsset();
                    StartCheckCleanRes = true;
                }
            }

            if (StartCheckCleanRes)
            {
                CheckCleanRes();
            }
        }

        // 检查回收
        void CheckCleanRes()
        {
            float now = Time.realtimeSinceStartup;
            if (now - m_LastCleanTime < m_CleanInternalTime)
            {
                return;
            }
            List<string> keys = new List<string>(m_AssetPool.Keys);
            for (int i = 0; i < keys.Count; ++i)
            {
                var tmp = m_AssetPool[keys[i]];
                if ((now - tmp.V_LastRequestTime > m_CanCleanTime) && tmp.V_IsResFinish)
                {
                    if (tmp.V_RefCount == 0)//使用引用计数且引用计数为0时
                    {
                        tmp.F_CleanAllAssets();
                        m_AssetPool.Remove(keys[i]);
                    }
                    else if (tmp.V_RefCount < 0)//不使用引用计数
                    {
                        tmp.F_UnloadAssetBundle();
                        m_AssetPool.Remove(keys[i]);
                    }
                }
            }
            m_LastCleanTime = Time.realtimeSinceStartup;
        }
        #endregion
        public void UnLoadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        // 减引用
        public void F_ReleaseAsset(string path)
        {
            if (m_AssetPool.ContainsKey(path))
            {
                m_AssetPool[path].F_Release();
            }
        }

        public bool F_IsLoadByAb()
        {
            return V_BuildType == 1;
        }   
    }
}


