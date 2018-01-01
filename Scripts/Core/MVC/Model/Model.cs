using System;
using System.Reflection;

namespace Core.MVC
{
    public class Model : Nofitier
    {
        // 模型名是只读的，只在构造函数处生成
        protected string m_modeName;

        public string GetModelName()
        {
            if (string.IsNullOrEmpty(m_modeName))
            {
                m_modeName = this.GetHashCode().ToString();
            }
            return m_modeName;
        }

        public bool IsModelNameValide()
        {
            return !HasEvent(m_modeName);
        }

        public void Refresh(Enum attribute, params object[] e)
        {
            RaiseEvent(string.Format("{0}{1}", GetModelName(), attribute), e);
        }
        
        public virtual void Destory()
        {

        }

    }
}

