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
        private SkillButtonCD mSkillButtonCD;
        void Start()
        {
            list[mSlot] = this;
            mPressTweens = transform.Find("FrameCenter").GetComponentsInChildren<UITweener>();
            mSkillButtonCD = GetComponent<SkillButtonCD>();
            mIsStarted = true;
        }
        private void OnDestroy()
        {
            list[mSlot] = null;
        }

        void ReloadSkill()
        {
            mIcon.SetActive(true);
            // set icontexture
        }

        private void DoOnPress(bool isPressed)
        {
            if (enabled)
            {
                if (isPressed)
                {
                    OnPressBegin();
                }
                else
                {
                    OnPressEnd();
                }
            }
        }

        public void OnPress(bool isPressed)
        {
            DoOnPress(isPressed);
        }

        private void OnDisable()
        {
            if (mIsPress)
            {
                OnPressEnd();
                if (mPressTweens != null)
                {
                    foreach (UITweener tween in mPressTweens)
                    {
                        tween.Sample(0, true);
                        tween.enabled = false;
                    }
                }
            }
        }

        private void OnPressBegin()
        {
            mIsPress = true;
            if (mPressMode == 2)
            {
                CacheOrUseSkill();
            }
        }

        private void OnPressEnd()
        {
            mIsPress = false;
            if (mPressMode == 1)
            {
                CacheOrUseSkill();
            }
            else if (mPressMode == 2)
            {
                // cancel active skill
            }
        }

        private void CacheOrUseSkill()
        {
            //try use skill
        }

        public bool IsPressed()
        {
            return mIsPress;
        }

        public void OnButtonStatusChanged(bool play_sfx = true)
        {
            int check_skill_id = mUseSkillID;
            bool pressable = true;
            bool old_enabled = enabled;
            enabled = pressable;
            GetComponent<Collider>().enabled = enabled;
            mIconTexture.color = enabled ? Color.white : Color.gray;
            if (!enabled && mChaseCircleTrans != null && mChaseCircleTrans.gameObject.activeInHierarchy)
            {
                mChaseCircleTrans.gameObject.SetActive(false);
            }
        }

        public static void OnButtonStatusChangedAll()
        {
            foreach (SkillButton sb in list)
            {
                if (sb != null)
                {
                    sb.OnButtonStatusChanged();
                }
            }
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
