using UnityEngine;
using System.Collections;
using Core.MVC;
using MU.Define;

namespace Core.Panel
{
    public abstract class BaseTips : View
    {
        public int V_PanelDepth = 0;
        public abstract void F_InitTipsInfo(object[] pPar);
        public abstract EM_TipsType F_GetTipsType();
        public abstract EM_NewTipsShowType F_GetShowTipsType();

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            OnCloseToDo();
            base.OnDestroy();
        }

        // 所有关闭的处理都要在这里处理
        protected abstract void OnCloseToDo();
    }
}

