float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 ReflectedView;
float ViewportWidth;
float ViewportHeight;

float3 Color;
float  ColorAmount;

float WaveLength;
float WaveHeight;
float WaveSpeed;
float Time;

texture ReflectionMap;

sampler2D reflectionMapSampler = sampler_state
{
	texture = <ReflectionMap>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Mirror;
	AddressV = Mirror;
};

texture WaterNormalMap;

sampler2D waterNormalSampler = sampler_state 
{
	texture = <WaterNormalMap>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position			  : POSITION0;
	float4 ReflectionPosition : TEXCOORD1;
	float2 NormalMapPosition  : TEXCOORD2;
};

float2 postProjToScreen(float4 position)
{
	float2 screenPos = position.xy / position.w;
	return 0.5f * (float2(screenPos.x, -screenPos.y) + 1);
}

float2 halfPixel()
{
	return 0.5f / float2(ViewportWidth, ViewportHeight);
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 wvp	= mul(World, mul(View, Projection));
	output.Position = mul(input.Position, wvp);
	
	float4x4 rwvp			  = mul(World, mul(ReflectedView, Projection));
	output.ReflectionPosition = mul(input.Position, rwvp);

	output.NormalMapPosition = input.UV/WaveLength;
	output.NormalMapPosition.y -= Time * WaveSpeed;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 normal	= tex2D(waterNormalSampler, input.NormalMapPosition) * 2 - 1;
	float2 UVOffset = WaveHeight * normal.rg;
		
    float2 reflectionUV = postProjToScreen(input.ReflectionPosition) + halfPixel();
	float3 reflection	= tex2D(reflectionMapSampler, reflectionUV + UVOffset);
	
	return float4(lerp(reflection, Color, ColorAmount), 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
