namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// 4x4 matrix for 2D/3D transformations (MonoGame compatibility)
/// </summary>
public struct Matrix
{
    // Matrix elements (row-major order)
    public float M11, M12, M13, M14;
    public float M21, M22, M23, M24;
    public float M31, M32, M33, M34;
    public float M41, M42, M43, M44;

    /// <summary>
    /// Identity matrix
    /// </summary>
    public static readonly Matrix Identity = new Matrix
    {
        M11 = 1f, M22 = 1f, M33 = 1f, M44 = 1f
    };

    /// <summary>
    /// Create a translation matrix
    /// </summary>
    public static Matrix CreateTranslation(float x, float y, float z)
    {
        var result = Identity;
        result.M41 = x;
        result.M42 = y;
        result.M43 = z;
        return result;
    }

    /// <summary>
    /// Create a translation matrix from a vector
    /// </summary>
    public static Matrix CreateTranslation(Vector2 position)
    {
        return CreateTranslation(position.X, position.Y, 0);
    }

    /// <summary>
    /// Create a scale matrix
    /// </summary>
    public static Matrix CreateScale(float x, float y, float z)
    {
        var result = Identity;
        result.M11 = x;
        result.M22 = y;
        result.M33 = z;
        return result;
    }

    /// <summary>
    /// Create a scale matrix from a scalar
    /// </summary>
    public static Matrix CreateScale(float scale)
    {
        return CreateScale(scale, scale, scale);
    }

    /// <summary>
    /// Create a rotation matrix around Z axis (2D rotation)
    /// </summary>
    public static Matrix CreateRotationZ(float radians)
    {
        var result = Identity;
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);
        result.M11 = cos;
        result.M12 = sin;
        result.M21 = -sin;
        result.M22 = cos;
        return result;
    }

    /// <summary>
    /// Create an orthographic projection matrix
    /// </summary>
    public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        var result = Identity;
        result.M11 = 2f / (right - left);
        result.M22 = 2f / (top - bottom);
        result.M33 = 1f / (zNearPlane - zFarPlane);
        result.M41 = (left + right) / (left - right);
        result.M42 = (top + bottom) / (bottom - top);
        result.M43 = zNearPlane / (zNearPlane - zFarPlane);
        return result;
    }

    /// <summary>
    /// Multiply two matrices
    /// </summary>
    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
        Matrix result;
        
        // Row 1
        result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
        result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
        result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
        result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
        
        // Row 2
        result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
        result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
        result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
        result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
        
        // Row 3
        result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
        result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
        result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
        result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
        
        // Row 4
        result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
        result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
        result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
        result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        
        return result;
    }

    /// <summary>
    /// Transform a vector by a matrix
    /// </summary>
    public static Vector2 Transform(Vector2 position, Matrix matrix)
    {
        return new Vector2(
            position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41,
            position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42
        );
    }
}
