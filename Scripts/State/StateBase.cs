using UnityEngine;
using TGame.Entity;

namespace TGameEngine
{
    public class StateRule
    {
        public static int[,] table = new int[StateDefine.STATE_MAX, StateDefine.STATE_MAX]
        {
            // 0 deny, 1 swap, 2 together 3 delay
            //  0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21
            {   1,  1,  0,  1,  1,  3,  1,  0,  3,  1,  3,  1,  3,  1,  0,  3,  2,  0,  0,  3,  1,  0},      //  0   STATE_STAND
            {   1,  1,  0,  1,  1,  3,  1,  1,  0,  0,  0,  2,  0,  0,  0,  0,  2,  0,  0,  0,  1,  0},      //  1   STATE_MOVE_TO     
            {   1,  1,  0,  1,  1,  1,  1,  1,  3,  1,  1,  1,  3,  1,  0,  3,  2,  2,  1,  1,  1,  1},      //  2   STATE_DEAD
            {   1,  1,  0,  1,  1,  0,  1,  1,  0,  0,  0,  1,  0,  0,  0,  0,  0,  0,  0,  0,  1,  0},      //  3   STATE_SECTION_SKILLING     
            {   1,  1,  0,  1,  1,  0,  1,  3,  0,  0,  0,  1,  0,  2,  0,  0,  0,  0,  0,  0,  1,  0},      //  4   STATE_SKILLING    
            {   1,  1,  0,  1,  1,  1,  1,  1,  0,  0,  1,  0,  0,  2,  0,  0,  2,  0,  0,  0,  1,  0},      //  5   STATE_HURT
            {   1,  1,  0,  1,  1,  0,  1,  1,  0,  0,  3,  2,  0,  0,  0,  0,  2,  0,  0,  0,  1,  0},      //  6   STATE_MOVE_GROUND 
            {   1,  1,  0,  1,  1,  0,  1,  1,  3,  0,  3,  0,  3,  0,  0,  3,  0,  0,  0,  3,  1,  0},      //  7   STATE_NAVIGATION
            {   1,  1,  0,  1,  1,  1,  1,  1,  1,  2,  1,  0,  3,  0,  0,  1,  0,  0,  0,  0,  1,  1},      //  8   STATE_HURT_BACK
            {   1,  1,  0,  3,  1,  1,  1,  1,  2,  1,  2,  1,  2,  2,  0,  2,  2,  0,  1,  2,  1,  2},      //  9   STATE_DAZED
            {   1,  1,  0,  1,  1,  1,  1,  1,  3,  0,  1,  0,  0,  2,  0,  3,  0,  0,  1,  0,  1,  1},      //  10  STATE_HURT_FLY
            {   1,  2,  0,  1,  1,  0,  2,  3,  0,  0,  0,  1,  0,  2,  0,  0,  2,  0,  0,  0,  1,  0},      //  11  STATE_MOVE_PERSIST
            {   1,  1,  0,  1,  1,  1,  1,  1,  1,  0,  1,  0,  3,  0,  0,  3,  0,  0,  1,  0,  1,  1},      //  12  STATE_HURT_BACK_FLY
            {   1,  1,  0,  3,  1,  3,  1,  1,  3,  3,  3,  2,  3,  1,  0,  3,  1,  0,  0,  3,  1,  3},      //  13  STATE_HOLD
            {   1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  1,  0,  0,  0,  0,  1,  3},      //  14  STATE_CINEMA
            {   1,  1,  0,  1,  1,  1,  1,  1,  1,  2,  1,  0,  3,  0,  0,  1,  0,  0,  1,  0,  1,  1},      //  15  STATE_HURT_HORI
            {   2,  2,  2,  0,  0,  2,  2,  1,  0,  2,  0,  2,  0,  0,  0,  0,  0,  0,  1,  0,  1,  2},      //  16  STATE_DRAG
            {   1,  1,  2,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1},      //  17  STATE_REPLACE
            {   1,  2,  0,  0,  1,  0,  2,  1,  0,  0,  0,  0,  0,  2,  0,  0,  0,  0,  0,  0,  1,  0},      //  18  STATE_LEADING
            {   1,  1,  0,  0,  1,  1,  1,  1,  1,  2,  1,  0,  0,  0,  0,  1,  0,  0,  0,  0,  1,  2},      //  19  STATE_PULL
            {   1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0},      //  20  STATE_PICK
            {   1,  1,  0,  1,  1,  1,  1,  1,  2,  2,  1,  0,  2,  2,  0,  0,  2,  0,  0,  2,  1,  1},      //  21  STATE_HURT_FLOAT
        };
    }

