using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TGameAsset
{
    public class AssetConfigItem
    {

        static AssetBundleManifest m_AssetManifest = null;
        static Dictionary<string, AssetBundle> m_DependAbDic = new Dictionary<string, AssetBundle>();

        #region 变量
        //原始资源引用
        UnityEngine.Object m_ResGameObj = null;
        //3个异步加载句柄：
        //资源包加载句柄,引用www加载ab时结果
        WWW m_LoadHandle = null;
        //资源包加载请求，引用loadfromfileasync和loadfrommemoryasync的结果
        AssetBundleCreateRequest m_Request = null;
        //资源加载请求，引用loadassetasync结果
        AssetBundleRequest m_AssetRequest = null;
        //资源包，引用各种加载出来的资源包
        AssetBundle m_AssetBundle = null;
        //资源加载完毕回调事件
        public List<Action<UnityEngine.Object>> m_OnResLoaded = new List<Action<UnityEngine.Object>>();
        //资源相对路径且没有后缀,作为基础路径，方便后续的各种拼接
        public string V_AssetPath = null;
        //资源是否已经加载完毕
        public bool V_IsResFinish = false;
        //资源类型
        public Type V_ResType;
        //是否采用异步
        public bool V_UseAsync = true;
        //是否实例化资源
        public bool V_IsInstantiate = true;
        //最后一次请求资源时间
        public float V_LastRequestTime = 0f;
        //加载优先级
        public int V_Priority = 0;
        //是否已经开始加载
        public bool V_HasStart = false;
        //是否使用引用计数
        public bool V_UseRef = false;
        //引用计数
        private int m_RefCount = 0;
        public int V_RefCount
        {
            get
            {
                if (V_UseRef) return m_RefCount;
                else return -1;
            }
        }
        //是否强制回调（加载失败也回调）
        public bool V_ForceCallBack = false;
        //是否需要加载依赖
        public bool V_LoadDependence = false;
        #endregion

        #region 加载回调管理
        public void F_AddFinishFun(Action<UnityEngine.Object> cb)
        {
            m_OnResLoaded.Add(cb);
        }
        public void F_RemoveFinishFun(Action<UnityEngine.Object> cb)
        {
            if (cb != null && m_OnResLoaded.Contains(cb))
            {
                m_OnResLoaded.Remove(cb);
            }
        }
        #endregion

        #region 资源加载管理
        /// <summary>
        /// 根据资源相对路径，获取可读写目录下该资源绝对路径+后缀
        /// </summary>
        /// <returns></returns>
        string GetPersistentAssetPath()
        {
            string assetPath = V_AssetPath;
            assetPath = assetPath.ToLower();
            string persistentPath = PathHelper.GetInstance().F_PathCombineRes(assetPath);
            if (PathHelper.GetInstance().F_CheckFileExists(persistentPath + PathHelper.ABFile))
            {
                assetPath = persistentPath + PathHelper.ABFile;
            }
            else if (PathHelper.GetInstance().F_CheckFileExists(persistentPath + PathHelper.EncrptFile))
            {
                assetPath = persistentPath + PathHelper.EncrptFile;
            }
            else if (PathHelper.GetInstance().F_CheckFileExists(persistentPath + ".jpg"))
            {
                assetPath = persistentPath + ".jpg";
            }
            else if (PathHelper.GetInstance().F_CheckFileExists(persistentPath + ".png"))
            {
                assetPath = persistentPath + ".png";
            }
            return assetPath;
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        public void F_StartLoadAsset()
        {
            V_HasStart = true;
            V_IsResFinish = false;
            StartLoadAsset();
        }

        void StartLoadAsset()
        {
            string absolutePath = GetPersistentAssetPath();
            if (V_UseAsync)
            {
                string abPath = string.Empty;
                if (PathHelper.GetInstance().F_CheckFileExists(absolutePath))
                {
                    abPath = absolutePath;
                }
                if (abPath.EndsWith(PathHelper.ABFile))
                {
                    AssetManager.GetInstance().StartCoroutine(LoadBundleImp(abPath));
                }
                else if (abPath.EndsWith(".jpg") || abPath.EndsWith(".png"))
                {
                    AssetManager.GetInstance().StartCoroutine(LoadBundleImpImage(abPath));
                }
                else
                {
                    if (AssetManager.GetInstance().F_IsLoadByAb())
                    {
                        if (abPath.EndsWith(PathHelper.EncrptFile))
                        {
                            //暂时不支持加密
                        }
                        else
                        {
                            AssetManager.GetInstance().StartCoroutine(LoadResImp());
                        }
                    }
                    else
                    {
                        AssetManager.GetInstance().StartCoroutine(LoadResImp());
                    }
                }
            }
            else
            {
                //同步加载
                try
                {
                    if (m_ResGameObj == null)
                    {
                        m_AssetBundle = null;
                        if (AssetManager.GetInstance().F_IsLoadByAb() &&
                            absolutePath.EndsWith(PathHelper.ABFile))
                        {
                            m_AssetBundle = AssetBundle.LoadFromFile(absolutePath);
                        }
                        if (m_AssetBundle != null)
                        {
                            string[] assets = m_AssetBundle.GetAllAssetNames();
                            m_ResGameObj = m_AssetBundle.LoadAsset(assets[0]);
                        }
                        else
                        {
                            m_ResGameObj = Resources.Load(V_AssetPath);
                        }
                    }
                }
                catch
                {

                }
                F_SendFinishEvent();
            }
        }

        /// <summary>
        /// 加载图片只能用www方式
        /// </summary>
        /// <param name="path">绝对路径</param>
        /// <returns></returns>
        IEnumerator LoadBundleImpImage(string path)
        {
            m_LoadHandle = new WWW(PathHelper.GetInstance().F_AddFilePro(path));
            yield return m_LoadHandle;
            if (m_LoadHandle != null && string.IsNullOrEmpty(m_LoadHandle.error) && m_LoadHandle.assetBundle != null)
            {
                m_AssetBundle = m_LoadHandle.assetBundle;
                string[] assets = m_AssetBundle.GetAllAssetNames();
                m_ResGameObj = m_AssetBundle.LoadAsset(assets[0]);
            }
            else if (m_LoadHandle != null && string.IsNullOrEmpty(m_LoadHandle.error) && m_LoadHandle.texture != null)
            {
                m_ResGameObj = m_LoadHandle.texture;
            }
            F_SendFinishEvent();
            if (m_LoadHandle != null) m_LoadHandle.Dispose();
            m_LoadHandle = null;
        }

        /// <summary>
        /// 加载资源imp
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadResImp()
        {
            ResourceRequest request = Resources.LoadAsync(V_AssetPath);
            yield return request;
            if (request != null && request.asset != null)
            {
                m_ResGameObj = request.asset;
            }
            m_AssetBundle = null;
            F_SendFinishEvent();
        }

        void LoadManiFest()
        {
            string abPath = PathHelper.GetInstance().F_PathCombineRes("ab");
            if (PathHelper.GetInstance().F_CheckFileExists(abPath))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(abPath);
                if (ab != null)
                {
                    m_AssetManifest = ab.LoadAsset("assetbundlemaniffest") as AssetBundleManifest;
                }
            }
        }
        /// <summary>
        /// 加载非加密bundle的依赖项
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckUnEncryDependence()
        {
            if (V_LoadDependence)
            {
                if (m_AssetManifest == null)
                {
                    LoadManiFest();
                }
                if (m_AssetManifest != null)
                {
                    // dps里面的路径是相对路径
                    string[] dps = m_AssetManifest.GetAllDependencies(V_AssetPath.ToLower() + PathHelper.ABFile);
                    for (int i = 0; i < dps.Length; ++i)
                    {
                        string dp = dps[i];
                        if (m_DependAbDic.ContainsKey(dp) && m_DependAbDic[dp] != null)
                        {
                            continue;
                        }
                        string absolutePath = PathHelper.GetInstance().F_PathCombineRes(dp);
                        if (AssetManager.V_LoadByFile)
                        {
                            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(absolutePath);
                            yield return request;
                            if (request != null && request.assetBundle != null)
                            {
                                m_DependAbDic[dp] = request.assetBundle;//依赖身份的ab一直不unload
                            }
                        }
                        else
                        {
                            WWW request = new WWW(PathHelper.GetInstance().F_AddFilePro(absolutePath));
                            yield return request;
                            if (request != null && string.IsNullOrEmpty(request.error) && request.assetBundle != null)
                            {
                                m_DependAbDic[dp] = request.assetBundle;
                            }
                            if (request != null) request.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载非加密bundle
        /// </summary>
        /// <param name="path">绝对路径</param>
        /// <returns></returns>
        IEnumerator LoadBundleImp(string path)
        {
            yield return AssetManager.GetInstance().StartCoroutine(CheckUnEncryDependence());
            if (AssetManager.V_LoadByFile)
            {
                m_Request = AssetBundle.LoadFromFileAsync(path);
                yield return m_Request;
                if (m_Request != null && m_Request.assetBundle != null)
                {
                    m_AssetBundle = m_Request.assetBundle;
                    string[] assets = m_AssetBundle.GetAllAssetNames();
                    m_AssetRequest = m_AssetBundle.LoadAssetAsync(assets[0]);
                    yield return m_AssetRequest;
                    m_ResGameObj = m_AssetRequest.asset;
                    F_SendFinishEvent();
                    m_Request = null;
                    m_AssetRequest = null;
                }
            }
            else
            {
                m_LoadHandle = new WWW(PathHelper.GetInstance().F_AddFilePro(path));
                yield return m_LoadHandle;
                if (m_LoadHandle != null && string.IsNullOrEmpty(m_LoadHandle.error) && m_LoadHandle.assetBundle != null)
                {
                    m_AssetBundle = m_LoadHandle.assetBundle;
                    string[] assets = m_AssetBundle.GetAllAssetNames();
                    m_AssetRequest = m_AssetBundle.LoadAssetAsync(assets[0]);
                    yield return m_AssetRequest;
                    m_ResGameObj = m_AssetRequest.asset;
                    F_SendFinishEvent();
                }
                else if (m_LoadHandle != null && string.IsNullOrEmpty(m_LoadHandle.error) && m_LoadHandle.texture != null)
                {
                    m_ResGameObj = m_LoadHandle.texture;
                    F_SendFinishEvent();
                }
                if (m_LoadHandle != null) m_LoadHandle.Dispose();
                m_LoadHandle = null;
            }
        }
        #endregion

        #region 加载完成
        public void F_SendFinishEvent()
        {
            if (m_OnResLoaded.Count > 0)
            {
                SendObjToAllAction();
            }
            V_IsResFinish = true;
        }

        void SendObjToAllAction()
        {
            for (int i = 0; i < m_OnResLoaded.Count; ++i)
            {
                if (m_OnResLoaded[i] != null)
                {
                    SendObjToAction(m_ResGameObj, m_OnResLoaded[i]);
                }
            }
            CleanAllFinishEvent();
        }

        void SendObjToAction(UnityEngine.Object obj, Action<UnityEngine.Object> OnResLoadedFun)
        {
            if (V_IsInstantiate && m_AssetBundle != null && obj != null && obj is GameObject)
            {
                V_UseRef = true;
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                if (go != null)
                {
                    AssetNode node = go.AddComponent<AssetNode>();
                    if (node != null)
                    {
                        node.V_AssetPath = V_AssetPath;
                        m_RefCount++;
                        OnResLoadedFun(go);
                    }
                }
            }
            else
            {
                if (obj != null)
                {
                    if (V_IsInstantiate)//实例化出来使用
                    {
                        UnityEngine.Object go = GameObject.Instantiate(obj);
                        if (go != null)
                        {
                            OnResLoadedFun(go);
                        }
                    }
                    else//直接使用原资源
                    {
                        OnResLoadedFun(obj);
                    }
                }
                else if (V_ForceCallBack)
                {
                    OnResLoadedFun(null);
                }
                
                if (m_AssetBundle != null)
                {
                    m_AssetBundle.Unload(false);
                }
            }
        }

        #region 加载场景，
        //加载场景不同其他资源，因为它不直接使用加载出来的资源包，
        //unity的加载场景接口scenemanager.loadscene接口会自己去找内存中没有unload的ab包，我们只需把相关ab包加载到内存即可，
        //当然切场景时要把它unload掉，让之从内存中卸载。
        public void F_AsyncLoadScene(string assetPath, string pLevel,Action<bool> callBack)
        {
            AssetManager.GetInstance().StartCoroutine(LoadScene(assetPath, pLevel, callBack));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetPath">相对路径，无后缀</param>
        /// <param name="pLevel">绝对路径，带后缀</param>
        /// <param name="callBack"></param>
        /// <param name="clearCallBack"></param>
        /// <returns></returns>
        IEnumerator LoadScene(string assetPath, string pLevelPath, Action<bool> callBack)
        {
            bool success = false;
            if (pLevelPath.EndsWith(PathHelper.ABFile))
            {
                m_Request = AssetBundle.LoadFromFileAsync(pLevelPath + PathHelper.ABFile);
                yield return m_Request;
                if (m_Request != null && m_Request.assetBundle != null)
                {
                    m_ResGameObj = m_Request.assetBundle;
                    success = true;
                }
            }
            else if (pLevelPath.EndsWith(PathHelper.EncrptFile))
            {
                m_LoadHandle = new WWW(PathHelper.GetInstance().F_AddFilePro(pLevelPath));
                yield return m_LoadHandle;
                if (m_LoadHandle != null && m_LoadHandle.assetBundle != null)
                {
                    m_ResGameObj = m_LoadHandle.assetBundle;
                    success = true;
                }

                if (m_LoadHandle != null) m_LoadHandle.Dispose();
                m_LoadHandle = null;
            }
            if (callBack != null)
            {
                callBack(success);
            }
        }

        /// <summary>
        /// 清楚旧场景
        /// </summary>
        public void F_CleanOldScene()
        {
            AssetBundle ab = m_ResGameObj as AssetBundle;
            if (ab != null)
            {
                ab.Unload(false);
            }
        }
        #endregion
        void CleanAllFinishEvent()
        {
            m_OnResLoaded.Clear();
        }
        #endregion

        #region 资源清理
        /// <summary>
        /// 清理原资源和引用该资源的实例
        /// </summary>
        public void F_CleanAllAssets()
        {
            CleanAllFinishEvent();
            m_ResGameObj = null;
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
            }
        }

        /// <summary>
        /// 清理原资源，不清理引用该资源的实例
        /// </summary>
        public void F_UnloadAssetBundle()
        {
            CleanAllFinishEvent();
            m_ResGameObj = null;
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(false);
            }
        }

        public void F_Release()
        {
            m_RefCount--;
        }
        #endregion
    }
}


