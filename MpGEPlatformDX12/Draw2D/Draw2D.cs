using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Draw2D;

namespace MpGEPlatformDX12.Draw2D
{
    public class Draw2D : Draw2DBase
    {
        public Effect.Effect FXSimple2D;
        public override void Init()
        {

            FXSimple2D = Effect.CreateEffect.CreateSimple2D();

            //Simple2D = new Effect.


        }

    }
}
