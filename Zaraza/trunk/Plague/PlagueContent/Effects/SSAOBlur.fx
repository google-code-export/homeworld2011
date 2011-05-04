/****************************************************/
/// Blur Parameters
/****************************************************/
float2 BlurDirection = float2(1,1);
/****************************************************/


/****************************************************/
/// GBuffer
/****************************************************/
float2  HalfPixel;

texture SSAOTexture;
sampler SSAOTextureSampler = sampler_state
{
	texture  = <SSAOTexture>;	
	AddressU = Mirror;
	AddressV = Mirror;    
};

texture GBufferNormal;
sampler GBufferNormalSampler = sampler_state
{
	texture   = <GBufferNormal>;	    
};

texture GBufferDepth;
sampler GBufferDepthSampler = sampler_state
{
	texture   = <GBufferDepth>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
	AddressU  = MIRROR;
	AddressV  = MIRROR;
};
/****************************************************/


/****************************************************/
/// VertexShaderInput
/****************************************************/
struct VertexShaderInput
{
	float3 Position : POSITION0;
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
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position		 = float4(input.Position, 1);
	output.UV			 = input.UV + HalfPixel;

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{		
	float color;	
		
	for(int x = -2; x <= 2; x++)
	{
		for(int y = -2; y <= 2; y++)
		{
			float2 shift = float2(x * 2 * HalfPixel.x, y * 2 * HalfPixel.y);
			float2 newUV = float2(input.UV + shift);

			float sample = tex2D(SSAOTextureSampler,newUV);				

			color += sample;;
		}
	}
	
	return color / 25.0f;
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
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
/****************************************************/
