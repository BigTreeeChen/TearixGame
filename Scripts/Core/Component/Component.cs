using TGame.Entity;

namespace TGameEngine
{
    /// <summary>
    /// 组件基类,只存entity，如果某个组件需要view，在entity里取即可，未必取得到
    /// </summary>
    public class Component
    {
        private int id_ = -1;
        protected Entity owner_ = null;

        public Component()
        {
        }

        public Component(int _id, Entity _owner)
        {
            init(_id, _owner);
        }

        public virtual void init(int _id, Entity _owner)
        {
            id_ = _id;
            owner_ = _owner;
        }

		//called after created ,but before model loaded
		public virtual void after_create()
        {	
		}

		public virtual void update()
		{ 
		}

        public int get_id()
        {
            return id_;
        }

        public Entity get_owner()
        {
            return owner_;
        }

        public void auto_update(bool _is_update)
        {
            owner_.V_ComRoot.auto_update_COM(id_, _is_update);
        }

        public virtual void delete_self()
        {
            owner_ = null;
        }
    }
}
