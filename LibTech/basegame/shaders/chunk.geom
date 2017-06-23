#version 450 core
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
out vec3 frag_Tangent;
out vec3 frag_Bitangent;
out vec2 frag_UV;
out float frag_TextureID;

void main() {
	mat3 ViewMat = mat3(MatRotation * MatTranslation);
	//mat3 NormalMatrix = transpose(inverse(mat3(ViewMat)));
	//vec3 Normal = NormalMatrix * CalcNormal();

	vec3 DeltaPos1 = geom_Vertex[1] - geom_Vertex[0];
	vec3 DeltaPos2 = geom_Vertex[2] - geom_Vertex[0];
	vec2 DeltaUV1 = geom_UV[1] - geom_UV[0];
	vec2 DeltaUV2 = geom_UV[2] - geom_UV[0];

	// World normal
	float R = 1.0f / (DeltaUV1.x * DeltaUV2.y - DeltaUV1.y * DeltaUV2.x);
	vec3 Normal = normalize(cross(DeltaPos1, DeltaPos2));
	vec3 Tangent = (DeltaPos1 * DeltaUV2.y - DeltaPos2 * DeltaUV1.y) * R;
	vec3 Bitangent = (DeltaPos2 * DeltaUV1.x - DeltaPos1 * DeltaUV2.x) * R;

	for (int i = 0; i < 3; i++) {
		frag_Pos = geom_Pos[i];
		frag_Normal = Normal;
		frag_Tangent = Tangent;
		frag_Bitangent = Bitangent;
		frag_UV = geom_UV[i];
		frag_TextureID = geom_TextureID[i];

		gl_Position = gl_PositionIn[i];
		EmitVertex();
	}

	EndPrimitive();
}