#version 330 core

out vec4 FragColor;

in vec4 fColor;
in vec2 fTexCoord;

uniform sampler2D uTex0;

void main()
{
    FragColor = texture(uTex0, fTexCoord);
}