using Avalonia;
using Avalonia.Data;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using OpenGLAvalonia.Core;
using OpenGLAvalonia.Core.Common;
using OpenGLAvalonia.Core.Extensions;
using OpenGLAvalonia.Core.Generics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Buffers;
using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;

namespace OpenGLAvalonia.Controls;

public unsafe class OpenGlCanvas : OpenGlControlBase
{
    private static readonly Color _clearColor = Color.FromArgb(100, 125, 135);

    public static readonly StyledProperty<ImmutableArray<float>> VertexBufferProperty =
        AvaloniaProperty.Register<OpenGlCanvas, ImmutableArray<float>>(nameof(VertexBuffer), defaultBindingMode: BindingMode.OneTime);

    public static readonly StyledProperty<ImmutableArray<uint>> IndexBufferProperty =
        AvaloniaProperty.Register<OpenGlCanvas, ImmutableArray<uint>>(nameof(IndexBuffer), defaultBindingMode: BindingMode.OneTime);

    public static readonly StyledProperty<Stream> TextureStreamProperty =
        AvaloniaProperty.Register<OpenGlCanvas, Stream>(nameof(IndexBuffer), defaultBindingMode: BindingMode.OneTime);

    private GL _gl = null!;
    private VertexArrayObjects<float, uint> _vao = null!;
    private GlShader _shader = null!;
    private GlTexture _texture = null!;

    private float _rotation = 0;
    private TimeOnly _prevTime = TimeOnly.MinValue;

    public ImmutableArray<float> VertexBuffer {
        get => GetValue(VertexBufferProperty);
        set => SetValue(VertexBufferProperty, value);
    }

    public ImmutableArray<uint> IndexBuffer {
        get => GetValue(IndexBufferProperty);
        set => SetValue(IndexBufferProperty, value);
    }

    public Stream TextureStream {
        get => GetValue(TextureStreamProperty);
        set => SetValue(TextureStreamProperty, value);
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        _gl = GL.GetApi(gl.GetProcAddress);

        BufferObjects<float> vbo = new(_gl, VertexBuffer.AsSpan(), BufferTargetARB.ArrayBuffer);
        BufferObjects<uint> ebo = new(_gl, IndexBuffer.AsSpan(), BufferTargetARB.ElementArrayBuffer);

        _vao = new(_gl, vbo, ebo);
        _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 9, 0);
        _vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 9, 3);
        _vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 9, 7);

        _shader = LoadShader(_gl, "Default");
        _texture = new GlTexture(_gl, TextureStream);

        _shader.Activate();
        _gl.Uniform1(_shader.GetUniform("uTex0"), 0);
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        ResetViewport();
        _shader.Activate();

        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        if ((currentTime - _prevTime) >= TimeSpan.FromMilliseconds(16)) {
            _rotation += 0.5f;
            _prevTime = currentTime;
        }

        Matrix4x4 model = Matrix4x4.CreateRotationY(_rotation.ToRadians(), new Vector3(0, 1, 0));
        Matrix4x4 view = Matrix4x4.CreateTranslation(new Vector3(0.0f, -0.5f, -2.0f));
        Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(45.0f.ToRadians(), (float)(Bounds.Height / Bounds.Width), 0.1f, 100.0f);

        int modelMatrixLocation = _shader.GetUniform("uModelMatrix");
        _gl.UniformMatrix4(modelMatrixLocation, 1, false, (float*)&model);

        int viewMatrixLocation = _shader.GetUniform("uViewMatrix");
        _gl.UniformMatrix4(viewMatrixLocation, 1, false, (float*)&view);

        int projMatrixLocation = _shader.GetUniform("uProjMatrix");
        _gl.UniformMatrix4(projMatrixLocation, 1, false, (float*)&proj);

        _texture.Bind();
        _vao.Bind();

        Draw();

        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
    }

    private void ResetViewport()
    {
        _gl.ClearColor(_clearColor);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.Enable(EnableCap.DepthTest);
        _gl.Viewport(0, 0, (uint)(Bounds.Width * App.DpiScale), (uint)(Bounds.Height * App.DpiScale));
    }

    private void Draw()
    {
        _gl.DrawElements(PrimitiveType.Triangles, (uint)IndexBuffer.Length, DrawElementsType.UnsignedInt, null);
    }

    private static GlShader LoadShader(GL gl, string shaderName)
    {
        using Stream vertexShaderStream = AssetLoader.Open(new Uri($"avares://{nameof(OpenGLAvalonia)}/Assets/Shaders/{shaderName}.vert"));
        int vertexBufferSize = (int)vertexShaderStream.Length;
        byte[] vertexShaderBuffer = ArrayPool<byte>.Shared.Rent(vertexBufferSize);
        Span<byte> vertexShaderSrc = vertexShaderBuffer.AsSpan()[..vertexBufferSize];
        vertexShaderStream.Read(vertexShaderSrc);

        using Stream fragmentShaderStream = AssetLoader.Open(new Uri($"avares://{nameof(OpenGLAvalonia)}/Assets/Shaders/{shaderName}.frag"));
        int fragmentShaderBufferSize = (int)fragmentShaderStream.Length;
        byte[] fragmentShaderBuffer = ArrayPool<byte>.Shared.Rent(fragmentShaderBufferSize);
        Span<byte> fragmentShaderSrc = fragmentShaderBuffer.AsSpan()[..fragmentShaderBufferSize];
        fragmentShaderStream.Read(fragmentShaderSrc);

        GlShader shader = new(gl, vertexShaderSrc, fragmentShaderSrc);

        ArrayPool<byte>.Shared.Return(vertexShaderBuffer);
        ArrayPool<byte>.Shared.Return(fragmentShaderBuffer);

        shader.Activate();
        return shader;
    }
}
