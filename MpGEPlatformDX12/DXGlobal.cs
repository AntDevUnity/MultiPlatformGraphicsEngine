using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Display;
using MpGe.Base;
using System.Threading;
using System;
using MpGe.Base;
using SharpDX.DXGI;
namespace MpGEPlatformDX12
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;

    public class DXGlobal
    {

      
        public static Display.DisplayDX12 Display;
        public static SwapChain3 swapChain;
        public static Device device;
      //  private Resource[] renderTargets = new Resource[FrameCount];

        private CommandAllocator commandAllocator;
        private CommandQueue commandQueue;
        private DescriptorHeap renderTargetViewHeap;

        private GraphicsCommandList commandList;
        private int rtvDescriptorSize;

        // Synchronization objects.
        private int frameIndex;
        private AutoResetEvent fenceEvent;

        private Fence fence;
        private int fenceValue;

    }
}
