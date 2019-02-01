using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Effect;
/// <summary>
/// The Draw2D namespace allows 2d drawing.
/// </summary>
namespace MpGe.Draw2D
{
    public class Draw2DBase
    {

        public EffectBase Simple2D
        {
            get;
            set;
        }

        public EffectBase Textured2D
        {
            get;
            set;
        }

        public Draw2DBase()
        {
            Init();
        }

        public virtual void Init()
        {

        }

        public virtual void RectTex(float x,float y,float w,float h,Texture.Texture2DBase tex)
        {

        }
        public virtual void Rect(float x,float y,float w,float h)
        {

        }

    }
}
