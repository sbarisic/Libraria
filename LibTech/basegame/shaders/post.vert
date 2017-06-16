#version 420 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

layout(location = 0) in vec3 vert_Vertex;
layout(location = 1) in vec2 vert_UV;

out vec2 frag_UV;

void main() {
	frag_UV = vert_UV;
	gl_Position = MatProjection * vec4(vert_Vertex, 1);
}