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
    using MpGe.Buffer;
    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;


    public class DisplayDX12 : DisplayBase
    {

        public const int NumFrameResources = 3;
        public const int SwapChainBufferCount = 2;

        private RenderForm _window;             // Main window.
        private bool _appPaused;          // Is the application paused?
        private bool _minimized;          // Is the application minimized?
        private bool _maximized;          // Is the application maximized?
        private bool _resizing;           // Are the resize bars being dragged?
        private bool _running;            // Is the application running?

        // Set true to use 4X MSAA (§4.1.8).
        private bool _m4xMsaaState;       // 4X MSAA enabled.
        private int _m4xMsaaQuality;      // Quality level of 4X MSAA.

       // private FormWindowState _lastWindowState = FormWindowState.Normal;

        private int _frameCount;
        private float _timeElapsed;

        private Factory4 _factory;
        private readonly Resource[] _swapChainBuffers = new Resource[SwapChainBufferCount];

        private AutoResetEvent _fenceEvent;

        public bool M4xMsaaState
        {
            get { return _m4xMsaaState; }
            set
            {
                if (_m4xMsaaState != value)
                {
                    _m4xMsaaState = value;

                    if (_running)
                    {
                        // Recreate the swapchain and buffers with new multisample settings.
                        CreateSwapChain();
                        OnResize();
                    }
                }
            }
        }

        protected DescriptorHeap RtvHeap { get; private set; }
        protected DescriptorHeap DsvHeap { get; private set; }

        protected int MsaaCount => M4xMsaaState ? 4 : 1;
        protected int MsaaQuality => M4xMsaaState ? _m4xMsaaQuality - 1 : 0;

       // protected GameTimer Timer { get; } = new GameTimer();

        protected Device Device { get; private set; }

        protected Fence Fence { get; private set; }
        protected long CurrentFence { get; set; }

        protected int RtvDescriptorSize { get; private set; }
        protected int DsvDescriptorSize { get; private set; }
        protected int CbvSrvUavDescriptorSize { get; private set; }

        public CommandQueue CommandQueue { get; private set; }
        public CommandAllocator DirectCmdListAlloc { get; private set; }
        public GraphicsCommandList CommandList { get; private set; }

        protected SwapChain3 SwapChain { get; private set; }
        protected Resource DepthStencilBuffer { get; private set; }

        protected ViewportF Viewport { get; set; }
        protected RectangleF ScissorRectangle { get; set; }

        protected string MainWindowCaption { get; set; } = "D3D12 Application";
        protected int ClientWidth { get; set; } = 1280;
        protected int ClientHeight { get; set; } = 720;

        protected float AspectRatio => (float)ClientWidth / ClientHeight;

        protected Format BackBufferFormat { get; } = Format.R8G8B8A8_UNorm;
        protected Format DepthStencilFormat { get; } = Format.D24_UNorm_S8_UInt;

        protected Resource CurrentBackBuffer => _swapChainBuffers[SwapChain.CurrentBackBufferIndex];
        public CpuDescriptorHandle CurrentBackBufferView
            => RtvHeap.CPUDescriptorHandleForHeapStart + SwapChain.CurrentBackBufferIndex * RtvDescriptorSize;
        public CpuDescriptorHandle DepthStencilView => DsvHeap.CPUDescriptorHandleForHeapStart;


        protected virtual void OnResize()
        {
            //Debug.Assert(Device != null);
           // Debug.Assert(SwapChain != null);
            //Debug.Assert(DirectCmdListAlloc != null);

            // Flush before changing any resources.
            FlushCommandQueue();

            CommandList.Reset(DirectCmdListAlloc, null);

            // Release the previous resources we will be recreating.
            foreach (Resource buffer in _swapChainBuffers)
                buffer?.Dispose();
            DepthStencilBuffer?.Dispose();

            // Resize the swap chain.
            SwapChain.ResizeBuffers(
                SwapChainBufferCount,
                ClientWidth, ClientHeight,
                BackBufferFormat,
                SwapChainFlags.AllowModeSwitch);

            CpuDescriptorHandle rtvHeapHandle = RtvHeap.CPUDescriptorHandleForHeapStart;
            for (int i = 0; i < SwapChainBufferCount; i++)
            {
                Resource backBuffer = SwapChain.GetBackBuffer<Resource>(i);
                _swapChainBuffers[i] = backBuffer;
                Device.CreateRenderTargetView(backBuffer, null, rtvHeapHandle);
                rtvHeapHandle += RtvDescriptorSize;
            }

            // Create the depth/stencil buffer and view.
            var depthStencilDesc = new ResourceDescription
            {
                Dimension = ResourceDimension.Texture2D,
                Alignment = 0,
                Width = ClientWidth,
                Height = ClientHeight,
                DepthOrArraySize = 1,
                MipLevels = 1,
                Format = Format.R24G8_Typeless,
                SampleDescription = new SampleDescription
                {
                    Count = MsaaCount,
                    Quality = MsaaQuality
                },
                Layout = TextureLayout.Unknown,
                Flags = ResourceFlags.AllowDepthStencil
            };
            var optClear = new ClearValue
            {
                Format = DepthStencilFormat,
                DepthStencil = new DepthStencilValue
                {
                    Depth = 1.0f,
                    Stencil = 0
                }
            };
            DepthStencilBuffer = Device.CreateCommittedResource(
                new HeapProperties(HeapType.Default),
                HeapFlags.None,
                depthStencilDesc,
                ResourceStates.Common,
                optClear);

            var depthStencilViewDesc = new DepthStencilViewDescription
            {
                Dimension = M4xMsaaState
                    ? DepthStencilViewDimension.Texture2DMultisampled
                    : DepthStencilViewDimension.Texture2D,
                Format = DepthStencilFormat
            };
            // Create descriptor to mip level 0 of entire resource using a depth stencil format.
            CpuDescriptorHandle dsvHeapHandle = DsvHeap.CPUDescriptorHandleForHeapStart;
            Device.CreateDepthStencilView(DepthStencilBuffer, depthStencilViewDesc, dsvHeapHandle);

            // Transition the resource from its initial state to be used as a depth buffer.
            CommandList.ResourceBarrierTransition(DepthStencilBuffer, ResourceStates.Common, ResourceStates.DepthWrite);

            // Execute the resize commands.
            CommandList.Close();
            CommandQueue.ExecuteCommandList(CommandList);

            // Wait until resize is complete.
            FlushCommandQueue();

            Viewport = new ViewportF(0, 0, ClientWidth, ClientHeight, 0.0f, 1.0f);
            ScissorRectangle = new RectangleF(0, 0, ClientWidth, ClientHeight);
        }


        /// <summary>
        /// Creates the Directx12 Display.
        /// </summary>
        /// <param name="metrics"></param>
        public DisplayDX12(DisplayMetrics metrics) : base(metrics)
        {
            Metrics = metrics;
            DXGlobal.Display = this;
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
            Main = this;
            Form = new RenderForm(Metrics.RequestName)
            {
                Width = Metrics.RequestWidth,
                Height = Metrics.RequestHeight
            };

            Form.Show();
            _window = Form;

            //InitMainWindow();
            InitDirect3D(Form);

            OnResize();

            //CommandList.Reset(DirectCmdListAlloc, null);

            BuildDescriptorHeaps();
           


            //LoadPipeline(Form);
            //LoadAssets();

            Console.WriteLine("Creating DirectX12 display");
            return new MpGe.Base.Result(true);
            //   return base.Request();
        }


        private void BuildDescriptorHeaps()
        {
            var cbvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
                Flags = DescriptorHeapFlags.ShaderVisible,
                NodeMask = 0
            };
            //_cbvHeap = Device.CreateDescriptorHeap(cbvHeapDesc);
            //_descriptorHeaps = new[] { _cbvHeap };
        }

        public override void BeginDraw()
        {
            // Reuse the memory associated with command recording.
            // We can only reset when the associated command lists have finished execution on the GPU.
            DirectCmdListAlloc.Reset();

            // A command list can be reset after it has been added to the command queue via ExecuteCommandList.
            // Reusing the command list reuses memory.
            CommandList.Reset(DirectCmdListAlloc, null);

            CommandList.SetViewport(Viewport);
            CommandList.SetScissorRectangles(ScissorRectangle);

            // Indicate a state transition on the resource usage.
            CommandList.ResourceBarrierTransition(CurrentBackBuffer, ResourceStates.Present, ResourceStates.RenderTarget);

            // Clear the back buffer and depth buffer.
            CommandList.ClearRenderTargetView(CurrentBackBufferView, Color.DarkBlue);
            CommandList.ClearDepthStencilView(DepthStencilView, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, 1.0f, 0);

            // Specify the buffers we are going to render to.
            CommandList.SetRenderTargets(CurrentBackBufferView, DepthStencilView);



            // TODO: API suggestion: rename descriptorHeapsOut to descriptorHeaps;
            // TODO: Add an overload for a setting a single SetDescriptorHeap?
            // TODO: Make requiring explicit length optional.
          //  CommandList.SetDescriptorHeaps(_descriptorHeaps.Length, _descriptorHeaps);

           // CommandList.SetGraphicsRootSignature(_rootSignature);

       //     CommandList.SetVertexBuffer(0, _boxGeo.VertexBufferView);
        //    CommandList.SetIndexBuffer(_boxGeo.IndexBufferView);
         //   CommandList.PrimitiveTopology = PrimitiveTopology.TriangleList;

            //CommandList.SetGraphicsRootDescriptorTable(0, _cbvHeap.GPUDescriptorHandleForHeapStart);

            //CommandList.DrawIndexedInstanced(_boxGeo.IndexCount, 1, 0, 0, 0);

         

        }
        public CpuDescriptorHandle rtvHandle;
        public override void EndDraw()
        {


            CommandList.ResourceBarrierTransition(CurrentBackBuffer, ResourceStates.RenderTarget, ResourceStates.Present);

            // Done recording commands.
            CommandList.Close();

            // Add the command list to the queue for execution.
            CommandQueue.ExecuteCommandList(CommandList);

            // Present the buffer to the screen. Presenting will automatically swap the back and front buffers.
            SwapChain.Present(0, PresentFlags.None);
            FlushCommandQueue();
            //swapChain.Present(0, PresentFlags.None);
            base.EndDraw();
        }

      
        public override void DrawBuffer(VertexBufferBase vb,MpGe.Effect.EffectBase eb)
        {

            var eff = eb as Effect.Effect;
           
            Buffer.VertexBufferDX12 buf = vb as Buffer.VertexBufferDX12;

            var list = eff.commandList;

            //   list.SetRenderTargets(rtvHandle, null);
            CommandList.PipelineState = eff.pipelineState;
            CommandList.SetDescriptorHeaps(eff._descriptorHeaps.Length,eff._descriptorHeaps);
            CommandList.SetGraphicsRootSignature(eff.Root);


        
            CommandList.SetVertexBuffer(0, buf.vertexBufferView);
            CommandList.SetIndexBuffer(buf.indexBufferView);
            CommandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            


         CommandList.SetGraphicsRootDescriptorTable(0, eff._cbvHeap.GPUDescriptorHandleForHeapStart);
           CommandList.DrawIndexedInstanced(buf.IndexCount, 1, 0, 0, 0);


            //CommandQueue.ExecuteCommandList(CommandList);
            //FlushCommandQueue();


        }


        public void WaitForPreviousFrame()
        {
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 

            int localFence = fenceValue;
            commandQueue.Signal(this.fence, localFence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (this.fence.CompletedValue < localFence)
            {
                this.fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }


        private void LoadAssets()
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
            rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

            // Create the command list.
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, null);

            aspectRation = viewport.Width / viewport.Height;

            // Command lists are created in the recording state, but there is nothing
            // to record yet. The main loop expects it to be closed, so close it now.
            commandList.Close();

            // Create synchronization objects.
            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;

            // Create an event handle to use for frame synchronization.
            fenceEvent = new AutoResetEvent(false);
        }
        float aspectRation;
        private void InitDirect3D(RenderForm form)
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

            _factory = new Factory4();

            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            DXGlobal.device = device;
            this.Device = device;

            Fence = Device.CreateFence(0, FenceFlags.None);
            _fenceEvent = new AutoResetEvent(false);

            RtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            DsvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.DepthStencilView);
            CbvSrvUavDescriptorSize = device.GetDescriptorHandleIncrementSize(
                DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);


            FeatureDataMultisampleQualityLevels msQualityLevels;
            msQualityLevels.Format = BackBufferFormat;
            msQualityLevels.SampleCount = 4;
            msQualityLevels.Flags = MultisampleQualityLevelFlags.None;
            msQualityLevels.QualityLevelCount = 0;
           // Debug.Assert(Device.CheckFeatureSupport(Feature.MultisampleQualityLevels, ref msQualityLevels));
            _m4xMsaaQuality = msQualityLevels.QualityLevelCount;

            CreateCommandObjects();
            CreateSwapChain();
            CreateRtvAndDsvDescriptorHeaps();

         
        }

        private void CreateCommandObjects()
        {
            var queueDesc = new CommandQueueDescription(CommandListType.Direct);
            CommandQueue = Device.CreateCommandQueue(queueDesc);

            DirectCmdListAlloc = Device.CreateCommandAllocator(CommandListType.Direct);

            CommandList = Device.CreateCommandList(
                0,
                CommandListType.Direct,
                DirectCmdListAlloc, // Associated command allocator.
                null);              // Initial PipelineStateObject.

            // Start off in a closed state.  This is because the first time we refer
            // to the command list we will Reset it, and it needs to be closed before
            // calling Reset.
            CommandList.Close();
        }

        private void CreateSwapChain()
        {
            // Release the previous swapchain we will be recreating.
            SwapChain?.Dispose();

            var sd = new SwapChainDescription
            {
                ModeDescription = new ModeDescription
                {
                    Width = ClientWidth,
                    Height = ClientHeight,
                    Format = BackBufferFormat,
                    RefreshRate = new Rational(60, 1),
                    Scaling = DisplayModeScaling.Unspecified,
                    ScanlineOrdering = DisplayModeScanlineOrder.Unspecified
                },
                SampleDescription = new SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = Usage.RenderTargetOutput,
                BufferCount = SwapChainBufferCount,
                SwapEffect = SwapEffect.FlipDiscard,
                Flags = SwapChainFlags.AllowModeSwitch,
                OutputHandle = _window.Handle,
                IsWindowed = true
            };

            using (var tempSwapChain = new SwapChain(_factory, CommandQueue, sd))
            {
                SwapChain = tempSwapChain.QueryInterface<SwapChain3>();
            }
        }

        private void CreateRtvAndDsvDescriptorHeaps()
        {
            var rtvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = RtvDescriptorCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };
            RtvHeap = Device.CreateDescriptorHeap(rtvHeapDesc);

            var dsvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = DsvDescriptorCount,
                Type = DescriptorHeapType.DepthStencilView
            };
            DsvHeap = Device.CreateDescriptorHeap(dsvHeapDesc);


            var srvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 1,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };

            shaderRenderViewHeap = device.CreateDescriptorHeap(srvHeapDesc);


        }

        DescriptorHeap shaderRenderViewHeap; 
        public static void FlushCQ()
        {
            Main.FlushCommandQueue();
        }

        public static DisplayDX12 Main = null;

        public void FlushCommandQueue()
        {
            // Advance the fence value to mark commands up to this fence point.
            CurrentFence++;

            // Add an instruction to the command queue to set a new fence point.  Because we
            // are on the GPU timeline, the new fence point won't be set until the GPU finishes
            // processing all the commands prior to this Signal().
            CommandQueue.Signal(Fence, CurrentFence);

            // Wait until the GPU has completed commands up to this fence point.
            if (Fence.CompletedValue < CurrentFence)
            {
                // Fire event when GPU hits current fence.
                Fence.SetEventOnCompletion(CurrentFence, _fenceEvent.SafeWaitHandle.DangerousGetHandle());

                // Wait until the GPU hits current fence event is fired.
                _fenceEvent.WaitOne();
            }
        }

        protected virtual int RtvDescriptorCount => SwapChainBufferCount;
        protected virtual int DsvDescriptorCount => 1;


        const int FrameCount = 2;

        public ViewportF viewport;
        public Rectangle scissorRect;
        // Pipeline objects.
        private SwapChain3 swapChain;
        private Device device;
        public readonly Resource[] renderTargets = new Resource[FrameCount];
        public CommandAllocator commandAllocator;
        public CommandQueue commandQueue;
        public RootSignature rootSignature;
        public DescriptorHeap renderTargetViewHeap;
      //  private PipelineState pipelineState;
        public GraphicsCommandList commandList;
        public int rtvDescriptorSize;

        // App resources.
        Resource vertexBuffer;
        VertexBufferView vertexBufferView;

        // Synchronization objects.
        public int frameIndex;
        private AutoResetEvent fenceEvent;

        private Fence fence;
        private int fenceValue;
    }
}
