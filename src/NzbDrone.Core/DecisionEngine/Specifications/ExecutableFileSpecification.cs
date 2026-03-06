using System.Text.RegularExpressions;
using NLog;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class ExecutableFileSpecification : IDownloadDecisionEngineSpecification
    {
        // Match common executable/script file extensions in release titles
        // Uses word boundary \b to avoid false positives (e.g., "exec" in title)
        private static readonly Regex ExecutableFileRegex = new Regex(
            @"\.(exe|lnk|bat|cmd|com|scr|pif|vbs|vbe|js|jse|ws|wsf|wsc|wsh|ps1|ps1xml|ps2|ps2xml|psc1|psc2|msi|msp|mst|jar|hta|cpl|reg|inf|dll)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public ExecutableFileSpecification(IConfigService configService, Logger logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            if (!_configService.RejectReleasesWithExecutableFiles)
            {
                return DownloadSpecDecision.Accept();
            }

            if (subject.Release == null)
            {
                return DownloadSpecDecision.Accept();
            }

            if (ExecutableFileRegex.IsMatch(subject.Release.Title))
            {
                _logger.Debug("Release title contains executable file extensions, rejecting.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.ExecutableFile, "Release title contains executable file extensions");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
