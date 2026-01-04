namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SamplerState class for texture sampling configuration
/// </summary>
public class SamplerState
{
    /// <summary>
    /// Built-in state object with point filtering and clamp texture addressing mode
    /// </summary>
    public static readonly SamplerState PointClamp = new SamplerState
    {
        Filter = TextureFilter.Point,
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp
    };
    
    /// <summary>
    /// Built-in state object with point filtering and wrap texture addressing mode
    /// </summary>
    public static readonly SamplerState PointWrap = new SamplerState
    {
        Filter = TextureFilter.Point,
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap
    };
    
    /// <summary>
    /// Built-in state object with linear filtering and clamp texture addressing mode
    /// </summary>
    public static readonly SamplerState LinearClamp = new SamplerState
    {
        Filter = TextureFilter.Linear,
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp
    };
    
    /// <summary>
    /// Built-in state object with linear filtering and wrap texture addressing mode
    /// </summary>
    public static readonly SamplerState LinearWrap = new SamplerState
    {
        Filter = TextureFilter.Linear,
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap
    };
    
    /// <summary>
    /// Built-in state object with anisotropic filtering and clamp texture addressing mode
    /// </summary>
    public static readonly SamplerState AnisotropicClamp = new SamplerState
    {
        Filter = TextureFilter.Anisotropic,
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp
    };
    
    /// <summary>
    /// Built-in state object with anisotropic filtering and wrap texture addressing mode
    /// </summary>
    public static readonly SamplerState AnisotropicWrap = new SamplerState
    {
        Filter = TextureFilter.Anisotropic,
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap
    };
    
    /// <summary>
    /// Gets or sets the texture filter mode
    /// </summary>
    public TextureFilter Filter { get; set; } = TextureFilter.Linear;
    
    /// <summary>
    /// Gets or sets the texture address mode for the U coordinate
    /// </summary>
    public TextureAddressMode AddressU { get; set; } = TextureAddressMode.Wrap;
    
    /// <summary>
    /// Gets or sets the texture address mode for the V coordinate
    /// </summary>
    public TextureAddressMode AddressV { get; set; } = TextureAddressMode.Wrap;
    
    /// <summary>
    /// Gets or sets the texture address mode for the W coordinate
    /// </summary>
    public TextureAddressMode AddressW { get; set; } = TextureAddressMode.Wrap;
    
    /// <summary>
    /// Gets or sets the maximum anisotropy
    /// </summary>
    public int MaxAnisotropy { get; set; } = 4;
    
    /// <summary>
    /// Gets or sets the maximum mipmap level
    /// </summary>
    public int MaxMipLevel { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the mipmap LOD bias
    /// </summary>
    public float MipMapLevelOfDetailBias { get; set; } = 0.0f;
}

/// <summary>
/// Defines filtering types during texture sampling
/// </summary>
public enum TextureFilter
{
    /// <summary>
    /// Use point filtering
    /// </summary>
    Point,
    
    /// <summary>
    /// Use linear filtering
    /// </summary>
    Linear,
    
    /// <summary>
    /// Use anisotropic filtering
    /// </summary>
    Anisotropic,
    
    /// <summary>
    /// Use point filtering for minification, linear filtering for magnification
    /// </summary>
    LinearMipPoint,
    
    /// <summary>
    /// Use point filtering for minification and magnification, linear filtering for mipmaps
    /// </summary>
    PointMipLinear,
    
    /// <summary>
    /// Use linear filtering for minification, point filtering for magnification
    /// </summary>
    MinLinearMagPointMipLinear,
    
    /// <summary>
    /// Use point filtering for minification, linear filtering for magnification and mipmaps
    /// </summary>
    MinPointMagLinearMipLinear,
    
    /// <summary>
    /// Use linear filtering for minification and magnification, point filtering for mipmaps
    /// </summary>
    MinLinearMagPointMipPoint,
    
    /// <summary>
    /// Use point filtering for minification, linear filtering for magnification, point filtering for mipmaps
    /// </summary>
    MinPointMagLinearMipPoint
}

/// <summary>
/// Defines modes for addressing texels using texture coordinates outside of the range [0, 1]
/// </summary>
public enum TextureAddressMode
{
    /// <summary>
    /// Texels outside the range [0, 1] are set to the texture color at 0 or 1, respectively
    /// </summary>
    Clamp,
    
    /// <summary>
    /// Tile the texture at every integer junction
    /// </summary>
    Wrap,
    
    /// <summary>
    /// Tile the texture at every integer junction, but mirror it at every odd integer
    /// </summary>
    Mirror,
    
    /// <summary>
    /// Similar to Wrap, except the texture is flipped at every integer junction
    /// </summary>
    Border
}
