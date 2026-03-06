using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTorrent;
using NLog;

namespace NzbDrone.Core.MediaFiles.TorrentInfo
{
    public interface ITorrentFileInfoReader
    {
        string GetHashFromTorrentFile(byte[] fileContents);
        IReadOnlyList<string> GetFileNamesFromTorrentFile(byte[] fileContents);
    }

    public class TorrentFileInfoReader : ITorrentFileInfoReader
    {
        private readonly Logger _logger;

        public TorrentFileInfoReader(Logger logger)
        {
            _logger = logger;
        }

        public string GetHashFromTorrentFile(byte[] fileContents)
        {
            try
            {
                return Torrent.Load(fileContents).InfoHashes.V1OrV2.ToHex();
            }
            catch
            {
                _logger.Trace("Invalid torrent file contents: {0}", Encoding.ASCII.GetString(fileContents));
                throw;
            }
        }

        public IReadOnlyList<string> GetFileNamesFromTorrentFile(byte[] fileContents)
        {
            try
            {
                var torrent = Torrent.Load(fileContents);
                return torrent.Files.Select(f => f.Path).ToList();
            }
            catch
            {
                _logger.Trace("Invalid torrent file contents: {0}", Encoding.ASCII.GetString(fileContents));
                throw;
            }
        }
    }
}
