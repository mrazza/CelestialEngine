// -----------------------------------------------------------------------
// <copyright file="MergeTargets.fx" company="">
// Copyright (C) 2011 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader renders a point light with specular effects.
/// </summary>
float4x4 viewProjection;
float lightPower;
float lightDecay;
float lightRange;
float3 lightPosition;
float2 cameraPosition;
float4 lightColor;
float specularStrength;
float layerDepth;

Texture normalMap;
sampler normalMapSampler = sampler_state
{
    texture = <normalMap>;
    magfilter = point;
    minfilter = point;
    mipfilter = point;
    AddressU = clamp;
    AddressV = clamp;
};

Texture optionsMap;
sampler optionsMapSampler = sampler_state
{
    texture = <optionsMap>;
    magfilter = point;
    minfilter = point;
    mipfilter = point;
    AddressU = clamp;
    AddressV = clamp;
};

Texture shadowMap;
sampler shadowMapSampler = sampler_state
{
    texture = <shadowMap>;
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
    float3 WorldPos : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float3 pos = mul(input.Position.xy - cameraPosition, float2x4(viewProjection[0], viewProjection[1])).xyz;
    output.Position = float4(pos, 1);
    output.TexCoord = float2((pos.x + 1) / 2.0f, (-pos.y + 1) / 2.0f);

    output.WorldPos = input.Position;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 shadow = tex2D(shadowMapSampler, input.TexCoord).rb;
    float4 options = tex2D(optionsMapSampler, input.TexCoord);
    int flag = (int)ceil(options.r * 255.0f);
    bool light = fmod(flag, 2) == 1;

    if (!light)
    {
        return float4(1, 1, 1, 0);
    }
    else if ((shadow.x == 0.0f && shadow.y > options.b) || options.b > layerDepth) // Only accept shadows onto you from objects higher than yourself... or if you're way above the light you tricky pixel.
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        float specularReflectivity = options.g;
        float3 normal = 2.0f * tex2D(normalMapSampler, input.TexCoord).rgb - 1.0f; // Get within [-1, 1]

        float3 lightDirection = lightPosition - input.WorldPos; // Get light direction vector
        float3 lightDirNorm = normalize(lightDirection); // Normalize the vector
        float3 halfVec = float3(0, 0, 1); // Found on google
        float3 lightColorAndAttenuation = 0;

        // If we're going to render light here calculate the color and attenuation
        if (length(lightDirection) < lightRange)
            lightColorAndAttenuation = (lightColor * pow(abs(1.0f / pow(lightRange, 2) * pow(length(lightDirection) - lightRange, 2)), lightDecay)).rgb;

        // Do specular calculations
        float amount = max(dot(normal, lightDirNorm), 0);
        float3 reflect = normalize(2 * amount * normal - lightDirNorm);
        float specular = min(pow(saturate(dot(reflect, halfVec)), 10), amount);

        return float4(saturate(lightColorAndAttenuation * lightPower + specular * lightColorAndAttenuation * specularStrength * specularReflectivity).rgb, 0);
    }
}

technique SpecularPointLight
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
