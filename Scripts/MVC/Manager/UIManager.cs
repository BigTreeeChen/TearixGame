using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MU.Define;
namespace Core.Panel
{
    public class UIManager
    {
        CWinList m_WinList;
        CAlertList m_AlertList;
        CFloatingPanelList m_FloatingPanelList;
        GameObject m_UICameraGo;

        private static UIManager m_Instance = null;
        public GameObject V_UICameraGo
        {
            get { return m_UICameraGo; }
        }

        public static UIManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new UIManager();
            }
            return m_Instance;
        }

        public void CreateUIRoot(Transform pRoot)
        {
            GameObject uiListRoot = new GameObject("AllViewListRoot");
            uiListRoot.transform.parent = pRoot;
            uiListRoot.transform.localPosition = new Vector3(9999f, 9999f, 0);
            uiListRoot.transform.localScale = Vector3.one;
            uiListRoot.layer = LayerMask.NameToLayer("UI");
            UIRoot root = uiListRoot.AddComponent<UIRoot>();
            root.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
            root.manualWidth = 1280;
            root.manualHeight = 720;
            root.fitWidth = true;
            root.fitHeight = false;
            uiListRoot.AddComponent<UIPanel>();
            //cwinlist
            GameObject winListGo = new GameObject("WinList");
            winListGo.layer = LayerMask.NameToLayer("UI");
            winListGo.transform.parent = uiListRoot.transform;
            winListGo.transform.localPosition = new Vector3(9999, 9999, 0);
            winListGo.transform.localScale = Vector3.one;
            m_WinList = winListGo.AddComponent<CWinList>();
            if (m_WinList != null)
            {
                m_WinList.F_InitWinInfo();
            }
            //CAlertList
            GameObject alertListGo = new GameObject("AlertList");
            alertListGo.layer = LayerMask.NameToLayer("UI");
            alertListGo.transform.parent = uiListRoot.transform;
            alertListGo.transform.localPosition = new Vector3(9999, 9999, 0);
            alertListGo.transform.localScale = Vector3.one;
            m_AlertList = alertListGo.AddComponent<CAlertList>();
            if (m_AlertList != null)
            {
                m_AlertList.F_InitAlertInfo();
                m_AlertList.F_InitTipsInfo();
            }
            //CFloatingPanelList
            GameObject floatingPanelGo = new GameObject("FloatingPanelList");
            floatingPanelGo.layer = LayerMask.NameToLayer("UI");
            floatingPanelGo.transform.parent = uiListRoot.transform;
            floatingPanelGo.transform.localPosition = new Vector3(9999, 9999, 0);
            floatingPanelGo.transform.localScale = Vector3.one;
            m_FloatingPanelList = floatingPanelGo.AddComponent<CFloatingPanelList>();
            if (m_FloatingPanelList != null)
            {
                m_FloatingPanelList.F_InitFloatInfo();
            }

            //uicamera
            m_UICameraGo = new GameObject("UI2DCamera");
            m_UICameraGo.transform.parent = uiListRoot.transform;
            m_UICameraGo.transform.localPosition = new Vector3(9999, 9999, 0);
            m_UICameraGo.layer = uiListRoot.layer;
            Camera camera = m_UICameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.orthographic = true;
            camera.orthographicSize = 1;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            camera.depth = 2;
            camera.farClipPlane = 10f;
            camera.nearClipPlane = -10f;
            m_UICameraGo.AddComponent<UICamera>();

        }

        public void F_ChangeTo3DCamera(bool b3D)
        {
            if (m_UICameraGo != null)
            {
                if (b3D)
                {
                    m_UICameraGo.transform.localPosition = new Vector3(9999, 9999, -700f);
                    Camera ca = m_UICameraGo.GetComponent<Camera>();
                    ca.orthographic = false;
                    ca.fieldOfView = 60;
                    ca.nearClipPlane = 0.1f;
                    ca.farClipPlane = 4.0f;
                }
                else
                {
                    m_UICameraGo.transform.localPosition = new Vector3(9999, 9999, 0f);
                    Camera ca = m_UICameraGo.GetComponent<Camera>();
                    ca.orthographic = true;
                    ca.fieldOfView = 60;
                    ca.nearClipPlane = -10f;
                    ca.farClipPlane = 10f;
                }
            }
        }

        public void F_ShowWin(EM_WinType pWinType, bool isFullScreen = false, Action<BaseWindow> finishCb = null,
                                bool isAsync = true, bool usePool = false, bool useSelfDepth = false)
        {
            m_WinList.F_ShowWin(pWinType, isFullScreen, finishCb, isAsync, usePool, useSelfDepth);
        }

        public void F_ShowTopWinEffect(EM_WinType winType)
        {
            m_WinList.F_ShowTopWinEffect(winType);
        }

        public void F_CloseWin(EM_WinType winType)
        {
            m_WinList.F_RemoveWinType(winType);
        }

        public void F_CloseAllWin()
        {
            m_WinList.F_CloseAllWin();
        }

        public void F_SetUnderFullScreenWin(EM_WinType winType)
        {
            m_WinList.F_SetUnderFullScreenWin(winType);
        }

        public void F_CloseOpenWinExcept()
        {

        }

        public BaseWindow F_GetWindowByType(EM_WinType winType)
        {
            if (m_WinList != null)
            {
                return m_WinList.F_GetWinByType(winType);
            }
            return null;
        }

        public void F_SetAllWindowShow(bool pShow)
        {
            m_WinList.F_SetAllWindowShow(pShow);
        }

        bool IsIngoreUICamera()
        {
            return m_WinList.V_FullScreenUICnt <= 0;
        }

        public void F_CaptureScreenShot(GameObject pParent, Action<GameObject> pFinishCallBack)
        {
            GameObject sceneCamera = null;
            if (sceneCamera != null && m_UICameraGo != null)
            {
                GameObject goParent = new GameObject("ScreenBG");
                if (goParent)
                {
                    goParent.layer = LayerMask.NameToLayer("UI");
                    goParent.transform.parent = pParent.transform;
                    goParent.transform.localPosition = Vector3.zero;
                    goParent.transform.localScale = Vector3.one;

                    GameObject goChild = GameObject.Instantiate(goParent) as GameObject;
                    if (goChild != null)
                    {
                        goChild.transform.parent = goParent.transform;
                        goChild.transform.localPosition = new Vector3(0, 0, 700);
                        goChild.transform.localScale = Vector3.one;
                        TextureBlurEffect eff = goChild.AddComponent<TextureBlurEffect>();
                        eff.F_BlurCameraTex(0.2f, sceneCamera, m_UICameraGo, IsIngoreUICamera());
                    }

                    if (pFinishCallBack != null)
                    {
                        pFinishCallBack(goParent);
                    }
                }
            }
            else
            {
                ;
            }
        }

        // 加载标签页
        public void F_ShowTabPanel(EM_WinType winType, GameObject pPanelObj, string panelPath, Action<UnityEngine.Object> cb)
        {
            m_WinList.F_ShowTabPanel(winType, pPanelObj, panelPath, cb); 
        }

        public void F_CloseAlertType(EM_AlertType type)
        {
            m_AlertList.F_RemoveAlertType(type);
        }
        // 清理所有ui
        public void F_CleanAllUI()
        {
            F_CloseAllWin();
        }
    }
}

