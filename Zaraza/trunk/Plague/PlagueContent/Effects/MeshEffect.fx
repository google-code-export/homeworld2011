/****************************************************/
/// Orientation
/****************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
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
	float2   Depth         : TEXCOORD3;
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
	float2   Depth         : TEXCOORD2;
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

	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;
    
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
/// Pixel Shader Diffuse Normal Function
/****************************************************/
PixelShaderOutput PSDNFunction(VSComplexOutput input)
{
	PixelShaderOutput output;

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);
	
	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	
	normal = normalize(mul(normal,input.TBN));				

	output.Color  = float4(texColor,0);
	output.Normal = float4(0.5f * (normal + 1.0f),0.0f);
	output.Depth  = input.Depth.x / input.Depth.y;
	
    return output;
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Normal Function
/****************************************************/
PixelShaderOutput PSDSNFunction(VSComplexOutput input) : COLOR0
{
	PixelShaderOutput output;

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);

	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);

	normal = normalize(mul(normal,input.TBN));	

	float2 specular = tex2D(SpecularMapSampler,input.UV);

	output.Color  = float4(texColor,specular.g);
	output.Normal = float4(0.5f * (normal + 1.0f),specular.r);
	output.Depth  = input.Depth.x / input.Depth.y;
		
	return output;
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Function
/****************************************************/
PixelShaderOutput PSDFunction(VSSimpleOutput input) : COLOR0
{
	PixelShaderOutput output;

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);
	
	float3 normal = normalize(input.Normal);		

	output.Color  = float4(texColor,0);
	output.Normal = float4(0.5f * (normal + 1.0f),0.0f);
	output.Depth  = input.Depth.x / input.Depth.y;
	
	return output;
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Function
/****************************************************/
PixelShaderOutput PSDSFunction(VSSimpleOutput input) : COLOR0
{
	PixelShaderOutput output;

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);

	float3 normal = normalize(input.Normal);		
	
	float2 specular = tex2D(SpecularMapSampler,input.UV);
	
	output.Color  = float4(texColor,specular.g);
	output.Normal = float4(0.5f * (normal + 1.0f),specular.r);
	output.Depth  = input.Depth.x / input.Depth.y;
		
	return output;
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