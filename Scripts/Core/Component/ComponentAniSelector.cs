using UnityEngine;
using System.Collections;
using SGameEngine;
using SGameSetting;
using System.Collections.Generic;
using TGame.Entity;

namespace TGameEngine
{
	public enum ANI_TYPE
    {
		ANI_GUARD = 21,     //按序保持原编号
		ANI_IDLE,
		ANI_DOWN = 26,
		ANI_STUN = 27,
		ANI_THROW,
		ANI_DIE,

        ANI_SPAWN = 30,     // 出生动画, 从30开始
        ANI_SPAWN_02 = 31,
        ANI_SPAWN_03 = 32,
        ANI_SPAWN_04 = 33,
        ANI_SPAWN_05 = 34,
        ANI_SPAWN_06 = 35,
        ANI_SPAWN_07 = 36,
        ANI_SPAWN_08 = 37,
        ANI_SPAWN_09 = 38,
        ANI_SPAWN_10 = 39,
        ANI_SPAWN_11 = 40,
        
        ANI_ACT    = 41,    // act emote
        ANI_ACT_02 = 42,
        ANI_ACT_03 = 43,
        ANI_ACT_04 = 44,
        ANI_ACT_05 = 45,
        ANI_ACT_06 = 46,
        ANI_ACT_07 = 47,
        ANI_ACT_08 = 48,
        ANI_ACT_09 = 49,
        ANI_ACT_10 = 50,
        ANI_ACT_11 = 51,
        ANI_ACT_12 = 52,
        ANI_ACT_13 = 53,
        ANI_ACT_14 = 54,
        ANI_ACT_15 = 55,
        ANI_ACT_16 = 56,
        ANI_ACT_17 = 57,
        ANI_ACT_18 = 58,
        ANI_ACT_19 = 59,
        ANI_ACT_20 = 60,
        ANI_ACT_21 = 61,

        ANI_CLOSE = 71,  //Trigger
        ANI_CLOSED = 72,
        ANI_OPEN = 73,
        ANI_OPENED = 74,

        ANI_HIT,
		ANI_HIT_LEFT_01,
		ANI_HIT_LEFT_02,
		ANI_HIT_LEFT_03,
		ANI_HIT_LEFT_04,
		ANI_HIT_RIGHT_01,
		ANI_HIT_RIGHT_02,
		ANI_HIT_RIGHT_03,
		ANI_HIT_RIGHT_04,
		ANI_HIT_MIDDLE_01,
		ANI_HIT_MIDDLE_02,
		ANI_HIT_MIDDLE_03,
		ANI_HIT_MIDDLE_04,
        ANI_HITFLY,
        ANI_DIEFLY,
        ANI_TURN_BEFORE,
        ANI_TURN_AFTER,
        ANI_SHAKE,
        ANI_FLOAT,
        ANI_DIEFLOAT,

		ANI_NONE = 100,
		ANI_WALK,
        ANI_WALK_LEFT,
        ANI_WALK_RIGHT,
        ANI_WALK_BACK,
        ANI_WALK_02,
        ANI_WALK_03,
        ANI_WALK_04,

		ANI_RUN = 120,
        ANI_RUN_LEFT,
        ANI_RUN_RIGHT,
        ANI_RUN_BACK,
        ANI_RUN_02,
        ANI_RUN_03,
        ANI_RUN_MASTER,
		ANI_RUN_BATTLE,

		ANI_RUSH = 140,
        ANI_RUSH_FRONT,
        ANI_RUSH_RIGHT,
		ANI_RUSH_02,
        ANI_RUSH_03,
        ANI_RUSH_MASTER,

        ANI_SHUFFLE_LEFT = 173,
        ANI_SHUFFLE_RIGHT = 174,

        ANI_SPAWN_12 = 190,
        ANI_SPAWN_13 = 191,
        ANI_SPAWN_14 = 192,
        ANI_SPAWN_15 = 193,
        ANI_SPAWN_16 = 194,
        ANI_SPAWN_17 = 195,
        ANI_SPAWN_18 = 196,
        ANI_SPAWN_19 = 197,

        ANI_ID_MAX = 240,
    };

    public class ComponentAniSelector : Component
    {
        public const float CROSS_FADE_TIME = 0.1f;

