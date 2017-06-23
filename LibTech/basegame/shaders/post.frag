#version 420 core

uniform mat4 MatTranslation2;
uniform mat4 MatRotation2;
uniform mat4 MatProjection2;

uniform vec2 Resolution;
uniform sampler2D SSAOBuffer;

// PBR
uniform sampler2D PositionBuffer;
uniform sampler2D ColorBuffer;
uniform sampler2D MetallicRoughnessAOHeightBuffer;
uniform sampler2D NormalBuffer;
uniform sampler2D EmissiveBuffer;

uniform vec3[] LightDir;

in vec2 frag_UV;

out vec4 out_Color;

float rand(vec2 co) {
	return fract(sin(mod(dot(co.xy, vec2(12.9898, 78.233)), 3.14)) * 43758.5453);
}

void main() {
	mat3 ViewMat = mat3(MatRotation2 * MatTranslation2);
	mat3 InvViewMat = inverse(ViewMat);

	vec4 Color = texture2D(ColorBuffer, frag_UV);

	vec3 Normal = texture2D(NormalBuffer, frag_UV).rgb;
	vec3 WorldNormal = InvViewMat * Normal;
	//out_Color = vec4(WorldNormal, 1); return;

	float Diff = clamp(dot(WorldNormal, LightDir[0]), 0.8, 1.0);
	out_Color = vec4(Color.rgb * Diff, Color.a);

	//out_Color = vec4(Normal, 1);

	//float SSAO = texture2D(SSAOBuffer, frag_UV).r;
	//float SSAO = 1;
	//out_Color = vec4(vec3(SSAO), 1.0);

	//out_Color = vec4(Color.rgb * SSAO, Color.a);
}