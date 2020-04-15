using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using BinaryBlister.Conversion.Types;

namespace BinaryBlister.Conversion
{
    public static class PlaylistConverter
    {
        internal static JsonSerializer _serializer = new JsonSerializer();

        public static LegacyPlaylist DeserializeLegacyPlaylist(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return DeserializeLegacyPlaylist(ms);
            }
        }

        public static LegacyPlaylist DeserializeLegacyPlaylist(string text)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return DeserializeLegacyPlaylist(ms);
            }
        }

        public static LegacyPlaylist DeserializeLegacyPlaylist(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return DeserializeLegacyPlaylist(sr);
            }
        }

        public static LegacyPlaylist DeserializeLegacyPlaylist(StreamReader reader)
        {
            using (JsonReader r = new JsonTextReader(reader))
            {
                return _serializer.Deserialize<LegacyPlaylist>(r);
            }
        }

        public static Playlist ConvertLegacyPlaylist(LegacyPlaylist legacy, ConversionFlags flags = ConversionFlags.Default)
        {
            bool ignoreInvalidHashes = FlagUtils.HasFlag(flags, ConversionFlags.IgnoreInvalidHashes);
            bool ignoreInvalidKeys = FlagUtils.HasFlag(flags, ConversionFlags.IgnoreInvalidKeys);
            bool ignoreInvalidCover = FlagUtils.HasFlag(flags, ConversionFlags.IgnoreInvalidCover);
            Playlist playlist = new Playlist(legacy.PlaylistTitle, legacy.PlaylistAuthor, legacy.PlaylistDescription);
            try
            {
                byte[] cover = Utils.ParseBase64Image(legacy.Image);
                string mimeType = MimeType.GetMimeType(cover);

                if (mimeType != MimeType.PNG && mimeType != MimeType.JPG)
                    throw new InvalidCoverException(mimeType);
                playlist.Cover = cover;
            }
            catch (InvalidBase64Exception ex)
            {
                if (ignoreInvalidCover) playlist.Cover = null;
                else throw ex;
            }
            foreach (var song in legacy.Songs)
            {
                Beatmap map = null;
                if (!string.IsNullOrEmpty(song.Hash))
                    map = new HashBeatmap(song.Hash);
                else if (!string.IsNullOrEmpty(song.Key))
                    map = new KeyBeatmap(song.Key);
                else if (!string.IsNullOrEmpty(song.LevelID));

                if (song.Hash != null)
                {
                    string hash = song.Hash.ToLower();
                    bool isValid = Utils.ValidHash(hash);
                    if (isValid == false)
                    {
                        if (ignoreInvalidHashes) continue;
                        else throw new InvalidMapHashException(hash);
                    }
                    map = new HashBeatmap(song.Hash);
                }
                else if (song.Key != null)
                {
                    string key = Utils.ParseKey(song.Key);
                    if (key == null)
                    {
                        if (ignoreInvalidKeys) continue;
                        else throw new InvalidMapKeyException(song.Key);
                    }
                    map = new KeyBeatmap(song.Key);
                }
                else if (song.LevelID != null)
                {
                    map = new LevelIDBeatmap(song.LevelID);
                }
                if (map != null)
                {
                    map.DateAdded = DateTime.Now;
                    playlist.Maps.Add(map);
                }
            }
            return playlist;
        }
    }
}
