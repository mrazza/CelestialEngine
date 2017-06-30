// -----------------------------------------------------------------------
// <copyright file="OptionsMapFlags.fx" company="">
// Copyright (C) 2012 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader renders the options map.
/// </summary>
/// <remarks>
/// The bits in the options map are distributed as follows:
/// Bit Number     |    1    2    3    4    5    6    7    8
/// ------------------------------------------------------------
/// RED Channel    | [---------Sprite Render Options----------]    Bits 1-8
/// GREEN Channel  | [------------Specular Reflect------------]    Bits 1-8
/// BLUE Channel   | [---------------Layer Depth--------------]    Bits 1-8
/// ALPHA Channel  |               The Alpha Channel
/// </remarks>
float pixelOptions;
float specularReflectivity;
float4x4 viewProjection;
float2 cameraPosition;
float layerDepth;

texture spriteTexture;
sampler spriteTextureSample = sampler_state
{
    Texture = (spriteTexture);
    magfilter = point;
    minfilter = point;
    mipfilter = point;
    AddressU = clamp;
    AddressV = clamp;
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
    float3 pos = mul(input.Position.xy - cameraPosition, float2x4(viewProjection[0], viewProjection[1])).xyz;
    output.Position = float4(pos, 1);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    float4 pixelColor = tex2D(spriteTextureSample, input.TexCoord);
    float redChannel = pixelOptions;
    float greenChannel = specularReflectivity;
    float blueChannel = layerDepth;

    if (pixelColor.a == 0.0f)
    {
        redChannel = 0.0f;
        greenChannel = 0.0f;
        blueChannel = 0.0f;
    }

    return float4(redChannel, greenChannel, blueChannel, pixelColor.a);
}

technique ApplyFlagsToOptionsMap
{
    pass MainPass
    {
        AlphaBlendEnable = true;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = InvSrcAlpha;

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
