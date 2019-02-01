cbuffer cbPerObject : register(b0)
{
    float4x4 gProj;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float3 uv : TEXCOORD;
};


Texture2D g_texture : register(t0);
SamplerState g_sampler : register(s0);


PSInput VSMain(float3 position : POSITION, float4 uv : TEXCOORD)
{
	PSInput result;

	// vout.PosH = mul(float4(vin.PosL, 1.0f), gWorldViewProj);

	result.position =  mul(gProj,float4(position,1.0f));

	//esult.position = mul(position,gProj);

	result.uv = uv.xyz;

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return float4(1,1,1,1);
}