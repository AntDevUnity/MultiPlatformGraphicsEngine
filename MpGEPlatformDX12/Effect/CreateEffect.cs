using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Effect;
using SharpDX.DXGI;
using System.Threading;
namespace MpGEPlatformDX12.Effect
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;


    public static class CreateEffect
    {

        public static Effect CreateSimple2D()
        {

            var fx = new Effect();
            fx.Init = (obj) =>
            {


                fx.Input = new[]
                {
                 new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
                 };



            };
            fx.LoadShaders("Data/Platform/DX12/Shader/2DSimple.hlsl");

            return fx;

        }
       

        

    }
}
