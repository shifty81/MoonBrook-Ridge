using Xunit;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Tests.Engine;

public class MathHelperTests
{
    [Fact]
    public void Clamp_ShouldReturnValue_WhenWithinRange()
    {
        var result = MathHelper.Clamp(5, 0, 10);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Clamp_ShouldReturnMin_WhenBelowRange()
    {
        var result = MathHelper.Clamp(-5, 0, 10);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Clamp_ShouldReturnMax_WhenAboveRange()
    {
        var result = MathHelper.Clamp(15, 0, 10);
        Assert.Equal(10, result);
    }

    [Fact]
    public void Lerp_ShouldReturnStart_WhenAmountIsZero()
    {
        var result = MathHelper.Lerp(0f, 10f, 0f);
        Assert.Equal(0f, result);
    }

    [Fact]
    public void Lerp_ShouldReturnEnd_WhenAmountIsOne()
    {
        var result = MathHelper.Lerp(0f, 10f, 1f);
        Assert.Equal(10f, result);
    }

    [Fact]
    public void Lerp_ShouldReturnMidpoint_WhenAmountIsHalf()
    {
        var result = MathHelper.Lerp(0f, 10f, 0.5f);
        Assert.Equal(5f, result);
    }

    [Fact]
    public void Max_ShouldReturnLargerValue()
    {
        Assert.Equal(10, MathHelper.Max(5, 10));
        Assert.Equal(10, MathHelper.Max(10, 5));
    }

    [Fact]
    public void Min_ShouldReturnSmallerValue()
    {
        Assert.Equal(5, MathHelper.Min(5, 10));
        Assert.Equal(5, MathHelper.Min(10, 5));
    }

    [Fact]
    public void WrapAngle_ShouldWrapAngleToNegativePiToPi()
    {
        // Test angle greater than Pi
        var result1 = MathHelper.WrapAngle(MathHelper.Pi + 1f);
        Assert.True(result1 >= -MathHelper.Pi && result1 <= MathHelper.Pi);

        // Test angle less than -Pi
        var result2 = MathHelper.WrapAngle(-MathHelper.Pi - 1f);
        Assert.True(result2 >= -MathHelper.Pi && result2 <= MathHelper.Pi);

        // Test angle within range
        var result3 = MathHelper.WrapAngle(0f);
        Assert.Equal(0f, result3);
    }

    [Fact]
    public void Distance_ShouldCalculateCorrectDistance()
    {
        var result = MathHelper.Distance(0f, 10f);
        Assert.Equal(10f, result);
    }

    [Fact]
    public void ToRadians_ShouldConvertDegreesToRadians()
    {
        var result = MathHelper.ToRadians(180f);
        Assert.Equal(MathHelper.Pi, result, 5);
    }

    [Fact]
    public void ToDegrees_ShouldConvertRadiansToDegrees()
    {
        var result = MathHelper.ToDegrees(MathHelper.Pi);
        Assert.Equal(180f, result, 5);
    }

    [Fact]
    public void SmoothStep_ShouldReturnZero_WhenAmountIsZero()
    {
        var result = MathHelper.SmoothStep(0f, 10f, 0f);
        Assert.Equal(0f, result);
    }

    [Fact]
    public void SmoothStep_ShouldReturnOne_WhenAmountIsOne()
    {
        var result = MathHelper.SmoothStep(0f, 10f, 1f);
        Assert.Equal(10f, result);
    }

    [Fact]
    public void Barycentric_ShouldCalculateCorrectly()
    {
        var result = MathHelper.Barycentric(0f, 5f, 10f, 0.5f, 0.5f);
        Assert.True(result >= 0f && result <= 10f);
    }

    [Fact]
    public void CatmullRom_ShouldCalculateCorrectly()
    {
        var result = MathHelper.CatmullRom(0f, 5f, 10f, 15f, 0.5f);
        Assert.True(result >= 0f && result <= 15f);
    }

    [Fact]
    public void Hermite_ShouldCalculateCorrectly()
    {
        var result = MathHelper.Hermite(0f, 1f, 10f, 1f, 0.5f);
        Assert.True(result >= 0f && result <= 10f);
    }
}
