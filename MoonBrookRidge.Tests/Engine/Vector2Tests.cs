using Xunit;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Tests.Engine;

public class Vector2Tests
{
    [Fact]
    public void Constructor_ShouldCreateVector2WithXY()
    {
        var vector = new Vector2(3f, 4f);
        Assert.Equal(3f, vector.X);
        Assert.Equal(4f, vector.Y);
    }

    [Fact]
    public void Constructor_ShouldCreateVector2WithSingleValue()
    {
        var vector = new Vector2(5f);
        Assert.Equal(5f, vector.X);
        Assert.Equal(5f, vector.Y);
    }

    [Fact]
    public void Length_ShouldCalculateCorrectLength()
    {
        var vector = new Vector2(3f, 4f);
        Assert.Equal(5f, vector.Length());
    }

    [Fact]
    public void LengthSquared_ShouldCalculateSquaredLength()
    {
        var vector = new Vector2(3f, 4f);
        Assert.Equal(25f, vector.LengthSquared());
    }

    [Fact]
    public void Normalize_ShouldCreateUnitVector()
    {
        var vector = new Vector2(3f, 4f);
        vector.Normalize();
        Assert.Equal(1f, vector.Length(), 5);
    }

    [Fact]
    public void Distance_ShouldCalculateCorrectDistance()
    {
        var v1 = new Vector2(0f, 0f);
        var v2 = new Vector2(3f, 4f);
        Assert.Equal(5f, Vector2.Distance(v1, v2));
    }

    [Fact]
    public void DistanceSquared_ShouldCalculateSquaredDistance()
    {
        var v1 = new Vector2(0f, 0f);
        var v2 = new Vector2(3f, 4f);
        Assert.Equal(25f, Vector2.DistanceSquared(v1, v2));
    }

    [Fact]
    public void Dot_ShouldCalculateDotProduct()
    {
        var v1 = new Vector2(2f, 3f);
        var v2 = new Vector2(4f, 5f);
        var result = Vector2.Dot(v1, v2);
        Assert.Equal(23f, result); // 2*4 + 3*5 = 8 + 15 = 23
    }

    [Fact]
    public void Add_ShouldAddVectors()
    {
        var v1 = new Vector2(1f, 2f);
        var v2 = new Vector2(3f, 4f);
        var result = v1 + v2;
        Assert.Equal(4f, result.X);
        Assert.Equal(6f, result.Y);
    }

    [Fact]
    public void Subtract_ShouldSubtractVectors()
    {
        var v1 = new Vector2(5f, 7f);
        var v2 = new Vector2(2f, 3f);
        var result = v1 - v2;
        Assert.Equal(3f, result.X);
        Assert.Equal(4f, result.Y);
    }

    [Fact]
    public void Multiply_ShouldScaleVector()
    {
        var vector = new Vector2(2f, 3f);
        var result = vector * 2f;
        Assert.Equal(4f, result.X);
        Assert.Equal(6f, result.Y);
    }

    [Fact]
    public void Divide_ShouldDivideVector()
    {
        var vector = new Vector2(6f, 8f);
        var result = vector / 2f;
        Assert.Equal(3f, result.X);
        Assert.Equal(4f, result.Y);
    }

    [Fact]
    public void Negate_ShouldNegateVector()
    {
        var vector = new Vector2(3f, 4f);
        var result = -vector;
        Assert.Equal(-3f, result.X);
        Assert.Equal(-4f, result.Y);
    }

    [Fact]
    public void Lerp_ShouldInterpolateVectors()
    {
        var v1 = new Vector2(0f, 0f);
        var v2 = new Vector2(10f, 10f);
        var result = Vector2.Lerp(v1, v2, 0.5f);
        Assert.Equal(5f, result.X);
        Assert.Equal(5f, result.Y);
    }

    [Fact]
    public void StaticProperties_ShouldHaveCorrectValues()
    {
        Assert.Equal(0f, Vector2.Zero.X);
        Assert.Equal(0f, Vector2.Zero.Y);

        Assert.Equal(1f, Vector2.One.X);
        Assert.Equal(1f, Vector2.One.Y);

        Assert.Equal(1f, Vector2.UnitX.X);
        Assert.Equal(0f, Vector2.UnitX.Y);

        Assert.Equal(0f, Vector2.UnitY.X);
        Assert.Equal(1f, Vector2.UnitY.Y);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForEqualVectors()
    {
        var v1 = new Vector2(3f, 4f);
        var v2 = new Vector2(3f, 4f);
        Assert.True(v1.Equals(v2));
        Assert.True(v1 == v2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentVectors()
    {
        var v1 = new Vector2(3f, 4f);
        var v2 = new Vector2(3f, 5f);
        Assert.False(v1.Equals(v2));
        Assert.True(v1 != v2);
    }
}
