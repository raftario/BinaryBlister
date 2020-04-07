using Xunit;
using Xunit.Abstractions;

namespace BinaryBlister.Test
{
    public class UnitTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WriteAndRead()
        {
            var playlist = new Playlist("Test", "Me", "A test playlist", new byte[] {0, 1, 2, 3});
            playlist.Maps.Add(new KeyBeatmap(1234));
            playlist.Maps.Add(new HashBeatmap(new byte[20]));
            playlist.Maps.Add(new ZipBeatmap(new byte[]{0, 1, 2, 3}));
            playlist.Maps.Add(new LevelIDBeatmap("Test"));

            var encoded = playlist.Write();

            var decoded = new Playlist(encoded);

            Assert.True(decoded.Title == playlist.Title);
            Assert.True(decoded.Author == playlist.Author);
            Assert.True(decoded.Description == playlist.Description);
            Assert.True(decoded.Cover.Length == playlist.Cover.Length);
            Assert.True(decoded.Maps.Count == playlist.Maps.Count);
        }
    }
}
