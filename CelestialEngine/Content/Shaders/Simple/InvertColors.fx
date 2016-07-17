// -----------------------------------------------------------------------
// <copyright file="InvertColors.fx" company="">
// Copyright (C) 2011 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader very simply inverts the colors of each pixel before it's rendered.
/// </summary>
uniform extern texture spriteTexture;
sampler spriteTextureSample = sampler_state
{
    Texture = <spriteTexture>;
};

float4 InvertPixelShader(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 pixelColor = tex2D(spriteTextureSample, texCoord);
    return float4(1 - pixelColor.rgb, pixelColor.a);
}

technique InvertTechique
{
    pass MainPass
    {
        AlphaBlendEnable = false;
        BlendOp = Add;
        SrcBlend = One;
        DestBlend = Zero;

        PixelShader = compile ps_4_0_level_9_1 InvertPixelShader();
    }
}