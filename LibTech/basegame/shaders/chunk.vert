#version 450 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

in vec3 in_Vertex;
in vec2 in_UV;
in float in_TextureID;

out vec3 geom_Vertex;
out vec3 geom_Pos;
out vec2 geom_UV;
out float geom_TextureID;

float rand(vec2 co) {
	return fract(sin(mod(dot(co.xy, vec2(12.9898, 78.233)), 3.14)) * 43758.5453);
}

void main() {
	//float RndOffset = rand(in_Vertex.xz) * 2;
	//vec3 Vertex = vec3(in_Vertex.x, in_Vertex.y + RndOffset, in_Vertex.z);
	vec3 Vertex = in_Vertex;

	mat4 ViewMat = MatRotation * MatTranslation;
	vec4 Pos = ViewMat * vec4(Vertex, 1);

	geom_Vertex = Vertex;
	geom_Pos = Pos.xyz;
	geom_UV = in_UV;
	geom_TextureID = in_TextureID;

	gl_Position = MatProjection * Pos;
}