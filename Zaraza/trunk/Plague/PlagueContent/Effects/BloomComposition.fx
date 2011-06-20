float2 HalfPixel;

texture Texture;
sampler TextureSampler = sampler_state
{
	texture	= <Texture>;
	MagFilter = Linear;
    MinFilter = Linear;
    Mipfilter = Linear;
};

texture Bloom;
sampler BloomSampler = sampler_state
{
	texture	= <Bloom>;
	MagFilter = Linear;
    MinFilter = Linear;
    Mipfilter = Linear;
};

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;

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

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, input.UV);
    float4 base = tex2D(TextureSampler, input.UV);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
    return base + bloom;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}