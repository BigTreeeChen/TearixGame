using UnityEngine;
using System.Collections;

public class TextureBlurEffect : MonoBehaviour
{
    int m_Iterations = 1;
    float m_fBlurSpread = 0.5f;
    RenderTexture m_BlurTex = null;
    static Shader m_BlurShader = null;
    static Material m_BlurMaterial = null;
    protected Shader blurShader
    {
        get
        {
            if (m_BlurShader == null)
            {
                m_BlurShader = Shader.Find("Hidden/BlurEffectConeTap");
            }
            return m_BlurShader;
        }
    }

    protected Material BlurMaterial
    {
        get
        {
            if (m_BlurMaterial == null)
            {
                m_BlurMaterial = new Material(blurShader);
                m_BlurMaterial.hideFlags = HideFlags.DontSave;
            }
            return m_BlurMaterial;
        }
    }

    bool Support()
    {
        if (!blurShader || !BlurMaterial.shader.isSupported)
        {
            enabled = false;
            return false;
        }
        return true;
    }

    //执行一次迭代模糊
    public void FourTapCone(RenderTexture src, RenderTexture dst, int iteration)
    {
        float off = 0.5f + iteration * m_fBlurSpread;
        Graphics.BlitMultiTap(src, dst, BlurMaterial,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
            );
    }

    // 采样一个1/4分辨率的图
    private void DownSample4x(RenderTexture src, RenderTexture dst)
    {
        float off = -1.0f;
        Graphics.BlitMultiTap(src, dst, BlurMaterial,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
            );
    }

    Vector2 GetScreenSize()
    {
#if UNITY_IPHONE
        return new Vector2(Screen.width, Screen.height);
#elif UNITY_ANDROID
        return new Vector2(Screen.width, Screen.height);
#else
        return new Vector2(1280, 720);
#endif
    }
    public void F_BlurCameraTex(float fTexScale, GameObject sceneCamera, GameObject uiCamera, bool bUIRender)
    {
        if (Support())
        {
            Vector2 v2Screen = GetScreenSize();
            float UIScreenWidth = 1280;
            float UIScreenHeight = UIScreenWidth * v2Screen.y / v2Screen.x;
            v2Screen.x = UIScreenWidth;
            v2Screen.y = UIScreenHeight;

            float width = Mathf.Ceil(v2Screen.x * fTexScale);
            float height = Mathf.Ceil(v2Screen.y * fTexScale);

            RenderTexture RenderTex = RenderTexture.GetTemporary((int)width, (int)height, 24, RenderTextureFormat.RGB565);
            sceneCamera.GetComponent<Camera>().targetTexture = RenderTex;
            sceneCamera.GetComponent<Camera>().Render();
            sceneCamera.GetComponent<Camera>().targetTexture = null;
            if (!bUIRender && uiCamera != null)
            {
                uiCamera.GetComponent<Camera>().targetTexture = RenderTex;
                uiCamera.GetComponent<Camera>().Render();
                uiCamera.GetComponent<Camera>().targetTexture = null;
            }

            //添加UI贴图
            UITexture uiTex = gameObject.GetComponent<UITexture>();
            if (uiTex == null)
            {
                uiTex = gameObject.AddComponent<UITexture>();
            }

            int rtW = RenderTex.width / 4;
            int rtH = RenderTex.height / 4;
            RenderTexture buffer = new RenderTexture(rtW, rtH, 0);

            //采样
            DownSample4x(RenderTex, buffer);
            //迭代模糊
            for(int i = 0; i < m_Iterations; ++i)
            {
                RenderTexture bufferTem = new RenderTexture(rtW, rtH, 0);
                FourTapCone(buffer, bufferTem, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = bufferTem;
            }
            //复制到贴图中
            if (m_BlurTex == null)
            {
                m_BlurTex = new RenderTexture(RenderTex.width, RenderTex.height, 0);
            }
            else
            {
                m_BlurTex.DiscardContents();
            }
            Graphics.Blit(buffer, m_BlurTex);
            RenderTexture.ReleaseTemporary(buffer);
            //设置UI贴图
            uiTex.mainTexture = m_BlurTex;
            uiTex.width = (int)v2Screen.x + 10;
            uiTex.height = (int)v2Screen.y + 10;
            //释放RenderTex
            RenderTexture.ReleaseTemporary(RenderTex);
        }
    }

    private void OnDestroy()
    {
        if (m_BlurTex != null)
        {
            GameObject.Destroy(m_BlurTex);
        }
    }
}
