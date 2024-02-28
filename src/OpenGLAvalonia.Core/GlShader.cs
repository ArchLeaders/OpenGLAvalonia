using Silk.NET.OpenGL;

namespace OpenGLAvalonia.Core;

public class GlShader : IDisposable
{
    private readonly uint _handle;
    private readonly GL _gl;

    public GlShader(GL gl, Span<byte> vertexShaderSource, Span<byte> fragmentShaderSource)
    {
        _gl = gl;
        _handle = gl.CreateProgram();
        uint vertexShader = AttachShader(ShaderType.VertexShader, vertexShaderSource);
        uint fragmentShader = AttachShader(ShaderType.FragmentShader, fragmentShaderSource);

        _gl.LinkProgram(_handle);
        _gl.GetProgram(_handle, GLEnum.LinkStatus, out int linkStatus);

        if (linkStatus == 0) {
            throw new Exception($"""
                Shader program failed to link: '{_gl.GetProgramInfoLog(_handle)}'
                """);
        }

        CleanupShader(vertexShader);
        CleanupShader(fragmentShader);
    }

    public void Activate()
    {
        _gl.UseProgram(_handle);
    }

    public int GetUniform(string name)
    {
        return _gl.GetUniformLocation(_handle, name);
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_handle);
        GC.SuppressFinalize(this);
    }

    private unsafe uint AttachShader(ShaderType shaderType, Span<byte> src)
    {
        uint shader = _gl.CreateShader(shaderType);

        fixed (byte* ptr = src) {
            _gl.ShaderSource(shader, ptr, [src.Length]);
            _gl.CompileShader(shader);
        }

        _gl.GetShader(shader, GLEnum.Shader, out int status);

        if (_gl.GetProgramInfoLog(_handle) is string log && !string.IsNullOrEmpty(log)) {
            throw new Exception($"""
                Error compiling shader of type '{shaderType}': {log}
                """);
        }

        _gl.AttachShader(_handle, shader);
        return shader;
    }

    private void CleanupShader(uint shader)
    {
        _gl.DetachShader(_handle, shader);
        _gl.DeleteShader(shader);
    }
}
