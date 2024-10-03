#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

float Alpha;
float Brightness;

Texture2D Texture2d;

sampler2D TextureSampler = sampler_state {
    Texture = (Texture2d);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct CubeVertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct CubeVertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

CubeVertexShaderOutput MainVS(in CubeVertexShaderInput input)
{
	CubeVertexShaderOutput output = (CubeVertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjectionMatrix);

	output.Color = input.Color;
    output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 MainPS(CubeVertexShaderOutput input) : COLOR
{
    float4 VertexTextureColor = tex2D(TextureSampler, input.TextureCoordinate);

    float4 color = saturate(VertexTextureColor * input.Color);
    color.a = Alpha;

	return color * Brightness;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
