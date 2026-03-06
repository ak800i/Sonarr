using System.Text.RegularExpressions;
using NLog;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class ExecutableFileSpecification : IDownloadDecisionEngineSpecification
    {
        private static readonly Regex ExecutableFileRegex = new Regex(@"\.(exe|lnk)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
                _logger.Debug("Release contains executable files (.exe or .lnk), rejecting.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.ExecutableFile, "Release contains executable files");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
