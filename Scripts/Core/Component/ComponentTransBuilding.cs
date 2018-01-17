using System.Collections.Generic;
using System;
using UnityEngine;
using TGame.Entity;

namespace TGameEngine
{

    public class ComponentTransBuilding : Component
    {
        protected new EntityFight owner_ { get { return (EntityFight)base.owner_; } }
        List<GameObject> m_LastShelterList = new List<GameObject>();
        List<GameObject> m_CurShelterList = new List<GameObject>();
        // 透明集
        Dictionary<GameObject, MatInfo> m_TransDic = new Dictionary<GameObject, MatInfo>();
        // 从透明变得不透明集
        Dictionary<GameObject, MatInfo> m_ReverseTransDic = new Dictionary<GameObject, MatInfo>();

        public override void update()
        {
            UpdateTheShelter();
        }

        void UpdateTheShelter()
        {
            if (owner_.m_FightView == null) return;
            var camera = NGUITools.FindCameraForLayer(owner_.m_FightView.gameObject.layer);
            Vector3 p1 = owner_.m_FightView.get_position();
            Vector3 p2 = p1 + Vector3.up * owner_.m_FightView.get_char_ctrl_height();
            var hits = Physics.CapsuleCastAll(p1, p2, owner_.m_FightView.get_char_ctrl_radius(), camera.transform.position - owner_.m_FightView.get_char_ctrl_center(), 1000);
            m_CurShelterList.Clear();
            for (int i = 0, imax = hits.Length; i < imax; ++i)
            {
                m_CurShelterList.Add(hits[i].transform.gameObject);
            }
            // 把m_ReverseTransDic中过期的去掉(已经变完全不透明了，可以换回原先的mat了)
            var reverseKeys = m_ReverseTransDic.Keys;
            foreach(var ele in reverseKeys)
            {
                if (m_ReverseTransDic[ele].v_StartTime + m_ReverseTransDic[ele].v_ChangeTime < Time.time)
                    m_ReverseTransDic.Remove(ele);
            }
            // 以前透明，现在不透明
            // 现在透明
        }
    }
}
