using Silk.NET.OpenGL;

namespace OpenGLAvalonia.Core.Generics;

public unsafe class BufferObjects<T> : IGlHandle where T : unmanaged
{
    private readonly GL _gl;
    private readonly BufferTargetARB _bufferObjectsType;
    private readonly uint _handle;

    public BufferObjects(GL gl, ReadOnlySpan<T> values, BufferTargetARB bufferObjectsType)
    {
        _gl = gl;
        _handle = gl.GenBuffer();
        _bufferObjectsType = bufferObjectsType;

        Bind();
        fixed (T* ptr = values) {
            gl.BufferData(bufferObjectsType, (nuint)(values.Length * sizeof(T)), ptr, BufferUsageARB.StaticDraw);
        }
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferObjectsType, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
        GC.SuppressFinalize(this);
    }
}
