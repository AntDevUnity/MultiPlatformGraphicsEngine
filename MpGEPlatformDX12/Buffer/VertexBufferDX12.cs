using SharpDX.DXGI;
using System.Threading;
using System;

using MpGe.Buffer;
namespace MpGEPlatformDX12.Buffer
{
    using MpGe.Data;
    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;



    public class VertexBufferDX12 : VertexBufferBase
    {


        public VertexBufferDX12(MpGe.Data.Vertex[] verts,short[] indices,int size,int stride) : base(verts,indices,size,stride)
        {


        }

        public GraphicsCommandList commandList = null;

        public override void InitBuffer(MpGe.Data.Vertex[] verts,short[] indices, int size, int stride)
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

            int indexBufferSize = Utilities.SizeOf(indices);

            indexBuffer = DXGlobal.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize), ResourceStates.GenericRead);

            IntPtr pIndexDataBegin = indexBuffer.Map(0);
            Utilities.Write(pIndexDataBegin, indices, 0, indices.Length);
            indexBuffer.Unmap(0);

            indexBufferView = new IndexBufferView();
            indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
            indexBufferView.Format = Format.R16_Typeless;
            indexBufferView.SizeInBytes = indexBufferSize;


            IndexCount = indices.Length;

        }
        public override void Update(Vertex[] verts,short[] indices)
        {
            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, verts, 0, verts.Length);
            vertexBuffer.Unmap(0);
            //base.Update(verts);
        }
        public int IndexCount = 0;
        public Resource vertexBuffer;
        public VertexBufferView vertexBufferView;
        public Resource indexBuffer;
        public IndexBufferView indexBufferView;

    }
}
