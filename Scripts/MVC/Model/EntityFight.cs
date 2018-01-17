using UnityEngine;
using System.Collections;
using System.Text;
using TGameEngine;

namespace TGame.Entity
{
    // 战斗实体
    public class EntityFight : Entity
    {
        public Vector3 V_Pos = new Vector3();
        public Vector3 V_Scale = new Vector3();
        public float V_Angle = 0;
        private bool is_render_enabled_ = true;
        protected bool attackable_ = true;
        public EntityFightView m_FightView
        {
            get {
                if (m_View != null)
                    return m_View as EntityFightView;
                return null;
            }
        }

        public bool F_IsPlayer()
        {
            return V_Type == EM_EntityType.Player || V_Type == EM_EntityType.User;
        }
        // 是否隐身
        public bool F_IsStealth()
        {
            // get if has stealth buff;
            return false;
        }

        public void set_visible(bool _visible, bool _is_set_hud = false)
        {
            if (m_View != null)
            {
                m_FightView.set_renderers_enable(_visible);
            }
            is_render_enabled_ = _visible;
        }

        public bool get_render_visible()
        {
            return is_render_enabled_;
        }

        public float get_scale()
        {
            return V_Scale.x;
        }

        public virtual void set_scale(float _scale)
        {
            V_Scale.Set(_scale, _scale, _scale);
            if (m_FightView != null)
            {
                m_FightView.F_TakeDataEffective(EM_EntityDataEffectiveToView.Scale);
            }
        }

        public void set_angle(float _angle)
        {
            V_Angle = _angle;
            if (m_FightView != null)
            {
                m_FightView.F_TakeDataEffective(EM_EntityDataEffectiveToView.Angle);
            }
        }

        public virtual void set_position(Vector3 _pos, bool _set_y = true)
        {
            float y = _pos.y;
            if (_set_y)
            {
                //y = SceneManager.Instance.get_ground_height(_pos);
            }

            V_Pos.Set(_pos.x, y, _pos.z);
            if (m_FightView != null)
            {
                m_FightView.F_TakeDataEffective(EM_EntityDataEffectiveToView.Position);
            }
        }

        public Vector3 get_position()
        {
            if (m_FightView != null) return m_FightView.get_position();
            return V_Pos;
        }
        #region about animator
        #endregion
    }

}
