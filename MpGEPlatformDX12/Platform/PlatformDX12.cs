using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe;
using MpGe.Base;
using MpGe.Platform;
using MpGe.Display;
using SharpDX.Windows;
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

        private Display.DisplayDX12 IntDis
        {
            get;
            set;
        }

        public override Result CreateDisplay(DisplayMetrics metrics)
        {
            //return base.CreateDisplay(metrics);
            Display = new Display.DisplayDX12(metrics);
            IntDis = Display as Display.DisplayDX12;
            return null;
        }

        public override void Run()
        {

            using (var loop = new RenderLoop(IntDis.Form))
            {

                Init?.Invoke();

                while (loop.NextFrame())
                {

                    Update?.Invoke();
                    Draw?.Invoke();

                }

            }

                base.Run();
        }

    }
}
