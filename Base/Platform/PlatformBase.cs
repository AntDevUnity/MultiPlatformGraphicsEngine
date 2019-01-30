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

        public void InitPlatform()
        {

        }

        public virtual Result CreateDisplay(DisplayMetrics metrics)
        {
            return new Result(false, new NotImplementedException("This platform is not fully implemented."));
        }

    }
}
