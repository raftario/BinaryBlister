namespace BinaryBlister.Conversion.Types
{
    internal static class FlagUtils
    {
        internal static bool HasFlag(ConversionFlags a, ConversionFlags b) => (a & b) == b;
    }
}
