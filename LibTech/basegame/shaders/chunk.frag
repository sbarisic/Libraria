#version 420 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

uniform sampler2D Textures[1024];

in vec3 frag_Pos;
in vec3 frag_Normal;
in vec2 frag_UV;
in float frag_TextureID;

layout(location = 0) out vec3 out_Position;
layout(location = 1) out vec3 out_Normal;
layout(location = 2) out vec4 out_Color;

vec3 normals(vec3 pos) {
  vec3 fdx = dFdx(pos);
  vec3 fdy = dFdy(pos);
  return normalize(cross(fdx, fdy));
}

void main() {
	uint I = uint(round(frag_TextureID));

	out_Position = frag_Pos;
	out_Normal = normalize(frag_Normal);
	out_Color = texture2D(Textures[I], frag_UV);
}