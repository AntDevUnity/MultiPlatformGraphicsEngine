using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Effect;
using SharpDX.DXGI;
using System.Threading;
using System.Runtime.InteropServices;
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


              //  fx.Root = _rootSignature;
               // DXGlobal.Root = _rootSignature;

                fx.Input = new[]
                {
                 new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
                 };

                var cb = new Simple2DConst
                {
                    Proj = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
                };

             //   _projBuf.CopyData(0, ref cb);


            };
            fx.LoadShaders("Data/Platform/DX12/Shader/2DSimple.hlsl");
            var cb2 = new Simple2DConst
            {
                Proj = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
            };
            //_projBuf.CopyData(0, ref cb2);


            return fx;

        }


    }
}
