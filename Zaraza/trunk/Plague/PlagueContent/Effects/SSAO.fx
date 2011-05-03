#define NUMSAMPLES 8


/****************************************************/
/// Orientation
/****************************************************/
float4x4 Projection;
float4x4 View;
float3   CornerFrustrum;
/****************************************************/


/****************************************************/
/// SSAO Parameters
/****************************************************/
float SampleRadius;
float DistanceScale;

texture DitherTexture;
sampler DitherTextureSampler = sampler_state
{
	texture = <DitherTexture>;
};
/****************************************************/


/****************************************************/
/// GBuffer
/****************************************************/
float2  HalfPixel;

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
};
/****************************************************/


/****************************************************/
/// Samples
/****************************************************/
float4 Samples[8] =
{
	float4( 0.355512, -0.709318, -0.102371,  0.0 ),
	float4( 0.534186,  0.71511,  -0.115167,  0.0 ),
	float4(-0.87866,   0.157139, -0.115167,  0.0 ),
	float4( 0.140679, -0.475516, -0.0639818, 0.0 ),
	float4(-0.207641,  0.414286,  0.187755,  0.0 ),
	float4(-0.277332, -0.371262,  0.187755,  0.0 ),
	float4( 0.63864,  -0.114214,  0.262857,  0.0 ),
	float4(-0.184051,  0.622119,  0.262857,  0.0 )
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
	float3 ViewDirection : TEXCOORD1;
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
	output.ViewDirection = float3(-CornerFrustrum.x * input.Position.x,
								   CornerFrustrum.y * input.Position.y,
								   CornerFrustrum.z);

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 ViewDirection = normalize(input.ViewDirection);
		
	float depth = tex2D(GBufferDepthSampler, input.UV);
	
	float3 se = depth * ViewDirection;

	float3 randNormal = tex2D(DitherTextureSampler, input.UV * 200.0f).xyz;

	float3 normal = tex2D(GBufferNormalSampler,input.UV);
	normal = 2.0f * normal - 1.0f;
	normal = mul(normal,View);

	float finalColor = 0.0f;

	for(int i = 0; i < NUMSAMPLES; i++)
	{
		float3 ray = reflect(Samples[i].xyz,randNormal) * SampleRadius;

		if(dot(ray,normal) < 0) ray += normal * SampleRadius;

		float4 sample = float4(se + ray,1.0f);

		float4 ss = mul(sample,Projection);

		float2 sampleUV = 0.5f * ss.xy/ss.w + float2(0.5f,0.5f);

		float sampleDepth = tex2D(GBufferDepthSampler,sampleUV);

		if(sampleDepth == 1.0f)
		{
			finalColor += 1.0f;
		}
		else
		{
			float occlusion = DistanceScale * max(sampleDepth - depth, 0.0f);			
			finalColor += 1.0f / (1.0f + occlusion * occlusion * 0.1f);
		}
	}
	
	return float4(finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, 1.0f);
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
