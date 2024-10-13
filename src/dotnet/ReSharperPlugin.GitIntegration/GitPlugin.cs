using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Settings;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Icons.CommonThemedIcons;
using JetBrains.Common.Util;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Reflection;
using JetBrains.ReSharper.Daemon.Impl;
using ReSharperPlugin.GitIntegration.Git;
using ReSharperPlugin.GitIntegration.SettingsPage;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.PsiGen;

namespace ReSharperPlugin.GitIntegration;

[SolutionComponent]
public class GitPlugin
{
    private readonly Lifetime _lifetime;
    private readonly ISolution _solution;
    private readonly ISettingsStore _settingsStore;
    private readonly IDaemon _daemon;
    private readonly IProperty<int> _commitCount;

    public event EventHandler<int> CommitCountChanged; 
    
    protected virtual void OnCommitCountChanged(int newCount)
    {
        CommitCountChanged?.Invoke(this, newCount);
        _daemon.Invalidate("Settings changed");
        Git();
    }

    public GitPlugin(Lifetime lifetime, ISolution solution, IDaemon daemon, ISettingsStore settingsStore)
    {
        _lifetime = lifetime;
        _solution = solution;
        _settingsStore = settingsStore;
        _daemon = daemon;
        _commitCount = _settingsStore.BindToContextLive(_lifetime,ContextRange.ApplicationWide)
            .GetValueProperty<GitPluginSettings, int>(_lifetime, settings => settings.CommitCounter);
        _commitCount.Change.Advise(_lifetime, changeEvent =>
        {
            OnCommitCountChanged(changeEvent.New);
        });
    }

    private void Git()
    {
        var gitRepoChecker = new GitRepositoryChecker(_solution);
        var isInGitRepo = gitRepoChecker.IsPartOfGitRepository();
        if (!isInGitRepo) return;
        var gitCommitGetter = new GitRecentCommitsGetter(_solution, _commitCount.Value);
        var commitMessage = gitCommitGetter.GetGitRecentCommits();
        MessageBus.Publish(commitMessage);
    }
}