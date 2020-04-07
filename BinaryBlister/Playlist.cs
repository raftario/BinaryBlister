using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BinaryBlister
{
    public class Playlist
    {
        internal const string MagicNumberString = "Blist.v3";
        internal static readonly Encoding Encoding = Encoding.UTF8;

        internal static readonly byte[] MagicNumber = Encoding.GetBytes(MagicNumberString);

        public string Title;
        public string Author;
        public string? Description;
        public byte[]? Cover;
        public List<Beatmap> Maps;

        public Playlist(string title, string author, string? description = null, byte[]? cover = null)
        {
            Title = title;
            Author = author;
            Description = description;
            Cover = cover;
            Maps = new List<Beatmap>();
        }

        public Playlist(Stream stream)
        {
            ReadMagicNumber(stream);

            using var gzStream = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new BinaryBlisterReader(gzStream);

            Title = reader.ReadShortString();
            Author = reader.ReadShortString();
            Description = reader.ReadOptionalLongString();
            Cover = reader.ReadOptionalBytes();

            var mapCount = (int) reader.ReadUInt32();
            var maps = new List<Beatmap>(mapCount);
            for (var i = 0; i < mapCount; i++)
            {
                maps[i] = Beatmap.Read(reader);
            }
            Maps = maps;
        }

        public void Write(Stream stream)
        {
            WriteMagicNumber(stream);

            using var gzStream = new GZipStream(stream, CompressionMode.Compress);
            using var writer = new BinaryBlisterWriter(gzStream);

            writer.WriteShortString(Title);
            writer.WriteShortString(Author);
            writer.WriteLongString(Description);
            writer.WriteBytes(Cover);

            writer.Write((uint) Maps.Count);
            foreach (var map in Maps)
            {
            }
        }

        private static void ReadMagicNumber(Stream stream)
        {
            foreach (var b in MagicNumber)
            {
                if (stream.ReadByte() != b)
                {
                    throw new InvalidMagicNumberException();
                }
            }
        }

        private static void WriteMagicNumber(Stream stream)
        {
            foreach (var b in MagicNumber)
            {
                stream.WriteByte(b);
            }
        }
    }
}
