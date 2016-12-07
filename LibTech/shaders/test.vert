#version 330 core

layout(location = 0) in vec3 Vert;

uniform mat4 ViewMatrix;

void main() {
	gl_Position =  ViewMatrix * vec4(Vert, 1);
}