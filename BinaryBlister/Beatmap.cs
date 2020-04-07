using System;

namespace BinaryBlister
{
    public abstract class Beatmap
    {
        public DateTimeOffset DateAdded;

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
    }

    public class KeyBeatmap : Beatmap
    {
        internal const byte Type = 0;
        public uint Key;

        internal KeyBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Key = reader.ReadUInt32();
        }
    }

    public class HashBeatmap : Beatmap
    {
        internal const byte Type = 1;
        public byte[] Hash;

        internal HashBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Hash = reader.ReadBytes(20);
        }
    }

    public class ZipBeatmap : Beatmap
    {
        internal const byte Type = 2;
        public byte[] Zip;

        internal ZipBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            Zip = reader.ReadBytes();
        }
    }

    public class LevelIdBeatmap : Beatmap
    {
        internal const byte Type = 3;
        public string LevelID;

        internal LevelIdBeatmap(BinaryBlisterReader reader) : base(reader)
        {
            LevelID = reader.ReadShortString();
        }
    }
}
