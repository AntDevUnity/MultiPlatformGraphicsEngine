using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;
using System.Drawing;
namespace MpGEPlatformDX12.Texture
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;
    using System.Runtime.InteropServices;



    public class Texture2D : MpGe.Texture.Texture2DBase
    {

        public Texture2D(string path)
        {

            Load(path);

        }


        Resource texture;
        public override void Load(string path)
        {

            Bitmap bm = new Bitmap(path);

            Width = bm.Width;
            Height = bm.Height;
            Depth = 4;

            var textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, Width, Height);
            texture = DXGlobal.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

            long uploadBufferSize = GetRequiredIntermediateSize(this.texture, 0, 1);

            // Create the GPU upload buffer.
            var textureUploadHeap = DXGlobal.device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, Width,Height), ResourceStates.GenericRead);

            // Copy data to the intermediate upload heap and then schedule a copy 
            // from the upload heap to the Texture2D.
            byte[] textureData = new byte[Width * Height * 4];

            int loc = 0;
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    var pix = bm.GetPixel(x, y);
                    textureData[loc++] = pix.R;
                    textureData[loc++] = pix.G;
                    textureData[loc++] = pix.B;
                    textureData[loc++] = 255;
                }
            }

            var handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
            textureUploadHeap.WriteToSubresource(0, null, ptr, Depth * Width, textureData.Length);
            handle.Free();

            DXGlobal.Display.CommandList.Reset(DXGlobal.Display.DirectCmdListAlloc, null);

            DXGlobal.Display.CommandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);

            DXGlobal.Display.CommandList.ResourceBarrierTransition(this.texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);

            // Describe and create a SRV for the texture.
            var srvDesc = new ShaderResourceViewDescription
            {
                Shader4ComponentMapping = D3DXUtilities.DefaultComponentMapping(),
                Format = textureDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = { MipLevels = 1 },
            };

            //DXGlobal.device.CreateShaderResourceView(this.texture, srvDesc, shaderRenderViewHeap.CPUDescriptorHandleForHeapStart);

            DXGlobal.Display.CommandList.Close();
            DXGlobal.Display.CommandQueue.ExecuteCommandList(DXGlobal.Display.CommandList);

        }
        


        public class D3DXUtilities
        {

            public const int ComponentMappingMask = 0x7;

            public const int ComponentMappingShift = 3;

            public const int ComponentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (ComponentMappingShift * 4));

            public static int ComponentMapping(int src0, int src1, int src2, int src3)
            {

                return ((((src0) & ComponentMappingMask) |
                (((src1) & ComponentMappingMask) << ComponentMappingShift) |
                                                                    (((src2) & ComponentMappingMask) << (ComponentMappingShift * 2)) |
                                                                    (((src3) & ComponentMappingMask) << (ComponentMappingShift * 3)) |
                                                                    ComponentMappingAlwaysSetBitAvoidingZeromemMistakes));
            }

            public static int DefaultComponentMapping()
            {
                return ComponentMapping(0, 1, 2, 3);
            }

            public static int ComponentMapping(int ComponentToExtract, int Mapping)
            {
                return ((Mapping >> (ComponentMappingShift * ComponentToExtract) & ComponentMappingMask));
            }
        }

        private long GetRequiredIntermediateSize(Resource destinationResource, int firstSubresource, int NumSubresources)
        {
            var desc = destinationResource.Description;
            long requiredSize;
            DXGlobal.device.GetCopyableFootprints(ref desc, firstSubresource, NumSubresources, 0, null, null, null, out requiredSize);
            return requiredSize;
        }

    }
}
