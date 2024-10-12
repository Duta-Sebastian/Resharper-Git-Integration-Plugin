using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;

namespace ReSharperPlugin.GitIntegration.SettingsPage;

[OptionsPage(PID, PageTitle, null,
    ParentId = CodeInspectionPage.PID)]
public class GitPluginOptionsPage : BeSimpleOptionsPage
{
    private const string PID = nameof(GitPluginOptionsPage);
    private const string PageTitle = "Git commit highligher";

    private readonly Lifetime _lifetime;

    public GitPluginOptionsPage(Lifetime lifetime,
        OptionsPageContext optionsPageContext,
        OptionsSettingsSmartContext optionsSettingsSmartContext,
        bool wrapInScrollablePanel = false)
        : base(lifetime, optionsPageContext,
            optionsSettingsSmartContext,
            wrapInScrollablePanel)
    {
        _lifetime = lifetime;

        AddKeyword("Git", "Commit", "Highlight");

        AddText("This page sets how many commits are being highlighted.");
        AddSpacer();

        AddHeader("Options");
        AddText("Specify how many recent Git commits to highlight in the editor.");
        AddIntOption((GitPluginSettings x) => x.CommitCounter, "Number of commits to highlight");
    }
}