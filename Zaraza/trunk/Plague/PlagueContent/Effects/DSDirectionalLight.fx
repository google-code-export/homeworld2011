/****************************************************/
/// Orientation
/****************************************************/
float4x4 InverseViewProjection;
float4x4 LightViewProjection;
float3	 CameraPosition;
/****************************************************/


/****************************************************/
/// Light 
/****************************************************/
float3 LightDirection;
float3 LightColor;
float  LightIntensity;
/****************************************************/


/****************************************************/
/// Shadows
/****************************************************/
float DepthBias = 0.00001f;

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
	texture = <ShadowMap>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
/****************************************************/


/****************************************************/
/// G-Buffer
/****************************************************/
float2  HalfPixel;

texture GBufferNormal;
sampler GBufferNormalSampler = sampler_state
{
	texture = <GBufferNormal>;
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
/// Phong
/****************************************************/
float4 Phong(float3 Position, float3 N, float SpecularPower)
{

	float3 Diffuse = 0;
	float Specular = 0;

	float NL = dot(N,-normalize(LightDirection));
	
	if(NL > 0)
	{
		Diffuse = NL * LightColor;

		if(SpecularPower > 0)
		{		
			float3 R  = normalize(reflect(LightDirection,N));
			float3 E = normalize(CameraPosition - Position.xyz);
			Specular = pow(saturate(dot(R,E)), SpecularPower * 100);
		}	
	}

	return float4(Diffuse,Specular);
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 NormalData = tex2D(GBufferNormalSampler, input.UV);
	
	float3 Normal = 2.0f * NormalData.xyz - 1.0f;

	float Depth = tex2D(GBufferDepthSampler, input.UV);

	float4 Position = 1.0f;		

	Position.x = input.UV.x * 2.0f - 1.0f;
	Position.y = -(input.UV.y * 2.0f - 1.0f);
	Position.z = Depth;
	
	Position = mul(Position,InverseViewProjection);
	Position /= Position.w;
	
	float4 LightScreenPos = mul(Position, LightViewProjection);
	LightScreenPos /= LightScreenPos.w;

	float2 LightUV = 0.5f * (float2(LightScreenPos.x,-LightScreenPos.y) + 1.0f);
	
	float ShadowFactor = 1;	
	
	float realDepth = LightScreenPos.z - DepthBias;

	float lightDepth = tex2D(ShadowMapSampler, LightUV).r;
			
	if(lightDepth < realDepth) ShadowFactor = 0;	

	float4 output = Phong(Position.xyz,Normal,NormalData.w);

	output *= LightIntensity * ShadowFactor;   	

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
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
/****************************************************/