using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Effect;
using SharpDX.DXGI;
using System.Threading;


namespace MpGEPlatformDX12.Effect
{

    using SharpDX;
    using SharpDX.Direct3D12;
    using SharpDX.Windows;


    public class Effect : EffectBase
    {

        public ShaderBytecode GeoCode;
        public ShaderBytecode VertexCode;
        public ShaderBytecode FragCode;
        public ShaderBytecode RTCode;

        public RootSignature Root;

        public InputElement[] Input = null;

        public PipelineState pipelineState;


        public void LoadShaders(string path)
        {

            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
            Root = DXGlobal.device.CreateRootSignature(rootSignatureDesc.Serialize());

            VertexCode = LoadVertex(path);
            FragCode = LoadFrag(path);
            SetupShader();

            var psoDesc = new GraphicsPipelineStateDescription()
            {
                InputLayout = new InputLayoutDescription(Input),
                RootSignature = Root,
                VertexShader = VertexCode,
                PixelShader = FragCode,
                RasterizerState = RasterizerStateDescription.Default(),
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                Flags = PipelineStateFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            psoDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            pipelineState = DXGlobal.device.CreateGraphicsPipelineState(psoDesc);

            commandList = DXGlobal.device.CreateCommandList(CommandListType.Direct, DXGlobal.Display.commandAllocator, pipelineState);



        }

        public void BeginRen()
        {
            //commandAllocator.Reset();

            // However, when ExecuteCommandList() is called on a particular command 
            // list, that command list can then be reset at any time and must be before 
            // re-recording.
            commandList.Reset(DXGlobal.Display.commandAllocator, pipelineState);

            commandList.SetGraphicsRootSignature(DXGlobal.Display.rootSignature);
            commandList.SetViewport(DXGlobal.Display.viewport);
            commandList.SetScissorRectangles(DXGlobal.Display.scissorRect);
            commandList.ResourceBarrierTransition(DXGlobal.Display.renderTargets[DXGlobal.Display.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);


            var rtvHandle = DXGlobal.Display.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += DXGlobal.Display.frameIndex * DXGlobal.Display.rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);

            commandList.ClearRenderTargetView(rtvHandle, new Color4(0.3f, 0.2F, 0.4f, 1), 0, null);



        }

        public void EndRen()
        {

            commandList.ResourceBarrierTransition(DXGlobal.Display.renderTargets[DXGlobal.Display.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            commandList.Close();

            DXGlobal.Display.commandQueue.ExecuteCommandList(commandList);


        }

        public GraphicsCommandList commandList;


        public virtual void SetupShader()
        {
            Init?.Invoke(null);

        }

        public ShaderBytecode LoadVertex(string path)
        {

#if DEBUG
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, "VSMain", "vs_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, "VSMain", "vs_5_0"));
#endif
            return vertexShader;

        }

        public ShaderBytecode LoadFrag(string path)
        {

#if DEBUG
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(path, "PSMain", "ps_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0"));
#endif

            return pixelShader;
        }

    }
}
