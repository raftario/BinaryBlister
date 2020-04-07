using System;

namespace BinaryBlister.Conversion.Types
{
    [Flags]
    public enum ConversionFlags : byte
    {
        Strict = 0,
        IgnoreInvalidHashes = 1,
        IgnoreInvalidKeys = 2,
        IgnoreInvalidCover = 3,
        Loose = 7,
        Default = 4
    }
}
