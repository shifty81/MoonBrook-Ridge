using System;

namespace MoonBrookEngine.Audio;

/// <summary>
/// Represents a sound effect that can be played
/// Similar to MonoGame's SoundEffect
/// </summary>
public class SoundEffect : IDisposable
{
    private readonly AudioEngine _audioEngine;
    private readonly uint _buffer;
    private bool _isDisposed;
    
    public string Name { get; }
    
    internal SoundEffect(AudioEngine audioEngine, uint buffer, string name)
    {
        _audioEngine = audioEngine;
        _buffer = buffer;
        Name = name;
    }
    
    /// <summary>
    /// Create a sound effect from WAV data
    /// </summary>
    public static SoundEffect? FromWavData(AudioEngine audioEngine, byte[] wavData, string name = "")
    {
        // Parse WAV header (simplified - assumes standard format)
        if (wavData.Length < 44)
        {
            Console.WriteLine("Invalid WAV data - too short");
            return null;
        }
        
        // Check RIFF header
        if (wavData[0] != 'R' || wavData[1] != 'I' || wavData[2] != 'F' || wavData[3] != 'F')
        {
            Console.WriteLine("Invalid WAV data - not RIFF");
            return null;
        }
        
        // Check WAVE format
        if (wavData[8] != 'W' || wavData[9] != 'A' || wavData[10] != 'V' || wavData[11] != 'E')
        {
            Console.WriteLine("Invalid WAV data - not WAVE");
            return null;
        }
        
        // Parse format chunk
        int channels = BitConverter.ToInt16(wavData, 22);
        int sampleRate = BitConverter.ToInt32(wavData, 24);
        int bitsPerSample = BitConverter.ToInt16(wavData, 34);
        
        // Find data chunk
        int dataOffset = 44;
        while (dataOffset < wavData.Length - 8)
        {
            if (wavData[dataOffset] == 'd' && wavData[dataOffset + 1] == 'a' &&
                wavData[dataOffset + 2] == 't' && wavData[dataOffset + 3] == 'a')
            {
                break;
            }
            dataOffset++;
        }
        
        if (dataOffset >= wavData.Length - 8)
        {
            Console.WriteLine("Invalid WAV data - no data chunk");
            return null;
        }
        
        int dataSize = BitConverter.ToInt32(wavData, dataOffset + 4);
        int dataStart = dataOffset + 8;
        
        // Extract PCM data
        byte[] pcmData = new byte[dataSize];
        Array.Copy(wavData, dataStart, pcmData, 0, System.Math.Min(dataSize, wavData.Length - dataStart));
        
        // Create buffer
        uint buffer = audioEngine.CreateBuffer(pcmData, channels, sampleRate, bitsPerSample);
        if (buffer == 0)
        {
            Console.WriteLine("Failed to create audio buffer");
            return null;
        }
        
        return new SoundEffect(audioEngine, buffer, name);
    }
    
    /// <summary>
    /// Play the sound effect once
    /// </summary>
    public void Play()
    {
        Play(1.0f, 0f, 0f);
    }
    
    /// <summary>
    /// Play the sound effect with volume, pitch, and pan
    /// </summary>
    public void Play(float volume, float pitch, float pan)
    {
        if (_isDisposed) return;
        
        var source = _audioEngine.CreateSource();
        if (source == 0) return;
        
        _audioEngine.SetSourceBuffer(source, _buffer);
        _audioEngine.SetSourceVolume(source, volume);
        _audioEngine.SetSourcePitch(source, 1.0f + pitch);
        _audioEngine.SetSourcePosition(source, pan, 0, 0);
        _audioEngine.SetSourceLooping(source, false);
        _audioEngine.Play(source);
        
        // Note: In a full implementation, we'd track this source and delete it when done
        // For now, sources will leak - this is a known limitation for simple one-off playback
    }
    
    /// <summary>
    /// Create an instance for more control
    /// </summary>
    public SoundEffectInstance CreateInstance()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SoundEffect));
        
        return new SoundEffectInstance(_audioEngine, _buffer, Name);
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        
        _audioEngine.DeleteBuffer(_buffer);
        _isDisposed = true;
    }
}
