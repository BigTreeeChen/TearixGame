using UnityEngine;
using System.Collections;

namespace TGameAsset
{
    public class AssetNode : MonoBehaviour
    {
        private string m_AssetPath = "";
        public string V_AssetPath
        {
            set
            {
                m_AssetPath = value;
            }
        }

        void OnDestroy()
        {
            if (!string.IsNullOrEmpty(m_AssetPath))
            {
                AssetManager.GetInstance().F_ReleaseAsset(m_AssetPath);
            }
        }
    }
}

