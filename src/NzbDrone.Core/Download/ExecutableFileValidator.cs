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
        // Match common executable/script file extensions that could be security risks
        // Includes: Windows executables, scripts, installers, and shortcuts
        private static readonly Regex ExecutableFileRegex = new Regex(
            @"\.(exe|lnk|bat|cmd|com|scr|pif|vbs|vbe|js|jse|ws|wsf|wsc|wsh|ps1|ps1xml|ps2|ps2xml|psc1|psc2|msi|msp|mst|jar|hta|cpl|reg|inf|dll)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
