using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace TGameAsset
{
    public class PathHelper
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public const string FilePrefix = "file:///";
        public const string ZIP = ".zip";
        public const string Apk = ".apk";
        public const string ABFile = ".assetbundle";
        public const string EncrptFile = ".bytes";

        private static PathHelper m_Instance = null;
        public static PathHelper GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new PathHelper();
            return m_Instance;
        }

        /// <summary>
        /// 只读目录（安装包目录）
        /// </summary>
        public string V_BuildAssetBundlePath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// 只读目录，安卓的可能被拒绝，可以考虑新建目录代替它
        /// </summary>
        public string V_ResPath
        {
            get
            {
#if UNITY_IPHONE
                return Application.temporaryCachePath;
#elif UNITY_ANDROID
                return Application.persistentDataPath;
#else
                return Application.persistentDataPath;
#endif
            }
        }

        /// <summary>
        /// 拼接只读目录
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        public string F_PathCombineStream(string path)
        {
            if (path == null) return "";
            return Path.Combine(V_BuildAssetBundlePath, path).Replace("\\", "/");
        }

        /// <summary>
        /// 拼接读写路径
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        public string F_PathCombineRes(string path)
        {
            if (path == null) return "";
            return Path.Combine(V_ResPath, path).Replace("\\", "/");
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath">绝对路径</param>
        /// <returns></returns>
        public bool F_CheckFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 加上file协议
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string F_AddFilePro(string path)
        {
            if (!path.Contains(FilePrefix))
            {
                return FilePrefix + path;
            }
            else
            {
                return path;
            }
        }
    }
}

