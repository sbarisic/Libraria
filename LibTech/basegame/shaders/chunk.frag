#version 450 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

uniform sampler2D Textures[512];

in vec3 frag_Pos;
in vec3 frag_Normal;
in vec3 frag_Tangent;
in vec3 frag_Bitangent;
in vec2 frag_UV;
in float frag_TextureID;

layout(location = 0) out vec3 out_Position;
layout(location = 1) out vec4 out_Color;
layout(location = 2) out vec4 out_MetallicRoughnessAOHeight;
layout(location = 3) out vec3 out_Normal;
layout(location = 4) out vec3 out_Emissive;

vec3 normals(vec3 pos) {
  vec3 fdx = dFdx(pos);
  vec3 fdy = dFdy(pos);
  return normalize(cross(fdx, fdy));
}

vec2 CalcUV(int X, int Y) {
	const float B = 1.0 / 3.0;
	vec2 A = frag_UV / 3.0;
	return A + vec2(B * X, B * Y);
}

void main() {
	uint I = uint(round(frag_TextureID));

	out_Position = frag_Pos;
	out_Color = texture2D(Textures[I], CalcUV(0, 0));
	out_MetallicRoughnessAOHeight = vec4(texture2D(Textures[I], CalcUV(1, 0)).r, texture2D(Textures[I], CalcUV(0, 1)).r,
		texture2D(Textures[I], CalcUV(1, 1)).r, texture2D(Textures[I], CalcUV(0, 2)).r);

	/////////////////////

	vec3 NormalTex = normalize(texture2D(Textures[I], CalcUV(2, 0)).rgb * 2 - 1) * vec3(1, 1, 1);

	mat3 ViewMat = mat3(MatRotation * MatTranslation);
	//mat3 InvViewMat = inverse(ViewMat);

	mat3 TBN = mat3(frag_Tangent, frag_Bitangent, frag_Normal);
	out_Normal = ViewMat * (TBN * NormalTex);

	//out_Normal = frag_Normal;

	//////////////////

	out_Emissive = texture2D(Textures[I], CalcUV(2, 1)).rgb;
}