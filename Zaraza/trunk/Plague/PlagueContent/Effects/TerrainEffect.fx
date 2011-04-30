/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
/****************************************************/


/****************************************************/
/// Textures
/****************************************************/
float TextureTiling;

texture BaseTexture;
sampler BaseTextureSampler = sampler_state 
{
	texture = <BaseTexture>;
};

texture RTexture;
sampler RTextureSampler = sampler_state 
{
	texture = <RTexture>;
};

texture GTexture;
sampler GTextureSampler = sampler_state 
{
	texture = <GTexture>;
};

texture BTexture;
sampler BTextureSampler = sampler_state 
{
	texture = <BTexture>;
};

texture WeightMap;
sampler WeightMapSampler = sampler_state 
{
	texture = <WeightMap>;
};
/****************************************************/


/****************************************************/
/// VertexShaderInput
/****************************************************/
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal	: NORMAL0;
	float2 UV		: TEXCOORD0;	
};
/****************************************************/


/****************************************************/
/// VertexShaderOutput
/****************************************************/
struct VertexShaderOutput
{
    float4 Position		 : POSITION0;
	float2 UV			 : TEXCOORD0;
	float4 Normal		 : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float2 Depth		 : TEXCOORD3;
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    	
	output.UV			 = input.UV;
	
	output.Normal		 = mul(input.Normal, World);

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	
	output.Position		 = mul(worldPosition,ViewProjection);
	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;

	return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderOutput
/****************************************************/
struct PixelShaderOutput
{
	float4 Color  : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth  : COLOR2;
};
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	float3 baseTex = tex2D(BaseTextureSampler, input.UV * TextureTiling);
	float3 rTex	   = tex2D(RTextureSampler,	   input.UV * TextureTiling);
	float3 gTex	   = tex2D(GTextureSampler,	   input.UV * TextureTiling);
	float3 bTex	   = tex2D(BTextureSampler,	   input.UV * TextureTiling);
	float3 wMap	   = tex2D(WeightMapSampler,   input.UV);

	float3 texColor = clamp(1.0f - wMap.r - wMap.g - wMap.b, 0 , 1);
	texColor *= baseTex;

	texColor += wMap.r * rTex + wMap.g * gTex + wMap.b * bTex;

	PixelShaderOutput output;
	
	output.Color  = float4(texColor,0);
	output.Normal = 0.5f * (-input.Normal + 1.0f);
	output.Depth  = input.Depth.x / input.Depth.y;			

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
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader  = compile ps_3_0 PixelShaderFunction();
    }
}
/****************************************************/