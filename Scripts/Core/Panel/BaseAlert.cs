using UnityEngine;
using System.Collections;
using Core.MVC;
using MU.Define;
using System;
namespace Core.Panel
{
    [RequireComponent(typeof(UIPanel))]
    public abstract class BaseAlert : View
    {
        [SerializeField]
        protected UIButton m_CloseBtn;
        // 当前窗口Panel的深度偏移
        public int V_PanelDepth = 0;
        // 当前窗口加上地下附属panel的总深度
        public int V_NowPanelDepth = 0;
        public bool V_IsFullScreen = false;
        //关闭Alert时传给关闭回调的数据
        protected object[] m_RetStr;
        protected Action<EM_AlertType, object[]> m_CloseCallBack = null;
        GameObject m_goScreenShotBG = null;
        // 二级窗口底下mask大小
        protected static readonly Vector3 m_SecondLvMaskWH = new Vector3(3000, 3000, 0);
        public abstract void F_InitTipsInfo(object[] pPar);
        public abstract EM_AlertType F_GetAlertType();
        public abstract EM_NewTipsShowType F_GetShowTipsType();

        protected virtual void OnClickCloseBtn()
        {
            //UIManager.GetInstance().F_CloseAlertType(F_GetAlertType());
            Destroy(gameObject);
        }
        protected virtual void Start()
        {
            if (null != m_CloseBtn)
            {
                EventDelegate.Add(m_CloseBtn.onClick, OnClickCloseBtn);
            }
        }

        protected override void OnDestroy()
        {
            OnCloseToDo();
            base.OnDestroy();
            if (m_goScreenShotBG != null)
                GameObject.Destroy(m_goScreenShotBG);
        }

        // 所有关闭的处理都要在这里处理
        protected virtual void OnCloseToDo()
        {
            if (m_CloseCallBack != null)
            {
                m_CloseCallBack(F_GetAlertType(), m_RetStr);
            }
        }

        public abstract bool F_CheckCanOpen();

        public abstract void F_OpenAlertPara(object[] pPar, Action<EM_AlertType, object[]> pCloseCbFun);

    }
}

