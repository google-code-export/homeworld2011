/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
/****************************************************/


/****************************************************/
/// Depth
/****************************************************/
float  DepthPrecision;
float3 LightPosition;
/****************************************************/


/****************************************************/
/// Textures
/****************************************************/
float TextureTiling;

texture BaseTexture;
sampler BaseTextureSampler = sampler_state 
{
	texture = <BaseTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
};

texture RTexture;
sampler RTextureSampler = sampler_state 
{
	texture = <RTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
};

texture GTexture;
sampler GTextureSampler = sampler_state 
{
	texture = <GTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
};

texture BTexture;
sampler BTextureSampler = sampler_state 
{
	texture = <BTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
};

texture WeightMap;
sampler WeightMapSampler = sampler_state 
{
	texture = <WeightMap>;
	AddressU = WRAP;
	AddressV = WRAP;
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
/// VSDepthWriteInput
/****************************************************/
struct VSDepthWriteInput
{
    float4 Position : POSITION0;	
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
	float3 Depth		 : TEXCOORD3;
};
/****************************************************/


/****************************************************/
/// VSDepthWriteOutput
/****************************************************/
struct VSDepthWriteOuput
{
    float4 Position      : POSITION0;	
	float4 WorldPosition : TEXCOORD0;
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

	float4 viewPosition  = mul(worldPosition,View);
	
	output.Position		 = mul(worldPosition,ViewProjection);
	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;
	output.Depth.z		 = viewPosition.z;

	return output;
}
/****************************************************/


/****************************************************/
/// VSDepth Write
/****************************************************/
VSDepthWriteOuput VSDepthWrite(VSDepthWriteInput input)
{
    VSDepthWriteOuput output;
	
	float4 worldPosition = mul(input.Position, World);
	output.Position		 = mul(worldPosition,ViewProjection);
	output.WorldPosition = worldPosition;

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
	float3 baseTex = tex2D(BaseTextureSampler, input.UV * TextureTiling);
	float3 rTex	   = tex2D(RTextureSampler,	   input.UV * TextureTiling);
	float3 gTex	   = tex2D(GTextureSampler,	   input.UV * TextureTiling);
	float3 bTex	   = tex2D(BTextureSampler,	   input.UV * TextureTiling);
	float3 wMap	   = tex2D(WeightMapSampler,   input.UV);

	float3 texColor = clamp(1.0f - wMap.r - wMap.g - wMap.b, 0 , 1);
	texColor *= baseTex;

	texColor += wMap.r * rTex + wMap.g * gTex + wMap.b * bTex;

	PixelShaderOutput output;
	
	output.Color   = float4(texColor,0);
	output.Normal  = 0.5f * (-input.Normal + 1.0f);
	output.Depth   = input.Depth.x / input.Depth.y;
	
	output.SSAODepth = input.Depth.z;

    return output;
}
/****************************************************/


/****************************************************/
/// PSDepthWrite
/****************************************************/
float4 PSDepthWrite(VSDepthWriteOuput input) : COLOR0
{		
	input.WorldPosition /= input.WorldPosition.w;

	float depth = max(0.01f, length(LightPosition - input.WorldPosition)) / DepthPrecision;

	return float4(depth,depth * depth,0,1);
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


/****************************************************/
/// Depth Write Technique
/****************************************************/
technique DepthWrite
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VSDepthWrite();
        PixelShader  = compile ps_3_0 PSDepthWrite();
	}
}
/****************************************************/