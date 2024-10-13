using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.GitIntegration.Highlighting;

[RegisterConfigurableSeverity(
    SeverityId,
    CompoundItemName: null,
    CompoundItemNameResourceType: null,
    CompoundItemNameResourceName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: null,
    TitleResourceType: typeof(Resources),
    TitleResourceName: nameof(Resources.SampleHighlightingTitle),
    Description: null,
    DescriptionResourceType: typeof(Resources),
    DescriptionResourceName: nameof(Resources.SampleHighlightingDescription),
    DefaultSeverity: Severity.WARNING)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.ERROR,
    OverloadResolvePriority = 0,
    ToolTipFormatStringResourceType = typeof(Resources),
    ToolTipFormatStringResourceName = nameof(Resources.SampleHighlightingToolTipFormat))]
public class GitHighlighting(DocumentRange range, string commitMessage) : IHighlighting
{
    private const string SeverityId = "Whitespace"; // Unique severity ID

    private DocumentRange Range { get; } = range;

    public bool IsValid() => Range.IsValid();

    public DocumentRange CalculateRange() => Range;

    public string ToolTip => string.Format(Resources.SampleHighlightingToolTipFormat, commitMessage);
    public string ErrorStripeToolTip => ToolTip;
}