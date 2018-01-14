// -----------------------------------------------------------------------
// <copyright file="NormalMap.fx" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader renders the normal map.
/// </summary>
float3x3 normalTransformationMatrix;
float4x4 viewProjection;
float2 cameraPosition;

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
    float3 normalVector = mul(normalize(2.0f * pixelColor.rgb - 1.0), normalTransformationMatrix);

    return float4((normalVector + 1.0) / 2.0f, pixelColor.a);
}

technique RenderNormalMap
{
    pass MainPass
    {
        AlphaBlendEnable = true;
        BlendOp = Add;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
