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
/// Clip Plane
/****************************************************/
bool ClipPlaneEnabled = false;
float4 ClipPlane;
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
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture RTexture;
sampler RTextureSampler = sampler_state 
{
	texture = <RTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture GTexture;
sampler GTextureSampler = sampler_state 
{
	texture = <GTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture BTexture;
sampler BTextureSampler = sampler_state 
{
	texture = <BTexture>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture WeightMap;
sampler WeightMapSampler = sampler_state 
{
	texture = <WeightMap>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture NormalMap;
sampler NormalMapSampler = sampler_state 
{
	texture   = <NormalMap>;
	AddressU  = WRAP;
	AddressV  = WRAP;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
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
/// VSDepthWriteOutput2
/****************************************************/
struct VSDepthWriteOuput2
{
    float4 Position		  : POSITION0;	
	float2 ScreenPosition : TEXCOORD0;
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    	
	output.UV			 = input.UV;
	
	output.Normal		 = input.Normal;
		
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
/// VSDepth Write2
/****************************************************/
VSDepthWriteOuput2 VSDepthWrite2(VSDepthWriteInput input)
{
    VSDepthWriteOuput2 output;

	float4 worldPosition	= mul(input.Position, World);
	output.Position			= mul(worldPosition,ViewProjection);
	output.ScreenPosition.x = output.Position.z;
	output.ScreenPosition.y = output.Position.w;	

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
PixelShaderOutput PixelShaderFunction2(VertexShaderOutput input)
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

	float3 normal	= normalize(tex2D(NormalMapSampler, input.UV * TextureTiling) * 2 - 1);
	
	normal		  = normalize(mul(normal,TBN));


	float3 texColor = clamp(1.0f - wMap.r - wMap.g - wMap.b, 0 , 1);
	texColor *= baseTex;

	texColor += wMap.r * rTex + wMap.g * gTex + wMap.b * bTex;

	PixelShaderOutput output;
	
	output.Color   = float4(texColor,0);
	output.Normal  = float4(0.5f * (normal + 1.0f),0);
	output.Depth   = input.Depth.x / input.Depth.y;
	
	output.SSAODepth = input.Depth.z;

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
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
/// PSDepthWrite2
/****************************************************/
float4 PSDepthWrite2(VSDepthWriteOuput2 input) : COLOR0
{		
	float depth = input.ScreenPosition.x/input.ScreenPosition.y;
	return float4(depth,0,0,1);
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
/// Technique1
/****************************************************/
technique Technique2
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader  = compile ps_3_0 PixelShaderFunction2();
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


/****************************************************/
/// Depth Write Technique 2
/****************************************************/
technique DepthWrite2
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VSDepthWrite2();
        PixelShader  = compile ps_2_0 PSDepthWrite2();
	}
}
/****************************************************/