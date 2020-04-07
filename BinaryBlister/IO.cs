using System.IO;

namespace BinaryBlister
{
    internal class BinaryBlisterReader : BinaryReader
    {
        public BinaryBlisterReader(Stream input) : base(input)
        {
        }

        public string ReadShortString()
        {
            var length = ReadByte();
            if (length == 0)
            {
                return "";
            }
            var bytes = ReadBytes(length);
            return Playlist.Encoding.GetString(bytes);
        }

        public string? ReadOptionalLongString()
        {
            var length = ReadUInt16();
            if (length == 0)
            {
                return null;
            }
            var bytes = ReadBytes(length);
            return Playlist.Encoding.GetString(bytes);
        }
    }
}
