using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TGameEngine;

namespace TGame.Entity
{
    public class EntityFightView : EntityView
    {
        public EntityFight V_EntityFight
        {
            get { return V_Entity as EntityFight; }
        }
        protected string cur_ani_name_ = null;
        protected float cur_ani_start_time_ = 0;
        protected float norm_time_ = 0;
        protected float speed_rate_ = 0;
        protected Animator m_Animator;
        protected bool m_IsSpawning;

        private const float DISSOLVE_START_AMOUNT = 0.4f;
        private const float DISSOLVE_END_AMMOUT = 1f;
        public const float DISSOLVE_TOTAL_TIME = 1.5f;

        protected delegate void on_spawn_finish_callback();
        protected event on_spawn_finish_callback on_spawn_finish_;

        protected Transform m_PosTransform;

        #region override funtion
        public override void F_DoRegComponents()
        {

        }

        public override void F_RetSet()
        {

        }

        public override string F_GetResPath()
        {
            throw new NotImplementedException();
        }

        public override void F_InitView()
        {
            throw new NotImplementedException();
        }

        protected override void InitListenEvent()
        {
            throw new NotImplementedException();
        }

        protected override void InitViewScript()
        {
            throw new NotImplementedException();
        }

        protected override void OnViewLoadFinish(UnityEngine.Object obj)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void set_renderers_enable(bool _visible)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    if (renderers[i] != null)
                    {
                        renderers[i].enabled = _visible;
                    }
                }
            }
        }

        public IEnumerator do_dissolve_effect_coroutine()
        {
            if (this == null || gameObject == null)
            {
                yield break;
            }

            float startTime;
            System.Object[] p = new System.Object[4] { DISSOLVE_START_AMOUNT, DISSOLVE_END_AMMOUT, 0f, DISSOLVE_TOTAL_TIME };//staramount end amout time
            if (V_Entity.V_ComRoot.get_COM(ComponentDefine.ComponentBuffMat) != null)
            {
                //ComponentBuffMat.MatInfo matInfo = buff_material_COM.AddBuffMat(0, p, ComponentBuffMat.BUFF_MAT_TYPE.DIE_DISSOLVE, ComponentBuffMat.Priority.DIE);

                startTime = Time.time;
                float ctime = 0;
                do
                {
                    ctime = Time.time - startTime;

                    //matInfo.p[2] = ctime;

                    yield return new WaitForEndOfFrame();

                } while (ctime <= DISSOLVE_TOTAL_TIME);
            }
        }

        public bool is_spawning
        {
            get { return m_IsSpawning; }
            set
            {
                if (m_IsSpawning && !value && on_spawn_finish_ != null)
                {
                    on_spawn_finish_();
                }
                m_IsSpawning = value;
            }
        }

        public virtual void after_mesh_change()
        {
            /*每次mesh改变都记录一下新mesh的基本信息，用来作为一些改material需求的基本素材，比如死亡时挂死亡material，需要赋值原纹理等
            if (buff_material_COM != null)
            {
                buff_material_COM.init_mat();
            }
            */
        }

        public ObjBehavior get_obj_behavior()
        {
            if (this && gameObject != null)
            {
                return gameObject.GetComponent<ObjBehavior>();
            }
            return null;
        }

        public void F_TakeDataEffective(EM_EntityDataEffectiveToView type)
        {
            switch(type)
            {
                case EM_EntityDataEffectiveToView.Scale:
                    m_PosTransform.localScale = V_EntityFight.V_Scale;
                    break;
                case EM_EntityDataEffectiveToView.Angle:
                    m_PosTransform.eulerAngles = new Vector3(m_PosTransform.eulerAngles.x, V_EntityFight.V_Angle, m_PosTransform.eulerAngles.z);
                    break;
                case EM_EntityDataEffectiveToView.Position:
                    m_PosTransform.position = V_EntityFight.V_Pos;
                    break;
                default:
                    break;
            }
        }

        #region 体积，位置，朝向相关
        public Vector3 get_char_ctrl_center()
        {
            if (this && gameObject != null)
            {
                CharacterController controller = gameObject.GetComponent<CharacterController>();
                if (controller != null)
                {
                    return controller.center;
                }
            }
            return V_EntityFight.V_Pos;
        }

        public float get_char_ctrl_height()
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                return controller.height;
            }
            return 2f;
        }

        public float get_char_ctrl_radius()
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            if (controller != null)
            {
                return controller.radius;
            }
            return 0.5f;
        }

        public void look_at(EntityFightView _target)
        {
            if (_target != null)
            {
                Vector3 tar_pos = _target.get_position();
                tar_pos = new Vector3(tar_pos.x, get_position().y, tar_pos.z);
                look_at(tar_pos);
            }
        }

        public void look_at(Vector3 _pos)
        {
            Vector3 oldPos = m_PosTransform.position;
            m_PosTransform.LookAt(_pos);
            m_PosTransform.position = oldPos;
        }

        
        public Quaternion get_rotation()
        {
            return m_PosTransform.rotation;
        }

        public float get_angle()
        {
            return m_PosTransform.eulerAngles.y;
        }

        public Vector3 get_forward()
        {
            return m_PosTransform.forward;
        }

        public Vector3 get_position()
        {
            if (this && gameObject != null && m_PosTransform != null)
            {
                return m_PosTransform.position;
            }
            return V_EntityFight.V_Pos;
        }

        public Vector3 transform_direction(Vector3 _dir)
        {
            return m_PosTransform.TransformDirection(_dir);
        }

        #endregion

        #region about animator
        public void set_animator_speed(float _speed)
        {
            if (m_Animator != null)
            {
                m_Animator.speed = _speed;
            }
        }

        public float get_animator_speed()
        {
            if (m_Animator != null)
            {
                return m_Animator.speed;
            }
            return 1;
        }

        public string get_cur_ani_name()
        {
            return cur_ani_name_;
        }

        string fix_anim_name(string _anim_name)
        {
            ComponentAniSelector ani_sel = (ComponentAniSelector)V_Entity.V_ComRoot.get_COM(ComponentDefine.ComponentAniSelector);
            if (ani_sel != null)
            {
                return ani_sel.fix_anim_name(_anim_name);
            }
            return _anim_name;
        }

        public virtual bool raw_play_animation(string _raw_anim_name, float _transit_time = ComponentAniSelector.CROSS_FADE_TIME, float _speed_rate = 1f, float _norm_time = 0, bool _from_server = true)
        {
            string fixed_anim_name = fix_anim_name(_raw_anim_name);

            norm_time_ = _norm_time;
            speed_rate_ = _speed_rate;
            cur_ani_name_ = fixed_anim_name;
            if (_from_server)
            {
                cur_ani_start_time_ = Time.time;
            }

            if (!string.IsNullOrEmpty(cur_ani_name_))
            {
                animator_crossfade(cur_ani_name_, _transit_time, norm_time_, speed_rate_);
            }

            OptimatorAnimator(V_Entity.V_ModelId, cur_ani_name_);
            return true;
        }

        public void animator_crossfade(string _anim_name, float _transit_time, float _norm_time, float _speed_rate)
        {
            if (m_Animator != null)
            {
                m_Animator.CrossFade(_anim_name, _transit_time, -1, _norm_time);
                m_Animator.speed = _speed_rate;
            }
        }

        void OptimatorAnimator(int modelId, string anim_name)
        {
            if (m_Animator == null) return;
            // 读配置初始化，凡是在配置里配置的，都不优化
            Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
            if (dic.ContainsKey(modelId) && dic[modelId].Contains(anim_name))
            {
                if (m_Animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                    m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
            else
            {
                if (m_Animator.cullingMode != AnimatorCullingMode.CullUpdateTransforms)
                    m_Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            }
        }
        #endregion
    }
}

