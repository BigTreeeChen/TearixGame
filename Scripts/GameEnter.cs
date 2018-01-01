using UnityEngine;
using System.Collections;
using Core.Panel;

public class GameEnter : MonoBehaviour {

    private void Awake()
    {
        //创建UI根节点
        UIManager.GetInstance().CreateUIRoot(transform);
        //创建battleviewlist(管理各个实体viewlist)
        //创建EntityPool根节点
        //读取本地配置（读策划配置文件）
        //加载战斗场景SceneManager.LoadScene("1004");
    }

    private void Update()
    {
        // 每帧执行一次的函数们
        // 定时清理
        // if past time > 60 AssetManager.getinstance().UnLoadUnusedAssets();
    }
}
