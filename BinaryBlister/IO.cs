using System;
using System.IO;
using System.IO.Compression;

namespace BinaryBlister
{
    internal class BinaryBlisterReader : BinaryReader
    {
        public BinaryBlisterReader(GZipStream input) : base(input, Playlist.Encoding)
        {
            if (BitConverter.IsLittleEndian != Playlist.LittleEndian)
            {
                throw new UnsupportedEndiannessException();
            }
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

        public DateTimeOffset ReaDateTimeOffset()
        {
            var timestamp = (long) ReadUInt64();
            return DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }
    }

    internal class BinaryBlisterWriter : BinaryWriter
    {
        public BinaryBlisterWriter(GZipStream output) : base(output, Playlist.Encoding)
        {
            if (BitConverter.IsLittleEndian != Playlist.LittleEndian)
            {
                throw new UnsupportedEndiannessException();
            }
        }

        public void WriteShortString(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Write((byte) 0);
                return;
            }

#pragma warning disable CS8602 // Fix your shit Microsoft, this can't be null
            var bytes = Playlist.Encoding.GetBytes(s);
#pragma warning restore CS8602
            Write((byte) bytes.Length);
            Write(bytes);
        }

        public void WriteLongString(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Write((ushort) 0);
                return;
            }

#pragma warning disable CS8602 // Fix your shit Microsoft, this can't be null
            var bytes = Playlist.Encoding.GetBytes(s);
#pragma warning restore CS8602
            Write((ushort) bytes.Length);
            Write(bytes);
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

        public void WriteDateTimeOffset(DateTimeOffset dto)
        {
            var timestamp = dto.ToUnixTimeSeconds();
            Write((ulong) timestamp);
        }
    }
}
