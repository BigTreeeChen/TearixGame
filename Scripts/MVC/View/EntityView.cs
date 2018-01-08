using TGameEngine;
using System.Collections;
using UnityEngine;
using Core.MVC;
using TGameAsset;

namespace TGame.Entity
{
    public abstract class EntityView : View
    {
        public Entity V_Entity;

        public abstract void F_DoRegComponents();
        public abstract void F_RetSet();

        // 加载一个EntityView所需要的接口
        public virtual void F_InitView()
        {
            InitListenEvent();
            InitViewScript();
            InitEntityView();
        }

        protected virtual void InitEntityView()
        {
            AssetManager.GetInstance().F_GetAsset(F_GetResPath(), OnViewLoadFinish, true);
        }

        protected abstract void InitListenEvent();
        protected abstract void InitViewScript();
        protected abstract void OnViewLoadFinish(Object obj);
        public abstract string F_GetResPath();

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

