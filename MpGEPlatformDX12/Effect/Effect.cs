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

         //   pipelineState = device.CreateGraphicsPipelineState(psoDesc);



        }

        public virtual void SetupShader()
        {


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
