using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Display;
using MpGe.Platform;
using MpGe.Base;
/// <summary>
/// The app class is the general class for maintaiing and using the application.
/// </summary>
namespace MpGe.App
{
    /// <summary>
    /// The application class. What code/classes it uses internally are dependent on the platform-extension used.
    /// </summary>
    public class Application
    {

        /// <summary>
        /// The applications Display.
        /// </summary>
        public DisplayBase Display
        {
            get;
            set;
        }

        /// <summary>
        /// The applications platform.
        /// </summary>
        public PlatformBase Platform
        {
            get;
            set;
        }

        public PlatformImplentation Implentation
        {
            get;
            set;
        }

        /// <summary>
        /// This sets the applications platform.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <param name="displayMetrics">The required display metrics.</param>
        public void SetPlatform(PlatformBase platform, DisplayMetrics displayMetrics)
        {
            platform.CreateDisplay(displayMetrics);
            Platform = platform;
            Display = platform.Display;
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public void Run()
        {

            Console.WriteLine("Running application.");

            while (true)
            {

            }

        }

        /// <summary>
        /// Override this so you can initialize your app.
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// Override this so you can update your app.
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Override this so you can draw your app.
        /// </summary>
        public virtual void Draw()
        {

        }

    }
}
