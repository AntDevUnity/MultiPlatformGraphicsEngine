using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Create
{
    public class CreateBase
    {

        public virtual Effect.EffectBase CreateEffect()
        {
            return null;
        }

        public virtual Draw2D.Draw2DBase CreateDraw2D()
        {
            return null;
        }

        public virtual Texture.Texture2DBase LoadTexture(string path)
        {
            return null;
        }

    }
}
