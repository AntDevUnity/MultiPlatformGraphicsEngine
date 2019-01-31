using SharpDX.DXGI;
using System.Threading;
using System;

using MpGe.Buffer;
namespace MpGEPlatformDX12.Buffer
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;



    public class VertexBufferDX12 : VertexBufferBase
    {


        public VertexBufferDX12(MpGe.Data.Vertex[] verts,int size,int stride) : base(verts,size,stride)
        {


        }

        public override void InitBuffer(MpGe.Data.Vertex[] verts, int size, int stride)
        {

            int vertexBufferSize = Utilities.SizeOf(verts);

            vertexBuffer = DXGlobal.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);

            // Copy the triangle data to the vertex buffer.
            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, verts, 0, verts.Length);
            vertexBuffer.Unmap(0);

            // Initialize the vertex buffer view.
            vertexBufferView = new VertexBufferView();
            vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
            vertexBufferView.StrideInBytes = Utilities.SizeOf<MpGe.Data.Vertex>();
            vertexBufferView.SizeInBytes = vertexBufferSize;

        }

        Resource vertexBuffer;
        VertexBufferView vertexBufferView;

    }
}
