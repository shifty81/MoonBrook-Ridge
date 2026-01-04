namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// Exception thrown when content loading fails
/// </summary>
public class ContentLoadException : Exception
{
    public ContentLoadException() : base()
    {
    }
    
    public ContentLoadException(string message) : base(message)
    {
    }
    
    public ContentLoadException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
