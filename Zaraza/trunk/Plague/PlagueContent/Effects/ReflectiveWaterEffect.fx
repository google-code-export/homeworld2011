float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;

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
texture RefractionMap;

float Bias;

float3 Ambient;

bool FogEnabled = false;
float3 FogColor;
float2 FogRange;

bool   SunlightEnabled = false;
float3 SunlightDirection;
float3 SunlightDiffuse;
float3 SunlightSpecular;

float SpecularStrength;

float3 CameraPosition;

float3x3 TBN = float3x3(float3(1,0,0),
						float3(0,0,1),
					    float3(0,1,0));

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
	float3 WorldPosition	  : TEXCOORD3;
	float  Depth			  : TEXCOORD4;
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

	float4x4 wvp	= mul(World,ViewProjection);
	output.Position = mul(input.Position, wvp);
	
	float4x4 rwvp			  = mul(World, mul(ReflectedView, Projection));
	output.ReflectionPosition = mul(input.Position, rwvp);

	output.NormalMapPosition = input.UV/WaveLength;
	output.NormalMapPosition.y -= Time * WaveSpeed;

	output.WorldPosition = mul(input.Position,World);
	
	output.Depth		 = output.Position.z;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if(!SunlightEnabled) return float4(0,0,0,1);

	float3 normal	= normalize(tex2D(waterNormalSampler, input.NormalMapPosition) * 2 - 1);
	float2 UVOffset = WaveHeight * normal.rg;
		
    float2 reflectionUV = postProjToScreen(input.ReflectionPosition) + halfPixel();
	float3 reflection	= tex2D(reflectionMapSampler, reflectionUV + UVOffset);
	float3 refraction	= tex2D(refractionMapSampler, reflectionUV + UVOffset);	
		
	float3 output = lerp(lerp(refraction,reflection,Bias), Color * SunlightDiffuse, ColorAmount);			

	float3 viewDirection = normalize(CameraPosition - input.WorldPosition);
	
	float3 reflectionVector = reflect(normalize(SunlightDirection), normalize(mul(normal,TBN)));
	float specular			= dot(normalize(reflectionVector), viewDirection);
	
	specular = pow(specular, SpecularStrength);

	output += SunlightSpecular * specular;

	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

	return float4(output, 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
