using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace OpenGLAvalonia.Core.Common;

public class GlTexture : IGlHandle
{
    private readonly GL _gl;
    private readonly uint _handle;

    public unsafe GlTexture(GL gl, Stream stream)
    {
        _gl = gl;
        _handle = gl.GenTexture();

        Bind();

        using (Image<Rgba32> image = Image.Load<Rgba32>(stream)) {
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            image.Mutate(x => x.Flip(FlipMode.Vertical));
            image.ProcessPixelRows(accessor => {
                for (int y = 0; y < accessor.Height; y++) {
                    fixed(Rgba32* ptr = accessor.GetRowSpan(y)) {
                        gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                    }
                }
            });
        }

        gl.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        gl.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        gl.TextureParameter(_handle, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        gl.TextureParameter(_handle, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        // gl.TextureParameter(_handle, TextureParameterName.TextureBaseLevel, 0);
        // gl.TextureParameter(_handle, TextureParameterName.TextureMaxLevel, 8);
        gl.GenerateTextureMipmap(_handle);
    }

    public void Bind()
    {
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_handle);
        GC.SuppressFinalize(this);
    }
}
