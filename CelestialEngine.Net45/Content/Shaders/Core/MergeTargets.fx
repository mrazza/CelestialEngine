// -----------------------------------------------------------------------
// <copyright file="MergeTargets.fx" company="">
// Copyright (C) 2011 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader is used to combine the render targets into the final image.
/// </summary>
texture colorMap;
texture lightMap;

sampler colorSampler = sampler_state
{
    Texture = (colorMap);
    AddressU = clamp;
    AddressV = clamp;
    magfilter = point;
    minfilter = point;
    mipfilter = point;
};

sampler lightSampler = sampler_state
{
    Texture = (lightMap);
    AddressU = clamp;
    AddressV = clamp;
    magfilter = point;
    minfilter = point;
    mipfilter = point;
};

struct VertexShaderInput
{
    float3 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 diffuseColor = tex2D(colorSampler, input.TexCoord).rgb;
    float3 light = tex2D(lightSampler, input.TexCoord).rgb;
    return float4(diffuseColor * light, 1);
}

technique MergeRenderTargets
{
    pass MainPass
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
