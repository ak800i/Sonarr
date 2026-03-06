using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using NzbDrone.Core.Configuration;

namespace NzbDrone.Core.Download
{
    public interface IExecutableFileValidator
    {
        bool ContainsExecutableFiles(IEnumerable<string> filePaths);
        IReadOnlyList<string> GetExecutableFiles(IEnumerable<string> filePaths);
    }

    public class ExecutableFileValidator : IExecutableFileValidator
    {
        // Match files ending with .exe or .lnk (case insensitive)
        private static readonly Regex ExecutableFileRegex = new Regex(@"\.(exe|lnk)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public ExecutableFileValidator(IConfigService configService, Logger logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public bool ContainsExecutableFiles(IEnumerable<string> filePaths)
        {
            if (!_configService.RejectReleasesWithExecutableFiles)
            {
                return false;
            }

            return filePaths.Any(path => ExecutableFileRegex.IsMatch(path));
        }

        public IReadOnlyList<string> GetExecutableFiles(IEnumerable<string> filePaths)
        {
            return filePaths
                .Where(path => ExecutableFileRegex.IsMatch(path))
                .Select(path => Path.GetFileName(path))
                .ToList();
        }
    }
}
