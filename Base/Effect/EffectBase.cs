using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Effect
{
    /// <summary>
    /// This is the base class for effects. Effects manage and enable use of Shaders, which are dependent on each platform.
    /// </summary>
    public class EffectBase
    {

        public string ShaderPath
        {
            get;
            set;
        }

        public string GeoPath
        {
            get;
            set;
        }

        public string VertPath
        {
            get;
            set;
        }

        public string FragPath
        {
            get;
            set;
        }

        public string RayTracePath
        {
            get;
            set;
        }

        public virtual void LoadShader(string path)
        {

        }

        public virtual void LoadGeo(string path)
        {

        }

        public virtual void LoadVert(string path)
        {

        }

        public virtual void LoadFrag(string path)
        {

        }

    }
}
