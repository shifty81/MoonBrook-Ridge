namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible GraphicsAdapter stub for display information
/// </summary>
public class GraphicsAdapter
{
    private static GraphicsAdapter? _defaultAdapter;
    
    /// <summary>
    /// Gets the default graphics adapter
    /// </summary>
    public static GraphicsAdapter DefaultAdapter
    {
        get
        {
            if (_defaultAdapter == null)
            {
                _defaultAdapter = new GraphicsAdapter();
            }
            return _defaultAdapter;
        }
    }
    
    /// <summary>
    /// Gets the current display mode
    /// </summary>
    public DisplayMode CurrentDisplayMode { get; } = new DisplayMode(1920, 1080);
    
    /// <summary>
    /// Gets supported display modes (stub - returns common resolutions)
    /// </summary>
    public DisplayModeCollection SupportedDisplayModes { get; } = new DisplayModeCollection();
}

/// <summary>
/// Represents a display mode
/// </summary>
public class DisplayMode
{
    public int Width { get; }
    public int Height { get; }
    public SurfaceFormat Format { get; }
    
    public DisplayMode(int width, int height, SurfaceFormat format = SurfaceFormat.Color)
    {
        Width = width;
        Height = height;
        Format = format;
    }
    
    public float AspectRatio => (float)Width / Height;
}

/// <summary>
/// Collection of display modes
/// </summary>
public class DisplayModeCollection : IEnumerable<DisplayMode>
{
    private List<DisplayMode> _modes = new List<DisplayMode>
    {
        new DisplayMode(1920, 1080),
        new DisplayMode(1680, 1050),
        new DisplayMode(1600, 900),
        new DisplayMode(1440, 900),
        new DisplayMode(1366, 768),
        new DisplayMode(1280, 1024),
        new DisplayMode(1280, 800),
        new DisplayMode(1280, 720),
        new DisplayMode(1024, 768),
        new DisplayMode(800, 600)
    };
    
    public IEnumerator<DisplayMode> GetEnumerator() => _modes.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _modes.GetEnumerator();
}

/// <summary>
/// Defines surface format types
/// </summary>
public enum SurfaceFormat
{
    Color,
    Bgr565,
    Bgra5551,
    Bgra4444,
    Dxt1,
    Dxt3,
    Dxt5,
    NormalizedByte2,
    NormalizedByte4,
    Rgba1010102,
    Rg32,
    Rgba64,
    Alpha8,
    Single,
    Vector2,
    Vector4,
    HalfSingle,
    HalfVector2,
    HalfVector4,
    HdrBlendable
}
