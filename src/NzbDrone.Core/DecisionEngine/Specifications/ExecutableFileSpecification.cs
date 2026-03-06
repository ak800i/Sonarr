using System.Text.RegularExpressions;
using NLog;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class ExecutableFileSpecification : IDownloadDecisionEngineSpecification
    {
        private static readonly Regex ExecutableFileRegex = new Regex(@"\.(exe|lnk)(?:\s|$|\.)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Logger _logger;

        public ExecutableFileSpecification(Logger logger)
        {
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
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
