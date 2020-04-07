using System;

namespace BinaryBlister
{
    /// <summary>
    /// The exception thrown when trying to read a playlist with an invalid magic number
    /// </summary>
    public class InvalidMagicNumberException : Exception
    {
    }

    /// <summary>
    /// The exception thrown when trying to read a playlist containing a map with an invalid type
    /// </summary>
    public class InvalidBeatmapTypeException : Exception
    {
    }

    /// <summary>
    /// The exception thrown when trying to read or write a playlist using a big-endian CPU
    /// </summary>
    public class UnsupportedEndiannessException : Exception
    {
    }
}
