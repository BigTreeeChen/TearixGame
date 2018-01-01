using UnityEngine;
using System.Collections;

namespace TGame.UI
{
    public class JoyStickUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_JoyStickSprite;
        [SerializeField]
        GameObject m_JoyStickBg;

        public static JoyStickUI instance { get { return mInstance; } }
        private static JoyStickUI mInstance;

        UICamera m_Camera;
        Vector2 m_MoveDelta;
        Vector2 m_BeginPressPos;//按下时的位置，以此为中心显示JoyStick
        bool m_HasPress = false;
        bool m_IsMoving = false;

        void Awake()
        {
            m_Camera = NGUITools.FindInParents<UIRoot>(transform).transform.GetComponentInChildren<UICamera>();
            m_JoyStickSprite.SetActive(false);
        }

        void Update()
        {
            if (m_MoveDelta.sqrMagnitude > 0.01)
            {
                //move
            }
        }

        void OnPress(bool press)
        {
            m_MoveDelta = Vector2.zero;
            m_HasPress = press;
            if (press)
            {
                m_BeginPressPos = transform.InverseTransformPoint(m_Camera.cachedCamera.ScreenToWorldPoint(UICamera.currentTouch.pos));
                m_JoyStickBg.transform.localPosition = m_BeginPressPos;
            }
            else
            {
                if (m_IsMoving)
                {
                    m_IsMoving = false;
                    //stop move
                }
                m_JoyStickBg.transform.localPosition = Vector2.zero;
                m_JoyStickSprite.SetActive(false);
            }
        }


        int sign = 1;
        void OnDrag(Vector2 delta)
        {
            if (m_HasPress)
            {
                m_IsMoving = true;
                Vector2 curPos = transform.InverseTransformPoint(m_Camera.cachedCamera.ScreenToWorldPoint(UICamera.currentTouch.pos));
                m_MoveDelta = curPos - m_BeginPressPos;
                sign = m_MoveDelta.y > 0 ? 1 : -1;
                if (!m_JoyStickSprite.activeSelf)
                    m_JoyStickSprite.SetActive(true);
                m_JoyStickSprite.transform.localPosition = curPos;
                m_JoyStickSprite.transform.localEulerAngles = new Vector3(0, 0, sign * Vector2.Angle(Vector2.right, m_MoveDelta.normalized));
            }
        }
    }
}

