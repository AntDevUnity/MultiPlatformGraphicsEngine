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

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct Simple2DConst
        {
            public Matrix Proj;
        }

        public static Effect CreateSimple2D()
        {

            var fx = new Effect();
            fx.Init = (obj) =>
            {

                BuildDescriptorHeaps();
                BuildConstantBuffers();

                fx.Input = new[]
                {
                 new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
                 };

                var cb = new Simple2DConst
                {
                    Proj = Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
                };

                _projBuf.CopyData(0, ref cb);


            };
            fx.LoadShaders("Data/Platform/DX12/Shader/2DSimple.hlsl");

         

            return fx;

        }
        private static void BuildDescriptorHeaps()
        {
            var cbvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
                Flags = DescriptorHeapFlags.ShaderVisible,
                NodeMask = 0
            };
            _cbvHeap = DXGlobal.device.CreateDescriptorHeap(cbvHeapDesc);
            _descriptorHeaps = new[] { _cbvHeap };
        }

        static DescriptorHeap _cbvHeap;

        static private DescriptorHeap[] _descriptorHeaps;



        private static void BuildConstantBuffers()
        {
            int sizeInBytes = DXUtil1.D3DUtil.CalcConstantBufferByteSize<Simple2DConst>();

            _projBuf = new DXUtil2.UploadBuffer<Simple2DConst>(DXGlobal.device, 1, true);


            var cbvDesc = new ConstantBufferViewDescription
            {
                BufferLocation = _projBuf.Resource.GPUVirtualAddress,
                SizeInBytes = sizeInBytes
            };
            CpuDescriptorHandle cbvHeapHandle = _cbvHeap.CPUDescriptorHandleForHeapStart;
            DXGlobal.device.CreateConstantBufferView(cbvDesc, cbvHeapHandle);
        }

        static DXUtil2.UploadBuffer<Simple2DConst> _projBuf = null;

    }
}
