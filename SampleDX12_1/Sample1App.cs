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
using MpGe.Create;

namespace SampleDX12_1
{
    /// <summary>
    /// This is sample 1, using the DirectX12 Platform-Extension.
    /// </summary>
    public class Sample1App : Application
    {

        public CreateBase Create = null;
        public MpGe.Draw2D.Draw2DBase Draw = null;

        public Sample1App()
        {
            SetPlatform(new PlatformDX12(), new DisplayMetrics(20, 20, 800, 600, "Sample1-Dx12", ColorFormat.RGB));

            Create = Global.Creator;

            var tex = Create.LoadTexture("Data/Tex/tex1.png");

            Platform.Init = () =>
            {
                Create = Global.Creator;

                Draw = Create.CreateDraw2D();

                Console.WriteLine("Init App");

            };

            Platform.Update = () =>
            {
                Console.WriteLine("Update");
            };

            var x = 20;

            Platform.Draw = () =>
            {

                //x = x + 

                Draw.RectTex(x, 200, 200, 200, tex);
               // Draw.Rect(400, 400, 300, 300);
            };

        }

    }
}
