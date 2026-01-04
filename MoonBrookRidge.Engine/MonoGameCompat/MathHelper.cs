namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible MathHelper utility class
/// </summary>
public static class MathHelper
{
    /// <summary>
    /// Represents the value of pi
    /// </summary>
    public const float Pi = (float)Math.PI;
    
    /// <summary>
    /// Represents the value of pi times two
    /// </summary>
    public const float TwoPi = (float)(Math.PI * 2);
    
    /// <summary>
    /// Represents the value of pi divided by two
    /// </summary>
    public const float PiOver2 = (float)(Math.PI / 2);
    
    /// <summary>
    /// Represents the value of pi divided by four
    /// </summary>
    public const float PiOver4 = (float)(Math.PI / 4);
    
    /// <summary>
    /// Restricts a value to be within a specified range
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
        return Math.Clamp(value, min, max);
    }
    
    /// <summary>
    /// Restricts a value to be within a specified range
    /// </summary>
    public static int Clamp(int value, int min, int max)
    {
        return Math.Clamp(value, min, max);
    }
    
    /// <summary>
    /// Restricts a value to be within a specified range
    /// </summary>
    public static double Clamp(double value, double min, double max)
    {
        return Math.Clamp(value, min, max);
    }
    
    /// <summary>
    /// Linearly interpolates between two values
    /// </summary>
    public static float Lerp(float a, float b, float amount)
    {
        return a + (b - a) * amount;
    }
    
    /// <summary>
    /// Linearly interpolates between two values
    /// </summary>
    public static double Lerp(double a, double b, double amount)
    {
        return a + (b - a) * amount;
    }
    
    /// <summary>
    /// Returns the greater of two values
    /// </summary>
    public static float Max(float a, float b)
    {
        return Math.Max(a, b);
    }
    
    /// <summary>
    /// Returns the greater of two values
    /// </summary>
    public static int Max(int a, int b)
    {
        return Math.Max(a, b);
    }
    
    /// <summary>
    /// Returns the greater of two values
    /// </summary>
    public static double Max(double a, double b)
    {
        return Math.Max(a, b);
    }
    
    /// <summary>
    /// Returns the lesser of two values
    /// </summary>
    public static float Min(float a, float b)
    {
        return Math.Min(a, b);
    }
    
    /// <summary>
    /// Returns the lesser of two values
    /// </summary>
    public static int Min(int a, int b)
    {
        return Math.Min(a, b);
    }
    
    /// <summary>
    /// Returns the lesser of two values
    /// </summary>
    public static double Min(double a, double b)
    {
        return Math.Min(a, b);
    }
    
    /// <summary>
    /// Converts radians to degrees
    /// </summary>
    public static float ToDegrees(float radians)
    {
        return radians * (180f / Pi);
    }
    
    /// <summary>
    /// Converts degrees to radians
    /// </summary>
    public static float ToRadians(float degrees)
    {
        return degrees * (Pi / 180f);
    }
    
    /// <summary>
    /// Reduces a given angle to a value between π and -π
    /// </summary>
    public static float WrapAngle(float angle)
    {
        angle = (float)Math.IEEERemainder(angle, TwoPi);
        if (angle <= -Pi)
        {
            angle += TwoPi;
        }
        else if (angle > Pi)
        {
            angle -= TwoPi;
        }
        return angle;
    }
    
    /// <summary>
    /// Returns the Cartesian coordinate for one axis of a point that is defined by a given triangle and two normalized barycentric (areal) coordinates
    /// </summary>
    public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
    {
        return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
    }
    
    /// <summary>
    /// Performs a Catmull-Rom interpolation using the specified positions
    /// </summary>
    public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
    {
        double amountSquared = amount * amount;
        double amountCubed = amountSquared * amount;
        return (float)(0.5 * (2.0 * value2 +
            (value3 - value1) * amount +
            (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
            (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
    }
    
    /// <summary>
    /// Calculates the absolute value of the difference of two values
    /// </summary>
    public static float Distance(float value1, float value2)
    {
        return Math.Abs(value1 - value2);
    }
    
    /// <summary>
    /// Performs a Hermite spline interpolation
    /// </summary>
    public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
    {
        double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
        double sCubed = s * s * s;
        double sSquared = s * s;

        if (amount == 0f)
            result = value1;
        else if (amount == 1f)
            result = value2;
        else
            result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                t1 * s +
                v1;
        return (float)result;
    }
    
    /// <summary>
    /// Interpolates between two values using a cubic equation
    /// </summary>
    public static float SmoothStep(float value1, float value2, float amount)
    {
        float result = Clamp(amount, 0f, 1f);
        result = Hermite(value1, 0f, value2, 0f, result);
        return result;
    }
}
