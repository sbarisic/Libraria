#version 420 core
#extension GL_EXT_geometry_shader4 : enable

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;

in vec3 geom_Vertex[];
in vec3 geom_Pos[];
in vec2 geom_UV[];
in float geom_TextureID[];

out vec3 frag_Pos;
out vec3 frag_Normal;
out vec2 frag_UV;
out float frag_TextureID;

vec3 CalcNormal() {
	vec3 A = geom_Vertex[0] - geom_Vertex[1];
	vec3 B = geom_Vertex[2] - geom_Vertex[1];
	return -normalize(cross(A, B));
}  

void main() {
	mat4 ViewMat = MatRotation * MatTranslation;
	mat3 NormalMatrix = transpose(inverse(mat3(ViewMat)));

	for (int i = 0; i < 3; i++) {
		frag_Pos = geom_Pos[i];
		frag_Normal = NormalMatrix * CalcNormal();
		frag_UV = geom_UV[i];
		frag_TextureID = geom_TextureID[i];

		gl_Position = gl_PositionIn[i];
		EmitVertex();
	}

	EndPrimitive();
}