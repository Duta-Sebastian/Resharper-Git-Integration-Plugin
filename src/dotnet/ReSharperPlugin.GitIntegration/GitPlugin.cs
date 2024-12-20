using System;
using System.Threading.Tasks;
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
    private readonly ISolution _solution;
    private GitChangesWatcher _gitChangesWatcher;
    private bool _isInGitRepo;

    public GitPlugin(Lifetime lifetime, ISolution solution, IDaemon daemon, ISettingsStore settingsStore)
    {
        _lifetime = lifetime;
        _solution = solution;
        _daemon = daemon;

        _commitCount = settingsStore.BindToContextLive
                (_lifetime, ContextRange.ApplicationWide)
            .GetValueProperty<GitPluginSettings, int>
                (_lifetime, settings => settings.CommitCounter);

        _commitCount.Change.Advise(_lifetime, OnCommitCountChanged);

        InitializeGitWatcher();
        if (_isInGitRepo)
            _ = PublishRecentGitCommits();
        else Console.WriteLine(@"Not in git");
    }

    protected virtual async void OnCommitCountChanged()
    {
        await PublishRecentGitCommits();
        _daemon.Invalidate("Settings changed");
    }

    protected virtual async void OnGitChangesDetected(object sender, EventArgs e)
    {
        await PublishRecentGitCommits();
        _daemon.Invalidate("Git changed"); 
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

    private async Task PublishRecentGitCommits()
    {
        var gitCommitGetter = new GitRecentCommitsGetter(_solution, _commitCount.Value);
        var commitMessageUnformatted = await Task.Run(() =>
        gitCommitGetter.GetGitRecentCommits(), _lifetime.ToCancellationToken());

        var gitCommitFormatter = new GitCommitFormatter(commitMessageUnformatted,
            _solution.SolutionDirectory.FullPath);
        var commitMessageFormatted = await Task.Run(() =>
            gitCommitFormatter.FormatCommitMessage(), _lifetime.ToCancellationToken());

        GitMessageBus.Publish(commitMessageFormatted);
    }
}