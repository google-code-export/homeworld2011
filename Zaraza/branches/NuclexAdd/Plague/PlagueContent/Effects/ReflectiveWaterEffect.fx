/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;

float3x3 TBN = float3x3(float3(1,0,0),
						float3(0,0,1),
					    float3(0,1,0));
/****************************************************/


/****************************************************/
/// Water
/****************************************************/
float3 Color;
float  ColorAmount;

float WaveLength;
float WaveHeight;
float WaveSpeed;
float Time;
float Bias;
float SpecularStrength;
/****************************************************/


/****************************************************/
/// Reflection/Refraction
/****************************************************/
float4x4 ReflectedView;
float ViewportWidth;
float ViewportHeight;

texture ReflectionMap;
texture RefractionMap;

sampler2D reflectionMapSampler = sampler_state
{
	texture = <ReflectionMap>;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
	AddressU = Mirror;
	AddressV = Mirror;
};

sampler2D refractionMapSampler = sampler_state
{
	texture = <RefractionMap>;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
	AddressU = Mirror;
	AddressV = Mirror;
};
/****************************************************/


/****************************************************/
/// Normal Map
/****************************************************/
texture WaterNormalMap;

sampler2D waterNormalSampler = sampler_state 
{
	texture = <WaterNormalMap>;
};
/****************************************************/


/****************************************************/
/// VertexShaderInput
/****************************************************/
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};
/****************************************************/


/****************************************************/
/// VertexShaderOutput
/****************************************************/
struct VertexShaderOutput
{
    float4 Position			  : POSITION0;
	float4 ReflectionPosition : TEXCOORD1;
	float2 NormalMapPosition  : TEXCOORD2;
	float3 WorldPosition	  : TEXCOORD3;
	float3 Depth			  : TEXCOORD4;
};
/****************************************************/


/****************************************************/
/// PostProjToScreen
/****************************************************/
float2 postProjToScreen(float4 position)
{
	float2 screenPos = position.xy / position.w;
	return 0.5f * (float2(screenPos.x, -screenPos.y) + 1);
}
/****************************************************/


/****************************************************/
/// halfPixel
/****************************************************/
float2 halfPixel()
{
	return 0.5f / float2(ViewportWidth, ViewportHeight);
}
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;		
	
	float4 worldPosition = mul(input.Position,World);
	output.Position = mul(worldPosition, ViewProjection);
	
	float4x4 rwvp			  = mul(World, mul(ReflectedView, Projection));
	output.ReflectionPosition = mul(input.Position, rwvp);

	output.NormalMapPosition = input.UV/WaveLength;
	output.NormalMapPosition.y -= Time * WaveSpeed;

	output.WorldPosition = mul(input.Position,World);
	
	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;
	output.Depth.z		 = mul(worldPosition,View).z;

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderOutput
/****************************************************/
struct PixelShaderOutput
{
	float4 Color	 : COLOR0;
	float4 Normal	 : COLOR1;
	float4 Depth	 : COLOR2;
	float4 SSAODepth : COLOR3;
};
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{	
	PixelShaderOutput output;

	float3 normal	= normalize(tex2D(waterNormalSampler, input.NormalMapPosition) * 2 - 1);
	float2 UVOffset = WaveHeight * normal.rg;
		
    float2 reflectionUV = postProjToScreen(input.ReflectionPosition) + halfPixel();
	float3 reflection	= tex2D(reflectionMapSampler, reflectionUV + UVOffset);
	float3 refraction	= tex2D(refractionMapSampler, reflectionUV + UVOffset);	
		
	output.Color = float4(lerp(lerp(refraction,reflection,Bias), Color, ColorAmount),0);			
	
	normal		  = normalize(mul(normal,TBN));
	output.Normal = float4(0.5f * (normal + 1.0f),SpecularStrength);
	
	output.Depth	 = input.Depth.x / input.Depth.y;	
	output.SSAODepth = input.Depth.z;				

	return output;
}
/****************************************************/


/****************************************************/
/// Technique1
/****************************************************/
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
/****************************************************/