float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
float3 CameraPosition;

float3 Ambient;

bool   SunlightEnabled = false;
float3 SunlightDirection;
float3 SunlightDiffuse;
float3 SunlightSpecular;

float TextureTiling;

texture BaseTexture;
texture RTexture;
texture GTexture;
texture BTexture;
texture WeightMap;

bool FogEnabled = false;
float3 FogColor;
float2 FogRange;

bool ClipPlaneEnabled = false;
float4 ClipPlane;

sampler BaseTextureSampler = sampler_state 
{
	texture = <BaseTexture>;
};

sampler RTextureSampler = sampler_state 
{
	texture = <RTexture>;
};

sampler GTextureSampler = sampler_state 
{
	texture = <GTexture>;
};

sampler BTextureSampler = sampler_state 
{
	texture = <BTexture>;
};

sampler WeightMapSampler = sampler_state 
{
	texture = <WeightMap>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal	: NORMAL0;
	float2 UV		: TEXCOORD0;	
};

struct VertexShaderOutput
{
    float4 Position		 : POSITION0;
	float2 UV			 : TEXCOORD0;
	float4 Normal		 : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float  Depth		 : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    	
	output.UV			 = input.UV;
	output.Normal		 = mul(input.Normal,   World);
	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	
	output.Position		 = mul(worldPosition,ViewProjection);
	output.Depth		 = output.Position.z;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}	
	
	float3 baseTex = tex2D(BaseTextureSampler, input.UV * TextureTiling);
	float3 rTex	   = tex2D(RTextureSampler,	   input.UV * TextureTiling);
	float3 gTex	   = tex2D(GTextureSampler,	   input.UV * TextureTiling);
	float3 bTex	   = tex2D(BTextureSampler,	   input.UV * TextureTiling);
	float3 wMap	   = tex2D(WeightMapSampler,   input.UV);


	float3 texColor = clamp(1.0f - wMap.r - wMap.g - wMap.b, 0 , 1);
	texColor *= baseTex;

	texColor += wMap.r * rTex + wMap.g * gTex + wMap.b * bTex;

	float3 output = Ambient * texColor;

	if(SunlightEnabled)
	{
		output += saturate(dot(normalize(SunlightDirection), normalize(input.Normal))) * SunlightDiffuse * texColor;		
	}

	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

    return float4(output,1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
