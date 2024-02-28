using Silk.NET.OpenGL;

namespace OpenGLAvalonia.Core.Generics;

public class VertexArrayObjects<TVertices, TIndices> : IGlHandle where TVertices : unmanaged where TIndices : unmanaged
{
    private readonly GL _gl;
    private readonly uint _handle;
    private readonly BufferObjects<TVertices> _vbo;
    private readonly BufferObjects<TIndices> _ebo;

    public VertexArrayObjects(GL gl, BufferObjects<TVertices> vertexBufferObjects, BufferObjects<TIndices> indexBufferObjects)
    {
        _gl = gl;
        _handle = gl.CreateVertexArray();
        _vbo = vertexBufferObjects;
        _ebo = indexBufferObjects;

        Bind();
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
    {
        _gl.VertexAttribPointer(index, count, type, normalized: false, vertexSize * (uint)sizeof(TVertices), (void*)(offset * sizeof(TVertices)));
        _gl.EnableVertexAttribArray(index);
    }

    public void Bind()
    {
        _vbo.Bind();
        _ebo.Bind();
        _gl.BindVertexArray(_handle);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_handle);
        GC.SuppressFinalize(this);
    }
}
