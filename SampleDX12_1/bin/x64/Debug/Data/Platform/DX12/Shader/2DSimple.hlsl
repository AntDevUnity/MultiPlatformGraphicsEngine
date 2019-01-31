cbuffer cbPerObject : register(b0)
{
    float4x4 gProj;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(float4 position : POSITION, float4 color : COLOR)
{
	PSInput result;

	// vout.PosH = mul(float4(vin.PosL, 1.0f), gWorldViewProj);

	result.position = position; //mul(position,gProj);

	//esult.position = mul(position,gProj);

	result.color = float4(1,1,1,1);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return input.color;
}