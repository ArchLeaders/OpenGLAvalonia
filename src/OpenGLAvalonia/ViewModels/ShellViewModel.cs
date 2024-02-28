using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Immutable;

namespace OpenGLAvalonia.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private ImmutableArray<float> _vertexBuffer = [
        //  X     Y      Z      R      G      B  A
        -0.5f, 0.0f,  0.5f, 0.83f, 0.70f, 0.44f, 1, 0.0f, 0.0f,
        -0.5f, 0.0f, -0.5f, 0.83f, 0.70f, 0.44f, 1, 5.0f, 0.0f,
         0.5f, 0.0f, -0.5f, 0.83f, 0.70f, 0.44f, 1, 0.0f, 0.0f,
         0.5f, 0.0f,  0.5f, 0.83f, 0.70f, 0.44f, 1, 5.0f, 0.0f,
         0.0f, 0.8f,  0.0f, 0.83f, 0.70f, 0.44f, 1, 2.5f, 5.0f,
    ];

    [ObservableProperty]
    private ImmutableArray<uint> _indexBuffer = [
        0, 1, 2,
        0, 2, 3,
        0, 1, 4,
        1, 2, 4,
        2, 3, 4,
        3, 0, 4,
    ];

    [ObservableProperty]
    private Stream _texture = AssetLoader.Open(new($"avares://{nameof(OpenGLAvalonia)}/Assets/Textures/DemoTexture.tif"));
}
