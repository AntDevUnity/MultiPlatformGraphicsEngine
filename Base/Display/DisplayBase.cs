using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Base;

/// <summary>
/// Screen implementations.
/// </summary>
namespace MpGe.Display
{
    /// <summary>
    /// Contains information and methods for interacting with the current screen(Visual output) being used in your app.
    /// </summary>
    public class DisplayBase
    {

        /// <summary>
        /// The current or null screen metrics.
        /// </summary>
        public DisplayMetrics Metrics
        {
            
            get
            {
                return _Metrics;
            }
            set
            {
                _Metrics = value;
            }
        }
        protected internal DisplayMetrics _Metrics = null;


        /// <summary>
        /// The current X-Coord of the desktop display (If any)
        /// </summary>
        public int DesktopX
        {
            get;
            set;
        }

        /// <summary>
        /// the current Y-Coord of the desktop display (If any)
        /// </summary>
        public int DesktopY
        {
            get;
            set;
        }

        /// <summary>
        /// The current width of the display.
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        ///  The current height of the display.
        /// </summary>
        public int Height
        {
            get;
            set;
        }
        
        /// <summary>
        /// The name of the display.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The color format of the display.
        /// </summary>
        public ColorFormat Format
        {
            get;
            set;
        }

        public DisplayBase(DisplayMetrics metrics)
        {
            SetMetrics(metrics);
        }

        protected internal void SetMetrics(DisplayMetrics metrics)
        {
            Metrics = metrics;
            DesktopX = metrics.RequestDesktopX;
            DesktopY = metrics.RequestDesktopY;
            Width = metrics.RequestWidth;
            Height = metrics.RequestHeight;
        }

        public virtual Result Request()
        {
            return new Result(false,new NotImplementedException("This display has not been implmented fully."));
        }


    }
    
}
