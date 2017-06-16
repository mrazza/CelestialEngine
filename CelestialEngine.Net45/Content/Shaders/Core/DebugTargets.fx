// -----------------------------------------------------------------------
// <copyright file="DebugTargets.fx" company="">
// Copyright (C) 2017 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

/// <summary>
/// This shader is used to draw the render targets directly to the screen without merging to diagnose issues.
/// </summary>
texture colorMap;
texture optionsMap;
texture shadowMap;
texture lightMap;
texture renderMap;

sampler colorSampler = sampler_state
{
	Texture = (colorMap);
	AddressU = clamp;
	AddressV = clamp;
	magfilter = point;
	minfilter = point;
	mipfilter = point;
};

sampler optionsSampler = sampler_state
{
	Texture = (optionsMap);
	AddressU = clamp;
	AddressV = clamp;
	magfilter = point;
	minfilter = point;
	mipfilter = point;
};

sampler shadowSampler = sampler_state
{
	Texture = (shadowMap);
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

sampler renderMapSampler = sampler_state
{
	Texture = (renderMap);
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

float4 SinglePixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 color = tex2D(renderMapSampler, input.TexCoord).rgb;
	return float4(color, 1);
}

float4 MultiPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 source;

	if (input.TexCoord.x < 0 && input.TexCoord.y < 0) 
	{
		// Top-left quad
		source = tex2D(colorSampler, input.TexCoord / 2.0).rgb;
	}
	else if (input.TexCoord.x > 0 && input.TexCoord.y < 0) 
	{
		// Top-right quad
		source = tex2D(optionsSampler, (input.TexCoord / 2.0) + float2(1.0, 0.0)).rgb;
	}
	else if (input.TexCoord.x < 0 && input.TexCoord.y > 0) 
	{
		// Bottom-left quad
		source = tex2D(shadowSampler, (input.TexCoord / 2.0) + float2(0.0, 1.0)).rgb;
	}
	else if (input.TexCoord.x > 0 && input.TexCoord.y > 0) 
	{
		// Bottom-right quad
		source = tex2D(lightSampler, (input.TexCoord / 2.0) + float2(1.0, 1.0)).rgb;
	}
	else 
	{
		source = float3(0, 0, 0);
	}

	return float4(source, 1);
}

technique MergeRenderTargets
{
	pass SinglePass
	{
		AlphaBlendEnable = false;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = Zero;

		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 SinglePixelShaderFunction();
	}

	pass MultiPass
	{
		AlphaBlendEnable = false;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = Zero;

		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 MultiPixelShaderFunction();
	}
}
