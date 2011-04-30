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
    float4 Position : POSITION0;
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	output.Position = float4(input.Position,1);
    return output;
};
/****************************************************/


/****************************************************/
/// PixelShaderOutput
/****************************************************/
struct PixelShaderOutput
{
	float4 Color : COLOR0;
	float4 Normal: COLOR1;
	float4 Depth : COLOR2;
};
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	
	output.Color = 0.0f;
	output.Color.a = 0.0f;

	output.Normal.rgb = 0.5f;
	output.Normal.a = 0.0f;

	output.Depth = 1.0f;

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
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
/****************************************************/