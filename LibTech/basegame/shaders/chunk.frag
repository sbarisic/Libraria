#version 420 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

uniform sampler2D Textures[1024];

in vec3 frag_Pos;
in vec3 frag_Normal;
in vec3 frag_Color;
in vec2 frag_UV;
in float frag_TextureID;

out vec4 out_Color;

vec3 normals(vec3 pos) {
  vec3 fdx = dFdx(pos);
  vec3 fdy = dFdy(pos);
  return normalize(cross(fdx, fdy));
}

void main() {
	/*vec3 N = normals(-frag_Pos);
	mat4 M = inverse(MatProjection) * MatRotation;

	vec3 C = (M * vec4(N, 0)).xyz;
	//vec4 Tex = texture2D(Tex0, frag_UV);
	vec4 Tex = vec4(1, 1, 1, 1);
	out_Color = vec4(C * Tex.rgb, Tex.a);*/

	uint I = uint(round(frag_TextureID));
	vec3 Clr = texture2D(Textures[I], frag_UV).rgb + (frag_Normal / 10);
	out_Color = vec4(Clr, 1.0f);

	//out_Color = vec4(frag_Normal, 1.0f);
}