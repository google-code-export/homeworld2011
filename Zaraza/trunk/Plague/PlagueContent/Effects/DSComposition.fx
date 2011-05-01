/****************************************************/
/// Composition
/****************************************************/
float2  HalfPixel;

texture GBufferColor;
sampler GBufferColorSampler = sampler_state
{
	texture = <GBufferColor>;
};

texture GBufferDepth;
sampler GBufferDepthSampler = sampler_state
{
	texture	  = <GBufferDepth>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

texture LightMap;
sampler LightMapSampler = sampler_state
{
	texture = <LightMap>;
};

texture SSAOTexture;
sampler SSAOSampler = sampler_state
{
	texture = <SSAOTexture>;
};
/****************************************************/


/****************************************************/
/// Ambient
/****************************************************/
float3 Ambient;
/****************************************************/


/****************************************************/
/// Fog
/****************************************************/
bool FogEnabled = false;
float3 FogColor;
float2 FogRange;
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
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0; 
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = float4(input.Position, 1);
	output.UV		= input.UV + HalfPixel;

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 Color = tex2D(GBufferColorSampler,input.UV);
	float4 Light = tex2D(LightMapSampler,input.UV);
	float  SSAO  = tex2D(SSAOSampler,input.UV);

	float3 output = Color.xyz * (Ambient + Light.w + Color.w);
	output += Color.xyz * Light.xyz;	
	output *= SSAO;
    
	if(FogEnabled)
	{	
		float Depth = tex2D(GBufferDepthSampler,input.UV);
		output = lerp(output,FogColor,saturate((Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

	return float4(output,1);
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
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
/****************************************************/