using UnityEngine;
using System.Collections;
using TGameEngine;

namespace TGame.Entity
{
    // 抽象实体
    public class Entity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public long V_ID;
        /// <summary>
        /// 实体类型
        /// </summary>
        public EM_EntityType V_Type;
        /// <summary>
        /// modelid
        /// </summary>
        public int V_ModelId;
        /// <summary>
        /// 在entityview加载完资源后赋值m_View，方便entity层访问，view被删后记得置null
        /// </summary>
        public EntityView m_View;

        public ComponentRoot V_ComRoot;
        public virtual void F_DoRegComponents() { }
        public virtual void F_Delete() { }

        public TGameEngine.Component F_GetCom(int _id)
        {
            if (V_ComRoot == null) return null;
            return V_ComRoot.get_COM(_id);
        }

    }
}

