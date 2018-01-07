using TGameEngine;
using System.Collections;
using UnityEngine;
using Core.MVC;

namespace TGame.Entity
{
    public abstract class EntityView : View
    {
        public Entity V_Entity;

        public abstract void F_DoRegComponents();
        public abstract void F_RetSet();
        public abstract void F_Delete();

        // 加载一个EntityView所需要的接口
        public abstract void F_InitView();
        public abstract void F_OnViewLoadFinish(Object obj);
        public abstract string F_GetResPath();
    }
}

