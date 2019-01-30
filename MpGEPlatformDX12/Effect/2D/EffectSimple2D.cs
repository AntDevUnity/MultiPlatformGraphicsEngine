using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Effect;
using SharpDX.DXGI;
using System.Threading;


namespace MpGEPlatformDX12.Effect._2D
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;


    public class EffectSimple2D : Effect
    {

        public EffectSimple2D()
        {

            LoadShaders("Data/Platform/DX12/Shader/2DSimple.hlsl");

        }

        public override void SetupShader()
        {

            Input = new[]
            {
                 new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
            };



        }

    }
}
