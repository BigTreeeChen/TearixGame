using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using TGame.Entity;

namespace TGameEngine
{
    public class ComponentRoot
    {
        protected Dictionary<int, Component> component_dict_ = new Dictionary<int, Component>();
		private List<Component> component_update_ = new List<Component>();

        public virtual void update()
        {
			for (int i = component_update_.Count - 1; i >= 0; --i)
			{
				component_update_[i].update();
			}
        }

        public bool has_COM(int _id)
        {
            return component_dict_.ContainsKey(_id);
        }

        public Component get_COM(int _id)
        {
            Component com;
            if (component_dict_.TryGetValue(_id, out com))
            {
                return com;
            }
            return null;
        }

		private bool find_com(int _id)
		{
			for (int i = 0; i < component_update_.Count; ++i)
			{
				if (component_update_[i].get_id() == _id)
				{
					return true;
				}
			}
			return false;
		}

        public bool auto_update_COM(int _id, bool _is_update)
        {
            Component com;

            if (!component_dict_.TryGetValue(_id, out com))
            {
                return false;
            }

            if (_is_update)
            {
				if (!find_com(_id))
                {
                    component_update_.Add(com);
                }
            }
            else
            {
				if (find_com(_id))
                {
					component_update_.Remove(com);
                }
            }
            return true;
        }

        public bool register_COM(int _id, Component _com, Entity _owner, bool _is_update = false)
        {
            if (component_dict_.ContainsKey(_id))
            {
				unregister_COM(_id);
            }
            component_dict_.Add(_id, _com);

			_com.init(_id, _owner);
            
            if (_is_update)
            {
                component_update_.Add(_com);
            }
            
            return true;
        }

        public bool unregister_COM(int _id)
        {
            Component com;

            if (!component_dict_.TryGetValue(_id, out com))
            {
                return false;
            }

            component_dict_.Remove(_id);
            if (find_com(_id))
            {
                component_update_.Remove(com);
            }
            com.delete_self();
            return true;
        }

        public void unregister_all_COM()
        {
            foreach (KeyValuePair<int, Component> kvp in component_dict_)
            {
                kvp.Value.delete_self();
            }
            component_dict_.Clear();
            component_update_.Clear();
        }

		//you must call this method if this component root unused!
		public virtual void delete_self()
		{
			unregister_all_COM();
		}
    }
}
