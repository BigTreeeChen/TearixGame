using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MU.Define;
namespace Core.Panel
{
    public class CFloatingPanelList : MonoBehaviour
    {

        Dictionary<EM_FloatGroupType, string> m_FloatingPathDic = new Dictionary<EM_FloatGroupType, string>();
        Dictionary<EM_FloatGroupType, List<GameObject>> m_FloatGroupList = new Dictionary<EM_FloatGroupType, List<GameObject>>();
        bool m_bShowAll = true;
        public void F_InitFloatInfo()
        {

        }

        public void F_ShowFloatingPanel(EM_FloatGroupType pGroupType, object[] pParam, bool pHead = false)
        {

        }

        public void F_CloseFloat(EM_FloatGroupType pGroupType)
        {

        }

        public void F_SetAllFloat(bool flag)
        {

        }

        public void F_CloseAllFloat()
        {

        }

        int CountFloatingDepth(EM_FloatGroupType pGroupType)
        {
            int maxDepth = (int)EM_DepthRange.MinFloatingDepth;
            maxDepth += (int)pGroupType;
            if (maxDepth >= (int)EM_DepthRange.MaxFloatingDepth)
            {
                Debug.LogError("层级过高");
            }
            return maxDepth;
        }
    }
}


