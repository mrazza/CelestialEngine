// -----------------------------------------------------------------------
// <copyright file="MergeTargets.fx" company="">
// Copyright (C) 2011 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader renders simple ambient light over the entire viewport.
/// </summary>
float3 lightColor;
float lightIntensity;

Texture optionsMap;
sampler optionsMapSampler = sampler_state
{
    texture = <optionsMap>;
    magfilter = point;
    minfilter = point;
    mipfilter = point;
    AddressU = mirror;
    AddressV = mirror;
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
    float4 options = tex2D(optionsMapSampler, input.TexCoord);
    int flag = (int)round(options.r * 255);
    bool light = fmod(flag, 2) == 1;

    if (!light)
    {
        return float4(1, 1, 1, 0);
    }
    else
    {
        return float4(lightColor.rgb * lightIntensity, 0);
    }
}

technique AmbientLight
{
    pass MainPass
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
