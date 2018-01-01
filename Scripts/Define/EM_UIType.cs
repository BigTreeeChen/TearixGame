namespace MU.Define
{
    //UI深度管理
    public enum EM_DepthRange
    {
        MinWinDepth = 0,
        MaxWinDepth = 90,

        MinFloatingDepth = 110,
        MaxFloatingDepth = 190,

        MinTipsDepth = 210,
        MaxTipsDepth = 290,

        MinAlertDepth = 310,
        MaxAlertDepth = 390,
    }

    //窗口类型
    public enum EM_WinType
    {
        None = 0,
        WinMainUI = 1,
        WinLoginView = 2,
        WinResDownLoadView = 3,
        WinLoadingView = 4,
    }

    //弹窗类型
    public enum EM_AlertType
    {
        None = 0,
        SystemTips = 1,
        SystemOneAlert = 2,
        SystemTwoAlert = 3,
        SystemThirdAlert = 4,
        AlertNotice = 5,
    }

    //浮动提示
    public enum EM_FloatGroupType
    {
        NewEquip,
        NewTitle,
        QuickUseTip,
    }

    //短暂提示分组
    public enum EM_TipsType
    {
        SysInfo,
        SystemTips,
        SystemExpTips,
        SystemNoticeTips,
        SystemIconPickTips,
        SystemIconGetTips,
    }

    //短暂提示出现时，如果又出现同类型该如何处理
    public enum EM_NewTipsShowType
    {
        //不用管释放存在，直接显示
        ShowNew,
        //给当前提示信息发消息（让当前提示ui处理)
        Notice,
        //替换
        Replace,
        //不存在才显示
        CheckLive,
    }
}