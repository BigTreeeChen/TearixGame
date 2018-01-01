using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MU.Define;
using TGameAsset;

namespace Core.Panel
{
    public class CWinList : MonoBehaviour
    {

        #region 变量
        //正在加载的窗口列表，用于当异步加载收到打开多个相同窗口请求时过滤
        List<EM_WinType> m_LoadingWinList = new List<EM_WinType>();
        //所有已经打开的窗口对象
        Dictionary<EM_WinType, BaseWindow> m_WinDic = new Dictionary<EM_WinType, BaseWindow>();
        //窗口所有路径信息
        public static Dictionary<EM_WinType, string> m_WinPathDic = new Dictionary<EM_WinType, string>();
        //全屏界面数量
        int m_FullScreenUICnt = 0;
        public int V_FullScreenUICnt
        {
            get { return m_FullScreenUICnt; }
        }
        #endregion
        #region private fun
        void InitWinResPath()
        {
            string ResPath = "WinLogin";
            m_WinPathDic.Add(EM_WinType.WinLoginView, ResPath);
        }

        #endregion
        public void F_InitWinInfo()
        {
            InitWinResPath();
        }

        public void F_ShowWin(EM_WinType pWinType, bool bIsFullScreen, Action<BaseWindow> finishCb,
            bool bIsAsync, bool usePool = false, bool useSelfDepth = false)
        {
            string winPath = "";
            if (!m_WinPathDic.TryGetValue(pWinType, out winPath))
            {
                return;
            }

            //重置摇杆reset joystick
            //查看是否已经开启过
            if (m_WinDic.ContainsKey(pWinType))
            {
                return;
            }
            else if (m_LoadingWinList.Contains(pWinType))
            {
                return;
            }
            else
            {
                m_LoadingWinList.Add(pWinType);
            }

            GameObject goScreenShot = null;
            //全屏的关闭主页面
            if (bIsFullScreen)
            {
                //关闭SceneCamera
                m_FullScreenUICnt++;
                ShowMainUI(false);
                //开始截屏->加载资源前截屏是为了去除截屏卡对UI动画的影响
                UIManager.GetInstance().F_CaptureScreenShot(gameObject, (go) =>
                {
                    goScreenShot = go;
                });
            }

            //get asset by winpath
            AssetManager.GetInstance().F_GetAsset(winPath, (winObj) =>
            {
                m_LoadingWinList.Remove(pWinType);
                GameObject WindowObj = (GameObject)winObj;
                WindowObj.SetActive(true);
                WindowObj.transform.parent = gameObject.transform;
                WindowObj.transform.localPosition = Vector3.zero;
                WindowObj.transform.localScale = Vector3.one;
                WindowObj.transform.localRotation = Quaternion.identity;
                WindowObj.layer = LayerMask.NameToLayer("UI");
                UIPanel WinPanel = WindowObj.GetComponent<UIPanel>();
                BaseWindow WinScp = WindowObj.GetComponent<BaseWindow>();

                WinScp.V_PrefabPath = winPath;
                WinScp.V_UsePool = usePool;
                if (WinPanel != null)
                {
                    if (useSelfDepth)//有些win希望使用自己的depth，比如提高depth挡住飘字弹窗效果等
                    {
                        WinPanel.depth -= 1;//把mask放在panel下一层
                        if (goScreenShot != null)
                        {
                            WinScp.SetScreenShotBG(goScreenShot);
                        }
                        WinPanel.depth += 1;
                    }
                    else
                    {
                        WinPanel.depth = FindOpenWinMaxDepth() + 1;
                        if (bIsFullScreen)
                        {
                            WinPanel.depth -= 1;
                            if (goScreenShot != null)
                            {
                                WinScp.SetScreenShotBG(goScreenShot);
                            }
                            WinPanel.depth += 1;//全屏后，下面有一层mask，这样是防止和mask冲突
                        }
                        WinScp.V_PanelDepth = OrderPanelDepth(WindowObj, WinPanel.depth);
                    }
                    WinScp.V_IsShowMask = bIsFullScreen;
                    WinScp.V_IsNeedToCloseMainCamera = bIsFullScreen;
                    WinScp.V_NowPanelDepth = WinScp.V_PanelDepth;
                    if (WinScp.F_CheckCanOpen())
                    {
                        WinScp.F_CloseMutexWin();
                        if (WinScp.V_WindowEffectType == EM_WindowEffectType.TopWindow &&
                            bIsFullScreen)
                        {
                            //设置初始移动位置
                            WindowObj.transform.localPosition = new Vector3(-1300, 15, 0);
                            WinScp.F_ShowLoadWait();
                            F_ShowTopWinEffect(pWinType);
                        }
                        if (!m_WinDic.ContainsKey(pWinType))
                        {
                            m_WinDic.Add(pWinType, WinScp);
                        }
                        if (finishCb != null)
                        {
                            finishCb(WinScp);
                        }
                    }
                    else
                    {
                        Destroy(WindowObj);
                        return;
                    }
                }
                else
                {
                    Destroy(WindowObj);
                    return;
                }
            });
        }