        class ANI_TYPE_DIC_COMPARE : EqualityComparer<ANI_TYPE> 
        {
            public override int GetHashCode(ANI_TYPE type)
            {
                return (int)type;
            }

            public override bool Equals(ANI_TYPE x, ANI_TYPE y)
            {
                return (int)x == (int)y;
            }
        }
        static ANI_TYPE_DIC_COMPARE anim_type_compare = new ANI_TYPE_DIC_COMPARE();

        public static Dictionary<ANI_TYPE, string> ANI_TYPE_DICT = new Dictionary<ANI_TYPE, string>(anim_type_compare);

        public static HashSet<string> SPECIAL_ANIM_TIME = new HashSet<string>();  // 死亡之类的动作时间由状态控制

        struct AnimClipInfo
        {
            public AnimationClip clip_;
            public float length_;
            public WrapMode wrap_mode_;
        }
        Dictionary<string, AnimClipInfo> ani_info_list_ = new Dictionary<string, AnimClipInfo>();

        void check_animator_state(RuntimeAnimatorController _runtime_animator_controller, int _avatar_id)
        {
#if false//
            UnityEditor.Animations.AnimatorController animator_controller = (UnityEditor.Animations.AnimatorController)_runtime_animator_controller;
            UnityEditor.Animations.AnimatorStateMachine state_machine = animator_controller.GetLayer(0).stateMachine;

            for (int i = 0; i < state_machine.stateCount; i++)
            {
                UnityEditor.Animations.AnimatorState state = state_machine.GetState(i);
                AnimationClip clip = (AnimationClip)state.GetMotion();

                if (clip == null)
                {
                    SGameEngine.SGameLog.log(string.Format("State {0} has not Anim! model: {1}, baseprefab: {2}", state.name, _avatar_id, sculpt.data[_avatar_id].baseprefab));
                }
                else if (clip.name != state.name)
                {
                    SGameEngine.SGameLog.log_error(string.Format("State name is not same as Anim Name: {0}, {1}, model: {2}, baseprefab: {3}", state.name, clip.name, _avatar_id, sculpt.data[_avatar_id].baseprefab));
                }
            }
#endif
        }

        public void init_anim_list(Animator _animator, int _avatar_id)
        {
            ani_info_list_.Clear();

            if (_animator == null || _animator.runtimeAnimatorController == null || _animator.runtimeAnimatorController.GetType() != typeof(AnimatorOverrideController))
            {
                //机关等允许没有动作
                //SGameLog.log_error(string.Format("!!!!!!!!!!!!!!!!! No use AnimatorOverrideController: {0} !!!!!!!!!!!!!!!!!!", _avatar_id));
                return;
            }

            AnimatorOverrideController animator_controller = (AnimatorOverrideController)_animator.runtimeAnimatorController;

#if UNITY_EDITOR && !BUNDLE
            check_animator_state(animator_controller.runtimeAnimatorController, _avatar_id);
#endif

            AnimationClipPair[] anim_clips = animator_controller.clips;

            for (int i = 0; i < anim_clips.Length; ++i)
            {
                AnimClipInfo clipInfo = new AnimClipInfo();

                string name =  anim_clips[i].originalClip.name;
                clipInfo.clip_ = anim_clips[i].overrideClip == null ? anim_clips[i].originalClip : anim_clips[i].overrideClip;
                clipInfo.wrap_mode_ = clipInfo.clip_.wrapMode;
                if (clipInfo.wrap_mode_ == WrapMode.Loop || SPECIAL_ANIM_TIME.Contains(name)) {
                    clipInfo.length_ = float.MaxValue;
                } else {
                    clipInfo.length_ = clipInfo.clip_.length;
                }

                ani_info_list_.Add(name, clipInfo);
            }
        }

