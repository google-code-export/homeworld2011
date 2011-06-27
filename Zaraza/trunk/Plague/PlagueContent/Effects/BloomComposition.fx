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


/****************************************************/
/// Fog of War
/****************************************************/
bool   FogOfWarEnabled = false;
float2 FogOfWarSize;
float4x4 InverseViewProjection;
float3 FogColor;

texture FogOfWar;
sampler FogOfWarSampler = sampler_state
{
	texture = <FogOfWar>;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture FogOfWar2;
sampler FogOfWar2Sampler = sampler_state
{
	texture = <FogOfWar2>;
	MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
/****************************************************/
texture GBufferDepth;
sampler GBufferDepthSampler = sampler_state
{
	texture	  = <GBufferDepth>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

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

	float3 output = base + bloom;
	if(FogOfWarEnabled)
	{
		float Depth = tex2D(GBufferDepthSampler,input.UV);
		float4 Position = 1.0f;		
		Position.x = input.UV.x * 2.0f - 1.0f;
		Position.y = -(input.UV.y * 2.0f - 1.0f);
		Position.z = Depth;
	
		Position = mul(Position,InverseViewProjection);
		Position /= Position.w;

		float fogSample  = tex2D(FogOfWarSampler,Position.xz / FogOfWarSize).r;
		float fog2Sample = tex2D(FogOfWar2Sampler,Position.xz / FogOfWarSize).r;
		
		output = lerp(FogColor,output,saturate(fogSample + 0.5f * fog2Sample));		
	}

    return float4(output,1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}