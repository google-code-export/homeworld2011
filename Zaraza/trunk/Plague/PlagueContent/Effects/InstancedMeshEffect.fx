/****************************************************/
/// Orientation
/****************************************************/
float4x4 View;
float4x4 Projection;
float4x4 ViewProjection;
float3	 CameraPosition;
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
float DepthPrecision;
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
/// VSDepthWriteInput
/****************************************************/
struct VSDepthWriteInput
{
    float4 Position : POSITION0;	
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
/// VSDepth Write
/****************************************************/
VSDepthWriteOuput VSDepthWrite(VSDepthWriteInput input, float4x4 instanceTransform : BLENDWEIGHT)
{
    VSDepthWriteOuput output;

	float4x4 world		 = transpose(instanceTransform);
	float4 worldPosition = mul(input.Position, world);
	output.Position		 = mul(worldPosition,ViewProjection);
	output.WorldPosition = worldPosition;

    return output;
}
/****************************************************/


/****************************************************/
/// VSDepth Write2
/****************************************************/
VSDepthWriteOuput2 VSDepthWrite2(VSDepthWriteInput input, float4x4 instanceTransform : BLENDWEIGHT)
{
    VSDepthWriteOuput2 output;

	float4x4 world			= transpose(instanceTransform);
	float4 worldPosition	= mul(input.Position, world);
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
		if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);	
	
	float3 normal = normalize(tex2D(NormalsMapSampler,input.UV) * 2.0 - 1.0);
	normal = normalize(mul(normal,input.TBN));					

    return PixelColor(texColor,normal,input.Depth,0,0);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Normal Function
/****************************************************/
PixelShaderOutput PSDSNFunction(VSComplexOutput input)
{
		if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}

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
PixelShaderOutput PSDFunction(VSSimpleOutput input)
{
		if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);
	
	float3 normal = normalize(input.Normal);		

    return PixelColor(texColor,normal,input.Depth,0,0);
}
/****************************************************/


/****************************************************/
/// Pixel Shader Diffuse Specular Function
/****************************************************/
PixelShaderOutput PSDSFunction(VSSimpleOutput input)
{
		if (ClipPlaneEnabled)
	{
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));		
	}

	float3 texColor = tex2D(DiffuseMapSampler,input.UV);

	float3 normal = normalize(input.Normal);		
	
	float2 specular = tex2D(SpecularMapSampler,input.UV);
	
    return PixelColor(texColor,normal,input.Depth,specular.x,specular.y);
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
/// PSDepthWrite
/****************************************************/
float4 PSDepthWrite(VSDepthWriteOuput input) : COLOR0
{		
	input.WorldPosition /= input.WorldPosition.w;

	float depth = max(0.01f, length(CameraPosition - input.WorldPosition)) / DepthPrecision;

	return float4(depth,depth * depth,0,1);
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