cbuffer cbPerObject : register(b0)
{
    float4x4 gProj;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float2 uv : TEXCOORD;
};


Texture2D g_texture : register(t0);
SamplerState g_sampler : register(s0);


PSInput VSMain(float4 position : POSITION, float4 uv : TEXCOORD)
{
	PSInput result;

	// vout.PosH = mul(float4(vin.PosL, 1.0f), gWorldViewProj);

	result.position = position;

	//esult.position = mul(position,gProj);

	result.uv = uv;

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return float4(1,1,1,1);
}