	public class StateBase : TGameEngine.Component
    {
        public static float SLIDE_THRESHOLD = 0.001f;
        public static float SLIDE_SAMPLE_RANGE = 1f;
        public static float SLIDE_MOVE_MAX = 0.184f;

        public float MIN_MAGNITUDE_MOVE_TO = 0.2f;

        protected new EntityFight owner_ { get { return (EntityFight)base.owner_; } }

		protected int state_index_;
		protected ComponentState state_COM;
        protected float ani_start_time_;

		public virtual int init(ComponentState state, int index, params object[] p)
		{
			base.owner_ = state.get_owner();
			state_index_ = index;
			state_COM = state;
			return StateRetCode.RT_OK;
		}

		public override void update()
		{
			base.update();
		}

        public virtual int destroy()
		{
			base.owner_ = null;
			state_COM = null;
			return StateRetCode.RT_OK;
		}

		public new EntityFight get_owner()
		{
			return owner_;
		}

        public bool is_arrive_xz(Vector3 _target_pos)
        {
            Vector3 delta = owner_.get_position() - _target_pos;
            delta = new Vector2(delta.x, delta.z);
            if (delta.magnitude < MIN_MAGNITUDE_MOVE_TO)
            {
                return true;
            }
            return false;
        }

        public float calc_angle(Vector3 delta)
        {
            return (90 - Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg);
        }

        public void calc_navmesh_y(ref Vector3 pos)
        {
            RaycastHit hit = new RaycastHit();
            bool hitted = Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, out hit);
            if (hitted)
            {
                pos.y = hit.point.y;
            }
        }

        private Vector3 calc_move_delta(float _speed, float _angle, out Vector3 _dst_pos)
        {
            float x_dir = Mathf.Sin(Mathf.Deg2Rad * _angle);
            float z_dir = Mathf.Cos(Mathf.Deg2Rad * _angle);
            Vector3 move_dir = new Vector3(x_dir, 0, z_dir);
            
            Vector3 delta = move_dir * _speed * Time.deltaTime;
            Vector3 cur_pos = owner_.get_position();
            calc_navmesh_y(ref cur_pos);
            _dst_pos = cur_pos + delta;
            calc_navmesh_y(ref _dst_pos);
            NavMeshHit hit = new NavMeshHit();
            if (true/*SceneManager.Instance.ray_cast(cur_pos, _dst_pos, out hit)*/)
            {
                delta = Vector3.zero;
            }
            else
            {
                if (hit.distance > 100000)
                    delta = Vector3.zero;
                else
                    delta = hit.position - cur_pos;
            }
            return delta;
        }
        
        private bool check_need_slide(Vector3 _delta)
        {
            if (Mathf.Abs(_delta.x) < SLIDE_THRESHOLD && Mathf.Abs(_delta.z) < SLIDE_THRESHOLD)
            {
                return true;
            }
            return false;
        }
        
