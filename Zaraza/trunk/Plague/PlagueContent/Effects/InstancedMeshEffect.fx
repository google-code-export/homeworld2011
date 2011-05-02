/****************************************************/
/// Orientation
/****************************************************/
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
float3	 CameraPosition;
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
	float3   Depth         : TEXCOORD2;
	float3	 Normal	       : TEXCOORD3;	
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
	float3   Depth         : TEXCOORD2;
	float3x3 TBN	       : TEXCOORD3;	
};
/****************************************************/


/****************************************************/
/// Vertex Shader Simple Function
/****************************************************/
VSSimpleOutput VSSimpleFunction(VSSimpleInput input, float4x4 instanceTransform : BLENDWEIGHT)
{
    VSSimpleOutput output;

	output.UV			 = input.UV;

	float4x4 world		 = transpose(instanceTransform);
	float4 worldPosition = mul(input.Position, world);
	output.WorldPosition = worldPosition;
	output.Position		 = mul(worldPosition,ViewProjection);

	output.Normal		 = mul(input.Normal  , world);

	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;
	output.Depth.z		 = mul(worldPosition,View).z;

    return output;
}
/****************************************************/


/****************************************************/
/// Vertex Shader Complex Function
/****************************************************/
VSComplexOutput VSComplexFunction(VSComplexInput input, float4x4 instanceTransform : BLENDWEIGHT)
{
    VSComplexOutput output;

	output.UV			 = input.UV;

	float4x4 world		 = transpose(instanceTransform);
	float4 worldPosition = mul(input.Position, world);
	output.WorldPosition = worldPosition;
	output.Position		 = mul(worldPosition,ViewProjection);

	output.TBN[0]		 = mul(input.Tangent , world);
	output.TBN[1]		 = mul(input.Binormal, world);
	output.TBN[2]		 = mul(input.Normal  , world);

	output.Depth.x		 = output.Position.z;
	output.Depth.y		 = output.Position.w;
	output.Depth.z		 = mul(worldPosition,View).z;

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
/// Pixel Color
/****************************************************/
PixelShaderOutput PixelColor(float3 texColor,float3 normal,float3 depth,float specular, float selfIllumination)
{
	PixelShaderOutput output;

	output.Color	 = float4(texColor,selfIllumination);
	output.Normal	 = float4(0.5f * (normal + 1.0f),specular);
	output.Depth	 = depth.x / depth.y;	
	output.SSAODepth = depth.z;		
	
	return output;
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Normal Function
/****************************************************/
PixelShaderOutput PSDNFunction(VSComplexOutput input)
{
	float3 texColor = tex2D(DiffuseMapSampler,input.UV);	
	
	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	normal = normalize(mul(normal,input.TBN));					

    return PixelColor(texColor,normal,input.Depth,0,0);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Normal Function
/****************************************************/
PixelShaderOutput PSDSNFunction(VSComplexOutput input) : COLOR0
{
	float3 texColor = tex2D(DiffuseMapSampler,input.UV);

	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	normal = normalize(mul(normal,input.TBN));	

	float2 specular = tex2D(SpecularMapSampler,input.UV);

    return PixelColor(texColor,normal,input.Depth,specular.x,specular.y);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Function
/****************************************************/
PixelShaderOutput PSDFunction(VSSimpleOutput input) : COLOR0
{
	float3 texColor = tex2D(DiffuseMapSampler,input.UV);
	
	float3 normal = normalize(input.Normal);		

    return PixelColor(texColor,normal,input.Depth,0,0);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Function
/****************************************************/
PixelShaderOutput PSDSFunction(VSSimpleOutput input) : COLOR0
{
	float3 texColor = tex2D(DiffuseMapSampler,input.UV);

	float3 normal = normalize(input.Normal);		
	
	float2 specular = tex2D(SpecularMapSampler,input.UV);
	
    return PixelColor(texColor,normal,input.Depth,specular.x,specular.y);
}
/****************************************************/


/****************************************************/
/// Diffuse-Specular-Normal Technique
/****************************************************/
technique DiffuseSpecularNormalTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VSComplexFunction();
        PixelShader  = compile ps_3_0 PSDSNFunction();
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
        VertexShader = compile vs_3_0 VSComplexFunction();
        PixelShader  = compile ps_3_0 PSDNFunction();
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
        VertexShader = compile vs_3_0 VSSimpleFunction();
        PixelShader  = compile ps_3_0 PSDSFunction();
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
        VertexShader = compile vs_3_0 VSSimpleFunction();
        PixelShader  = compile ps_3_0 PSDFunction();
    }
}
/****************************************************/