using System;
using System.Globalization;
using System.IO;

namespace BinaryBlister
{
    public abstract class Beatmap
    {
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

    public class KeyBeatmap : Beatmap
    {
        internal const byte Type = 0;
        public uint Key;

        public KeyBeatmap(uint key)
        {
            Key = key;
        }

        public KeyBeatmap(string key)
        {
            Key = Convert.ToUInt32(key, 16);
        }

        internal KeyBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Key = reader.ReadUInt32();
        }

        internal override void Write(BinaryBlisterWriter writer)
        {
            writer.Write(Type);
            writer.Write(Key);
        }
    }

    public class HashBeatmap : Beatmap
    {
        internal const byte Type = 1;
        public byte[] Hash;

        public HashBeatmap(byte[] hash)
        {
            Hash = hash;
        }

        public HashBeatmap(string hash)
        {
            Hash = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                Hash[i] = Convert.ToByte(hash.Substring(i * 2, 2), 16);
            }
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

    public class ZipBeatmap : Beatmap
    {
        internal const byte Type = 2;
        public byte[] Zip;

        public ZipBeatmap(byte[] zip)
        {
            Zip = zip;
        }

        public ZipBeatmap(Stream zip)
        {
            var length = (int) zip.Length;
            Zip = new byte[length];
            zip.Read(Zip, 0, length);
        }

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

    public class LevelIdBeatmap : Beatmap
    {
        internal const byte Type = 3;
        public string LevelID;

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
