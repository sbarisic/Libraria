#version 420 core

uniform vec2 Resolution;
uniform sampler2D Textures[2];

in vec2 frag_UV;

out vec4 out_Color;

float rand(vec2 co) {
	return fract(sin(mod(dot(co.xy, vec2(12.9898, 78.233)), 3.14)) * 43758.5453);
}

void main() {
	vec4 Color = texture2D(Textures[0], frag_UV);
	float Occlusion = texture2D(Textures[1], frag_UV).r;

	out_Color = vec4(Color.rgb * Occlusion, Color.a);
	//out_Color = vec4(vec3(rand(frag_UV)), 1);
	//out_Color = vec4(Occlusion, Occlusion, Occlusion, 1);
}