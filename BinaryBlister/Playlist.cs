using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BinaryBlister
{
    /// <summary>
    /// Represents a playlist
    /// </summary>
    public class Playlist
    {
        internal const string MagicNumberString = "Blist.v3";
        internal const bool LittleEndian = true;
        internal static readonly Encoding Encoding = Encoding.UTF8;

        internal static readonly byte[] MagicNumber = Encoding.GetBytes(MagicNumberString);

        /// <summary>
        /// Playlist title
        /// </summary>
        public string Title;

        /// <summary>
        /// Playlist author
        /// </summary>
        public string Author;

        /// <summary>
        /// Optional playlist description
        /// </summary>
        public string? Description;

        /// <summary>
        /// Optional playlist cover
        /// </summary>
        public byte[]? Cover;

        /// <summary>
        /// Maps
        /// </summary>
        public List<Beatmap> Maps;

        /// <summary>
        /// Creates a new empty playlist with the specified metadata
        /// </summary>
        /// <param name="title">Playlist title</param>
        /// <param name="author">Playlist author</param>
        /// <param name="description">Optional playlist description</param>
        /// <param name="cover">Optional playlist cover</param>
        public Playlist(string title, string author, string? description = null, byte[]? cover = null)
        {
            Title = title;
            Author = author;
            Description = description;
            Cover = cover;
            Maps = new List<Beatmap>();
        }

        /// <summary>
        /// Reads a playlist from a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="dispose">Whether to dispose of the stream after reading is finished</param>
        public Playlist(Stream stream, bool dispose = false)
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
                maps.Add(Beatmap.Read(reader));
            }
            Maps = maps;

            if (dispose)
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Reads a playlist from a buffer
        /// </summary>
        /// <param name="buffer">Binary buffer</param>
        public Playlist(byte[] buffer) : this(new MemoryStream(buffer), true)
        {
        }

        /// <summary>
        /// Reads a playlist from a file
        /// </summary>
        /// <param name="filename">File name</param>
        public Playlist(string filename) : this(new FileStream(filename, FileMode.Open), true)
        {
        }

        /// <summary>
        /// Writes the playlist to a stream
        /// </summary>
        /// <param name="stream">Stream</param>
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
                map.Write(writer);
            }
        }

        /// <summary>
        /// Writes the playlist to a buffer and returns it
        /// </summary>
        /// <returns>Binary buffer</returns>
        public byte[] Write()
        {
            using var stream = new MemoryStream();
            Write(stream);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the playlist to a file
        /// </summary>
        /// <param name="playlistFilename">File name</param>
        public void Write(string playlistFilename)
        {
            using var stream = new FileStream(playlistFilename, FileMode.Create);
            Write(stream);
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
