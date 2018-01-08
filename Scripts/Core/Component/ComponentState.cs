using System.Collections.Generic;
using UnityEngine;
using TGame.Entity;

namespace TGameEngine
{
    public class StateUtil
    {
        public static void set_bit(int _nr, ref int[] _addr)
        {
            int mask = 1 << (_nr & 0x1f);
            _addr[_nr >> 5] |= mask;
        }

        public static void clear_bit(int _nr, ref int[] _addr)
        {
            int mask = 1 << (_nr & 0x1f);
            _addr[_nr >> 5] &= (~mask);
        }

        public static bool test_bit(int _nr, ref int[] _addr)
        {
            int mask = 1 << (_nr & 0x1f);
            return (_addr[_nr >> 5] & mask) != 0;
        }

        public static int find_next_set_bit(ref int[] _addr, int _offset)
        {
            int length = (sizeof(int) * StateDefine.MAX_BITSET) << 3;
            int i;
            for (i = _offset; i < length; i++)
            {
                if (StateUtil.test_bit(i, ref _addr))
                {
                    return i;
                }
            }
            return length;
        }

        public static int MK_MSG(int _id, int _state)
        {
            return _id + (_state << 8);
        }
    }

    public class StateDesc
    {
        public int[] deny = new int[StateDefine.MAX_BITSET];
        public int[] swap = new int[StateDefine.MAX_BITSET];
        public int[] dely = new int[StateDefine.MAX_BITSET];
        public StateBase handle;
    };

    public class ComponentState : TGameEngine.Component
    {
        // state data
        private int[] run_set_ = new int[StateDefine.MAX_BITSET];
        private int[] pause_set_ = new int[StateDefine.MAX_BITSET];
        private int[] last_set_ = new int[StateDefine.MAX_BITSET];
        private int[] next_set_ = new int[StateDefine.MAX_BITSET];
        private object[] next_param_;
        private object[][] data_ = new object[StateDefine.STATE_MAX][];
        private object[][] msg_queue_ = new object[StateLimit.MAX_MESSAGE][];
        private int qhead_ = 0;
        private int qtail_ = 0;
        private int msg_tm_ = 0;

        // retry 
        public static int MSG_RETRY_MAX = 1000;

        // state desc
        public StateDesc[] v_state = new StateDesc[StateDefine.STATE_MAX];

        public static void set_msg_retry_max(int _retry_max)
        {
            MSG_RETRY_MAX = _retry_max;
        }

