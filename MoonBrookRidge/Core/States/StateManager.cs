using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.States;

/// <summary>
/// Manages game state transitions (menu, gameplay, etc.)
/// </summary>
public class StateManager
{
    private GameState _currentState;
    private readonly Game1 _game;

    public StateManager(Game1 game)
    {
        _game = game;
    }

    public void ChangeState(GameState newState)
    {
        _currentState?.UnloadContent();
        _currentState = newState;
        _currentState?.Initialize();
        _currentState?.LoadContent();
    }

    public void Update(GameTime gameTime)
    {
        _currentState?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentState?.Draw(spriteBatch);
    }

    public GameState CurrentState => _currentState;
}
