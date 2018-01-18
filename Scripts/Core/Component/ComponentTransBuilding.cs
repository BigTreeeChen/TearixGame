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
        List<GameObject> tmp = new List<GameObject>();


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
                {
                    owner_.V_ChangeMatCom.F_RemoveMat(m_ReverseTransDic[ele]);
                    m_ReverseTransDic.Remove(ele);
                }
            }

            tmp.Clear();
            // 以前透明，现在不透明
            foreach(var ele in m_LastShelterList)
            {
                if (!m_CurShelterList.Contains(ele))
                {
                    if (m_TransDic.ContainsKey(ele))
                    {
                        owner_.V_ChangeMatCom.F_RemoveMat(m_TransDic[ele]);
                        m_TransDic.Remove(ele);
                    }

                    if (!m_ReverseTransDic.ContainsKey(ele))
                    {
                        var matInfo = owner_.V_ChangeMatCom.F_AddMat(0, new object[] { 0.3, 1 }, EM_MatType.SoftChangeAlpha);
                        m_ReverseTransDic[ele] = matInfo;
                    }
                    tmp.Add(ele);
                }
            }

            // 把m_LastShelterList中不透明的去掉，因为新的m_LastShelterList中需要去掉
            foreach (var ele in tmp)
                m_LastShelterList.Remove(ele);

            // 现在透明
            foreach(var ele in m_CurShelterList)
            {
                if (!m_LastShelterList.Contains(ele))
                {
                    if (m_ReverseTransDic.ContainsKey(ele))
                    {
                        owner_.V_ChangeMatCom.F_RemoveMat(m_ReverseTransDic[ele]);
                        m_ReverseTransDic.Remove(ele);
                    }

                    if (!m_TransDic.ContainsKey(ele))
                    {
                        var matInfo = owner_.V_ChangeMatCom.F_AddMat(0, new object[] { 1, 0.3 }, EM_MatType.SoftChangeAlpha);
                        m_TransDic[ele] = matInfo;
                    }

                    m_LastShelterList.Add(ele);
                }
            }
        }
    }
}
