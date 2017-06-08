#version 330 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

uniform sampler2D Tex0;
uniform sampler2D Tex1;
uniform sampler2D Tex2;
uniform sampler2D Tex3;
uniform sampler2D Tex4;

in vec3 frag_Pos;
in vec3 frag_Normal;
in vec3 frag_Color;
in vec2 frag_UV;

out vec4 out_Color;

vec3 normals(vec3 pos) {
  vec3 fdx = dFdx(pos);
  vec3 fdy = dFdy(pos);
  return normalize(cross(fdx, fdy));
}

void main() {
	vec3 N = normals(-frag_Pos);
	mat4 M = inverse(MatProjection) * MatRotation;

	vec3 C = (M * vec4(N, 0)).xyz;
	//vec4 Tex = texture2D(Tex0, frag_UV);
	vec4 Tex = vec4(1, 1, 1, 1);
	out_Color = vec4(C * Tex.rgb, Tex.a);

	//out_Color = vec4(frag_Normal, 1.0f);
}