using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.GitIntegration.Highlighting;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    null,
    null,
    HighlightingGroupIds.CodeSmell,
    null,
    typeof(Resources),
    nameof(Resources.GithubIntegrationPluginTitle),
    null,
    typeof(Resources),
    nameof(Resources.GithubIntegrationPluginDescription),
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.ERROR,
    OverloadResolvePriority = 0,
    ToolTipFormatStringResourceType = typeof(Resources),
    ToolTipFormatStringResourceName = nameof(Resources.GithubIntegrationPluginToolTipFormat))]
public class GitHighlighting(DocumentRange range, string commitMessage) : IHighlighting
{
    private const string SeverityId = "Github Integration Plugin";

    private DocumentRange Range { get; } = range;

    public bool IsValid()
    {
        return Range.IsValid();
    }

    public DocumentRange CalculateRange()
    {
        return Range;
    }

    public string ToolTip => string.Format(Resources.GithubIntegrationPluginToolTipFormat, commitMessage);
    public string ErrorStripeToolTip => ToolTip;
}