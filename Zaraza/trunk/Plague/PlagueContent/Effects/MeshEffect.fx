/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
float3	 CameraPosition;
/****************************************************/


/****************************************************/
/// Sunlight
/****************************************************/
float3 Ambient;

bool   SunlightEnabled = false;
float3 SunlightDirection;
float3 SunlightDiffuse;
float3 SunlightSpecular;
/****************************************************/


/****************************************************/
/// Fog
/****************************************************/
bool FogEnabled = false;
float3 FogColor;
float2 FogRange;
/****************************************************/


/****************************************************/
/// Clip Plane
/****************************************************/
bool ClipPlaneEnabled = false;
float4 ClipPlane;
/****************************************************/


/****************************************************/
/// Diffuse Map
/****************************************************/
texture DiffuseMap;

sampler DiffuseMapSampler = sampler_state
{
	texture	= <DiffuseMap>;
};
/****************************************************/


/****************************************************/
/// Specular Map
/****************************************************/
texture SpecularMap;

sampler SpecularMapSampler = sampler_state
{
	texture	= <SpecularMap>;
};
/****************************************************/


/****************************************************/
/// Normal Map
/****************************************************/
texture NormalsMap;

sampler NormalsMapSampler = sampler_state
{
	texture	= <NormalsMap>;
};
/****************************************************/


/****************************************************/
/// VSSimpleInput
/****************************************************/
struct VSSimpleInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
    float3 Normal   : NORMAL0;    
};
/****************************************************/


/****************************************************/
/// VSComplexInput
/****************************************************/
struct VSComplexInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
    float3 Normal   : NORMAL0;    
    float3 Binormal : BINORMAL0;
    float3 Tangent  : TANGENT0;
};
/****************************************************/


/****************************************************/
/// VSSimpleOutput
/****************************************************/
struct VSSimpleOutput
{
    float4	 Position	   : POSITION0;
	float2	 UV			   : TEXCOORD0;
	float3	 WorldPosition : TEXCOORD1;
	float    Depth         : TEXCOORD3;
	float3	 Normal	       : TEXCOORD2;	
};
/****************************************************/


/****************************************************/
/// VSComplexOutput
/****************************************************/
struct VSComplexOutput
{
    float4	 Position	   : POSITION0;
	float2	 UV			   : TEXCOORD0;
	float3	 WorldPosition : TEXCOORD1;
	float    Depth         : TEXCOORD2;
	float3x3 TBN	       : TEXCOORD3;	
};
/****************************************************/


/****************************************************/
/// Vertex Shader Simple Function
/****************************************************/
VSSimpleOutput VSSimpleFunction(VSSimpleInput input)
{
    VSSimpleOutput output;

	output.UV			 = input.UV;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	output.Position		 = mul(worldPosition,ViewProjection);

	output.Normal		 = mul(input.Normal  , World);
	output.Depth		 = output.Position.z;
    
	return output;
}
/****************************************************/


/****************************************************/
/// Vertex Shader Complex Function
/****************************************************/
VSComplexOutput VSComplexFunction(VSComplexInput input)
{
    VSComplexOutput output;

	output.UV			 = input.UV;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	output.Position		 = mul(worldPosition,ViewProjection);

	output.TBN[0]		 = mul(input.Tangent , World);
	output.TBN[1]		 = mul(input.Binormal, World);
	output.TBN[2]		 = mul(input.Normal  , World);

	output.Depth		 = output.Position.z;

    return output;
}
/****************************************************/


