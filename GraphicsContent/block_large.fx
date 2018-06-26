// Block.fx

static float3 light_dir = normalize(float3(2,4,1));

float4x4 matView;
float4x4 matProj;
float3 viewPos;
float3 fogColor;
float2 fogRange;

texture tex_skin;
sampler2D ss_skin = sampler_state {
	texture = <tex_skin>;
	addressU = Clamp;
	addressV = Clamp;
	mipfilter = LINEAR;
	minfilter = LINEAR;
	magfilter = LINEAR;
};

struct VS_IN {
	float4 Pos	: POSITION0;
	float2 Tex	: TEXCOORD0;
	float3 Nor	: NORMAL;
};

struct VS_OUT {
	float4 Pos	: POSITION0;
	float2 Tex	: TEXCOORD0;
	float  Fog : TEXCOORD1;
	float3 Nor	: NORMAL;
};

struct PS_IN {
	float2 Tex	: TEXCOORD0;
	float  Fog : TEXCOORD1;
	float3 Nor	: NORMAL;
};


VS_OUT vs_block(VS_IN input, in float4x4 matWorld : TEXCOORD1) {
	VS_OUT output;
	//
	float4 wPos = mul(input.Pos, transpose(matWorld));
	float4 vPos = mul(wPos, matView);
	output.Pos = mul(vPos, matProj);
	output.Nor = input.Nor;
	output.Tex = input.Tex;
	//
	float dist = distance(wPos.xyz, viewPos);
	output.Fog = (dist - fogRange.x) / (fogRange.y - fogRange.x);
	//
	return output;
}

float4 ps_block(PS_IN input) : COLOR0 {
	float4 col = tex2D(ss_skin, input.Tex);
	float lit = dot(input.Nor, light_dir);
	col.rgb *= 0.2 + 0.8 * lit;
	col.rgb = lerp(col.rgb, fogColor, saturate(input.Fog));
	return col;
}


technique block_fx {
	pass p1 {
		VertexShader = compile vs_3_0 vs_block();
		PixelShader = compile ps_3_0 ps_block();
	}
}