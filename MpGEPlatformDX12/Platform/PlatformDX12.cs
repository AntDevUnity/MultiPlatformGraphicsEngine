using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe;
using MpGe.Base;
using MpGe.Platform;
using MpGe.Display;

/// <summary>
/// This is the DirectX Platform-Extension.
/// </summary>
namespace MpGEPlatformDX12.Platform
{
    public class PlatformDX12 : PlatformBase
    {

        public PlatformDX12()
        {

        }

        public override Result CreateDisplay(DisplayMetrics metrics)
        {
            //return base.CreateDisplay(metrics);
            Display = new Display.DisplayDX12(metrics);
            return null;
        }

    }
}
