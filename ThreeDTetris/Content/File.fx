#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

//matrices
float4x4 xViewProjection;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShader SimpleVertexShader(float4 inPos : POSITION)
{
    VertexShaderOutput Output = (VertexShaderOutput) 0;

    Output.Position = mul(inPos, xViewProjection);
    Output.Color = 1.0f;

    return Output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
        VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};