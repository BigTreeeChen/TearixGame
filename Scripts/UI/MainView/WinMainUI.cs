using UnityEngine;
using System.Collections;
using Core.Panel;
using MU.Define;
using System;

namespace TGame.UI
{
    public class WinMainUI : BaseWindow
    {
        public override bool F_CheckCanOpen()
        {
            return true;
        }

        public override EM_WinType F_GetWinType()
        {
            return EM_WinType.WinMainUI;
        }
    }
}

