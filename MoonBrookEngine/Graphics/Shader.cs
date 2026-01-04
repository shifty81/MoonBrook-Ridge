using Silk.NET.OpenGL;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// OpenGL shader program
/// </summary>
public class Shader : IDisposable
{
    private GL _gl;
    private uint _program;
    private bool _disposed;
    
    public uint Program => _program;
    
    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;
        
        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);
        
        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);
        _gl.LinkProgram(_program);
        
        // Check for linking errors
        _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int status);
        if (status == 0)
        {
            string log = _gl.GetProgramInfoLog(_program);
            throw new Exception($"Shader program linking failed: {log}");
        }
        
        // Clean up shaders (they're now linked into the program)
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }
    
    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);
        
        // Check for compilation errors
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
        {
            string log = _gl.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation failed ({type}): {log}");
        }
        
        return shader;
    }
    
    public void Use()
    {
        _gl.UseProgram(_program);
    }
    
    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(_program, name);
        if (location >= 0)
        {
            _gl.Uniform1(location, value);
        }
    }
    
    public void SetUniform(string name, float value)
    {
        int location = _gl.GetUniformLocation(_program, name);
        if (location >= 0)
        {
            _gl.Uniform1(location, value);
        }
    }
    
    public unsafe void SetUniform(string name, System.Numerics.Matrix4x4 value)
    {
        int location = _gl.GetUniformLocation(_program, name);
        if (location >= 0)
        {
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }
    }
    
    public unsafe void SetUniform(string name, System.Numerics.Vector4 value)
    {
        int location = _gl.GetUniformLocation(_program, name);
        if (location >= 0)
        {
            _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _gl.DeleteProgram(_program);
            _disposed = true;
        }
    }
}
