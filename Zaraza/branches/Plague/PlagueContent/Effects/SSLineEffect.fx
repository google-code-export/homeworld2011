/****************************************************/
/// VertexShaderInput
/****************************************************/
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Color	: COLOR0;
};
/****************************************************/


/****************************************************/
/// VertexShaderOutput
/****************************************************/
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Color	: COLOR0;
};
/****************************************************/


/****************************************************/
/// VertexShaderFunction
/****************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;


	output.Position = float4(2.0f * input.Position.x - 1.0f,
							 2.0f *	input.Position.y - 1.0f,
									0,1);

	output.Color	= input.Color;

    return output;
}
/****************************************************/


/****************************************************/
/// PixelShaderFunction
/****************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(input.Color,1);
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
        PixelShader  = compile ps_2_0 PixelShaderFunction();
    }
}
/****************************************************/
