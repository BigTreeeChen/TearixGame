using System.Collections.Generic;
using System;
using UnityEngine;

namespace TGameEngine
{
    public enum EM_MatType
    {
        DieDissolve,
        ChangeColor,
        SoftChangeAlpha,
        HurtHightLight,
        MonsterAtkWarn,
    }

    // Smallest, Highest
    public enum EM_MatPriority
    {
        Default,
    }

    public class MatInfo : IComparable
    {
        public float v_StartTime;
        public float v_EndTime;
        public float v_ChangeTime;//切换mat dura
        public EM_MatType v_Type;
        public EM_MatPriority v_Pri;
        public System.Object[] v_Params;

        public int CompareTo(object obj)
        {
            MatInfo b = obj as MatInfo;
            if (v_Pri < b.v_Pri) return -1;
            if (v_Pri > b.v_Pri) return 1;
            if (v_StartTime < b.v_StartTime) return -1;
            if (v_StartTime > b.v_StartTime) return 1;
            return 0;
        }
    }

    public class ComponentChangeMat : Component
    {
        List<MatInfo> m_MatList = new List<MatInfo>();
        Dictionary<Renderer, Material> m_InitMat = new Dictionary<Renderer, Material>();

        public void F_InitMat()
        {
            Renderer[] renders = owner_.m_View.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renders.Length; ++i)
            {
                m_InitMat.Add(renders[i], renders[i].sharedMaterial);
            }
        }

        public void F_AddMat(float lastTime, System.Object[] args, EM_MatType matType,
            float changeTime = 0.8f, EM_MatPriority matPri = EM_MatPriority.Default)
        {
            MatInfo matInfo = new MatInfo();
            matInfo.v_ChangeTime = changeTime;
            matInfo.v_StartTime = Time.time;
            matInfo.v_EndTime = matInfo.v_StartTime + lastTime;
            matInfo.v_Params = args;
            matInfo.v_Pri = matPri;
            matInfo.v_Type = matType;
            m_MatList.Add(matInfo);
            OnMatListChange();
        }

        public void F_RemoveMat(MatInfo m)
        {
            m_MatList.Remove(m);
            OnMatListChange();
        }

        void OnMatListChange()
        {
            if (m_InitMat.Count < 1)
                F_InitMat();
            if (m_MatList.Count < 1)
            {
                // 复原
                Recover();
                return;
            }
            Recover();
            m_MatList.Sort();
            BeginChangeMat(m_MatList[0]);
        }

        void BeginChangeMat(MatInfo m)
        {
            foreach (var ele in m_InitMat)
            {
                var render = ele.Key;
                switch (m.v_Type)
                {
                    case EM_MatType.DieDissolve:
                    case EM_MatType.HurtHightLight:
                    case EM_MatType.MonsterAtkWarn:
                        AssignNewMat(render, new Material(Shader.Find("")));
                        ChangeMatTexture(render, ele.Value.mainTexture);
                        break;
                    case EM_MatType.ChangeColor:
                        Color col = (Color)m.v_Params[0];
                        ChangeMatColor(render, col);
                        break;
                    case EM_MatType.SoftChangeAlpha:
                        // 在updat里慢慢变过去
                        break;
                    default:
                        break;

                }
            }
        }

        void Recover()
        {
            foreach (var ele in m_InitMat)
            {
                ele.Key.material = ele.Value;
            }
        }

        public override void update()
        {
            if (m_MatList.Count < 1) return;
            bool curChange = false;
            for (int i = m_MatList.Count - 1; i >= 0; --i)
            {
                var matInfo = m_MatList[i];
                if (matInfo.v_StartTime != matInfo.v_EndTime && Time.time > matInfo.v_EndTime)
                {
                    m_MatList.RemoveAt(i);
                    if (i == 0) curChange = true;
                }
            }

            if (curChange)
            {
                OnMatListChange();
            }

            UpdateForCurrentMat();
        }

        void UpdateForCurrentMat()
        {
            if (m_MatList.Count < 1) return;
            var matInfo = m_MatList[0];
            float pastPercent = (Time.time - matInfo.v_StartTime) / matInfo.v_ChangeTime;
            switch (matInfo.v_Type)
            {
                case EM_MatType.DieDissolve:
                    // 溶解材质要一直挂在上面,所以需要额外传totalTime进来
                    float from = (float)matInfo.v_Params[0];
                    float to = (float)matInfo.v_Params[1];
                    float totalTime = (float)matInfo.v_Params[2];
                    float pass = Time.time - matInfo.v_StartTime;
                    foreach (var ele in m_InitMat)
                    {
                        ChangeMatFloat(ele.Key, "_Amount", pass / totalTime);
                    }
                    break;
                case EM_MatType.SoftChangeAlpha:
                    if (pastPercent > 1) break;
                    foreach (var ele in m_InitMat)
                    {
                        Color col = ele.Value.color;
                        col.a = Mathf.Lerp((float)matInfo.v_Params[0], (float)matInfo.v_Params[1], pastPercent);
                        ChangeMatColor(ele.Key, col);
                    }
                    break;
                default:
                    break;
            }
        }

        void AssignNewMat(Renderer render, Material mat)
        {
            render.material = mat;
        }

        void ChangeMatTexture(Renderer render, Texture tex)
        {
            render.material.mainTexture = tex;
        }

        void ChangeMatColor(Renderer render, Color col)
        {
            render.material.color = col;
        }

        void ChangeMatFloat(Renderer render, string property, float value)
        {
            render.material.SetFloat(property, value);
        }
    }
}