        public void F_ShowTopWinEffect(EM_WinType winType)
        {
            BaseWindow ShowWin = null;
            if (m_WinDic.TryGetValue(winType, out ShowWin))
            {
                ShowWin.F_ShowTopWindowEffect();
            }
        }

        public void F_RemoveWinType(EM_WinType winType)
        {
            BaseWindow needToCloseWin = null;
            if (m_WinDic.TryGetValue(winType, out needToCloseWin))
            {
                if (needToCloseWin.V_IsNeedToCloseMainCamera)
                {
                    m_FullScreenUICnt--;
                    if (m_FullScreenUICnt <= 0)
                    {
                        //open scene camera
                        ShowMainUI(true);
                        m_FullScreenUICnt = 0;
                    }
                }
                m_WinDic.Remove(winType);
                needToCloseWin.F_OnCloseToDo();
                if (needToCloseWin.V_UsePool)
                {
                    //check in
                }
                else
                {
                    GameObject.Destroy(needToCloseWin.gameObject);
                }
            }
        }

        //设置界面低于所有全屏界面
        public void F_SetUnderFullScreenWin(EM_WinType winType)
        {
            if ((m_FullScreenUICnt > 0) && m_WinDic.ContainsKey(winType))
            {
                BaseWindow winScp = m_WinDic[winType];
                if (winScp != null)
                {
                    UIPanel winPanel = winScp.gameObject.GetComponent<UIPanel>();
                    if (winPanel != null)
                    {
                        int minDepth = 999;
                        foreach(KeyValuePair<EM_WinType, BaseWindow> item in m_WinDic)
                        {
                            if (item.Value.V_IsShowMask)
                            {
                                UIPanel panelTem = item.Value.GetComponent<UIPanel>();
                                if (panelTem != null && panelTem.depth < minDepth)
                                {
                                    minDepth = panelTem.depth - 4;
                                }
                            }
                        }
                        winPanel.depth = minDepth;
                        winScp.V_PanelDepth = OrderPanelDepth(winPanel.gameObject, winPanel.depth);
                        winScp.V_NowPanelDepth = winScp.V_PanelDepth;
                    }
                }
            }
        }

        void ShowMainUI(bool bShow)
        {
            if (m_WinDic.ContainsKey(EM_WinType.WinMainUI))
            {
                //show or hide main ui through set alpha;
            }
        }

        //关闭所有窗口
        public void F_CloseAllWin()
        {
            List<EM_WinType> allOpenWin = new List<EM_WinType>(m_WinDic.Keys);
            for (int i = 0; i < allOpenWin.Count; ++i)
            {
                F_RemoveWinType(allOpenWin[i]);
            }
        }

        public void F_CloseOpenWin(EM_WinType[] NotCloseWinList)
        {
            List<EM_WinType> allOpenWin = new List<EM_WinType>(m_WinDic.Keys);
            List<int> closeWinList = new List<int>();
            for (int i = 0; i < NotCloseWinList.Length; ++i)
            {
                int index = allOpenWin.FindIndex(a => { return a == NotCloseWinList[i]; });
                if (index != -1) closeWinList.Add(index);
            }

            for(int i = 0; i < allOpenWin.Count; ++i)
            {
                if (closeWinList.Contains(i)) continue;
                F_RemoveWinType(allOpenWin[i]);
            }
        }

        public BaseWindow F_GetWinByType(EM_WinType winType)
        {
            if (m_WinDic.ContainsKey(winType))
            {
                return m_WinDic[winType];
            }
            return null;
        }

        public void F_SetAllWindowShow(bool bShow)
        {
            List<EM_WinType> allOpenWin = new List<EM_WinType>(m_WinDic.Keys);
            for (int i = 0; i < allOpenWin.Count; ++i)
            {
                BaseWindow openWin = m_WinDic[allOpenWin[i]];
                if (openWin != null)
                    openWin.gameObject.SetActive(bShow);
            }
        }