        string get_instead_anim(string _anim_name)
        {
            Entity obj = owner_ as Entity;

            if (obj.V_Type == EM_EntityType.Trigger)
            {
                return null;
            }

            string instead_anim_name = null;
            int modelId = obj.V_ModelId;

            if (ani_info_list_.ContainsKey(ANI_TYPE_DICT[ANI_TYPE.ANI_GUARD]))
            {
                instead_anim_name = ANI_TYPE_DICT[ANI_TYPE.ANI_GUARD];
            }
            else if (ani_info_list_.ContainsKey(ANI_TYPE_DICT[ANI_TYPE.ANI_WALK]))
            {
                instead_anim_name = ANI_TYPE_DICT[ANI_TYPE.ANI_WALK];
            }
            else
            {
                foreach (string ani in ani_info_list_.Keys)
                {
                    if (ani.IndexOf("default") != -1)
                    {
                        continue;
                    }
                    instead_anim_name = ani;
                }
            }

            SGameEngine.SGameLog.log_error(string.Format("model: {0}, No such animation: {1}, instead of {2}.", modelId, _anim_name, instead_anim_name));

            return instead_anim_name;
        }

        public string fix_anim_name(string _anim_name)
        {
            if (ani_info_list_.Count == 0)
            {
                return _anim_name;
            }

            if (ani_info_list_.ContainsKey(_anim_name))
            {
                return _anim_name;
            }

            return get_instead_anim(_anim_name);
        }

        public bool is_anim_exist(ANI_TYPE _ani_type)
        {
            return ani_info_list_.ContainsKey(ani_type_2_name(_ani_type));
        }

        public bool is_anim_name_exist(string _ani_name)
        {
            return ani_info_list_.ContainsKey(_ani_name);
        }

        public float raw_get_anim_time(string _anim_name)
        {
            if (ani_info_list_.ContainsKey(_anim_name))
            {
                return ani_info_list_[_anim_name].length_;
            }
            return 0;
        }

        public string ani_type_2_name(ANI_TYPE _ani_type)
        {
            return get_anim_by_type(_ani_type, owner_.V_ModelId);
        }

        public static bool is_spec_rush(ANI_TYPE _ani_type)
        {
            return _ani_type == ANI_TYPE.ANI_RUSH_FRONT || _ani_type == ANI_TYPE.ANI_RUSH_RIGHT;
        }

        // 直接返回固定列表里的动作名
        public static string get_anim_by_type(ANI_TYPE _ani_type, int _model_id = 0)
        {
            // > 240 拼接技能动作名
            if (_ani_type >= ANI_TYPE.ANI_ID_MAX && _model_id > 0)
            {
                return string.Format("skill_{0}{1:D2}", _model_id, (int)_ani_type - (int)ANI_TYPE.ANI_ID_MAX);
            }

            if (ANI_TYPE_DICT.Count == 0)
            {
                init_ani_type_dict();
            }

            if (ANI_TYPE_DICT.ContainsKey(_ani_type))
            {
                return ANI_TYPE_DICT[_ani_type];
            }
            else
            {
                SGameEngine.SGameLog.log_error(string.Format("ani_type not have {0}, model:{1}", _ani_type, _model_id));
                return ANI_TYPE_DICT[ANI_TYPE.ANI_GUARD];
            }
        }

        public static bool is_spawn_anim(ANI_TYPE _ani_type)
        {
            return (_ani_type >= ANI_TYPE.ANI_SPAWN && _ani_type <= ANI_TYPE.ANI_SPAWN_11) || (_ani_type >= ANI_TYPE.ANI_SPAWN_12 && _ani_type <= ANI_TYPE.ANI_SPAWN_19);
        }

        public static string get_raw_ani_name_by_type(ANI_TYPE _ani_type)
        {
            if (ANI_TYPE_DICT.ContainsKey(_ani_type))
            {
                return ANI_TYPE_DICT[_ani_type];
            }
            return null;
        }

