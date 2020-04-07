using System;
using System.IO;

namespace BinaryBlister
{
    /// <summary>
    /// Represents a beatmap entry in the playlist
    /// </summary>
    public abstract class Beatmap
    {
        /// <summary>
        /// Date this entry was added to the playlist
        /// </summary>
        public DateTimeOffset DateAdded;

        internal Beatmap()
        {
            DateAdded = DateTimeOffset.Now;
        }

        internal Beatmap(BinaryBlisterReader reader)
        {
            var timestamp = (long) reader.ReadUInt64();
            DateAdded = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }

        internal static Beatmap Read(BinaryBlisterReader reader)
        {
            var type = reader.ReadByte();
            return type switch
            {
                KeyBeatmap.Type => new KeyBeatmap(reader),
                HashBeatmap.Type => new HashBeatmap(reader),
                ZipBeatmap.Type => new ZipBeatmap(reader),
                LevelIdBeatmap.Type => new LevelIdBeatmap(reader),
                _ => throw new InvalidBeatmapTypeException(),
            };
        }

        internal abstract void Write(BinaryBlisterWriter writer);
    }

    /// <summary>
    /// Represents a map entry identified by a Beatsaver key
    /// </summary>
    public class KeyBeatmap : Beatmap
    {
        internal const byte Type = 0;

        /// <summary>
        /// Beatsaver key
        /// </summary>
        public int Key;

        /// <summary>
        /// Creates a new key identified beatmap
        /// </summary>
        /// <param name="key">Beatsaver key</param>
        public KeyBeatmap(int key)
        {
            Key = key;
        }

        /// <summary>
        /// Creates a new key identified beatmap
        /// </summary>
        /// <param name="key">Hex Beatsaver key</param>
        public KeyBeatmap(string key)
        {
            Key = (int) Convert.ToUInt32(key, 16);
        }

        /// <summary>
        /// Hex Beatsaver key
        /// </summary>
        /// <returns></returns>
        public string KeyString()
        {
            return Convert.ToString(Key, 16);
        }

        internal KeyBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Key = (int) reader.ReadUInt32();
        }

        internal override void Write(BinaryBlisterWriter writer)
        {
            writer.Write(Type);
            writer.Write(Key);
        }
    }

    /// <summary>
    /// Represents a map entry identified by a beatmap hash
    /// </summary>
    public class HashBeatmap : Beatmap
    {
        internal const byte Type = 1;

        /// <summary>
        /// Beatmap hash
        /// </summary>
        public byte[] Hash;

        /// <summary>
        /// Creates a new hash identified beatmap
        /// </summary>
        /// <param name="hash">Beatmap hash</param>
        public HashBeatmap(byte[] hash)
        {
            Hash = hash;
        }

        /// <summary>
        /// Creates a new hash identified beatmap
        /// </summary>
        /// <param name="hash">Hex beatmap hash</param>
        public HashBeatmap(string hash)
        {
            Hash = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                Hash[i] = Convert.ToByte(hash.Substring(i * 2, 2), 16);
            }
        }

        /// <summary>
        /// Hex beatmap hash
        /// </summary>
        /// <returns></returns>
        public string HashString()
        {
            return BitConverter.ToString(Hash);
        }

        internal HashBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Hash = reader.ReadBytes(20);
        }

        internal override void Write(BinaryBlisterWriter writer)
        {
            writer.Write(Type);
            writer.Write(Hash);
        }
    }

    /// <summary>
    /// Represents a map entry with its whole zip contained inside the playlist
    /// </summary>
    public class ZipBeatmap : Beatmap
    {
        internal const byte Type = 2;

        /// <summary>
        /// Binary contents of the zip
        /// </summary>
        public byte[] Zip;

        /// <summary>
        /// Creates a new self-contained beatmap
        /// </summary>
        /// <param name="zip">Zip</param>
        public ZipBeatmap(byte[] zip)
        {
            Zip = zip;
        }

        /// <summary>
        /// Creates a new self-contained beatmap
        /// </summary>
        /// <param name="zip">Zip stream</param>
        public ZipBeatmap(Stream zip)
        {
            var length = (int) zip.Length;
            Zip = new byte[length];
            zip.Read(Zip, 0, length);
        }

        /// <summary>
        /// Creates a new self-contained beatmap
        /// </summary>
        /// <param name="zipFilename">Zip file name</param>
        public ZipBeatmap(string zipFilename)
        {
            Zip = File.ReadAllBytes(zipFilename);
        }

        internal ZipBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Zip = reader.ReadBytes();
        }

        internal override void Write(BinaryBlisterWriter writer)
        {
            writer.Write(Type);
            writer.WriteBytes(Zip);
        }
    }

    /// <summary>
    /// Represents a map entry identified by a level ID
    /// </summary>
    public class LevelIdBeatmap : Beatmap
    {
        internal const byte Type = 3;

        /// <summary>
        /// Beatmap level ID
        /// </summary>
        public string LevelID;

        /// <summary>
        /// Creates a new level ID identified beatmap
        /// </summary>
        /// <param name="levelID">Beatmap level ID</param>
        public LevelIdBeatmap(string levelID)
        {
            LevelID = levelID;
        }

        internal LevelIdBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            LevelID = reader.ReadShortString();
        }

        internal override void Write(BinaryBlisterWriter writer)
        {
            writer.Write(Type);
            writer.WriteShortString(LevelID);
        }
    }
}
