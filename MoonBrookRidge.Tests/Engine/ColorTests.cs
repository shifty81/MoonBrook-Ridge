using Xunit;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Tests.Engine;

public class ColorTests
{
    [Fact]
    public void Constructor_ShouldCreateColorWithRGBA()
    {
        var color = new Color(255, 128, 64, 32);
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(64, color.B);
        Assert.Equal(32, color.A);
    }

    [Fact]
    public void Constructor_ShouldCreateColorWithRGB_AndDefaultAlpha()
    {
        var color = new Color(255, 128, 64);
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(64, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Constructor_ShouldCreateColorFromFloats()
    {
        var color = new Color(1f, 0.5f, 0.25f);
        Assert.Equal(255, color.R);
        Assert.InRange(color.G, (byte)126, (byte)128); // Allow tolerance for rounding
        Assert.InRange(color.B, (byte)62, (byte)64);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Constructor_ShouldCreateColorFromFloatsWithAlpha()
    {
        var color = new Color(1f, 0.5f, 0.25f, 0.125f);
        Assert.Equal(255, color.R);
        Assert.InRange(color.G, (byte)126, (byte)128);
        Assert.InRange(color.B, (byte)62, (byte)64);
        Assert.InRange(color.A, (byte)30, (byte)32);
    }

    [Fact]
    public void Multiply_ShouldScaleColorComponents()
    {
        var color = new Color(100, 100, 100, 100);
        var result = color * 0.5f;
        Assert.Equal(50, result.R);
        Assert.Equal(50, result.G);
        Assert.Equal(50, result.B);
        Assert.Equal(100, result.A); // Alpha is not scaled
    }

    [Fact]
    public void Lerp_ShouldInterpolateColors()
    {
        var color1 = Color.Black;
        var color2 = Color.White;
        var result = Color.Lerp(color1, color2, 0.5f);
        Assert.InRange(result.R, (byte)126, (byte)128);
        Assert.InRange(result.G, (byte)126, (byte)128);
        Assert.InRange(result.B, (byte)126, (byte)128);
        Assert.Equal(255, result.A);
    }

    [Fact]
    public void PredefinedColors_ShouldHaveCorrectValues()
    {
        Assert.Equal(255, Color.White.R);
        Assert.Equal(255, Color.White.G);
        Assert.Equal(255, Color.White.B);

        Assert.Equal(0, Color.Black.R);
        Assert.Equal(0, Color.Black.G);
        Assert.Equal(0, Color.Black.B);

        Assert.Equal(255, Color.Red.R);
        Assert.Equal(0, Color.Red.G);
        Assert.Equal(0, Color.Red.B);

        Assert.Equal(0, Color.Green.R);
        Assert.Equal(255, Color.Green.G);
        Assert.Equal(0, Color.Green.B);

        Assert.Equal(0, Color.Blue.R);
        Assert.Equal(0, Color.Blue.G);
        Assert.Equal(255, Color.Blue.B);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForEqualColors()
    {
        var color1 = new Color(255, 128, 64, 32);
        var color2 = new Color(255, 128, 64, 32);
        Assert.True(color1.Equals(color2));
        Assert.True(color1 == color2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentColors()
    {
        var color1 = new Color(255, 128, 64, 32);
        var color2 = new Color(255, 128, 64, 31);
        Assert.False(color1.Equals(color2));
        Assert.True(color1 != color2);
    }
}