        public ComponentState()
        {
            int i, j;

            for (i = 0; i < StateDefine.STATE_MAX; i++)
            {
                data_[i] = new object[StateLimit.MAX_PARAM];
            }

            for (i = 0; i < StateLimit.MAX_MESSAGE; i++)
            {
                msg_queue_[i] = new object[StateLimit.MESSAGE_PARAM_NUM];
            }

            for (i = 0; i < StateDefine.MAX_BITSET; i++)
            {
                run_set_[i] = 0;
                pause_set_[i] = 0;
                last_set_[i] = 0;
                next_set_[i] = 0;
            }

            next_param_ = null;

            for (i = 0; i < StateDefine.STATE_MAX; i++)
            {
                for (j = 0; j < StateLimit.MAX_PARAM; j++)
                {
                    data_[i][j] = null;
                }
            }

            for (i = 0; i < StateDefine.STATE_MAX; ++i)
            {
                StateDesc desc = get_desc(i);
                for (j = 0; j < StateDefine.STATE_MAX; ++j)
                {
                    if (StateRule.table[i, j] == 0)
                    {
                        StateUtil.set_bit(j, ref desc.deny);
                    }
                    else if (StateRule.table[i, j] == 1)
                    {
                        StateUtil.set_bit(j, ref desc.swap);
                    }
                    else if (StateRule.table[i, j] == 3)
                    {
                        StateUtil.set_bit(j, ref desc.dely);
                    }
                }
            }

            /*
            set_state_handle(StateDefine.STATE_STAND, new StateStand());
            set_state_handle(StateDefine.STATE_MOVE_TO, new StateMoveTo());
            set_state_handle(StateDefine.STATE_DEAD, new StateDead());
            set_state_handle(StateDefine.STATE_SECTION_SKILLING, new StateSectionSkilling());
            set_state_handle(StateDefine.STATE_SKILLING, new StateSkilling());
            set_state_handle(StateDefine.STATE_HURT, new StateHurt());
            set_state_handle(StateDefine.STATE_MOVE_GROUND, new StateMoveGround());
            set_state_handle(StateDefine.STATE_NAVIGATION, new StateNavigation());
            set_state_handle(StateDefine.STATE_HURT_BACK, new StateHurtBack());
            set_state_handle(StateDefine.STATE_DAZED, new StateDazed());
            set_state_handle(StateDefine.STATE_HURT_FLY, new StateHurtFly());
            set_state_handle(StateDefine.STATE_HURT_FLOAT, new StateHurtFloat());
            set_state_handle(StateDefine.STATE_MOVE_PERSIST, new StateMovePersist());
            set_state_handle(StateDefine.STATE_HURT_BACK_FLY, new StateHurtBackFly());
            set_state_handle(StateDefine.STATE_HOLD, new StateHold());
            set_state_handle(StateDefine.STATE_CINEMA, new StateCinema());
            set_state_handle(StateDefine.STATE_HURT_HORI, new StateHurtHori());
            set_state_handle(StateDefine.STATE_DRAG, new StateDrag());
            set_state_handle(StateDefine.STATE_REPLACE, new StateReplace());
            set_state_handle(StateDefine.STATE_LEADING, new StateLeading());
            set_state_handle(StateDefine.STATE_PULL, new StatePull());
            set_state_handle(StateDefine.STATE_PICK, new StatePick());
            */
        }

        public int set_state_handle(int _state, StateBase _handle)
        {
            if (_state >= 0 && _state < StateDefine.STATE_MAX)
            {
                v_state[_state].handle = _handle;
                return StateRetCode.RT_OK;
            }
            return StateRetCode.RT_ER;
        }

        public StateDesc get_desc(int _index)
        {
            if (v_state[_index] == null)
            {
                int i;
                v_state[_index] = new StateDesc();
                for (i = 0; i < StateDefine.MAX_BITSET; i++)
                {
                    v_state[_index].deny[i] = 0;
                    v_state[_index].swap[i] = 0;
                    v_state[_index].dely[i] = 0;
                }
            }
            return v_state[_index];
        }

        public int del_state(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                if (StateUtil.test_bit(_index, ref run_set_))
                {
                    StateUtil.clear_bit(_index, ref run_set_);
                    if (v_state[_index].handle != null)
                    {
                        v_state[_index].handle.destroy();
                    }
                    return StateRetCode.RT_OK;
                }
            }
            return StateRetCode.RT_ER;
        }

        public int del_all_state()
        {
            for (int index = 0; index < StateDefine.STATE_MAX; index++)
            {
                del_state(index);
            }
            clear_msg_queue();
            return StateRetCode.RT_OK;
        }

        public int del_hurt_states()
        {
            del_state(StateDefine.STATE_HURT);
            del_state(StateDefine.STATE_HURT_BACK);
            del_state(StateDefine.STATE_HURT_BACK_FLY);
            del_state(StateDefine.STATE_HURT_FLOAT);
            del_state(StateDefine.STATE_HURT_FLY);
            del_state(StateDefine.STATE_HURT_HORI);
            return StateRetCode.RT_OK;
        }

        public int del_state_from_msg_queue(int _index)
        {
            int head = qhead_;
            int tail = qtail_;
            bool is_found = false;
            while (head < tail)
            {
                int idx = head % StateLimit.MAX_MESSAGE;
                int msg = (int)msg_queue_[idx][0];
                int msgid = msg & 0x000000FF;
                int state = (msg >> 8) & 0x0000FFFF;
                if (state == _index && msgid == StateMsg.SYS_SET_STATE)
                {
                    msg_queue_[idx][0] = StateMsg.SYS_DEL_STATE + StateDefine.STATE_MAX * 256;
                    is_found = true;
                }
                head++;
            }
            return is_found ? StateRetCode.RT_OK : StateRetCode.RT_ER;
        }

