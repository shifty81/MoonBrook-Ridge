using Xunit;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Tests.Engine;

public class GameTimeTests
{
    [Fact]
    public void Constructor_ShouldCreateGameTime_WithTimeSpans()
    {
        var totalTime = TimeSpan.FromSeconds(10);
        var elapsedTime = TimeSpan.FromSeconds(0.016);

        var gameTime = new GameTime(totalTime, elapsedTime);

        Assert.Equal(totalTime, gameTime.TotalGameTime);
        Assert.Equal(elapsedTime, gameTime.ElapsedGameTime);
    }

    [Fact]
    public void Constructor_ShouldCreateGameTime_WithDoubles()
    {
        var gameTime = new GameTime(10.0, 0.016);

        Assert.Equal(10.0, gameTime.TotalGameTime.TotalSeconds, 5);
        Assert.Equal(0.016, gameTime.ElapsedGameTime.TotalSeconds, 5);
    }

    [Fact]
    public void TotalGameTime_ShouldAccumulateOverTime()
    {
        var time1 = new GameTime(1.0, 0.016);
        var time2 = new GameTime(2.0, 0.016);

        Assert.True(time2.TotalGameTime > time1.TotalGameTime);
    }

    [Fact]
    public void ElapsedGameTime_ShouldRepresentFrameTime()
    {
        var elapsedTime = 0.01667; // ~60 FPS in seconds
        var gameTime = new GameTime(1.0, elapsedTime);

        Assert.Equal(16.67, gameTime.ElapsedGameTime.TotalMilliseconds, 1);
    }
}
