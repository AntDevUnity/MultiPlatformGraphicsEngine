using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe;
using MpGe.App;
using MpGe.Base;
using MpGe.Display;
using MpGe.Platform;
using MpGEPlatformDX12;
using MpGEPlatformDX12.Platform;
namespace SampleDX12_1
{
    /// <summary>
    /// This is sample 1, using the DirectX12 Platform-Extension.
    /// </summary>
    public class Sample1App : Application
    {

        public Sample1App()
        {
            SetPlatform(new PlatformDX12(), new DisplayMetrics(20, 20, 800, 600, "Sample1-Dx12", ColorFormat.RGB));
        }

    }
}
