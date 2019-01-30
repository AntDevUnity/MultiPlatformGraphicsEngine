using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Display;
using MpGe.Base;
using SharpDX.Windows;

namespace MpGEPlatformDX12.Display
{
    public class DisplayDX12 : DisplayBase
    {

        /// <summary>
        /// Creates the Directx12 Display.
        /// </summary>
        /// <param name="metrics"></param>
        public DisplayDX12(DisplayMetrics metrics) : base(metrics)
        {
            Metrics = metrics;
         
        }
        
        /// <summary>
        /// The internal RenderForm;
        /// </summary>
        public RenderForm Form
        {
            get;
            set;
        }

        public RenderLoop Loop
        {
            get;
            set;
        }


        /// <summary>
        /// Requests the required display.
        /// </summary>
        /// <returns></returns>
        public override Result Request()
        {

            Form = new RenderForm(Metrics.RequestName)
            {
                Width = Metrics.RequestWidth,
                Height = Metrics.RequestHeight
            };

            Form.Show();

            Console.WriteLine("Creating DirectX12 display");
            return new Result(true);
            //   return base.Request();
        }
    }
}
