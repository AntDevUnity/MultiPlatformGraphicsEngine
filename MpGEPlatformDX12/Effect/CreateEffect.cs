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

       public static Effect CreateTextured2D()
        {
            var fx = new Effect();
            /*
            fx.RootMake = ()=>{
                var cbvTable = new DescriptorRange(DescriptorRangeType.ConstantBufferView,1, 0,0);

                var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
        // Root Parameters
        new[]
        {
            
            
                    new RootParameter(ShaderVisibility.Pixel,
                        new DescriptorRange()
                        {
                            RangeType = DescriptorRangeType.ShaderResourceView,
                            DescriptorCount =1,
                            RegisterSpace = 0,
                            OffsetInDescriptorsFromTableStart = 0,
                           
                            //BaseShaderRegister = 0,
                            
                        })
        },
        // Samplers
        new[]
        {
                    new StaticSamplerDescription(ShaderVisibility.Pixel, 0,0)
                    {
                        Filter = Filter.MinimumMinMagMipPoint,
                        AddressUVW = TextureAddressMode.Border,
                      //  RegisterSpace = 0
                    }
        });

                fx.Root = DXGlobal.device.CreateRootSignature(0, rootSignatureDesc.Serialize());


            };
            */
            fx.Init = (obj) =>
            {


                //  fx.Root = _rootSignature;
                // DXGlobal.Root = _rootSignature;

                fx.Input = new[]
                {
                 new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("TEXCOORD",0,Format.R32G32_Float,12,0)
                 };

                var cb = new Simple2DConst
                {
                    Proj = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
                };

                //   _projBuf.CopyData(0, ref cb);


            };
            fx.LoadShaders("Data/Platform/DX12/Shader/2DTextured.hlsl");
            var cb2 = new Simple2DConst
            {
                Proj = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
            };
            //_projBuf.CopyData(0, ref cb2);


            return fx;
        }

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
