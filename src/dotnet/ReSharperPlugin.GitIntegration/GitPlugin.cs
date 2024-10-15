using System;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharperPlugin.GitIntegration.Git;
using ReSharperPlugin.GitIntegration.SettingsPage;

namespace ReSharperPlugin.GitIntegration;

[SolutionComponent]
public class GitPlugin
{
    private readonly IProperty<int> _commitCount;
    private readonly IDaemon _daemon;
    private readonly Lifetime _lifetime;
    private readonly ISettingsStore _settingsStore;
    private readonly ISolution _solution;
    private GitChangesWatcher _gitChangesWatcher;
    private bool _isInGitRepo;

    public GitPlugin(Lifetime lifetime, ISolution solution, IDaemon daemon, ISettingsStore settingsStore)
    {
        _lifetime = lifetime;
        _solution = solution;
        _settingsStore = settingsStore;
        _daemon = daemon;

        _commitCount = _settingsStore.BindToContextLive
                (_lifetime, ContextRange.ApplicationWide)
            .GetValueProperty<GitPluginSettings, int>
                (_lifetime, settings => settings.CommitCounter);

        _commitCount.Change.Advise(_lifetime, OnCommitCountChanged);

        InitializeGitWatcher();
        if (_isInGitRepo) PublishRecentGitCommits();
        else Console.WriteLine(@"Not in git");
    }

    protected virtual void OnCommitCountChanged()
    {
        _daemon.Invalidate("Settings changed");
        PublishRecentGitCommits();
    }

    protected virtual void OnGitChangesDetected(object sender, EventArgs e)
    {
        _daemon.Invalidate("Git changed");
        PublishRecentGitCommits();
    }

    private void InitializeGitWatcher()
    {
        var gitRepoChecker = new GitRepositoryChecker(_solution);
        _isInGitRepo = gitRepoChecker.IsPartOfGitRepository();
        if (!_isInGitRepo) return;

        _gitChangesWatcher = new GitChangesWatcher(
            _solution.SolutionDirectory.FullPath + "\\.git");
        _gitChangesWatcher.ChangesDetected += OnGitChangesDetected;
    }

    private void PublishRecentGitCommits()
    {
        var gitCommitGetter = new GitRecentCommitsGetter(_solution, _commitCount.Value);
        var commitMessageUnformatted = gitCommitGetter.GetGitRecentCommits();

        var gitCommitFormatter = new GitCommitFormatter(commitMessageUnformatted,
            _solution.SolutionDirectory.FullPath);
        var commitMessageFormatted = gitCommitFormatter.FormatCommitMessage();

        MessageBus.Publish(commitMessageFormatted);
    }
}