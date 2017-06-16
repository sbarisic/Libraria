#version 420 core

uniform mat4 MatTranslation;
uniform mat4 MatRotation;
uniform mat4 MatProjection;
uniform mat4 MatProjection2;

uniform sampler2D Textures[3];
uniform vec3 SSAOKernel[16];

in vec2 frag_UV;

out float out_Occlusion;

float rand(vec2 co) {
	return fract(sin(mod(dot(co.xy, vec2(12.9898, 78.233)), 3.14)) * 43758.5453);
}

float CalculateOcclusion(sampler2D PositionBuffer, sampler2D NormalBuffer) {
	const float Radius = 10.0;
	const float Bias = 0.025;

	float KernelSize = SSAOKernel.length();

	vec3 FragPos = texture2D(PositionBuffer, frag_UV).xyz;
	vec3 Normal = texture2D(NormalBuffer, frag_UV).xyz;
	vec3 RandomVec = vec3(rand(frag_UV) * 2 - 1, rand(frag_UV * 7919) * 2 - 1, 0);

	vec3 Tangent = normalize(RandomVec - Normal * dot(RandomVec, Normal));
	vec3 Bitangent = cross(Normal, Tangent);
	mat3 TBN = mat3(Tangent, Bitangent, Normal);

	float Occlusion = 0.0;

	for (int i = 0; i < KernelSize; i++) {
		vec3 Sample = TBN * SSAOKernel[i];
		Sample = FragPos + Sample * Radius;

		vec4 Offset = vec4(Sample, 1.0);
		Offset = MatProjection2 * Offset;
		Offset.xyz /= Offset.w;
		Offset.xyz = Offset.xyz * 0.5 + 0.5;

		float SampleDepth = texture2D(PositionBuffer, Offset.xy).z;

		float RangeCheck = smoothstep(0.0, 1.0, Radius / abs(FragPos.z - SampleDepth));
		Occlusion += (SampleDepth >= Sample.z + Bias ? 1.0 : 0.0) * RangeCheck;
	}

	Occlusion = 1.0 - (Occlusion / KernelSize);
	return clamp(Occlusion, 0.1, 0.9);
}

void main() {
	out_Occlusion = CalculateOcclusion(Textures[0], Textures[1]);
}