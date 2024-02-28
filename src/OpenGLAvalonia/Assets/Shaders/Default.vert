#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec4 vColor;
layout (location = 2) in vec2 vTex;

out vec4 fColor;
out vec2 fTexCoord;

uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uProjMatrix;

void main()
{
    gl_Position = uProjMatrix * uViewMatrix * uModelMatrix * vec4(vPos, 1.0);

    fColor = vColor;
    fTexCoord = vTex;
}