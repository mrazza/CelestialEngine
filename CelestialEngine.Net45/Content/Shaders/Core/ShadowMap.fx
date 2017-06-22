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