/****************************************************/
/// ClipMe
/****************************************************/
void ClipMe(float3 worldPosition)
{
	if (ClipPlaneEnabled)
	{
		clip(dot(float4(worldPosition, 1), ClipPlane));		
	}
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Normal Function
/****************************************************/
float4 PSDNFunction(VSComplexOutput input) : COLOR0
{
	ClipMe(input.WorldPosition);
	 
	float3 diffuseTex = tex2D(DiffuseMapSampler,input.UV);
	float3 output = Ambient * diffuseTex;

	if(SunlightEnabled)
	{
		float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	 
		normal = normalize(mul(normal,input.TBN));		

		float3 lightDirection = -normalize(SunlightDirection);
	
		float NdotL = saturate(dot(normal,lightDirection));
		float3 diffuse = NdotL * SunlightDiffuse;

		output = diffuse * diffuseTex;	
	}

	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

    return float4(output, 1);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Normal Function
/****************************************************/
float4 PSDSNFunction(VSComplexOutput input) : COLOR0
{
	ClipMe(input.WorldPosition);

	float3 diffuseTex = tex2D(DiffuseMapSampler,input.UV);
	float3 output = Ambient * diffuseTex;

	if(SunlightEnabled)
	{
		float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	 
		normal = normalize(mul(normal,input.TBN));	

		float3 lightDirection = -normalize(SunlightDirection);
	
		float NdotL = saturate(dot(normal,lightDirection));
		float3 diffuse = NdotL * SunlightDiffuse;

		output += diffuse * diffuseTex;	

		if(NdotL > 0)
		{	
			float4 specularColor = tex2D(SpecularMapSampler,input.UV);

			float3 viewDirection = normalize(CameraPosition - input.WorldPosition);
	
			float3 reflectionVector = reflect(lightDirection,normal);
			float  specularValue	= dot(normalize(reflectionVector), viewDirection);
	
			specularValue = pow(specularValue, specularColor.a * 100);
		
			output += specularValue * specularColor.rgb * SunlightSpecular;

		}	
	}

	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

    return float4(output, 1);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Function
/****************************************************/
float4 PSDFunction(VSSimpleOutput input) : COLOR0
{
	ClipMe(input.WorldPosition);

	float3 diffuseTex = tex2D(DiffuseMapSampler,input.UV);
	float3 output = Ambient * diffuseTex;

	if(SunlightEnabled)
	{
		float3 normal = normalize(input.Normal);		

		float3 lightDirection = -normalize(SunlightDirection);
	
		float NdotL = saturate(dot(normal,lightDirection));
		float3 diffuse = NdotL * SunlightDiffuse;

		output += diffuse * diffuseTex;		
	}
	
	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

    return float4(output, 1);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Function
/****************************************************/
float4 PSDSFunction(VSSimpleOutput input) : COLOR0
{
	ClipMe(input.WorldPosition);

	float3 diffuseTex = tex2D(DiffuseMapSampler,input.UV);
	float3 output = Ambient * diffuseTex;

	if(SunlightEnabled)
	{
		float3 normal = normalize(input.Normal);		

		float3 lightDirection = -normalize(SunlightDirection);
	
		float NdotL = saturate(dot(normal,lightDirection));
		float3 diffuse = NdotL * SunlightDiffuse;

		output += diffuse * diffuseTex;	

		if(NdotL > 0)
		{	
			float4 specularColor = tex2D(SpecularMapSampler,input.UV);

			float3 viewDirection = normalize(CameraPosition - input.WorldPosition);
	
			float3 reflectionVector = reflect(lightDirection,normal);
			float  specularValue	= dot(normalize(reflectionVector), viewDirection);
	
			specularValue = pow(specularValue, specularColor.a * 100);
		
			output += specularValue * specularColor.rgb * SunlightSpecular;

		}	
	}

	if(FogEnabled)
	{	
		output = lerp(output,FogColor,saturate((input.Depth - FogRange.x)/(FogRange.y - FogRange.x)));
	}

    return float4(output, 1);
}
/****************************************************/


/****************************************************/
/// Diffuse-Specular-Normal Technique
/****************************************************/
technique DiffuseSpecularNormalTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VSComplexFunction();
        PixelShader  = compile ps_2_0 PSDSNFunction();
    }
}
/****************************************************/


/****************************************************/
/// Diffuse-Normal Technique
/****************************************************/
technique DiffuseNormalTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VSComplexFunction();
        PixelShader  = compile ps_2_0 PSDNFunction();
    }
}
/****************************************************/


/****************************************************/
/// Diffuse-Specular Technique
/****************************************************/
technique DiffuseSpecularTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VSSimpleFunction();
        PixelShader  = compile ps_2_0 PSDSFunction();
    }
}
/****************************************************/


/****************************************************/
/// Diffuse Technique
/****************************************************/
technique DiffuseTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VSSimpleFunction();
        PixelShader  = compile ps_2_0 PSDFunction();
    }
}
/****************************************************/