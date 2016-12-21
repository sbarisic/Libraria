#version 330 core

uniform sampler2D Tex0;
uniform sampler2D Tex1;
uniform sampler2D Tex2;
uniform sampler2D Tex3;
uniform sampler2D Tex4;

in vec3 frag_Color;
in vec2 frag_UV;

out vec4 out_Color;

float median(float r, float g, float b) {
    return max(min(r, g), min(max(r, g), b));
}

void main() {
    vec3 sample = texture(Tex0, frag_UV).rgb;
    float sigDist = median(sample.r, sample.g, sample.b) - 0.5;
    float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);

    out_Color = mix(vec4(0, 0, 0, 0), vec4(frag_Color, 1), opacity);
}