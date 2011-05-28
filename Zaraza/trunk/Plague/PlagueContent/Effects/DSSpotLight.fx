/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 ViewProjection;
float4x4 InverseViewProjection;
float3	 CameraPosition;
/****************************************************/


/****************************************************/
/// Light
/****************************************************/
float3	 LightColor;
float3	 LightPosition;
float3	 LightDirection;
float	 LightIntensity;
float	 LightRadius;
float	 LightAngleCos;
float	 LightFarPlane;
float	 LinearAttenuation;
float    QuadraticAttenuation;
float4x4 LightViewProjection;

texture AttenuationTexture;
sampler AttenuationTextureSampler = sampler_state
{
	texture = <AttenuationTexture>;
};
/****************************************************/


/****************************************************/
/// Shadows
/****************************************************/
bool   ShadowsEnabled;
float  DepthPrecision;
float  DepthBias;
float2 ShadowMapOffset;

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
	texture   = <ShadowMap>;
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
/// VertexShaderInput
/****************************************************/
struct VertexShaderInput
{
    float3 Position : POSITION0;
};
/****************************************************/


/****************************************************/
/// VertexShaderOutput
/****************************************************/
struct VertexShaderOutput
{
    float4 Position		  : POSITION0;
	float4 ScreenPosition : TEXCOORD0;
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(float4(input.Position,1), World);
    
	output.Position		  = mul(worldPosition, ViewProjection);
	output.ScreenPosition = output.Position;

    return output;
}
/****************************************************/


/****************************************************/
/// Phong
/****************************************************/
float4 Phong(float3 Position, float3 N, float radialAttenuation, float SpecularPower)
{
	float3 L = LightPosition - Position;
	
	float distance = saturate(1.0f - length(L)/LightFarPlane);
	float attenuation = (LinearAttenuation * distance) + (QuadraticAttenuation * distance * distance);
	attenuation = min(attenuation,radialAttenuation);
	
	L = normalize(L);

	float3 Diffuse = 0;
	float Specular = 0;

	float DL = dot(normalize(LightDirection),-L);
	
	if(DL > LightAngleCos)
	{
		float NL = saturate(dot(N,L));

		Diffuse = NL * LightColor;

		if(SpecularPower > 0)
		{		
			float3 R = normalize(reflect(-L,N));
			float3 E = normalize(CameraPosition - Position.xyz);
			Specular = pow(saturate(dot(R,E)), SpecularPower * 100);
		}	
	}

	float4 result = float4(Diffuse,Specular);
	result *= attenuation * LightIntensity;	

	return result;
}
/****************************************************/


/****************************************************/
/// Lambert
/****************************************************/
float4 Lambert(float3 Position, float3 N, float radialAttenuation)
{
	float3 L = LightPosition - Position;
	
	float distance = saturate(1.0f - length(L)/LightFarPlane);
	float attenuation = (LinearAttenuation * distance) + (QuadraticAttenuation * distance * distance);
	attenuation = min(attenuation,radialAttenuation);
	
	L = normalize(L);

	float3 Diffuse = 0;
	float Specular = 0;

	float DL = dot(normalize(LightDirection),-L);
	
	if(DL > LightAngleCos)
	{
		float NL = saturate(dot(N,L));

		Diffuse = NL * LightColor;		
	}

	float4 result = float4(Diffuse,0);
	result *= attenuation * LightIntensity;	

	return result;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	input.ScreenPosition.xy /= input.ScreenPosition.w;
	float2 UV = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1.0f);
	UV += HalfPixel;

	float4 NormalData = tex2D(GBufferNormalSampler,UV);
	float3 Normal = 2.0f * NormalData.xyz - 1.0f;

	float Depth = tex2D(GBufferDepthSampler,UV);

	float4 Position = 1.0f;
	
	Position.xy = input.ScreenPosition.xy;
	Position.z  = Depth;
	
	Position = mul(Position,InverseViewProjection);
	Position /= Position.w;

	float4 LightScreenPos = mul(Position, LightViewProjection);
	LightScreenPos /= LightScreenPos.w;
	
	float2 LightUV = 0.5f * (float2(LightScreenPos.x,-LightScreenPos.y) + 1.0f);

	float Attenuation = tex2D(AttenuationTextureSampler, LightUV).r;		

	LightUV /= 4.0f;

	LightUV.x += ShadowMapOffset.x * 0.25f;
	LightUV.y += ShadowMapOffset.y * 0.25f;

	float ShadowFactor = 1;	

	if(ShadowsEnabled)
	{			
		half realDepth = (length(LightPosition - Position) / DepthPrecision) - DepthBias;
				
		if (realDepth < 1)
		{
			float2 moments = tex2D(ShadowMapSampler, LightUV);
		
			half lit_factor = (realDepth <= moments.x);
		
			half E_x2 = moments.y;
			half Ex_2 = moments.x * moments.x;

			half variance = min(max(E_x2 - Ex_2, 0.0) + 1.0f / 10000.0f, 1.0);

			half m_d = (moments.x - realDepth);

			half p = variance / (variance + m_d * m_d);
		
			ShadowFactor = saturate(max(lit_factor, p));
		}
	}

	float3 L = LightPosition - Position.xyz;
	
    return ShadowFactor * Phong(Position.xyz,Normal,Attenuation,NormalData.w);
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction2
/****************************************************/
float4 PixelShaderFunction2(VertexShaderOutput input) : COLOR0
{
	input.ScreenPosition.xy /= input.ScreenPosition.w;
	float2 UV = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1.0f);
	UV -= HalfPixel;

	float4 NormalData = tex2D(GBufferNormalSampler,UV);
	float3 Normal = 2.0f * NormalData.xyz - 1.0f;

	float Depth = tex2D(GBufferDepthSampler,UV);

	float4 Position = 1.0f;
	
	Position.xy = input.ScreenPosition.xy;
	Position.z  = Depth;
	
	Position = mul(Position,InverseViewProjection);
	Position /= Position.w;

	float4 LightScreenPos = mul(Position, LightViewProjection);
	LightScreenPos /= LightScreenPos.w;
	
	float2 LightUV = 0.5f * (float2(LightScreenPos.x,-LightScreenPos.y) + 1.0f);

	float Attenuation = tex2D(AttenuationTextureSampler, LightUV).r;		

	float ShadowFactor = 1;	

	LightUV /= 4.0f;

	LightUV.x += ShadowMapOffset.x * 0.25f;
	LightUV.y += ShadowMapOffset.y * 0.25f;

	if(ShadowsEnabled)
	{			
		half realDepth = (length(LightPosition - Position) / DepthPrecision) - DepthBias;
				
		if (realDepth < 1)
		{
			float2 moments = tex2D(ShadowMapSampler, LightUV);
		
			half lit_factor = (realDepth <= moments.x);
		
			half E_x2 = moments.y;
			half Ex_2 = moments.x * moments.x;

			half variance = min(max(E_x2 - Ex_2, 0.0) + 1.0f / 10000.0f, 1.0);

			half m_d = (moments.x - realDepth);

			half p = variance / (variance + m_d * m_d);
		
			ShadowFactor = saturate(max(lit_factor, p));
		}
	}

    return ShadowFactor * Lambert(Position.xyz,Normal,Attenuation);
}
/****************************************************/


/****************************************************/
/// Phong
/****************************************************/
technique PhongTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
/****************************************************/


/****************************************************/
/// Lambert
/****************************************************/
technique LambertTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction2();
    }
}
/****************************************************/