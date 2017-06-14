#version 420 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

in vec3 vert_Vertex;
in vec3 vert_Normal;
in vec3 vert_Color;
in vec2 vert_UV;
in float vert_TextureID;

out vec3 frag_Pos;
out vec3 frag_Normal;
out vec3 frag_Color;
out vec2 frag_UV;
out float frag_TextureID;

void main() {
	frag_Normal = vert_Normal;
	frag_Color = vert_Color;
	frag_UV = vert_UV;
	frag_TextureID = vert_TextureID;

	mat4 ViewMat = MatProjection * MatRotation * MatTranslation;
	vec4 Pos = ViewMat * vec4(vert_Vertex, 1);

	frag_Pos = Pos.xyz;
	gl_Position = Pos;
}