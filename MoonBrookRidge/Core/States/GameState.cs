using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.States;

/// <summary>
/// Base class for all game states (main menu, gameplay, pause menu, etc.)
/// </summary>
public abstract class GameState
{
    protected Game1 Game { get; private set; }

    public GameState(Game1 game)
    {
        Game = game;
    }

    public virtual void Initialize() { }
    
    public virtual void LoadContent() { }
    
    public abstract void Update(GameTime gameTime);
    
    public abstract void Draw(SpriteBatch spriteBatch);
    
    public virtual void UnloadContent() { }
}
