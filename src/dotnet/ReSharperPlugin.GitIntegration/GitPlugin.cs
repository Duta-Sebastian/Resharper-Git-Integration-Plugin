using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Settings;
using JetBrains.Application.Threading;
using JetBrains.Common.Util;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using ReSharperPlugin.GitIntegration.Git;
using ReSharperPlugin.GitIntegration.SettingsPage;

namespace ReSharperPlugin.GitIntegration;

[SolutionComponent]
public class GitPlugin
{
    private readonly Lifetime _lifetime;
    private readonly ISolution _solution;
    private readonly ISettingsStore _settingsStore;
    private readonly IProperty<int> _commitCount;
    private CancellationTokenSource _cancellationTokenSource;
    public event EventHandler<int> CommitCountChanged; 
    
    protected virtual async void OnCommitCountChanged(int newCount)
    {
        CommitCountChanged?.Invoke(this, newCount);
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        await Task.Run(() =>
        {
            Git(cancellationToken);
        }, cancellationToken);
    }

    public GitPlugin(Lifetime lifetime, ISolution solution, ISettingsStore settingsStore)
    {
        _lifetime = lifetime;
        _solution = solution;
        _settingsStore = settingsStore;
        _commitCount = _settingsStore.BindToContextLive(_lifetime,ContextRange.ApplicationWide)
            .GetValueProperty<GitPluginSettings, int>(_lifetime, settings => settings.CommitCounter);
        _cancellationTokenSource = new CancellationTokenSource();
        _commitCount.Change.Advise(_lifetime, changeEvent =>
        {
            OnCommitCountChanged(changeEvent.New);
        });
        Git(_cancellationTokenSource.Token);
    }

    private void Git(CancellationToken cancellationToken)
    {
        var gitRepoChecker = new GitRepositoryChecker(_solution);
        
        _solution.Locks.ExecuteOrQueueEx("Check if solution is in git",
             () => Task.Run(async () => 
                 await gitRepoChecker.IsPartOfGitRepository(cancellationToken), cancellationToken));
    }
}