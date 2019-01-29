using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Display
{
    /// <summary>
    /// DisplayMetrics allow you to specify the required config of screen you want.
    /// </summary>
    public class DisplayMetrics
    {

        /// <summary>
        /// The requested X-position(If-any) on the Desktop
        /// </summary>
        public int RequestDesktopX
        {
            get;
            set;
        }

        /// <summary>
        /// The requested Y-position(If-any) on the desktop.
        /// </summary>
        public int RequestDesktopY
        {
            get;
            set;
        }

        /// <summary>
        /// The requested Width of the screen.
        /// </summary>
        public int RequestWidth
        {
            get;
            set;
        }


        /// <summary>
        /// The requested Height of the screen.
        /// </summary>
        public int RequestHeight
        {
            get;
            set;
        }

        public ColorFormat RequestFormat
        {
            get;
            set;
        }

        public string RequestName
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs the display metrics to the required specification.
        /// </summary>
        /// <param name="deskX">The requested desktop X position.</param>
        /// <param name="deskY">The requested desktop Y position.</param>
        /// <param name="width">The requested width.</param>
        /// <param name="height">The requested height.</param>
        /// <param name="format">The requested ColorFormat.</param>
        /// <param name="name">The requested name.</param>
        public DisplayMetrics(int deskX,int deskY,int width,int height,string name = "Display01",ColorFormat format = ColorFormat.Any)
        {

            RequestDesktopX = deskX;
            RequestDesktopY = deskY;
            RequestWidth = width;
            RequestHeight = height;
            RequestFormat = format;
            RequestName = name;
        }

    }
}