        private Vector3 choose_slide_pos(float _speed, Vector3 _dst_pos)
        {
            Vector3 cur_pos = owner_.get_position();
            calc_navmesh_y(ref cur_pos);
            Vector3 delta = _dst_pos - cur_pos;
            Vector2 delta2 = new Vector2 (delta.x, delta.z);
            if (delta2.magnitude > SLIDE_MOVE_MAX) 
            {
                delta = delta * SLIDE_MOVE_MAX / delta2.magnitude;      
                _dst_pos = cur_pos + delta;
                calc_navmesh_y(ref _dst_pos);
            }
            NavMeshHit hit=new NavMeshHit();
            if (true/*SceneManager.Instance.sample_nav_position(_dst_pos, out hit, SLIDE_SAMPLE_RANGE)*/)
            {
                return hit.position;
            }
            return cur_pos;
        }
        
        private Vector3 get_dir(float _angle)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * _angle);
            float z = Mathf.Cos(Mathf.Deg2Rad * _angle);
            return new Vector3(x, 0, z);
        }
        
        private float get_angle_y(Vector3 _v)
        {
            _v.y = 0f;
            if (_v.x >= 0)
            {
                return Vector3.Angle(Vector3.forward, _v);
            }
            else
            {
                return 360 - Vector3.Angle(Vector3.forward, _v);
            }
        }
        
        private void calc_speed_angle(out float _speed, out float _angle, float _scale = 1.0f)
        {
            Vector3 move_vc = new Vector3(0, 0, 0);
            Vector3 drag_vc = new Vector3(0, 0, 0);
            Vector3 final_vc;
            
            /*
            if (owner_.state_COM.is_state(StateDefine.STATE_MOVE_GROUND))
            {
                StateMoveGround move_ground = (StateMoveGround)(owner_.state_COM.get_desc(StateDefine.STATE_MOVE_GROUND).handle);
                float speed = owner_.get_speed();
                float angle = move_ground.get_angle();
                move_vc = get_dir(angle);
                move_vc = move_vc * speed * _scale;
            }
            
            if (owner_.state_COM.is_state(StateDefine.STATE_DRAG))
            {
                StateDrag drag = (StateDrag)(owner_.state_COM.get_desc(StateDefine.STATE_DRAG).handle);
                float drag_x = drag.get_drag_x();
                float drag_z = drag.get_drag_z();
                float drag_speed = drag.get_drag_speed();
                
                drag_vc.Set(drag_x, 0, drag_z);
                Vector3 cur_pos = owner_.get_position();
                cur_pos.y = 0;
                drag_vc = drag_vc - cur_pos;
                drag_vc.Normalize();
                drag_vc = drag_vc * drag_speed * _scale;
            }
            */
            final_vc = move_vc + drag_vc;
            _speed = final_vc.magnitude;
            _angle = get_angle_y(final_vc);
        }

        public Vector3 get_step_move_pos(Vector3 _begin_pos, float _scale = 1.0f)
        {
            float speed = 0.0f;
            float angle = 0.0f;
            calc_speed_angle(out speed, out angle, _scale);
            Vector3 dst_pos;
            Vector3 delta = calc_move_delta(speed, angle, out dst_pos);
            Vector3 pos = _begin_pos + delta;

            /*
            if (owner_.state_COM.is_state(StateDefine.STATE_DRAG) && !owner_.state_COM.is_state(StateDefine.STATE_MOVE_GROUND))
            {
                StateDrag drag = (StateDrag)(owner_.state_COM.get_desc(StateDefine.STATE_DRAG).handle);
                Vector3 drag_pos = new Vector3(drag.get_drag_x(), 0, drag.get_drag_z());
                Vector3 cur_pos = _begin_pos;
                cur_pos.y = 0;
                if (Vector3.Distance(drag_pos, cur_pos) < drag.get_drag_speed() * Time.deltaTime)
                {
                    pos = drag_pos;
                }
            }
            */
            if (check_need_slide(delta))
            {
                pos = choose_slide_pos(speed, dst_pos);
            }

            return pos;
        }

        public void step_move()
        {
            Vector3 pos = get_step_move_pos(owner_.get_position());
            owner_.set_position(pos, true);
        }

        public virtual void idle_begin()
        {

        }

        public virtual void idle_end()
        {

        }
    }
}
