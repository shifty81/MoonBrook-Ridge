using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles.Animations;

/// <summary>
/// Auto-generated tile animations from GameMaker tileset: tileset_sunnysideworld
/// This file is auto-generated. Do not edit manually.
/// </summary>
public static class SunnysideworldAnimations
{
    /// <summary>
    /// Tile animation sequences
    /// Key: Animation name, Value: Array of tile IDs representing frames
    /// </summary>
    public static readonly Dictionary<string, int[]> Animations = new Dictionary<string, int[]>
    {
        { "animation_1", new int[] { 147, 147, 147, 147, 150, 150, 150, 150 } },
        { "animation_2", new int[] { 148, 148, 148, 148, 151, 151, 151, 151 } },
        { "animation_3", new int[] { 149, 149, 149, 149, 152, 152, 152, 152 } },
        { "animation_4", new int[] { 211, 211, 211, 211, 214, 214, 214, 214 } },
        { "animation_5", new int[] { 213, 213, 213, 213, 216, 216, 216, 216 } },
        { "animation_6", new int[] { 275, 275, 275, 275, 278, 278, 278, 278 } },
        { "animation_7", new int[] { 276, 276, 276, 276, 279, 279, 279, 279 } },
        { "animation_8", new int[] { 277, 277, 277, 277, 280, 280, 280, 280 } },
        { "animation_9", new int[] { 340, 340, 340, 340, 343, 343, 343, 343 } },
        { "animation_10", new int[] { 339, 339, 339, 339, 342, 342, 342, 342 } },
        { "animation_11", new int[] { 341, 341, 341, 341, 344, 344, 344, 344 } },
        { "animation_12", new int[] { 403, 403, 403, 403, 406, 406, 406, 406 } },
        { "animation_13", new int[] { 405, 405, 405, 405, 408, 408, 408, 408 } },
        { "animation_14", new int[] { 478, 479, 480, 481 } },
        { "animation_15", new int[] { 212, 212, 212, 212, 215, 215, 215, 215 } },
        { "animation_16", new int[] { 153, 153, 153, 153, 217, 217, 217, 217 } },
        { "animation_17", new int[] { 414, 415, 416, 417 } },
        { "animation_18", new int[] { 91, 92, 93, 94 } },
        { "animation_19", new int[] { 155, 156, 157, 158 } },
        { "animation_20", new int[] { 219, 220, 221, 222 } },
        { "animation_21", new int[] { 283, 284, 285, 286 } },
        { "animation_22", new int[] { 410, 411, 412, 413 } },
        { "animation_23", new int[] { 542, 543, 544, 545 } },
        { "animation_24", new int[] { 1276, 1276, 1276, 1276, 1277, 1277, 1277, 1277 } },
        { "animation_25", new int[] { 1419, 1419, 1419, 1419, 1420, 1420, 1420, 1420, 1421, 1421, 1421, 1421, 1422, 1422, 1422, 1422 } },
    };
    
    /// <summary>
    /// Get the frame for an animation at a specific time
    /// </summary>
    /// <param name="animationName">Name of the animation</param>
    /// <param name="elapsedTime">Elapsed time in seconds</param>
    /// <param name="frameRate">Frames per second (default 5)</param>
    /// <returns>Tile ID for the current frame</returns>
    public static int GetAnimationFrame(string animationName, float elapsedTime, float frameRate = 5.0f)
    {
        if (!Animations.TryGetValue(animationName, out int[]? frames) || frames == null)
            return -1;
        
        int frameIndex = (int)(elapsedTime * frameRate) % frames.Length;
        return frames[frameIndex];
    }
}
