#version 330 core

uniform sampler2D Tex0;
uniform sampler2D Tex1;
uniform sampler2D Tex2;
uniform sampler2D Tex3;
uniform sampler2D Tex4;

in vec3 frag_Color;
in vec2 frag_UV;

out vec4 out_Color;

void main() {
	vec4 Mask = texture2D(Tex0, frag_UV);
	if (Mask.a <= 0.001)
		discard;

	vec4 Col0 = texture2D(Tex1, frag_UV);
	vec4 Col1 = texture2D(Tex2, frag_UV);
	vec4 Col2 = texture2D(Tex3, frag_UV);
	vec4 Col3 = texture2D(Tex4, frag_UV);

	vec4 Mix01 = mix(Col0, Col1, Mask.r);
	vec4 Mix012 = mix(Mix01, Col2, Mask.g);
	vec4 Mix0123 = mix(Mix012, Col3, Mask.b);

	out_Color = vec4(Mix0123.rgb * frag_Color, Mix0123.a * Mask.a);
}