        public bool is_state(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                return StateUtil.test_bit(_index, ref run_set_);
            }
            return false;
        }

        public int get_cur_state()
        {
            for (int index = 0; index < StateDefine.STATE_MAX; ++index)
            {
                if (StateUtil.test_bit(index, ref run_set_))
                {
                    return index;
                }
            }
            return StateDefine.STATE_MAX;
        }

        public bool is_last_state(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                return StateUtil.test_bit(_index, ref last_set_);
            }
            return false;
        }

        public bool is_next_state(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                return StateUtil.test_bit(_index, ref next_set_);
            }
            return false;
        }

        public bool is_next_state_hurts()
        {
            return is_next_state(StateDefine.STATE_HURT) ||
                   is_next_state(StateDefine.STATE_HURT_BACK) ||
                   is_next_state(StateDefine.STATE_HURT_BACK_FLY) ||
                   is_next_state(StateDefine.STATE_HURT_FLOAT) ||
                   is_next_state(StateDefine.STATE_HURT_FLY) ||
                   is_next_state(StateDefine.STATE_HURT_HORI);
        }

        public object get_next_state_param(int _index)
        {
            if (next_param_ != null)
            {
                if (next_param_.Length > _index)
                {
                    return next_param_[_index];
                }
            }
            return null;
        }

        public int send_msg(int _msg, params object[] _params)
        {
            return do_send_msg(_msg, _params);
        }

        public int post_msg(int _msg, params object[] _params)
        {
            if (qhead_ == qtail_)
            {
                int rt = do_send_msg(_msg, _params);
                if (rt != StateRetCode.RT_DE)
                {
                    return rt;
                }
            }

            if (qtail_ - qhead_ < StateLimit.MAX_MESSAGE)
            {
                int index = qtail_ % StateLimit.MAX_MESSAGE;
                msg_queue_[index][0] = _msg;
                _params.CopyTo(msg_queue_[index], 1);
                qtail_++;
                return StateRetCode.RT_DE;
            }
            else
            {
                return StateRetCode.RT_ER;
            }
        }

        public void set_param(int _state_index, int _param_index, object _value)
        {
            if (_state_index >= 0 && _state_index < StateDefine.STATE_MAX)
            {
                if (_param_index >= 0 && _param_index < StateLimit.MAX_PARAM)
                {
                    data_[_state_index][_param_index] = _value;
                }
            }
        }

        public object get_param(int _state_index, int _param_index)
        {
            if (_state_index >= 0 && _state_index < StateDefine.STATE_MAX)
            {
                if (_param_index >= 0 && _param_index < StateLimit.MAX_PARAM)
                {
                    return data_[_state_index][_param_index];
                }
            }
            return 0;
        }

        public void set_all(int _index, params object[] _params)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                _params.CopyTo(data_[_index], 0);
            }
        }

        public void unserialize()
        {
            del_all_state();

            // read state
        }

        public int force_msg(int _msg, params object[] _params)
        {
            int msgid = _msg & 0x000000FF;
            int state = (_msg >> 8) & 0x0000FFFF;

            if (msgid == StateMsg.SYS_SET_STATE)
            {
                if (state >= 0 && state < StateDefine.STATE_MAX)
                {
                    StateDesc desc = v_state[state];
                    int i;

                    StateUtil.set_bit(state, ref next_set_);
                    next_param_ = _params;

                    int index = -1;
                    while ((index = (int)StateUtil.find_next_set_bit(ref run_set_, index + 1)) < StateDefine.MAX_BITSET_LEN)
                    {
                        if (StateUtil.test_bit(index, ref desc.swap) || StateUtil.test_bit(index, ref desc.dely) || StateUtil.test_bit(index, ref desc.deny))
                        {
                            //del_state(index);
                            StateUtil.clear_bit(index, ref run_set_);
                            if (v_state[index].handle != null)
                            {
                                v_state[index].handle.destroy(); // means force delete state
                            }
                            StateUtil.set_bit(index, ref last_set_);
                        }
                    }

                    StateUtil.set_bit(state, ref run_set_);
                    if (v_state[state].handle != null)
                    {
                        v_state[state].handle.init(this, state, _params);
                    }
                    else
                    {
                        _params.CopyTo(data_[state], 0);
                    }

                    for (i = 0; i < StateDefine.MAX_BITSET; i++)
                    {
                        last_set_[i] = 0;
                        next_set_[i] = 0;
                    }
                    next_param_ = null;
                }
            }
            return StateRetCode.RT_OK;
        }

        public int do_send_msg(int _msg, params object[] _params)
        {
            int msgid = _msg & 0x000000FF;
            int state = (_msg >> 8) & 0x0000FFFF;

            if (msgid == StateMsg.SYS_SET_STATE)
            {
                if (state >= 0 && state < StateDefine.STATE_MAX)
                {
                    StateDesc desc = v_state[state];
                    int i;
                    for (i = 0; i < StateDefine.MAX_BITSET; ++i)
                    {
                        if ((desc.deny[i] & run_set_[i]) > 0)
                        {
                            return StateRetCode.RT_ER;
                        }
                    }

                    for (i = 0; i < StateDefine.MAX_BITSET; ++i)
                    {
                        if ((desc.dely[i] & run_set_[i]) > 0)
                        {
                            return StateRetCode.RT_DE;
                        }
                    }

                    StateUtil.set_bit(state, ref next_set_);
                    next_param_ = _params;

                    int index = -1;
                    while ((index = (int)StateUtil.find_next_set_bit(ref run_set_, index + 1)) < StateDefine.MAX_BITSET_LEN)
                    {
                        if (StateUtil.test_bit(index, ref desc.swap))
                        {
                            //del_state(index);
                            StateUtil.clear_bit(index, ref run_set_);
                            if (v_state[index].handle != null)
                            {
                                v_state[index].handle.destroy(); // means force delete state
                            }
                            StateUtil.set_bit(index, ref last_set_);
                        }
                    }

                    StateUtil.set_bit(state, ref run_set_);
                    if (v_state[state].handle != null)
                    {
                        v_state[state].handle.init(this, state, _params);
                    }
                    else
                    {
                        _params.CopyTo(data_[state], 0);
                    }

                    for (i = 0; i < StateDefine.MAX_BITSET; i++)
                    {
                        last_set_[i] = 0;
                        next_set_[i] = 0;
                    }
                    next_param_ = null;
                }
            }
            else if (msgid == StateMsg.SYS_DEL_STATE)
            {
                if (state >= 0 && state < StateDefine.STATE_MAX)
                {
                    del_state(state);
                }
            }
            else if (msgid == StateMsg.MSG_SETDATA)
            {
                if (state >= 0 && state < StateDefine.STATE_MAX)
                {
                    if ((int)_params[0] >= 0 && (int)(_params[0]) < StateLimit.MAX_PARAM)
                    {
                        set_param(state, (int)_params[0], _params[1]);
                    }
                }
            }
            else if (msgid == StateMsg.MSG_RESET)
            {
                int i;
                for (i = 0; i < StateDefine.MAX_BITSET; i++)
                {
                    run_set_[i] = 0;
                }
                qhead_ = qtail_ = 0;
            }
            return StateRetCode.RT_OK;
        }

        public void pause(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                StateUtil.set_bit(_index, ref pause_set_);
            }
        }

        public int can_be_state(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                StateDesc desc = v_state[_index];
                int i;
                for (i = 0; i < StateDefine.MAX_BITSET; ++i)
                {
                    if ((desc.deny[i] & run_set_[i]) > 0)
                    {
                        return StateRetCode.RT_ER;
                    }
                }

                int head = qhead_;
                int tail = qtail_;
                while (head < tail)
                {
                    int idx = head % StateLimit.MAX_MESSAGE;
                    int post_state = (int)msg_queue_[idx][0];
                    post_state = (post_state >> 8) & 0x0000FFFF;
                    if (StateUtil.test_bit(post_state, ref desc.deny))
                    {
                        return StateRetCode.RT_ER;
                    }
                    else
                    {
                        head++;
                    }
                }

                for (i = 0; i < StateDefine.MAX_BITSET; ++i)
                {
                    if ((desc.dely[i] & run_set_[i]) > 0)
                    {
                        return StateRetCode.RT_DE;
                    }
                }

                head = qhead_;
                tail = qtail_;
                while (head < tail)
                {
                    int idx = head % StateLimit.MAX_MESSAGE;
                    int post_state = (int)msg_queue_[idx][0];
                    post_state = (post_state >> 8) & 0x0000FFFF;
                    if (StateUtil.test_bit(post_state, ref desc.dely))
                    {
                        return StateRetCode.RT_DE;
                    }
                    else
                    {
                        head++;
                    }
                }
                return StateRetCode.RT_OK;
            }
            return StateRetCode.RT_ER;
        }

        public bool can_be_state_late(int _index)
        {
            if (_index >= 0 && _index < StateDefine.STATE_MAX)
            {
                StateDesc desc = v_state[_index];
                int i;
                for (i = 0; i < StateDefine.MAX_BITSET; ++i)
                {
                    if ((desc.deny[i] & run_set_[i]) > 0)
                    {
                        return false;
                    }
                }

                int head = qhead_;
                int tail = qtail_;
                while (head < tail)
                {
                    int idx = head % StateLimit.MAX_MESSAGE;
                    int post_state = (int)msg_queue_[idx][0];
                    post_state = (post_state >> 8) & 0x0000FFFF;
                    if (StateUtil.test_bit(post_state, ref desc.deny))
                    {
                        return false;
                    }
                    else
                    {
                        head++;
                    }
                }
                return true;
            }
            return false;
        }

        public int clear_msg_queue()
        {
            qtail_ = qhead_ = 0;
            return StateRetCode.RT_OK;
        }

        public int get_run_set(int _index)
        {
            return run_set_[_index];
        }

        public bool test_set(ref int[] _set)
        {
            for (int i = 0; i < StateDefine.MAX_BITSET; i++)
            {
                if ((run_set_[i] & _set[i]) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Update is called once per frame
        public override void update()
        {
            int i = 0;
            int index = -1;
            while ((index = StateUtil.find_next_set_bit(ref run_set_, index + 1)) < StateDefine.MAX_BITSET_LEN)
            {
                if (v_state[index].handle != null)
                {
                    if (!StateUtil.test_bit(index, ref pause_set_))
                    {
                        v_state[index].handle.update();
                    }
                    else
                    {
                        StateUtil.clear_bit(index, ref pause_set_);
                    }
                }
            }

            while (qhead_ < qtail_)
            {
                index = qhead_ % StateLimit.MAX_MESSAGE;
                int rt = do_send_msg((int)msg_queue_[index][0],
                    msg_queue_[index][1],
                    msg_queue_[index][2],
                    msg_queue_[index][3],
                    msg_queue_[index][4],
                    msg_queue_[index][5],
                    msg_queue_[index][6],
                    msg_queue_[index][7],
                    msg_queue_[index][8],
                    msg_queue_[index][9]);
                if (rt == StateRetCode.RT_DE)
                {
                    msg_tm_++;
                    if (msg_tm_ > MSG_RETRY_MAX)
                    {
                        qhead_++;
                        msg_tm_ = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    msg_tm_ = 0;
                    qhead_++;
                }
            }


            index = StateDefine.STATE_STAND;

            for (i = 0; i < StateDefine.MAX_BITSET; ++i)
            {
                if ((v_state[index].deny[i] & run_set_[i]) > 0) break;
                if ((v_state[index].dely[i] & run_set_[i]) > 0) break;
                if ((v_state[index].swap[i] & run_set_[i]) > 0) break;
            }

            if (i >= StateDefine.MAX_BITSET)
            {
                StateUtil.set_bit(index, ref run_set_);
                EntityFight spirit = (EntityFight)owner_;
                ANI_TYPE stand_ani = ANI_TYPE.ANI_GUARD;
                if (spirit.F_IsPlayer())
                {
                    stand_ani = ANI_TYPE.ANI_IDLE;
                }
                v_state[index].handle.init(this, index, false, stand_ani, new Vector3(), 0, 0, 0, 0f);
            }
        }
    }
}
