using UnityEngine;
using System.Collections.Generic;
using Core.MVC;
using MU.Define;

namespace Core.Panel
{
    [RequireComponent(typeof(UIPanel))]
    public abstract class BaseWindow : View
    {
        [SerializeField]
        protected UIButton m_CloseBtn;
        [SerializeField]
        protected UIPanel m_WinTitlePanel;
        public bool V_IsShowMask = false;
        public bool V_IsNeedToCloseMainCamera = false;
        public int V_PanelDepth = 0;
        public int V_NowPanelDepth = 0;
        //截图的模糊背景
        GameObject m_goScreenShotBG = null;
        //转菊花界面
        GameObject m_goWaitIcon = null;
        //是否使用对象池
        public bool V_UsePool = false;
        //是否播放特效
        public bool m_bPlayEffect = false;
        //是否使用通用背景
        public bool m_bAddCommonBg = true;

        GameObject m_goTopWindowCommonBG = null;

        EM_WindowEffectType m_WindowEffectType = EM_WindowEffectType.None;
        //控制标签内部的值
        public int m_TabValue = -1;

        //二级界面遮罩大小
        static readonly Vector3 m_SecondLevelMaskWH = new Vector3(3000, 3000, 0);

        #region 需要重载基类函数
        protected virtual void OnClickCloseBtn()
        {
            F_OnCloseToDo();
        }

        public abstract EM_WinType F_GetWinType();
        #endregion
        public EM_WindowEffectType V_WindowEffectType
        {
            get
            {
                return m_WindowEffectType;
            }
        }

        public GameObject V_ScreenShotBG
        {
            get { return m_goScreenShotBG; }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            if (null != m_CloseBtn)
            {
                //所有一级面板关闭按钮位置有问题，不得已如此
                if (m_WindowEffectType == EM_WindowEffectType.TopWindow)
                {
                    if (m_CloseBtn != null)
                    {
                        Vector3 v1 = m_CloseBtn.gameObject.transform.localPosition;
                        v1.y = 308;
                        m_CloseBtn.gameObject.transform.localPosition = v1;
                        m_CloseBtn.gameObject.SetActive(true);
                    }
                }
                EventDelegate.Add(m_CloseBtn.onClick, OnClickCloseBtn);
            }
            //reset Joystick
            ShowSecondWindowEffect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_goTopWindowCommonBG != null)
            {
                string path = "common/TopCommonBG";
                //check in pool
            }
            if (m_goScreenShotBG)
            {
                GameObject.Destroy(m_goScreenShotBG);
            }
        }

        //所有关闭的处理都在这里做，防止子类做了的事情，父类还做
        public virtual void F_OnCloseToDo()
        {
            //UIManager.GetInstance().F_CloseWin(F_GetWinType());
        }

        public abstract bool F_CheckCanOpen();

        #region 界面打开效果
        void ShowSecondWindowEffect()
        {
            if (m_CloseBtn != null)
            {
                m_CloseBtn.gameObject.SetActive(true);
            }
            if (!m_bPlayEffect)
            {
                V_IsLoaded = false;
                if (m_WindowEffectType == EM_WindowEffectType.SecondWindow)
                {
                    m_bPlayEffect = true;
                    //隐藏转菊花
                    if (m_goWaitIcon != null)
                    {
                        m_goWaitIcon.SetActive(false);
                    }
                    GameObject go = new GameObject("CloseMask");
                    if (go != null)
                    {
                        go.layer = LayerMask.NameToLayer("UI");
                        go.transform.parent = transform;
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localScale = Vector3.one;
                        UIWidget widget = go.AddComponent<UIWidget>();
                        BoxCollider collider = go.AddComponent<BoxCollider>();
                        if (collider != null && widget != null)
                        {
                            widget.width = (int)m_SecondLevelMaskWH.x;
                            widget.height = (int)m_SecondLevelMaskWH.y;
                            widget.depth = -10;
                            collider.size = m_SecondLevelMaskWH;
                        }
                    }
                }
            }
        }

        //播放顶级界面特效
        public void F_ShowTopWindowEffect()
        {
            if (m_CloseBtn != null)
            {
                m_CloseBtn.gameObject.SetActive(true);
            }
            if (!m_bPlayEffect)
            {
                V_IsLoaded = false;
                if (m_WindowEffectType == EM_WindowEffectType.TopWindow)
                {
                    m_bPlayEffect = true;
                    if (m_goWaitIcon != null)
                    {
                        m_goWaitIcon.SetActive(false);
                    }
                    if (m_bAddCommonBg)
                    {
                        string path = "common/TopCommonBG";
                        //getasset
                    }
                }
            }
        }

        public List<EventDelegate> V_OnEffectFinish = new List<EventDelegate>();

        void OnEffectFinishCallBack()
        {
            V_IsLoaded = true;
            if (V_OnEffectFinish.Count > 0)
            {
                EventDelegate.Execute(V_OnEffectFinish);
            }
        }

        protected void ShowTopWindowEffect()
        {
            m_WindowEffectType = EM_WindowEffectType.TopWindow;
        }

        protected void ShowSecondLevelWindowEffect()
        {
            m_WindowEffectType = EM_WindowEffectType.SecondWindow;
        }

        public virtual void F_ShowIndexTab(int pTabIndex)
        {

        }

        public void F_SetTabValue(int pTabValue)
        {
            m_TabValue = pTabValue;
        }

        //设置截屏背景节点
        public void SetScreenShotBG(GameObject pGoScreenShot)
        {
            m_goScreenShotBG = pGoScreenShot;
            m_goScreenShotBG.name = "ScreenBG_" + gameObject.name;
            UIPanel WinPanel = gameObject.GetComponent<UIPanel>();
            if (WinPanel && m_goScreenShotBG)
            {
                m_goScreenShotBG.SetActive(false);
                UIPanel goPanel = m_goScreenShotBG.AddComponent<UIPanel>();
                if (goPanel)
                {
                    goPanel.depth = WinPanel.depth;
                }
                m_goScreenShotBG.SetActive(true);
            }
        }

        //显示转菊花
        public void F_ShowLoadWait()
        {
            string strpath = "LoadWaiting";
            //getasset and set obj to m_goWaitIcon
        }

        //关闭互斥窗口
        public void F_CloseMutexWin()
        {
            List<EM_WinType> LIST = new List<EM_WinType>();//list of win that mutex from this win
            for(int i = 0; i < LIST.Count; ++i)
            {
                //UIManager.GetInstance().F_CloseWin(LIST[i]);
            }
        }
        #endregion
        #region 重新计算深度
        public void F_ShowTabCountDepth(int pAllDepth)
        {
            if (m_WinTitlePanel != null)
            {
                ++pAllDepth;
                //如果拥有Title Panel，需要渲染在当前窗口上面
                m_WinTitlePanel.depth = pAllDepth;
            }
            V_NowPanelDepth = pAllDepth;
        }
        #endregion
    }
}

