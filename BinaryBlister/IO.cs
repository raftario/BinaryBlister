using System.IO;
using System.IO.Compression;

namespace BinaryBlister
{
    internal class BinaryBlisterReader : BinaryReader
    {
        public BinaryBlisterReader(GZipStream input) : base(input, Playlist.Encoding)
        {
        }

        public string ReadShortString()
        {
            var length = ReadByte();
            return length == 0 ? "" : Playlist.Encoding.GetString(ReadBytes(length));
        }

        public string? ReadOptionalLongString()
        {
            var length = ReadUInt16();
            return length == 0 ? null : Playlist.Encoding.GetString(ReadBytes(length));
        }

        public byte[] ReadBytes()
        {
            var length = (int) ReadUInt32();
            return length == 0 ? new byte[0] : ReadBytes(length);
        }

        public byte[]? ReadOptionalBytes()
        {
            var length = (int) ReadUInt32();
            return length == 0 ? null : ReadBytes(length);
        }
    }

    internal class BinaryBlisterWriter : BinaryWriter
    {
        public BinaryBlisterWriter(GZipStream output) : base(output, Playlist.Encoding)
        {
        }

        public void WriteShortString(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Write((byte) 0);
                return;
            }

            Write((byte) s.Length);
            Write(Playlist.Encoding.GetBytes(s));
        }

        public void WriteLongString(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Write((ushort) 0);
                return;
            }

            Write((ushort) s.Length);
            Write(Playlist.Encoding.GetBytes(s));
        }

        public void WriteBytes(byte[]? b)
        {
            if (b == null || b.Length == 0)
            {
                Write((uint) 0);
                return;
            }

            Write((uint) b.Length);
            Write(b);
        }
    }
}
