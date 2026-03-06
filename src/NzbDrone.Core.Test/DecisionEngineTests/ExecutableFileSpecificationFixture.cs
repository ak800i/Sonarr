using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.DecisionEngineTests
{
    [TestFixture]
    public class ExecutableFileSpecificationFixture : CoreTest<ExecutableFileSpecification>
    {
        private RemoteEpisode _remoteEpisode;

        [SetUp]
        public void Setup()
        {
            _remoteEpisode = new RemoteEpisode
            {
                Release = new ReleaseInfo
                {
                    Title = "Series.Title.S01E01.720p.BluRay.x264",
                    DownloadProtocol = DownloadProtocol.Torrent
                }
            };
        }

        [Test]
        public void should_return_true_for_normal_release()
        {
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_return_true_if_release_is_null()
        {
            _remoteEpisode.Release = null;
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeTrue();
        }

        [TestCase("Series.Title.S01E01.HDTV.x264-LOL")]
        [TestCase("Series Title S02E05 720p WEB-DL.mkv")]
        [TestCase("Series.Name.S01E01.1080p.BluRay.DTS.x264")]
        [TestCase("Series.Title.Complete.Season.1.720p.BluRay")]
        [TestCase("Some.Series.S01E01.Execute.Plan.1080p.WEB")]
        public void should_return_true_for_normal_titles(string title)
        {
            _remoteEpisode.Release.Title = title;
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeTrue();
        }

        [TestCase("Series.Title.S01E01.HDTV.x264-LOL.exe")]
        [TestCase("Series.Title.S01E01.720p.BluRay.x264.exe ")]
        [TestCase("Series.Title.S01E01.HDTV.x264.setup.exe")]
        [TestCase("Series.Title.S01E01.720p.EXE.BluRay")]
        public void should_return_false_for_exe_files(string title)
        {
            _remoteEpisode.Release.Title = title;
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeFalse();
        }

        [TestCase("Series.Title.S01E01.HDTV.x264-LOL.lnk")]
        [TestCase("Series.Title.S01E01.720p.BluRay.x264.lnk ")]
        [TestCase("Series.Title.S01E01.HDTV.x264.setup.lnk")]
        [TestCase("Series.Title.S01E01.720p.LNK.BluRay")]
        public void should_return_false_for_lnk_files(string title)
        {
            _remoteEpisode.Release.Title = title;
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_be_case_insensitive_for_exe()
        {
            _remoteEpisode.Release.Title = "Series.Title.S01E01.EXE";
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_be_case_insensitive_for_lnk()
        {
            _remoteEpisode.Release.Title = "Series.Title.S01E01.LNK";
            Subject.IsSatisfiedBy(_remoteEpisode, new()).Accepted.Should().BeFalse();
        }
    }
}
