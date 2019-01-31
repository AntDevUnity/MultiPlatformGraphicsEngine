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
namespace MpGEPlatformDX12.Display
{
       using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;


    public class DisplayDX12 : DisplayBase
    {

        /// <summary>
        /// Creates the Directx12 Display.
        /// </summary>
        /// <param name="metrics"></param>
        public DisplayDX12(DisplayMetrics metrics) : base(metrics)
        {
            Metrics = metrics;
         
        }
        
        /// <summary>
        /// The internal RenderForm;
        /// </summary>
        public RenderForm Form
        {
            get;
            set;
        }

        public RenderLoop Loop
        {
            get;
            set;
        }


        /// <summary>
        /// Requests the required display.
        /// </summary>
        /// <returns></returns>
        public override MpGe.Base.Result Request()
        {

            Form = new RenderForm(Metrics.RequestName)
            {
                Width = Metrics.RequestWidth,
                Height = Metrics.RequestHeight
            };

            Form.Show();

            LoadPipeline(Form);
            LoadAssets();

            Console.WriteLine("Creating DirectX12 display");
            return new MpGe.Base.Result(true);
            //   return base.Request();
        }

        public override void BeginDraw()
        {
            PopulateCommandList();

        

            base.BeginDraw();
        }

        public override void EndDraw()
        {

            commandQueue.ExecuteCommandList(commandList);

            swapChain.Present(1, 0);

            WaitForPreviousFrame();
            base.EndDraw();
        }

        private void PopulateCommandList()
        {
            // Command list allocators can only be reset when the associated 
            // command lists have finished execution on the GPU; apps should use 
            // fences to determine GPU execution progress.
            commandAllocator.Reset();

            // However, when ExecuteCommandList() is called on a particular command 
            // list, that command list can then be reset at any time and must be before 
            // re-recording.
            commandList.Reset(commandAllocator, null);

            // Indicate that the back buffer will be used as a render target.
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;

            // Record commands.
            commandList.ClearRenderTargetView(rtvHandle, new Color4(0, 0.2F, 0.4f, 1), 0, null);

            // Indicate that the back buffer will now be used to present.
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            commandList.Close();
        }

        private void WaitForPreviousFrame()
        {
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 

            int fence = fenceValue;
            commandQueue.Signal(this.fence, fence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (this.fence.CompletedValue < fence)
            {
                this.fence.SetEventOnCompletion(fence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }


        private void LoadAssets()
        {
            // Create the command list.
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, null);

            // Command lists are created in the recording state, but there is nothing
            // to record yet. The main loop expects it to be closed, so close it now.
            commandList.Close();

            // Create synchronization objects.
            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;

            // Create an event handle to use for frame synchronization.
            fenceEvent = new AutoResetEvent(false);
        }

        private void LoadPipeline(RenderForm form)
        {

            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;

            viewport.Width = width;
            viewport.Height = height;
            viewport.MaxDepth = 1.0f;

            scissorRect.Right = width;
            scissorRect.Bottom = height;

#if DEBUG
            // Enable the D3D12 debug layer.
            {
                DebugInterface.Get().EnableDebugLayer();
            }
#endif
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            DXGlobal.device = device;
            using (var factory = new Factory4())
            {
                // Describe and create the command queue.
                var queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);


                // Describe and create the swap chain.
                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = FrameCount,
                    ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = form.Handle,
                    //Flags = SwapChainFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = true
                };

                var tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                frameIndex = swapChain.CurrentBackBufferIndex;
            }

            // Create descriptor heaps.
            // Describe and create a render target view (RTV) descriptor heap.
            var rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };

            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

            // Create frame resources.
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            for (int n = 0; n < FrameCount; n++)
            {
                renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
                device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
        }
        const int FrameCount = 2;

        private ViewportF viewport;
        private Rectangle scissorRect;
        // Pipeline objects.
        private SwapChain3 swapChain;
        private Device device;
        private readonly Resource[] renderTargets = new Resource[FrameCount];
        private CommandAllocator commandAllocator;
        private CommandQueue commandQueue;
        private RootSignature rootSignature;
        private DescriptorHeap renderTargetViewHeap;
        private PipelineState pipelineState;
        private GraphicsCommandList commandList;
        private int rtvDescriptorSize;

        // App resources.
        Resource vertexBuffer;
        VertexBufferView vertexBufferView;

        // Synchronization objects.
        private int frameIndex;
        private AutoResetEvent fenceEvent;

        private Fence fence;
        private int fenceValue;
    }
}
