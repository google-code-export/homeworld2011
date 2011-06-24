float2 HalfPixel;

texture Texture;

sampler TextureSampler = sampler_state
{
	texture	= <Texture>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

float BloomThreshold;

struct VertexShaderInput
{
    float3 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = float4(input.Position,1);
	output.UV = input.UV + HalfPixel;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

    float4 c = tex2D(TextureSampler, input.UV);
    
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}