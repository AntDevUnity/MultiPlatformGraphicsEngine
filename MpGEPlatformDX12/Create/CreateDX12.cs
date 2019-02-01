using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Create;
using MpGe.Draw2D;
using MpGe.Effect;

namespace MpGEPlatformDX12.Create
{
    public class CreateDX12 : CreateBase
    {

        public override Draw2DBase CreateDraw2D()
        {
            return new Draw2D.Draw2D();
            return base.CreateDraw2D();
        }

        public override EffectBase CreateEffect()
        {
            return new Effect.Effect();
            return base.CreateEffect();
        }

        public override MpGe.Texture.Texture2DBase LoadTexture(string path)
        {
            var tex = new Texture.Texture2D(path);
            return tex as MpGe.Texture.Texture2DBase;
        }


    }
}
