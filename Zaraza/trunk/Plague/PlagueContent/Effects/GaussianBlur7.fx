/****************************************************/
/// Texture
/****************************************************/
float2 TextureSize;

texture Texture;

sampler2D TextureSampler = sampler_state 
{
	texture = <Texture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};
/****************************************************/

float offsets[7] = { 0,  0.0007324219,  -0.0007324219,  0.00170898438,  -0.00170898438, 0.00268554688, -0.00268554688 };
float weights[7] = {0.3990503, 0.242036223, 0.242036223, 0.0540055856, 0.0540055856 ,0.00443304842,0.00443304842};

/****************************************************/
// Precalculated weights 
/****************************************************
float weights[15] = { 0.1061154,  0.1028506,  0.1028506,  0.09364651,  0.09364651,
					  0.0801001,  0.0801001,  0.06436224, 0.06436224,  0.04858317,
					  0.04858317, 0.03445063, 0.03445063, 0.02294906,  0.02294906 };
/****************************************************/


/****************************************************/
/// Precalculated Offsets
/****************************************************
float offsets[15] = { 0,				0.00125, -0.00125, 0.002916667, -0.002916667, 
					  0.004583334, -0.004583334,  0.00625,	  -0.00625,  0.007916667, 
					 -0.007916667,  0.009583334, -0.009583334, 0.01125,     -0.01125 };
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
	output.UV			 = input.UV; //+ (1 / TextureSize);

    return output;
}
/****************************************************/


/****************************************************/
/// BlurHorizontal
/****************************************************/
float4 BlurHorizontal(float4 Position : POSITION0, float2 UV : TEXCOORD0) : COLOR0
{
	float4 output = float4(0, 0, 0, 1);
	
	for (int i = 0; i < 7; i++)
	{
		output += tex2D(TextureSampler, UV + float2(offsets[i], 0)) * weights[i];
	}

	return output;
}
/****************************************************/


/****************************************************/
/// BlurVertical
/****************************************************/
float4 BlurVertical(float4 Position : POSITION0, float2 UV : TEXCOORD0) : COLOR0
{
	float4 output = float4(0, 0, 0, 1);
	
	for (int i = 0; i < 7; i++)
	{
		output += tex2D(TextureSampler, UV + float2(0, offsets[i])) * weights[i];
	}

	return output;
}
/****************************************************/


/****************************************************/
/// Technique1
/****************************************************/
technique Technique1
{
	pass Horizontal
	{
        VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 BlurHorizontal();
	}
	
	pass Vertical
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 BlurVertical();
	}
}
/****************************************************/