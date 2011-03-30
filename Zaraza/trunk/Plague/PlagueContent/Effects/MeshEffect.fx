float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
float3	 CameraPosition;

float3 SunLightDirection;
float3 SunLightAmbient;
float3 SunLightDiffuse;
float3 SunLightSpecular;

bool ClipPlaneEnabled = false;
float4 ClipPlane;

texture DiffuseMap;
texture SpecularMap;
texture NormalsMap;

sampler DiffuseMapSampler = sampler_state
{
	texture	= <DiffuseMap>;
};

sampler SpecularMapSampler = sampler_state
{
	texture	= <SpecularMap>;
};

sampler NormalsMapSampler = sampler_state
{
	texture	= <NormalsMap>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
    float3 Normal   : NORMAL0;    
    float3 Binormal : BINORMAL0;
    float3 Tangent  : TANGENT0;
};

struct VertexShaderOutput
{
    float4	 Position	   : POSITION0;
	float2	 UV			   : TEXCOORD0;
	float3	 WorldPosition : TEXCOORD1;
	float3x3 TBN	       : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.UV			 = input.UV;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	output.Position		 = mul(worldPosition,ViewProjection);

	output.TBN[0]		 = mul(input.Tangent , World);
	output.TBN[1]		 = mul(input.Binormal, World);
	output.TBN[2]		 = mul(input.Normal  , World);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}

	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	 
	normal = normalize(mul(normal,input.TBN));	

	float3 lightDirection = -normalize(SunLightDirection);
	
	float NdotL = saturate(dot(normal,lightDirection));
	float3 diffuse = NdotL * SunLightDiffuse;

	float3 output = (SunLightAmbient + diffuse) * tex2D(DiffuseMapSampler,input.UV);	

	if(NdotL > 0)
	{	
		float4 specularColor = tex2D(SpecularMapSampler,input.UV);

		float3 viewDirection = normalize(CameraPosition - input.WorldPosition);
	
		float3 reflectionVector = reflect(lightDirection,normal);
		float  specularValue	= dot(normalize(reflectionVector), viewDirection);
	
		specularValue = pow(specularValue, specularColor.a * 100);
		
		output += specularValue * specularColor.rgb * SunLightSpecular;

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
