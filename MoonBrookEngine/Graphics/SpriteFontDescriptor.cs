using System.Xml.Linq;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// Parses .spritefont XML descriptor files from MonoGame Content Pipeline format
/// </summary>
public class SpriteFontDescriptor
{
    public string FontName { get; set; } = "Arial";
    public float Size { get; set; } = 14f;
    public float Spacing { get; set; } = 0f;
    public bool UseKerning { get; set; } = true;
    public string Style { get; set; } = "Regular";
    public char? DefaultCharacter { get; set; }
    public List<CharacterRegion> CharacterRegions { get; set; } = new();

    public struct CharacterRegion
    {
        public char Start;
        public char End;
    }

    /// <summary>
    /// Parse a .spritefont XML file
    /// </summary>
    public static SpriteFontDescriptor Parse(string filePath)
    {
        var descriptor = new SpriteFontDescriptor();

        try
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";

            // Find the Asset element
            var asset = doc.Root?.Element("Asset");
            if (asset == null)
            {
                Console.WriteLine($"Warning: No Asset element found in {filePath}, using defaults");
                return descriptor;
            }

            // Parse font name
            var fontNameElement = asset.Element("FontName");
            if (fontNameElement != null)
            {
                descriptor.FontName = fontNameElement.Value.Trim();
            }

            // Parse size
            var sizeElement = asset.Element("Size");
            if (sizeElement != null && float.TryParse(sizeElement.Value, out float size))
            {
                descriptor.Size = size;
            }

            // Parse spacing
            var spacingElement = asset.Element("Spacing");
            if (spacingElement != null && float.TryParse(spacingElement.Value, out float spacing))
            {
                descriptor.Spacing = spacing;
            }

            // Parse kerning
            var kerningElement = asset.Element("UseKerning");
            if (kerningElement != null && bool.TryParse(kerningElement.Value, out bool useKerning))
            {
                descriptor.UseKerning = useKerning;
            }

            // Parse style
            var styleElement = asset.Element("Style");
            if (styleElement != null)
            {
                descriptor.Style = styleElement.Value.Trim();
            }

            // Parse default character
            var defaultCharElement = asset.Element("DefaultCharacter");
            if (defaultCharElement != null && !string.IsNullOrEmpty(defaultCharElement.Value))
            {
                descriptor.DefaultCharacter = defaultCharElement.Value[0];
            }

            // Parse character regions
            var regionsElement = asset.Element("CharacterRegions");
            if (regionsElement != null)
            {
                foreach (var regionElement in regionsElement.Elements("CharacterRegion"))
                {
                    var startElement = regionElement.Element("Start");
                    var endElement = regionElement.Element("End");

                    if (startElement != null && endElement != null)
                    {
                        char start = ParseCharacter(startElement.Value);
                        char end = ParseCharacter(endElement.Value);

                        descriptor.CharacterRegions.Add(new CharacterRegion
                        {
                            Start = start,
                            End = end
                        });
                    }
                }
            }

            // Default character region if none specified
            if (descriptor.CharacterRegions.Count == 0)
            {
                descriptor.CharacterRegions.Add(new CharacterRegion
                {
                    Start = ' ',  // ASCII 32
                    End = '~'     // ASCII 126
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing spritefont file {filePath}: {ex.Message}");
            Console.WriteLine("Using default character set (32-126)");
            
            // Return default descriptor
            descriptor.CharacterRegions.Add(new CharacterRegion
            {
                Start = ' ',
                End = '~'
            });
        }

        return descriptor;
    }

    /// <summary>
    /// Parse a character from XML (supports numeric codes like &#32;)
    /// </summary>
    private static char ParseCharacter(string value)
    {
        value = value.Trim();

        // Handle numeric character references (&#32; or &#x20;)
        if (value.StartsWith("&#"))
        {
            value = value.TrimStart('&', '#').TrimEnd(';');
            
            // Hex format (&#x20;)
            if (value.StartsWith("x") || value.StartsWith("X"))
            {
                if (int.TryParse(value.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out int code))
                {
                    return (char)code;
                }
            }
            // Decimal format (&#32;)
            else if (int.TryParse(value, out int code))
            {
                return (char)code;
            }
        }

        // Direct character
        return string.IsNullOrEmpty(value) ? ' ' : value[0];
    }

    /// <summary>
    /// Get all characters that should be included in the font
    /// </summary>
    public IEnumerable<char> GetCharacters()
    {
        var chars = new HashSet<char>();

        foreach (var region in CharacterRegions)
        {
            for (char c = region.Start; c <= region.End; c++)
            {
                chars.Add(c);
            }
        }

        return chars;
    }
}