		public static void init_ani_type_dict()
		{
			ANI_TYPE_DICT[ANI_TYPE.ANI_WALK] = "walk";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_LEFT] = "walk_left";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_RIGHT] = "walk_right";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_BACK] = "walk_back";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_02] = "walk_02";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_03] = "walk_03";
            ANI_TYPE_DICT[ANI_TYPE.ANI_WALK_04] = "walk_04";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUN] = "run";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_LEFT] = "run_left";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_RIGHT] = "run_right";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_BACK] = "run_back";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_MASTER] = "run";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_02] = "run_02";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_03] = "run_03";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH] = "run_fast";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH_FRONT] = "rush";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH_RIGHT] = "rush_tr";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH_02] = "run_fast_02";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH_03] = "run_fast_03";
            ANI_TYPE_DICT[ANI_TYPE.ANI_RUSH_MASTER] = "run";
			ANI_TYPE_DICT[ANI_TYPE.ANI_GUARD] = "guard";
			ANI_TYPE_DICT[ANI_TYPE.ANI_IDLE] = "idle";
			ANI_TYPE_DICT[ANI_TYPE.ANI_DOWN] = "down";
			ANI_TYPE_DICT[ANI_TYPE.ANI_RUN_BATTLE] = "run_battle";
			ANI_TYPE_DICT[ANI_TYPE.ANI_STUN] = "stun";
			ANI_TYPE_DICT[ANI_TYPE.ANI_THROW] = "throw";
            ANI_TYPE_DICT[ANI_TYPE.ANI_FLOAT] = "hitfloat";
			ANI_TYPE_DICT[ANI_TYPE.ANI_DIE] = "die";
            ANI_TYPE_DICT[ANI_TYPE.ANI_DIEFLOAT] = "diefloat";
            ANI_TYPE_DICT[ANI_TYPE.ANI_CLOSE] = "close";
            ANI_TYPE_DICT[ANI_TYPE.ANI_CLOSED] = "closed";
            ANI_TYPE_DICT[ANI_TYPE.ANI_OPEN] = "open";
            ANI_TYPE_DICT[ANI_TYPE.ANI_OPENED] = "opened";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN] = "spawn";
			ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_02] = "spawn_02";
			ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_03] = "spawn_03";
			ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_04] = "spawn_04";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_05] = "spawn_05";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_06] = "spawn_06";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_07] = "spawn_07";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_08] = "spawn_08";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_09] = "spawn_09";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_10] = "spawn_10";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_11] = "spawn_11";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT] = "act";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_02] = "act_02";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_03] = "act_03";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_04] = "act_04";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_05] = "act_05";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_06] = "act_06";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_07] = "act_07";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_08] = "act_08";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_09] = "act_09";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_10] = "act_10";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_11] = "act_11";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_12] = "act_12";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_13] = "act_13";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_14] = "act_14";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_15] = "act_15";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_16] = "act_16";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_17] = "act_17";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_18] = "act_18";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_19] = "act_19";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_20] = "act_20";
            ANI_TYPE_DICT[ANI_TYPE.ANI_ACT_21] = "act_21";
            ANI_TYPE_DICT[ANI_TYPE.ANI_HIT] = "hit";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_LEFT_01] = "hit_left_01";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_LEFT_02] = "hit_left_02";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_LEFT_03] = "hit_left_03";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_LEFT_04] = "hit_left_04";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_RIGHT_01] = "hit_right_01";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_RIGHT_02] = "hit_right_02";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_RIGHT_03] = "hit_right_03";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_RIGHT_04] = "hit_right_04";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_MIDDLE_01] = "hit_middle_01";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_MIDDLE_02] = "hit_middle_02";
            ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_MIDDLE_03] = "hit_middle_03";
			ANI_TYPE_DICT[ANI_TYPE.ANI_HIT_MIDDLE_04] = "hit_middle_04";
            ANI_TYPE_DICT[ANI_TYPE.ANI_HITFLY] = "hitfly";
            ANI_TYPE_DICT[ANI_TYPE.ANI_DIEFLY] = "diefly";
            ANI_TYPE_DICT[ANI_TYPE.ANI_TURN_BEFORE] = "turn_before";
            ANI_TYPE_DICT[ANI_TYPE.ANI_TURN_AFTER] = "turn_after";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SHAKE] = "act_shake";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SHUFFLE_LEFT] = "shuffle_left";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SHUFFLE_RIGHT] = "shuffle_right";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_12] = "spawn_12";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_13] = "spawn_13";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_14] = "spawn_14";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_15] = "spawn_15";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_16] = "spawn_16";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_17] = "spawn_17";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_18] = "spawn_18";
            ANI_TYPE_DICT[ANI_TYPE.ANI_SPAWN_19] = "spawn_19";

            SPECIAL_ANIM_TIME.Add("diefloat");
            SPECIAL_ANIM_TIME.Add("diefly");
            SPECIAL_ANIM_TIME.Add("die");
		}
    }
}
