using System.Numerics;

namespace OpenGLAvalonia.Core.Extensions;

public static class MathExtension
{
    public static float ToRadians(this float value)
    {
        return float.Pi / 180 * value;
    }
 }
