#version 330 core

uniform mat4 ViewMatrix;

in vec3 vert_Vertex;
in vec3 vert_Color;
in vec2 vert_UV;

out vec3 frag_ViewPos;
out vec3 frag_Color;
out vec2 frag_UV;

void main() {
	frag_Color = vert_Color;
	frag_UV = vert_UV;
	
	vec4 Pos = ViewMatrix * vec4(vert_Vertex, 1);
	frag_ViewPos = -Pos.xyz;
	gl_Position = Pos;
}