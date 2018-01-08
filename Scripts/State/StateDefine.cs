using UnityEngine;
using System.Collections;

namespace TGameEngine
{

    public struct StateDefine
    {
        public const int STATE_STAND            = 0;
        public const int STATE_MOVE_TO          = 1;
        public const int STATE_DEAD             = 2;
        public const int STATE_SECTION_SKILLING = 3;
        public const int STATE_SKILLING         = 4;  
        public const int STATE_HURT             = 5;  
        public const int STATE_MOVE_GROUND      = 6;
        public const int STATE_NAVIGATION       = 7;
        public const int STATE_HURT_BACK        = 8;
        public const int STATE_DAZED            = 9;
        public const int STATE_HURT_FLY         = 10;
        public const int STATE_MOVE_PERSIST     = 11;
        public const int STATE_HURT_BACK_FLY    = 12;
        public const int STATE_HOLD             = 13;
        public const int STATE_CINEMA           = 14;
        public const int STATE_HURT_HORI        = 15;
        public const int STATE_DRAG             = 16;
        public const int STATE_REPLACE          = 17;
        public const int STATE_LEADING          = 18;
        public const int STATE_PULL             = 19;
        public const int STATE_PICK           = 20;
        public const int STATE_HURT_FLOAT       = 21;
        public const int STATE_MAX              = 22;

        public static int MAX_BITSET = (STATE_MAX + 31) / 32;
		public static int MAX_BITSET_LEN = (sizeof(int) * MAX_BITSET)<<3;
    }

	public struct StateMsg
    { 
        public const int SYS_SET_STATE      = 0;
        public const int SYS_DEL_STATE      = 1;
        public const int MSG_INITIAL        = 3;
        public const int MSG_DESTROY        = 4;
        public const int MSG_GO_TOP         = 5;
        public const int MSG_UPDATE         = 6;
        public const int MSG_SETDATA        = 7;
        public const int MSG_RESET          = 8;
        public const int MSG_GETDATA        = 9;
        public const int MSG_USER           = 10;
    }

	public struct StateLimit
    {
        public const int MAX_PARAM          = 10;
		public const int MAX_MESSAGE        = 16;
		public const int MESSAGE_PARAM_NUM	= 10;
    }

	public struct StateRetCode
	{
		public const int RT_NO           	= 0;
		public const int RT_ER           	= -1;
		public const int RT_DE           	= -2; 
		public const int RT_OK           	= 1;
	}

    public struct StateIntFloatFactor
    { 
        public const float FLOAT2INT_MUL_FACTOR = 1000.0f;
        public const float INT2FLOAT_MUL_FACTOR = 0.001f;
    }
}
