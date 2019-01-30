using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Display;
using MpGe.Base;
namespace MpGe.Platform
{
    /// <summary>
    /// Platform base is the base class for implementing a new api/platform support.
    /// </summary>
    public class PlatformBase
    {
        /// <summary>
        /// The platforms current Display.
        /// </summary>
        public DisplayBase Display
        {
            get;
            set;
        }

        public InitApp Init
        {
            get;
            set;
        }

        public UpdateApp Update
        {
            get;
            set;
        }

        public DrawApp Draw
        {
            get;
            set;
        }

        public void InitPlatform()
        {

        }

        public virtual Result CreateDisplay(DisplayMetrics metrics)
        {
            return new Result(false, new NotImplementedException("This platform is not fully implemented."));
        }

        public virtual void Run()
        {

        }

    }

    public delegate void InitApp();
    public delegate void UpdateApp();
    public delegate void DrawApp();

}
