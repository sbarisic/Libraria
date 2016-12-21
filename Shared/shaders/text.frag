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
	vec4 Clr = texture2D(Tex0, frag_UV);
	out_Color = vec4(Clr.rgb * frag_Color, Clr.a);
}