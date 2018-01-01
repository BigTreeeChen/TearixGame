using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MU.Define;
using System;
using Core.MVC;

namespace Core.Panel
{
    public class CAlertList : MonoBehaviour
    {
        // 弹窗数据缓存
        Dictionary<EM_AlertType, object[]> m_AlertParaDic = new Dictionary<EM_AlertType, object[]>();
        // 所有已经打开的弹窗
        List<BaseAlert> m_ShowAlert = new List<BaseAlert>();
        // 所有已经打开的弹窗
        Dictionary<EM_AlertType, BaseAlert> m_AlertDic = new Dictionary<EM_AlertType, BaseAlert>();
        // 弹窗路径
        Dictionary<EM_AlertType, string> m_AlertPathDic = new Dictionary<EM_AlertType, string>();

        public void F_InitAlertInfo()
        {

        }

        public void F_RemoveAlertType(EM_AlertType type)
        {
            if (m_AlertDic.ContainsKey(type))
            {
                if (m_AlertDic[type] != null && m_AlertDic[type].gameObject != null)
                {
                    Destroy(m_AlertDic[type].gameObject);
                }
                m_ShowAlert.Remove(m_AlertDic[type]);
                m_AlertDic.Remove(type);
            }

            if (m_AlertParaDic.ContainsKey(type))
            {
                m_AlertParaDic.Remove(type);
            }
        }
        public void F_CloseAllAlert()
        {
            Dictionary<EM_AlertType, BaseAlert> alertDicTmp = new Dictionary<EM_AlertType, BaseAlert>();
            foreach(KeyValuePair<EM_AlertType, BaseAlert> item in m_AlertDic)
            {
                alertDicTmp.Add(item.Key, item.Value);
            }
            foreach(KeyValuePair<EM_AlertType, BaseAlert> item in alertDicTmp)
            {
                F_RemoveAlertType(item.Key);
            }
            m_AlertParaDic.Clear();
        }

        public BaseAlert F_GetAlert(EM_AlertType type)
        {
            if (m_AlertDic.ContainsKey(type))
            {
                return m_AlertDic[type];
            }
            return null;
        }

        // 打开系统弹窗
        public void F_ShowAlert(EM_AlertType alertType, object[] pPar = null, Action<BaseAlert> pFinishCb = null,
            Action<EM_AlertType, object[]> pCloseCb = null, bool pIsFullScreen = false, bool isMove = false)
        {

        }

        Dictionary<EM_TipsType, string> m_TipsRes = new Dictionary<EM_TipsType, string>();
        // 原项目中m_TipsList的使用感觉有问题，这里试着不用m_TipsList,只用m_TipsDic管理
        List<BaseTips> m_TipsList = new List<BaseTips>();
        Dictionary<EM_TipsType, BaseTips> m_TipsDic = new Dictionary<EM_TipsType, BaseTips>();
        Dictionary<EM_TipsType, List<object[]>> m_TipsParaDic = new Dictionary<EM_TipsType, List<object[]>>();
        // 初始化资源路径
        public void F_InitTipsInfo()
        {

        }

        public void F_ShowTips(EM_TipsType pTipType, object[] pPara, bool pShowAlertTop = false, bool Async = true)
        {

        }

        public void F_CloseTips(EM_TipsType type)
        {

        }

        public BaseTips F_GetTips(EM_TipsType type)
        {
            return null;
        }

        public bool F_ExistTip(EM_TipsType type)
        {
            return false;
        }

        public void F_CloseAllTips()
        {

        }

        public void F_SetAllTips(bool flag)
        {

        }
    }
}

