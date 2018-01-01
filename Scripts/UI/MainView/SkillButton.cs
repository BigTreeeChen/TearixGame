using UnityEngine;
using System.Collections;
using SGameSetting;

namespace TGame.UI
{
    public class SkillButton : MonoBehaviour
    {
        public static SkillButton[] list = new SkillButton[5];

        [SerializeField]
        GameObject mIcon;
        [SerializeField]
        UITexture mIconTexture;
        [SerializeField]
        Transform mChaseCircleTrans;
        [SerializeField]
        int mSlot;

        private UITweener[] mPressTweens;
        private int mUseSkillID;
        private int mPressMode;
        private bool mIsPress;
        private bool mIsStarted = false;

        void Start()
        {
            list[mSlot] = this;
            mPressTweens = transform.Find("FrameCenter").GetComponentsInChildren<UITweener>();
            mIsStarted = true;
        }

        void ReloadSkill()
        {
            mIcon.SetActive(true);
            // set icontexture
        }

        public static int GetPressMode(int skillID)
        {
            if (skill.data.ContainsKey(skillID))
            {
                return skill.data[skillID].press_mode;
            }
            return 2;
        }

        public static int GetUseSkillID(int _slot)
        {
            int job = 0;
            return _slot;
        }
    }

}
