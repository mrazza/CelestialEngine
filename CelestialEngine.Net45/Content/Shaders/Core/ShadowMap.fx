// -----------------------------------------------------------------------
// <copyright file="ShadowMap.fx" company="">
// Copyright (C) 2013 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader renders a shadow map used when rendering lights
/// </summary>
float4x4 viewProjection;
float2 cameraPosition;
float layerDepth;

struct VertexShaderInput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct GenericVertexShaderOutput
{
    float4 Position : SV_Position;
};

// This is the generic vertex shader for use by any pass
GenericVertexShaderOutput GenericVertexShader(VertexShaderInput input)
{
    GenericVertexShaderOutput output;

    float3 pos = mul(input.Position.xy - cameraPosition, float2x4(viewProjection[0], viewProjection[1])).xyz;
    output.Position = float4(pos, 1);

    return output;
}

// Pixel shader that will mask the rendered area as in shadow
float4 PSMaskShadow(GenericVertexShaderOutput input) : COLOR0
{
    return float4(0.0f, 1.0f, layerDepth, 1.0f);
}

// Pixel shader that will unmask the rendered area so as to not be in shadow
float4 PSUnmaskShadow(GenericVertexShaderOutput input) : COLOR0
{
    return float4(1.0f, 0.0f, layerDepth, 1.0f);
}

technique ShadowMap
{
    pass ShadowMask
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_4_0_level_9_1 GenericVertexShader();
        PixelShader = compile ps_4_0_level_9_1 PSMaskShadow();
    }

    pass ShadowUnmask
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_4_0_level_9_1 GenericVertexShader();
        PixelShader = compile ps_4_0_level_9_1 PSUnmaskShadow();
    }
}

Texture shadowMap;
sampler shadowMapSampler = sampler_state
{
    texture = <shadowMap>;
    AddressU = clamp;
    AddressV = clamp;
    MinFilter = linear;
    MagFilter = linear;
};

float2 lightPosition;
float maxBlurDistance;
float minBlurDistance;

struct TextureVertexShaderOutput
{
    float4 Position : SV_Position;
    float2 Tap : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
};

// This is the vertex shader that also calculates texel coords
TextureVertexShaderOutput BoxBlurTextureVertexShader(VertexShaderInput input)
{
    TextureVertexShaderOutput output;

    float2 direction = float2(-0.707, 0.707);
    float3 pos = mul(input.Position.xy - cameraPosition, float2x4(viewProjection[0], viewProjection[1])).xyz;
    output.Position = float4(pos, 1);
    output.WorldPos = input.Position.xyz;
    output.Tap = float2((pos.x + 1) / 2.0f, (-pos.y + 1) / 2.0f);

    return output;
}

// This pixel shader performs a gaussian blur
float4 PSGaussianBlur(TextureVertexShaderOutput input) : COLOR0
{
    float3 origShadow = tex2D(shadowMapSampler, input.Tap).rgb;

    if (origShadow.g == 0.0f)
    {
        return float4(origShadow, 1.0f);
    }

    float2 lightDistance = sqrt(saturate((distance(input.WorldPos.xy, lightPosition) - minBlurDistance) / maxBlurDistance));
    float orig = origShadow.r;

    // Inner Taps
    float2 direction = float2(-0.707, 0.707);
    float color = tex2D(shadowMapSampler, input.Tap + 0.003f * direction).r * .15f;
    color += tex2D(shadowMapSampler, input.Tap - 0.003f * direction).r * .075f;
    color += tex2D(shadowMapSampler, input.Tap + 0.006f * direction).r * .025f;
    color += tex2D(shadowMapSampler, input.Tap - 0.006f * direction).r * .15f;
    color += tex2D(shadowMapSampler, input.Tap + 0.012f * direction).r * .075f;
    color += tex2D(shadowMapSampler, input.Tap - 0.012f * direction).r * .025f;
    
    direction = float2(0.707, 0.707);
    color += tex2D(shadowMapSampler, input.Tap + 0.003f * direction).r * .15f;
    color += tex2D(shadowMapSampler, input.Tap - 0.003f * direction).r * .075f;
    color += tex2D(shadowMapSampler, input.Tap + 0.006f * direction).r * .025f;
    color += tex2D(shadowMapSampler, input.Tap - 0.006f * direction).r * .15f;
    color += tex2D(shadowMapSampler, input.Tap + 0.012f * direction).r * .075f;
    color += tex2D(shadowMapSampler, input.Tap - 0.012f * direction).r * .025f;

    color = min(color * lightDistance + (1.0f - lightDistance) * orig, orig).r; // Blend result based on distance

    return float4(color, 1.0f, origShadow.b, 1);
}

technique SoftenShadowMap
{
    pass BoxBlur
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_4_0_level_9_1 BoxBlurTextureVertexShader();
        PixelShader = compile ps_4_0_level_9_1 PSGaussianBlur();
    }
}