        //显示页签
        public void F_ShowTabPanel(EM_WinType winType, GameObject pPanelObj, string sPanelPath, 
            Action<UnityEngine.Object> callback, bool bShow = true)
        {
            if (m_WinDic.ContainsKey(winType))
            {
                if (pPanelObj != null)
                {
                    GameObject tabPanelGo = Instantiate(pPanelObj);
                    tabPanelGo.SetActive(bShow);
                    if (InitPanel(winType, tabPanelGo))
                    {
                        if (callback != null)
                        {
                            callback(tabPanelGo);
                        }
                    }
                }
                else if (sPanelPath != "")
                {
                    //get asset and InitPanel,last callback(look above)
                }
            }
        }

        // 初始化页签
        bool InitPanel(EM_WinType winType, GameObject tabPanelGo)
        {
            if (m_WinDic.ContainsKey(winType))
            {
                UIPanel panel = tabPanelGo.GetComponent<UIPanel>();
                if (panel != null)
                {
                    GameObject.DestroyImmediate(panel);
                }
                UIWidget widget = tabPanelGo.GetComponent<UIWidget>();
                if (widget == null)
                {
                    widget = tabPanelGo.AddComponent<UIWidget>();
                }

                //当前新增加的panel默认为窗口最大depth+1,防止重叠
                widget.depth = m_WinDic[winType].V_PanelDepth + 1;
                BaseTabPanel tabScp = tabPanelGo.GetComponent<BaseTabPanel>();
                if (tabScp != null)
                {
                    tabScp.V_PanelDepth = OrderPanelDepth(tabPanelGo, widget.depth);
                    //当前tab中的最大深度超过上限才更改
                    if (tabScp.V_PanelDepth > m_WinDic[winType].V_NowPanelDepth)
                    {
                        m_WinDic[winType].F_ShowTabCountDepth(tabScp.V_PanelDepth);
                    }
                    return true;
                }
                else
                {
                    Destroy(tabPanelGo);
                }
            }
            else
            {
                Destroy(tabPanelGo);
            }
            return false;
        }

        public BaseWindow F_FindTopOpenWin()
        {
            var iter = m_WinDic.GetEnumerator();
            int maxDepth = Int32.MinValue;
            BaseWindow result = null;
            while(iter.MoveNext())
            {
                var wnd = iter.Current.Value;
                if(wnd != null)
                {
                    if (maxDepth < wnd.V_NowPanelDepth)
                    {
                        maxDepth = wnd.V_NowPanelDepth;
                        result = wnd;
                    }
                }
            }
            iter.Dispose();
            return result;
        }
        /// <summary>
        /// 整理指定对象上面所有的孩子Panel的深度
        /// </summary>
        /// <param name="pOrderObj">整理指定对象</param>
        /// <param name="iStartDepth">起始深度</param>
        /// <returns>返回整理后的最大深度</returns>
        int OrderPanelDepth(GameObject pOrderObj, int iStartDepth)
        {
            int NowDepth = iStartDepth;
            UIPanel[] lastPanelArr = pOrderObj.GetComponentsInChildren<UIPanel>(true);
            for (int i = 0; i < lastPanelArr.Length; ++i)
            {
                ++NowDepth;
                lastPanelArr[i].depth = NowDepth;
            }
            return NowDepth;
        }

        // 找出一个gameobject中最深的panel depth
        int GetObjectMaxPanelDepth(GameObject obj)
        {
            int depth = 0;
            if (obj != null)
            {
                UIPanel[] panels = obj.GetComponentsInParent<UIPanel>(true);
                for(int i = 0; i < panels.Length; ++i)
                {
                    if (panels[i].depth > depth)
                    {
                        depth = panels[i].depth;
                    }
                }
            }
            return depth;
        }

        // 找到当前已经打开的窗口中最大深度
        int FindOpenWinMaxDepth()
        {
            List<EM_WinType> allOpenWin = new List<EM_WinType>(m_WinDic.Keys);
            int maxDepth = 0;
            for (int i = 0; i < allOpenWin.Count; ++i)
            {
                BaseWindow openWin = m_WinDic[allOpenWin[i]];
                if (openWin != null && openWin.V_NowPanelDepth > maxDepth)
                {
                    maxDepth = openWin.V_NowPanelDepth;
                }
            }
            return maxDepth;
        }
    }
}

